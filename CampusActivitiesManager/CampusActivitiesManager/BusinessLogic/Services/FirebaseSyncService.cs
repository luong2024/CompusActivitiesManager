using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CampusActivitiesManager.BusinessLogic.Interfaces;
using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.BusinessLogic.Services
{
    public class FirebaseSyncService : IFirebaseSyncService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FirebaseSyncService> _logger;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        private string? _cachedAccessToken;
        private DateTime _tokenExpiryUtc = DateTime.MinValue;
        private readonly SemaphoreSlim _authLock = new(1, 1);

        public string DatabaseUrl
        {
            get => FirebaseConfig.FirestoreBaseUrl;
            set { /* Fixed to Cloud Firestore */ }
        }

        public bool IsConfigured => !string.IsNullOrWhiteSpace(FirebaseConfig.ProjectId) &&
                                    !string.IsNullOrWhiteSpace(FirebaseConfig.PrivateKey);

        public FirebaseSyncService(ILogger<FirebaseSyncService> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(15)
            };
        }

        #region Google OAuth2 Authentication (RS256 JWT Exchange)

        private async Task<string?> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedAccessToken) && DateTime.UtcNow < _tokenExpiryUtc)
            {
                return _cachedAccessToken;
            }

            await _authLock.WaitAsync();
            try
            {
                if (!string.IsNullOrEmpty(_cachedAccessToken) && DateTime.UtcNow < _tokenExpiryUtc)
                {
                    return _cachedAccessToken;
                }

                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string headerJson = "{\"alg\":\"RS256\",\"typ\":\"JWT\"}";
                string claimJson = JsonSerializer.Serialize(new Dictionary<string, object>
                {
                    ["iss"] = FirebaseConfig.ClientEmail,
                    ["scope"] = "https://www.googleapis.com/auth/datastore https://www.googleapis.com/auth/identitytoolkit https://www.googleapis.com/auth/firebase",
                    ["aud"] = FirebaseConfig.TokenUri,
                    ["exp"] = now + 3600,
                    ["iat"] = now
                });

                string b64Header = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
                string b64Claim = Base64UrlEncode(Encoding.UTF8.GetBytes(claimJson));
                string unsignedJwt = $"{b64Header}.{b64Claim}";

                using var rsa = RSA.Create();
                rsa.ImportFromPem(FirebaseConfig.PrivateKey);
                byte[] signature = rsa.SignData(Encoding.UTF8.GetBytes(unsignedJwt), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string b64Signature = Base64UrlEncode(signature);
                string assertion = $"{unsignedJwt}.{b64Signature}";

                using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, FirebaseConfig.TokenUri)
                {
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer",
                        ["assertion"] = assertion
                    })
                };

                var response = await _httpClient.SendAsync(tokenRequest);
                if (!response.IsSuccessStatusCode)
                {
                    string err = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Google OAuth2 token request failed with {Status}: {Error}", response.StatusCode, err);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("access_token", out var tokenProp))
                {
                    _cachedAccessToken = tokenProp.GetString();
                    _tokenExpiryUtc = DateTime.UtcNow.AddMinutes(50);
                    return _cachedAccessToken;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to exchange service account JWT for Google access token");
                return null;
            }
            finally
            {
                _authLock.Release();
            }
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private async Task<HttpRequestMessage> CreateAuthorizedRequestAsync(HttpMethod method, string url)
        {
            var req = new HttpRequestMessage(method, url);
            string? token = await GetAccessTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return req;
        }

        #endregion

        #region Firestore Low-Level REST Engine

        private async Task<List<(string DocId, string? Payload, Dictionary<string, string> Fields)>> ReadFirestoreCollectionAsync(string collectionName)
        {
            var results = new List<(string, string?, Dictionary<string, string>)>();
            try
            {
                string url = $"{FirebaseConfig.FirestoreBaseUrl}/{collectionName}?pageSize=300";
                using var req = await CreateAuthorizedRequestAsync(HttpMethod.Get, url);
                using var res = await _httpClient.SendAsync(req);
                if (!res.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Firestore Read Collection '{Collection}' returned {Status}", collectionName, res.StatusCode);
                    return results;
                }

                var json = await res.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json)) return results;

                using var doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("documents", out var docsArray))
                    return results;

                foreach (var docEl in docsArray.EnumerateArray())
                {
                    string docName = docEl.GetProperty("name").GetString() ?? "";
                    string docId = docName.Split('/').Last();
                    string? payload = null;
                    var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    if (docEl.TryGetProperty("fields", out var fieldsEl))
                    {
                        foreach (var prop in fieldsEl.EnumerateObject())
                        {
                            var fval = prop.Value;
                            if (fval.TryGetProperty("stringValue", out var sval))
                            {
                                string str = sval.GetString() ?? "";
                                if (prop.Name.Equals("payload", StringComparison.OrdinalIgnoreCase))
                                {
                                    payload = str;
                                }
                                fields[prop.Name] = str;
                            }
                            else if (fval.TryGetProperty("integerValue", out var ival))
                            {
                                fields[prop.Name] = ival.GetString() ?? "";
                            }
                            else if (fval.TryGetProperty("booleanValue", out var bval))
                            {
                                fields[prop.Name] = bval.GetBoolean().ToString().ToLowerInvariant();
                            }
                        }
                    }

                    results.Add((docId, payload, fields));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error reading Firestore collection '{Collection}'", collectionName);
            }
            return results;
        }

        private async Task<bool> WriteFirestoreDocumentAsync(string collectionName, string docId, string payloadJson, Dictionary<string, string>? stringFields = null)
        {
            try
            {
                string safeDocId = Uri.EscapeDataString(docId.Replace("/", "_"));
                string url = $"{FirebaseConfig.FirestoreBaseUrl}/{collectionName}/{safeDocId}";

                var fieldsDict = new Dictionary<string, object>
                {
                    ["payload"] = new Dictionary<string, string> { ["stringValue"] = payloadJson },
                    ["updatedAt"] = new Dictionary<string, string> { ["stringValue"] = DateTime.UtcNow.ToString("O") }
                };

                if (stringFields != null)
                {
                    foreach (var kvp in stringFields)
                    {
                        if (!string.IsNullOrEmpty(kvp.Key))
                        {
                            fieldsDict[kvp.Key] = new Dictionary<string, string> { ["stringValue"] = kvp.Value ?? "" };
                        }
                    }
                }

                var bodyObj = new Dictionary<string, object> { ["fields"] = fieldsDict };
                string bodyJson = JsonSerializer.Serialize(bodyObj, _jsonOptions);

                using var req = await CreateAuthorizedRequestAsync(HttpMethod.Patch, url);
                req.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                using var res = await _httpClient.SendAsync(req);
                if (res.IsSuccessStatusCode)
                {
                    return true;
                }

                string err = await res.Content.ReadAsStringAsync();
                _logger.LogWarning("Firestore Write Document '{Collection}/{DocId}' failed with {Status}: {Error}", collectionName, docId, res.StatusCode, err);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Exception writing Firestore document '{Collection}/{DocId}'", collectionName, docId);
                return false;
            }
        }

        private async Task<bool> DeleteFirestoreDocumentAsync(string collectionName, string docId)
        {
            try
            {
                string safeDocId = Uri.EscapeDataString(docId.Replace("/", "_"));
                string url = $"{FirebaseConfig.FirestoreBaseUrl}/{collectionName}/{safeDocId}";
                using var req = await CreateAuthorizedRequestAsync(HttpMethod.Delete, url);
                using var res = await _httpClient.SendAsync(req);
                return res.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Exception deleting Firestore document '{Collection}/{DocId}'", collectionName, docId);
                return false;
            }
        }

        #endregion

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                string? token = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token)) return false;

                string url = $"{FirebaseConfig.FirestoreBaseUrl}/events?pageSize=1";
                using var req = await CreateAuthorizedRequestAsync(HttpMethod.Get, url);
                using var res = await _httpClient.SendAsync(req);
                return res.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Firebase Cloud Firestore connection test failed");
                return false;
            }
        }

        #region Events Sync

        public async Task<List<CampusEvent>?> PullEventsAsync()
        {
            var rawDocs = await ReadFirestoreCollectionAsync("events");
            if (rawDocs.Count == 0) return null;

            var list = new List<CampusEvent>();
            foreach (var item in rawDocs)
            {
                CampusEvent? evt = null;
                if (!string.IsNullOrWhiteSpace(item.Payload))
                {
                    try { evt = JsonSerializer.Deserialize<CampusEvent>(item.Payload, _jsonOptions); } catch { }
                }

                if (evt == null && item.Fields.Count > 0)
                {
                    int.TryParse(item.DocId, out int id);
                    if (id == 0 && item.Fields.TryGetValue("id", out var idStr))
                        int.TryParse(idStr, out id);

                    evt = new CampusEvent
                    {
                        ID = id > 0 ? id : 1,
                        Name = item.Fields.GetValueOrDefault("name", "Hoạt động"),
                        Description = item.Fields.GetValueOrDefault("description", ""),
                        Icon = item.Fields.GetValueOrDefault("icon", "💻"),
                        Status = item.Fields.GetValueOrDefault("status", "Sắp diễn ra"),
                        ApprovalStatus = item.Fields.GetValueOrDefault("approvalStatus", "Approved"),
                        IsApproved = item.Fields.GetValueOrDefault("isApproved", "true").Equals("true", StringComparison.OrdinalIgnoreCase),
                        ApprovedBy = item.Fields.GetValueOrDefault("approvedBy", "Admin"),
                        Location = item.Fields.GetValueOrDefault("location", "Hội trường A2"),
                        StartDate = item.Fields.GetValueOrDefault("startDate", DateTime.Now.AddDays(2).ToString("dd/MM/yyyy")),
                        EndDate = item.Fields.GetValueOrDefault("endDate", DateTime.Now.AddDays(2).ToString("dd/MM/yyyy")),
                        StartTime = item.Fields.GetValueOrDefault("startTime", "08:00"),
                        EndTime = item.Fields.GetValueOrDefault("endTime", "11:30")
                    };
                }

                if (evt != null && evt.ApprovalStatus != "Rejected" && evt.Status != "Đã từ chối")
                {
                    list.Add(evt);
                }
            }
            return list.OrderBy(e => e.ID).ToList();
        }

        public async Task<bool> PushEventsAsync(List<CampusEvent> events)
        {
            bool allOk = true;
            foreach (var evt in events)
            {
                bool ok = await PushEventAsync(evt);
                if (!ok) allOk = false;
            }
            return allOk;
        }

        public async Task<bool> PushEventAsync(CampusEvent evt)
        {
            if (evt.ApprovalStatus == "Rejected" || evt.Status == "Đã từ chối")
            {
                return await DeleteEventAsync(evt.ID);
            }

            var stringFields = new Dictionary<string, string>
            {
                ["id"] = evt.ID.ToString(),
                ["name"] = evt.Name ?? "",
                ["status"] = evt.Status ?? "",
                ["approvalStatus"] = evt.ApprovalStatus ?? "",
                ["location"] = evt.Location ?? "",
                ["startDate"] = evt.StartDate ?? "",
                ["createdByRole"] = evt.CreatedByRole ?? "",
                ["createdByName"] = evt.CreatedByName ?? ""
            };

            string payload = JsonSerializer.Serialize(evt, _jsonOptions);
            return await WriteFirestoreDocumentAsync("events", evt.ID.ToString(), payload, stringFields);
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            return await DeleteFirestoreDocumentAsync("events", eventId.ToString());
        }

        #endregion

        #region Users Sync

        public async Task<List<User>?> PullUsersAsync()
        {
            var rawDocs = await ReadFirestoreCollectionAsync("users");
            if (rawDocs.Count == 0) return null;

            var list = new List<User>();
            foreach (var item in rawDocs)
            {
                User? u = null;
                if (!string.IsNullOrWhiteSpace(item.Payload))
                {
                    try { u = JsonSerializer.Deserialize<User>(item.Payload, _jsonOptions); } catch { }
                }

                if (u == null && item.Fields.Count > 0)
                {
                    Enum.TryParse<Role>(item.Fields.GetValueOrDefault("role", "User"), true, out var role);
                    string email = item.Fields.GetValueOrDefault("email", "");
                    string username = item.Fields.GetValueOrDefault("studentCode", "");
                    if (string.IsNullOrEmpty(username)) username = email.Split('@').First();

                    u = new User
                    {
                        Id = item.DocId,
                        Username = username,
                        Email = email,
                        FullName = item.Fields.GetValueOrDefault("fullName", username),
                        Role = role,
                        PhoneNumber = item.Fields.GetValueOrDefault("phoneNumber", ""),
                        ClassName = item.Fields.GetValueOrDefault("className", ""),
                        IsActive = item.Fields.GetValueOrDefault("isActive", "true").Equals("true", StringComparison.OrdinalIgnoreCase),
                        TrainingPoints = 80
                    };
                }

                if (u != null)
                {
                    list.Add(u);
                }
            }
            return list;
        }

        public async Task<bool> PushUsersAsync(List<User> users)
        {
            bool allOk = true;
            foreach (var user in users)
            {
                bool userOk = await PushUserAsync(user);
                if (!userOk) allOk = false;
            }
            return allOk;
        }

        public async Task<bool> PushUserAsync(User user)
        {
            try
            {
                // 1. Đồng bộ tài khoản lên Firebase Authentication (bảng Users trên Firebase Console)
                _ = Task.Run(async () => await SyncUserToFirebaseAuthAsync(user));

                // 2. Đồng bộ thông tin chi tiết vào Cloud Firestore (collection 'users')
                string docId = !string.IsNullOrWhiteSpace(user.Id) ? user.Id : (!string.IsNullOrWhiteSpace(user.Username) ? user.Username : Guid.NewGuid().ToString("N")[..8]);
                var stringFields = new Dictionary<string, string>
                {
                    ["email"] = user.Email ?? "",
                    ["fullName"] = user.FullName ?? "",
                    ["role"] = user.Role.ToString(),
                    ["studentCode"] = user.Username ?? "",
                    ["phoneNumber"] = user.PhoneNumber ?? "",
                    ["className"] = user.ClassName ?? "",
                    ["isActive"] = user.IsActive.ToString().ToLowerInvariant()
                };

                string payload = JsonSerializer.Serialize(user, _jsonOptions);
                return await WriteFirestoreDocumentAsync("users", docId, payload, stringFields);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Lỗi đồng bộ người dùng {Email} lên Firebase", user.Email);
                return false;
            }
        }

        public async Task<bool> SyncUserToFirebaseAuthAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email)) return false;

            try
            {
                string? token = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token)) return false;

                string effectivePassword = !string.IsNullOrWhiteSpace(user.Password)
                    ? user.Password
                    : (!string.IsNullOrWhiteSpace(user.PasswordHash) ? user.PasswordHash : user.Role.GetDefaultPassword());

                if (string.IsNullOrWhiteSpace(effectivePassword) || effectivePassword.Length < 6)
                {
                    effectivePassword = "Password@123";
                }

                var createPayload = new
                {
                    email = user.Email.Trim(),
                    password = effectivePassword,
                    displayName = !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName.Trim() : user.Username,
                    emailVerified = true
                };

                string url = $"https://identitytoolkit.googleapis.com/v1/projects/{FirebaseConfig.ProjectId}/accounts";
                using var req = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Headers = { Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token) },
                    Content = new StringContent(JsonSerializer.Serialize(createPayload), Encoding.UTF8, "application/json")
                };

                using var res = await _httpClient.SendAsync(req);
                if (res.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully synced user {Email} to Firebase Authentication", user.Email);
                    return true;
                }

                string body = await res.Content.ReadAsStringAsync();
                if (body.Contains("EMAIL_EXISTS"))
                {
                    return true;
                }

                _logger.LogWarning("Firebase Authentication account sync for {Email} returned {Status}: {Body}", user.Email, res.StatusCode, body);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Exception syncing user {Email} to Firebase Authentication", user.Email);
                return false;
            }
        }

        #endregion

        #region Classes Sync

        public async Task<List<ClassRoom>?> PullClassesAsync()
        {
            var rawDocs = await ReadFirestoreCollectionAsync("classes");
            if (rawDocs.Count == 0) return null;

            var list = new List<ClassRoom>();
            foreach (var item in rawDocs)
            {
                if (!string.IsNullOrWhiteSpace(item.Payload))
                {
                    try
                    {
                        var cls = JsonSerializer.Deserialize<ClassRoom>(item.Payload, _jsonOptions);
                        if (cls != null) list.Add(cls);
                    }
                    catch { }
                }
            }
            return list;
        }

        public async Task<bool> PushClassesAsync(List<ClassRoom> classes)
        {
            bool allOk = true;
            foreach (var cls in classes)
            {
                string payload = JsonSerializer.Serialize(cls, _jsonOptions);
                var stringFields = new Dictionary<string, string>
                {
                    ["className"] = cls.ClassName ?? "",
                    ["advisorName"] = cls.AdvisorName ?? "",
                    ["department"] = cls.Department ?? ""
                };
                string clsDocId = !string.IsNullOrWhiteSpace(cls.ClassName) ? cls.ClassName : Guid.NewGuid().ToString("N")[..8];
                bool ok = await WriteFirestoreDocumentAsync("classes", clsDocId, payload, stringFields);
                if (!ok) allOk = false;
            }
            return allOk;
        }

        #endregion

        #region Attendance Sync

        public async Task<List<AttendanceRecord>?> PullAttendanceAsync()
        {
            var rawDocs = await ReadFirestoreCollectionAsync("attendance");
            if (rawDocs.Count == 0) return null;

            var list = new List<AttendanceRecord>();
            foreach (var item in rawDocs)
            {
                if (!string.IsNullOrWhiteSpace(item.Payload))
                {
                    try
                    {
                        var rec = JsonSerializer.Deserialize<AttendanceRecord>(item.Payload, _jsonOptions);
                        if (rec != null) list.Add(rec);
                    }
                    catch { }
                }
            }
            return list;
        }

        public async Task<bool> PushAttendanceAsync(List<AttendanceRecord> records)
        {
            bool allOk = true;
            foreach (var rec in records)
            {
                string payload = JsonSerializer.Serialize(rec, _jsonOptions);
                var stringFields = new Dictionary<string, string>
                {
                    ["id"] = rec.Id ?? "",
                    ["eventId"] = rec.EventId.ToString(),
                    ["studentCode"] = rec.StudentCode ?? "",
                    ["status"] = rec.Status ?? ""
                };
                string recDocId = !string.IsNullOrWhiteSpace(rec.Id) ? rec.Id : Guid.NewGuid().ToString("N")[..8];
                bool ok = await WriteFirestoreDocumentAsync("attendance", recDocId, payload, stringFields);
                if (!ok) allOk = false;
            }
            return allOk;
        }

        #endregion

        #region Password Resets Sync

        public async Task<List<PasswordResetRequest>?> PullPasswordResetRequestsAsync()
        {
            var rawDocs = await ReadFirestoreCollectionAsync("password_resets");
            if (rawDocs.Count == 0) return null;

            var list = new List<PasswordResetRequest>();
            foreach (var item in rawDocs)
            {
                if (!string.IsNullOrWhiteSpace(item.Payload))
                {
                    try
                    {
                        var req = JsonSerializer.Deserialize<PasswordResetRequest>(item.Payload, _jsonOptions);
                        if (req != null) list.Add(req);
                    }
                    catch { }
                }
            }
            return list;
        }

        public async Task<bool> PushPasswordResetRequestsAsync(List<PasswordResetRequest> requests)
        {
            bool allOk = true;
            foreach (var req in requests)
            {
                string payload = JsonSerializer.Serialize(req, _jsonOptions);
                var stringFields = new Dictionary<string, string>
                {
                    ["id"] = req.Id ?? "",
                    ["username"] = req.Username ?? "",
                    ["status"] = req.Status ?? ""
                };
                string reqDocId = !string.IsNullOrWhiteSpace(req.Id) ? req.Id : Guid.NewGuid().ToString("N")[..8];
                bool ok = await WriteFirestoreDocumentAsync("password_resets", reqDocId, payload, stringFields);
                if (!ok) allOk = false;
            }
            return allOk;
        }

        #endregion

        #region Notifications Sync

        public async Task<List<CampusNotificationDto>?> PullNotificationsAsync()
        {
            var rawDocs = await ReadFirestoreCollectionAsync("notifications");
            if (rawDocs.Count == 0) return null;

            var list = new List<CampusNotificationDto>();
            foreach (var item in rawDocs)
            {
                if (!string.IsNullOrWhiteSpace(item.Payload))
                {
                    try
                    {
                        var notif = JsonSerializer.Deserialize<CampusNotificationDto>(item.Payload, _jsonOptions);
                        if (notif != null) list.Add(notif);
                    }
                    catch { }
                }
            }
            return list;
        }

        public async Task<bool> PushNotificationsAsync(List<CampusNotificationDto> notifications)
        {
            bool allOk = true;
            foreach (var n in notifications)
            {
                string payload = JsonSerializer.Serialize(n, _jsonOptions);
                var stringFields = new Dictionary<string, string>
                {
                    ["id"] = n.Id ?? "",
                    ["title"] = n.Title ?? "",
                    ["targetUsername"] = n.TargetUsername ?? ""
                };
                string nDocId = !string.IsNullOrWhiteSpace(n.Id) ? n.Id : Guid.NewGuid().ToString("N")[..8];
                bool ok = await WriteFirestoreDocumentAsync("notifications", nDocId, payload, stringFields);
                if (!ok) allOk = false;
            }
            return allOk;
        }

        #endregion

        #region Point Audits Sync

        public async Task<List<PointAuditRecord>?> PullPointAuditsAsync()
        {
            var rawDocs = await ReadFirestoreCollectionAsync("point_audits");
            if (rawDocs.Count == 0) return null;

            var list = new List<PointAuditRecord>();
            foreach (var item in rawDocs)
            {
                if (!string.IsNullOrWhiteSpace(item.Payload))
                {
                    try
                    {
                        var audit = JsonSerializer.Deserialize<PointAuditRecord>(item.Payload, _jsonOptions);
                        if (audit != null) list.Add(audit);
                    }
                    catch { }
                }
            }
            return list;
        }

        public async Task<bool> PushPointAuditsAsync(List<PointAuditRecord> audits)
        {
            bool allOk = true;
            foreach (var a in audits)
            {
                string payload = JsonSerializer.Serialize(a, _jsonOptions);
                var stringFields = new Dictionary<string, string>
                {
                    ["id"] = a.Id ?? "",
                    ["studentCode"] = a.StudentCode ?? "",
                    ["delta"] = a.Delta.ToString()
                };
                string aDocId = !string.IsNullOrWhiteSpace(a.Id) ? a.Id : Guid.NewGuid().ToString("N")[..8];
                bool ok = await WriteFirestoreDocumentAsync("point_audits", aDocId, payload, stringFields);
                if (!ok) allOk = false;
            }
            return allOk;
        }

        #endregion
    }
}
