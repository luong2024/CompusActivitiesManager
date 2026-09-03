using System.Net.Http.Json;
using System.Text.Json;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    public class UserService : IUserService, IDataStore<User>
    {
        private readonly HttpClient _httpClient;
        private readonly ModalErrorHandler _errorHandler;
        private readonly ILogger<UserService> _logger;
        
        // Use 10.0.2.2 for Android Emulator, localhost for Windows
        private readonly string _baseUrl = DeviceInfo.Platform == DevicePlatform.Android 
            ? "http://10.0.2.2:5073/api/v1/accounts" 
            : "http://localhost:5073/api/v1/accounts";

        public UserService(ModalErrorHandler errorHandler, ILogger<UserService> logger)
        {
            _httpClient = new HttpClient();
            _errorHandler = errorHandler;
            _logger = logger;
        }

        public async Task<List<User>> GetUsersListAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                    };
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<User>>>(options);
                    if (result != null && result.Success && result.Data != null)
                    {
                        return result.Data;
                    }
                }
                return new List<User>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi API lấy danh sách người dùng");
                _errorHandler.HandleError(ex);
                return new List<User>();
            }
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
            return users.FirstOrDefault(u => u.Email == username || u.Username == username);
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

                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{user.Id}", payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật người dùng qua API");
                _errorHandler.HandleError(ex);
                return false;
            }
        }

        public async Task<bool> UpdateUserRoleAsync(string id, Role newRole)
        {
            try
            {
                var payload = new { role = newRole.ToString() };
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{id}", payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật vai trò người dùng");
                _errorHandler.HandleError(ex);
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            // API doesn't have delete yet, just return false
            return await Task.FromResult(false);
        }

        public async Task<bool> DeleteItemAsync(string id) => await DeleteUserAsync(id);

        public async Task<int> SaveUserAsync(User user)
        {
            try
            {
                var payload = new 
                {
                    email = user.Email,
                    password = "Password@123", // Default password for new users if not set
                    fullName = user.FullName,
                    role = user.Role.ToString(),
                    phoneNumber = user.PhoneNumber,
                    studentCode = user.Username
                };
                
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, payload);
                return response.IsSuccessStatusCode ? 1 : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu người dùng qua API");
                _errorHandler.HandleError(ex);
                return 0;
            }
        }

        public async Task<int> SaveItemAsync(User item) => await SaveUserAsync(item);

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
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{user.Id}", payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật người dùng qua API");
                _errorHandler.HandleError(ex);
                return false;
            }
        }

        public async Task<bool> UpdateItemAsync(User item) => await UpdateUserAsync(item);

        public async Task<bool> ToggleUserStatusAsync(string id)
        {
            try
            {
                // Gọi POST /api/v1/accounts/{id}/toggle-status
                var response = await _httpClient.PostAsync($"{_baseUrl}/{id}/toggle-status", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đổi trạng thái người dùng qua API");
                _errorHandler.HandleError(ex);
                return false;
            }
        }

        public async Task SeedDefaultUsersAsync()
        {
            // Do nothing, data is on Firebase
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
}
