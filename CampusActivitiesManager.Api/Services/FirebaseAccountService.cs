using CampusActivitiesManager.Api.Models;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CampusActivitiesManager.Api.Services
{
    public class FirebaseAccountService : IFirebaseAccountService
    {
        private readonly FirebaseAuth? _firebaseAuth;
        private readonly FirestoreDb? _firestoreDb;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FirebaseAccountService> _logger;

        public FirebaseAccountService(IConfiguration configuration, ILogger<FirebaseAccountService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            string? credPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")?.Trim('"', ' ');
            if (string.IsNullOrEmpty(credPath) || !File.Exists(credPath))
            {
                string defaultKey = @"D:\keys\campusacmanage-df24bcb494c7.json";
                if (File.Exists(defaultKey))
                {
                    credPath = defaultKey;
                }
            }
            if (!string.IsNullOrEmpty(credPath) && File.Exists(credPath))
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credPath);
            }

            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.GetApplicationDefault(),
                        ProjectId = "campusacmanage"
                    });
                }
                _firebaseAuth = FirebaseAuth.DefaultInstance;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "FirebaseAuth.DefaultInstance could not be obtained.");
                _firebaseAuth = null;
            }

            try
            {
                string projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT")?.Trim('"', ' ') ?? "campusacmanage";
                _firestoreDb = FirestoreDb.Create(projectId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "FirestoreDb could not be initialized.");
                _firestoreDb = null;
            }
        }

        private static string? FormatE164PhoneNumber(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return null;
            var digits = new string(phone.Trim().Where(c => char.IsDigit(c) || c == '+').ToArray());
            if (digits.StartsWith("0") && digits.Length >= 9 && digits.Length <= 11)
            {
                return "+84" + digits.Substring(1);
            }
            if (digits.StartsWith("+") && digits.Length >= 10 && digits.Length <= 15)
            {
                return digits;
            }
            return null;
        }

        public async Task<UserAccountDto> CreateAccountAsync(CreateAccountRequest request)
        {
            if (_firebaseAuth == null)
            {
                throw new InvalidOperationException("Firebase Authentication is not initialized. Check GOOGLE_APPLICATION_CREDENTIALS.");
            }

            string? e164Phone = FormatE164PhoneNumber(request.PhoneNumber);
            var userArgs = new UserRecordArgs
            {
                Email = request.Email,
                Password = request.Password,
                DisplayName = request.FullName,
                PhoneNumber = e164Phone
            };

            UserRecord userRecord;
            try
            {
                userRecord = await _firebaseAuth.CreateUserAsync(userArgs);
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode != AuthErrorCode.EmailAlreadyExists && userArgs.PhoneNumber != null)
            {
                // If phone number already exists or invalid in Firebase Auth, create user without Auth PhoneNumber and store PhoneNumber in Firestore
                userArgs.PhoneNumber = null;
                userRecord = await _firebaseAuth.CreateUserAsync(userArgs);
            }
            catch (ArgumentException) when (userArgs.PhoneNumber != null)
            {
                userArgs.PhoneNumber = null;
                userRecord = await _firebaseAuth.CreateUserAsync(userArgs);
            }

            string createdIso = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            if (_firestoreDb != null)
            {
                try
                {
                    DocumentReference docRef = _firestoreDb.Collection("users").Document(userRecord.Uid);
                    await docRef.SetAsync(new Dictionary<string, object?>
                    {
                        ["email"] = request.Email,
                        ["fullName"] = request.FullName,
                        ["role"] = request.Role,
                        ["phoneNumber"] = request.PhoneNumber ?? "",
                        ["studentCode"] = request.StudentCode ?? request.Email.Split('@')[0],
                        ["isActive"] = true,
                        ["isLocked"] = false,
                        ["createdAt"] = createdIso
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to persist user profile in Firestore for UID {Uid}", userRecord.Uid);
                }
            }

            return new UserAccountDto
            {
                Id = userRecord.Uid,
                Email = request.Email,
                FullName = request.FullName,
                Role = request.Role,
                PhoneNumber = request.PhoneNumber,
                StudentCode = request.StudentCode ?? request.Email.Split('@')[0],
                IsDisabled = false,
                IsActive = true,
                CreatedAt = createdIso
            };
        }

        public async Task<UserAccountDto?> GetAccountByIdAsync(string id)
        {
            UserRecord? authUser = null;
            if (_firebaseAuth != null)
            {
                try
                {
                    authUser = await _firebaseAuth.GetUserAsync(id);
                }
                catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
                {
                    authUser = null;
                }
            }

            Dictionary<string, object>? firestoreDict = null;
            if (_firestoreDb != null)
            {
                try
                {
                    var doc = await _firestoreDb.Collection("users").Document(id).GetSnapshotAsync();
                    if (doc.Exists)
                    {
                        firestoreDict = doc.ToDictionary();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not load Firestore metadata for UID {Uid}", id);
                }
            }

            if (authUser == null && firestoreDict == null)
            {
                return null;
            }

            string email = authUser?.Email ?? (firestoreDict != null && firestoreDict.TryGetValue("email", out var em) ? em?.ToString() ?? "" : "");
            string fullName = (firestoreDict != null && firestoreDict.TryGetValue("fullName", out var fn) && !string.IsNullOrWhiteSpace(fn?.ToString()))
                ? fn!.ToString()!
                : (authUser?.DisplayName ?? "");
            string role = (firestoreDict != null && firestoreDict.TryGetValue("role", out var r) && r != null)
                ? r.ToString()!
                : "Student";
            string? phoneNumber = (firestoreDict != null && firestoreDict.TryGetValue("phoneNumber", out var ph) && !string.IsNullOrWhiteSpace(ph?.ToString()))
                ? ph!.ToString()
                : authUser?.PhoneNumber;
            string? studentCode = (firestoreDict != null && firestoreDict.TryGetValue("studentCode", out var sc) && sc != null)
                ? sc.ToString()
                : (email.Contains('@') ? email.Split('@')[0] : id);
            string? avatarUrl = (firestoreDict != null && firestoreDict.TryGetValue("avatarUrl", out var av) && av != null)
                ? av.ToString()
                : authUser?.PhotoUrl;

            bool isLocked = (authUser?.Disabled ?? false) || (firestoreDict != null && firestoreDict.TryGetValue("isLocked", out var lk) && lk != null && Convert.ToBoolean(lk));
            bool isActive = !isLocked && (firestoreDict == null || !firestoreDict.TryGetValue("isActive", out var act) || act == null || Convert.ToBoolean(act));

            return new UserAccountDto
            {
                Id = authUser?.Uid ?? id,
                Email = email,
                FullName = fullName,
                Role = role,
                PhoneNumber = phoneNumber,
                StudentCode = studentCode,
                AvatarUrl = avatarUrl,
                IsDisabled = isLocked,
                IsActive = isActive,
                CreatedAt = (firestoreDict != null && firestoreDict.TryGetValue("createdAt", out var ca) && ca != null)
                    ? ca.ToString()!
                    : DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
        }

        public async Task<UserAccountDto?> GetAccountByEmailAsync(string email)
        {
            if (_firebaseAuth != null)
            {
                try
                {
                    UserRecord user = await _firebaseAuth.GetUserByEmailAsync(email);
                    return await GetAccountByIdAsync(user.Uid);
                }
                catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
                {
                    // Fallback to Firestore query by email below
                }
            }

            if (_firestoreDb != null)
            {
                try
                {
                    var query = _firestoreDb.Collection("users").WhereEqualTo("email", email).Limit(1);
                    var snap = await query.GetSnapshotAsync();
                    if (snap.Count > 0)
                    {
                        return await GetAccountByIdAsync(snap.Documents[0].Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error querying Firestore by email {Email}", email);
                }
            }

            return null;
        }

        public async Task<List<UserAccountDto>> GetAllAccountsAsync()
        {
            var usersById = new Dictionary<string, UserAccountDto>(StringComparer.OrdinalIgnoreCase);

            // 1. Read from Firestore "users" collection
            if (_firestoreDb != null)
            {
                try
                {
                    CollectionReference usersRef = _firestoreDb.Collection("users");
                    QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();
                    foreach (var doc in snapshot.Documents)
                    {
                        var dict = doc.ToDictionary();
                        bool isActive = !dict.ContainsKey("isActive") || dict["isActive"] == null || Convert.ToBoolean(dict["isActive"]);
                        bool isLocked = dict.ContainsKey("isLocked") && dict["isLocked"] != null && Convert.ToBoolean(dict["isLocked"]);
                        string email = dict.ContainsKey("email") ? dict["email"]?.ToString() ?? "" : "";

                        usersById[doc.Id] = new UserAccountDto
                        {
                            Id = doc.Id,
                            Email = email,
                            FullName = dict.ContainsKey("fullName") ? dict["fullName"]?.ToString() ?? "" : "",
                            Role = dict.ContainsKey("role") ? dict["role"]?.ToString() ?? "Student" : "Student",
                            PhoneNumber = dict.ContainsKey("phoneNumber") ? dict["phoneNumber"]?.ToString() : null,
                            StudentCode = dict.ContainsKey("studentCode") ? dict["studentCode"]?.ToString() : (email.Contains('@') ? email.Split('@')[0] : doc.Id),
                            AvatarUrl = dict.ContainsKey("avatarUrl") ? dict["avatarUrl"]?.ToString() : null,
                            IsDisabled = isLocked,
                            IsActive = isActive && !isLocked,
                            CreatedAt = dict.ContainsKey("createdAt") ? dict["createdAt"]?.ToString() ?? "" : ""
                        };
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting all accounts from Firestore");
                }
            }

            // 2. Also sync any accounts present in FirebaseAuth that are not yet in Firestore
            if (_firebaseAuth != null)
            {
                try
                {
                    var pagedEnumerable = _firebaseAuth.ListUsersAsync(null);
                    await foreach (var authUser in pagedEnumerable)
                    {
                        if (!usersById.TryGetValue(authUser.Uid, out var existingDto))
                        {
                            string email = authUser.Email ?? "";
                            string role = email.StartsWith("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Student";
                            var dto = new UserAccountDto
                            {
                                Id = authUser.Uid,
                                Email = email,
                                FullName = authUser.DisplayName ?? (email.Contains('@') ? email.Split('@')[0] : authUser.Uid),
                                Role = role,
                                PhoneNumber = authUser.PhoneNumber,
                                StudentCode = email.Contains('@') ? email.Split('@')[0] : authUser.Uid,
                                AvatarUrl = authUser.PhotoUrl,
                                IsDisabled = authUser.Disabled,
                                IsActive = !authUser.Disabled,
                                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                            };
                            usersById[authUser.Uid] = dto;

                            if (_firestoreDb != null)
                            {
                                try
                                {
                                    await _firestoreDb.Collection("users").Document(authUser.Uid).SetAsync(new Dictionary<string, object?>
                                    {
                                        ["email"] = dto.Email,
                                        ["fullName"] = dto.FullName,
                                        ["role"] = dto.Role,
                                        ["phoneNumber"] = dto.PhoneNumber ?? "",
                                        ["studentCode"] = dto.StudentCode,
                                        ["isActive"] = dto.IsActive,
                                        ["isLocked"] = dto.IsDisabled,
                                        ["createdAt"] = dto.CreatedAt
                                    }, SetOptions.MergeAll);
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error listing users from FirebaseAuth");
                }
            }

            return usersById.Values.ToList();
        }

        public async Task<UserAccountDto> UpdateAccountAsync(string id, UpdateAccountRequest request)
        {
            var currentAccount = await GetAccountByIdAsync(id);
            if (currentAccount == null)
            {
                throw new KeyNotFoundException($"Account with ID {id} not found");
            }

            if (_firebaseAuth != null)
            {
                try
                {
                    var userArgs = new UserRecordArgs { Uid = id };
                    bool hasAuthUpdates = false;
                    if (!string.IsNullOrEmpty(request.FullName))
                    {
                        userArgs.DisplayName = request.FullName;
                        hasAuthUpdates = true;
                    }
                    string? e164Phone = FormatE164PhoneNumber(request.PhoneNumber);
                    if (!string.IsNullOrEmpty(e164Phone))
                    {
                        userArgs.PhoneNumber = e164Phone;
                        hasAuthUpdates = true;
                    }
                    if (hasAuthUpdates)
                    {
                        try
                        {
                            await _firebaseAuth.UpdateUserAsync(userArgs);
                        }
                        catch when (userArgs.PhoneNumber != null && !string.IsNullOrEmpty(request.FullName))
                        {
                            userArgs.PhoneNumber = null;
                            await _firebaseAuth.UpdateUserAsync(userArgs);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not update FirebaseAuth record for UID {Uid}, continuing with Firestore update", id);
                }
            }

            string updatedIso = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            if (_firestoreDb != null)
            {
                try
                {
                    DocumentReference docRef = _firestoreDb.Collection("users").Document(id);
                    var updates = new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(request.FullName)) updates["fullName"] = request.FullName;
                    if (request.PhoneNumber != null) updates["phoneNumber"] = request.PhoneNumber;
                    if (request.AvatarUrl != null) updates["avatarUrl"] = request.AvatarUrl;
                    if (!string.IsNullOrEmpty(request.Role)) updates["role"] = request.Role;
                    updates["updatedAt"] = updatedIso;

                    await docRef.SetAsync(updates, SetOptions.MergeAll);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating Firestore document for UID {Uid}", id);
                }
            }

            return new UserAccountDto
            {
                Id = id,
                Email = currentAccount.Email,
                FullName = !string.IsNullOrEmpty(request.FullName) ? request.FullName : currentAccount.FullName,
                Role = !string.IsNullOrEmpty(request.Role) ? request.Role : currentAccount.Role,
                PhoneNumber = request.PhoneNumber ?? currentAccount.PhoneNumber,
                StudentCode = currentAccount.StudentCode,
                AvatarUrl = request.AvatarUrl ?? currentAccount.AvatarUrl,
                IsDisabled = currentAccount.IsDisabled,
                IsActive = currentAccount.IsActive,
                UpdatedAt = updatedIso
            };
        }

        public async Task<AccountStatusResponse> SetAccountLockStatusAsync(string id, bool isLocked)
        {
            var existing = await GetAccountByIdAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Account with ID {id} not found");
            }

            if (_firebaseAuth != null)
            {
                try
                {
                    var userArgs = new UserRecordArgs
                    {
                        Uid = id,
                        Disabled = isLocked
                    };
                    await _firebaseAuth.UpdateUserAsync(userArgs);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not update Disabled flag in FirebaseAuth for UID {Uid}", id);
                }
            }

            string updatedIso = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            if (_firestoreDb != null)
            {
                try
                {
                    DocumentReference docRef = _firestoreDb.Collection("users").Document(id);
                    await docRef.SetAsync(new Dictionary<string, object>
                    {
                        ["isActive"] = !isLocked,
                        ["isLocked"] = isLocked,
                        ["updatedAt"] = updatedIso
                    }, SetOptions.MergeAll);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating Firestore lock status for UID {Uid}", id);
                }
            }

            return new AccountStatusResponse
            {
                Id = id,
                IsLocked = isLocked,
                IsActive = !isLocked,
                Message = isLocked ? "Account locked successfully" : "Account unlocked successfully",
                UpdatedAt = updatedIso
            };
        }

        public async Task<LoginResponse?> AuthenticateAsync(string email, string password)
        {
            var user = await GetAccountByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            string token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Token = token,
                IsActive = user.IsActive && !user.IsDisabled
            };
        }

        private string GenerateJwtToken(UserAccountDto user)
        {
            string secretKey = _configuration["Jwt:SecretKey"] ?? "CampusActivitiesManager_DefaultSecretKey_ForTokenGeneration_2026_!@#";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("role", user.Role),
                new Claim("isActive", (user.IsActive && !user.IsDisabled).ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "CampusActivitiesManager.Api",
                audience: _configuration["Jwt:Audience"] ?? "CampusActivitiesManager.Client",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> DeleteAccountAsync(string id)
        {
            bool deletedAny = false;

            if (_firebaseAuth != null)
            {
                try
                {
                    await _firebaseAuth.DeleteUserAsync(id);
                    deletedAny = true;
                }
                catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
                {
                    // Continue to delete from Firestore if present
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error deleting user {Id} from FirebaseAuth", id);
                }
            }

            if (_firestoreDb != null)
            {
                try
                {
                    await _firestoreDb.Collection("users").Document(id).DeleteAsync();
                    deletedAny = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete user {Id} from Firestore", id);
                }
            }

            return deletedAny;
        }
    }
}
