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

        public string DatabaseUrl
        {
            get => Preferences.Default.Get(Constants.FirebaseDbUrlPrefKey, Constants.DefaultFirebaseDbUrl);
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string clean = value.Trim();
                    if (!clean.EndsWith("/")) clean += "/";
                    Preferences.Default.Set(Constants.FirebaseDbUrlPrefKey, clean);
                }
            }
        }

        public bool IsConfigured => !string.IsNullOrWhiteSpace(DatabaseUrl);

        public FirebaseSyncService(ILogger<FirebaseSyncService> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(8)
            };
        }

        private string GetCollectionUrl(string collectionName)
        {
            var baseUri = DatabaseUrl;
            if (!baseUri.EndsWith("/")) baseUri += "/";
            return $"{baseUri}{collectionName}.json";
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var url = GetCollectionUrl("ping");
                var response = await _httpClient.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Firebase connection test failed for {Url}", DatabaseUrl);
                return false;
            }
        }

        // ==================== EVENTS SYNC ====================

        public async Task<List<CampusEvent>?> PullEventsAsync()
        {
            try
            {
                var url = GetCollectionUrl("events");
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Firebase PullEvents returned status {StatusCode}", response.StatusCode);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json) || json == "null")
                    return null;

                return DeserializeList<CampusEvent>(json);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to pull events from Firebase");
                return null;
            }
        }

        public async Task<bool> PushEventsAsync(List<CampusEvent> events)
        {
            try
            {
                var url = GetCollectionUrl("events");
                var json = JsonSerializer.Serialize(events, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully pushed {Count} events to Firebase Cloud", events.Count);
                    return true;
                }
                _logger.LogWarning("Firebase PushEvents failed with status {StatusCode}", response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to push events to Firebase");
                return false;
            }
        }

        // ==================== USERS SYNC ====================

        public async Task<List<User>?> PullUsersAsync()
        {
            try
            {
                var url = GetCollectionUrl("users");
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json) || json == "null") return null;

                return DeserializeList<User>(json);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to pull users from Firebase");
                return null;
            }
        }

        public async Task<bool> PushUsersAsync(List<User> users)
        {
            try
            {
                var url = GetCollectionUrl("users");
                var json = JsonSerializer.Serialize(users, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully pushed {Count} users to Firebase Cloud", users.Count);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to push users to Firebase");
                return false;
            }
        }

        // ==================== CLASSES SYNC ====================

        public async Task<List<ClassRoom>?> PullClassesAsync()
        {
            try
            {
                var url = GetCollectionUrl("classes");
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json) || json == "null") return null;

                return DeserializeList<ClassRoom>(json);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to pull classes from Firebase");
                return null;
            }
        }

        public async Task<bool> PushClassesAsync(List<ClassRoom> classes)
        {
            try
            {
                var url = GetCollectionUrl("classes");
                var json = JsonSerializer.Serialize(classes, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to push classes to Firebase");
                return false;
            }
        }

        // ==================== ATTENDANCE SYNC ====================

        public async Task<List<AttendanceRecord>?> PullAttendanceAsync()
        {
            try
            {
                var url = GetCollectionUrl("attendance");
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json) || json == "null") return null;

                return DeserializeList<AttendanceRecord>(json);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to pull attendance from Firebase");
                return null;
            }
        }

        public async Task<bool> PushAttendanceAsync(List<AttendanceRecord> records)
        {
            try
            {
                var url = GetCollectionUrl("attendance");
                var json = JsonSerializer.Serialize(records, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to push attendance to Firebase");
                return false;
            }
        }

        /// <summary>
        /// Xử lý đặc thù của Firebase Realtime Database:
        /// Có thể trả về mảng [...] hoặc Dictionary object {"0": {...}, "1": {...}}
        /// </summary>
        private static List<T> DeserializeList<T>(string json)
        {
            var result = new List<T>();
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var elem in doc.RootElement.EnumerateArray())
                    {
                        if (elem.ValueKind != JsonValueKind.Null)
                        {
                            var item = JsonSerializer.Deserialize<T>(elem.GetRawText(), _jsonOptions);
                            if (item != null) result.Add(item);
                        }
                    }
                }
                else if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        if (prop.Value.ValueKind != JsonValueKind.Null)
                        {
                            var item = JsonSerializer.Deserialize<T>(prop.Value.GetRawText(), _jsonOptions);
                            if (item != null) result.Add(item);
                        }
                    }
                }
            }
            catch
            {
                // Fallback direct list deserialize
                var fallback = JsonSerializer.Deserialize<List<T>>(json, _jsonOptions);
                if (fallback != null) result.AddRange(fallback);
            }
            return result;
        }
    }
}
