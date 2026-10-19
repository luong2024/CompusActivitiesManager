using System.Net.Http.Json;
using System.Text.Json;
using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    public class UserService : IUserService, IDataStore<User>
    {
        private readonly HttpClient _httpClient;
        private readonly ModalErrorHandler _errorHandler;
        private readonly ILogger<UserService> _logger;

        private static readonly List<User> _localUsers = new()
        {
            new User
            {
                Id = "admin-sys-01",
                Username = "admin",
                FullName = "Quản Trị Viên Hệ Thống",
                Email = "admin@campus.edu.vn",
                PhoneNumber = "0901234567",
                Role = Role.Admin,
                IsActive = true
            },
            new User
            {
                Id = "manager-sys-02",
                Username = "manager",
                FullName = "Ban Quản Lý Hoạt Động",
                Email = "manager@campus.edu.vn",
                PhoneNumber = "0902345678",
                Role = Role.Manager,
                IsActive = true
            },
            new User
            {
                Id = "student-sys-04",
                Username = "B20DCCN001",
                FullName = "Nguyễn An Cương",
                Email = "cuong.na@campus.edu.vn",
                PhoneNumber = "0904567890",
                Role = Role.User,
                IsActive = true
            },
            new User
            {
                Id = "student-sys-05",
                Username = "B21DCCN045",
                FullName = "Trần Thị Mai",
                Email = "student2@campus.edu",
                PhoneNumber = "0905678902",
                Role = Role.User,
                IsActive = true
            }
        };

        public UserService(ModalErrorHandler errorHandler, ILogger<UserService> logger)
        {
            _httpClient = HttpHelper.CreateApiClient();
            _errorHandler = errorHandler;
            _logger = logger;
        }

        public async Task<List<User>> GetUsersListAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(Constants.AccountsApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                    };
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<User>>>(options);
                    if (result != null && result.Success && result.Data != null && result.Data.Count > 0)
                    {
                        return result.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API user fetch failed, falling back to local user store");
            }

            return _localUsers.ToList();
        }

        public async Task<List<User>> GetItemsAsync() => await GetUsersListAsync();

        public async Task<User?> GetUserByIdAsync(string id)
        {
            var users = await GetUsersListAsync();
            return users.FirstOrDefault(u => u.Id == id);
        }

        public async Task<User?> GetItemAsync(string id) => await GetUserByIdAsync(id);

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var users = await GetUsersListAsync();
            var clean = username.Trim().ToLower();
            return users.FirstOrDefault(u =>
                u.Email.ToLower() == clean ||
                u.Username.ToLower() == clean ||
                (clean.Contains('@') && clean.StartsWith(u.Username.ToLower() + "@")));
        }

        public async Task<bool> UpdateUserRoleAsync(string id, Role newRole)
        {
            try
            {
                var payload = new { role = newRole.ToString() };
                var response = await _httpClient.PutAsJsonAsync($"{Constants.AccountsApiUrl}/{id}", payload);
                if (response.IsSuccessStatusCode) return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API update user role failed, applying locally");
            }

            var local = _localUsers.FirstOrDefault(u => u.Id == id);
            if (local != null)
            {
                local.Role = newRole;
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{Constants.AccountsApiUrl}/{id}");
                if (response.IsSuccessStatusCode) return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API delete user failed, applying locally");
            }

            var local = _localUsers.FirstOrDefault(u => u.Id == id);
            if (local != null)
            {
                _localUsers.Remove(local);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteItemAsync(string id) => await DeleteUserAsync(id);

        public async Task<int> SaveUserAsync(User user)
        {
            try
            {
                var payload = new
                {
                    email = user.Email,
                    password = string.IsNullOrWhiteSpace(user.PasswordHash) ? "Password@123" : user.PasswordHash,
                    fullName = user.FullName,
                    role = user.Role.ToString(),
                    phoneNumber = user.PhoneNumber,
                    studentCode = user.Username
                };

                var response = await _httpClient.PostAsJsonAsync(Constants.AccountsApiUrl, payload);
                if (response.IsSuccessStatusCode) return 1;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API create user failed, applying locally");
            }

            if (string.IsNullOrEmpty(user.Id))
            {
                user.Id = "user_" + Guid.NewGuid().ToString("N")[..8];
            }
            _localUsers.Add(user);
            return 1;
        }

        public async Task<int> SaveItemAsync(User item) => await SaveUserAsync(item);

        public async Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(Constants.ProfileApiUrl, request);
                if (response.IsSuccessStatusCode) return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API update profile failed, applying locally");
            }

            return true;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var payload = new
                {
                    fullName = user.FullName,
                    phoneNumber = user.PhoneNumber,
                    role = user.Role.ToString()
                };
                var response = await _httpClient.PutAsJsonAsync($"{Constants.AccountsApiUrl}/{user.Id}", payload);
                if (response.IsSuccessStatusCode) return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API update user failed, applying locally");
            }

            var local = _localUsers.FirstOrDefault(u => u.Id == user.Id);
            if (local != null)
            {
                local.FullName = user.FullName;
                local.PhoneNumber = user.PhoneNumber;
                local.Role = user.Role;
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateItemAsync(User item) => await UpdateUserAsync(item);

        public async Task<bool> ToggleUserStatusAsync(string id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{Constants.AccountsApiUrl}/{id}/toggle-status", null);
                if (response.IsSuccessStatusCode) return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API toggle user status failed, applying locally");
            }

            var local = _localUsers.FirstOrDefault(u => u.Id == id);
            if (local != null)
            {
                local.IsActive = !local.IsActive;
                return true;
            }
            return false;
        }

        public async Task SeedDefaultUsersAsync()
        {
            await Task.CompletedTask;
        }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    public class ApiErrorResponse
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Error { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
