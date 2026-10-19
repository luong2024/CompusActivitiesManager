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
                    if (result != null && result.Success && result.Data != null)
                    {
                        return result.Data;
                    }
                }
                return new List<User>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user list from API");
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

        public async Task<bool> UpdateUserRoleAsync(string id, Role newRole)
        {
            try
            {
                var payload = new { role = newRole.ToString() };
                var response = await _httpClient.PutAsJsonAsync($"{Constants.AccountsApiUrl}/{id}", payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role via API");
                _errorHandler.HandleError(ex);
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{Constants.AccountsApiUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user via API");
                _errorHandler.HandleError(ex);
                return false;
            }
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
                return response.IsSuccessStatusCode ? 1 : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user via API");
                _errorHandler.HandleError(ex);
                return 0;
            }
        }

        public async Task<int> SaveItemAsync(User item) => await SaveUserAsync(item);

        public async Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(Constants.ProfileApiUrl, request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile via API");
                _errorHandler.HandleError(ex);
                return false;
            }
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
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user via API");
                _errorHandler.HandleError(ex);
                return false;
            }
        }

        public async Task<bool> UpdateItemAsync(User item) => await UpdateUserAsync(item);

        public async Task<bool> ToggleUserStatusAsync(string id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{Constants.AccountsApiUrl}/{id}/toggle-status", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user status via API");
                _errorHandler.HandleError(ex);
                return false;
            }
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
