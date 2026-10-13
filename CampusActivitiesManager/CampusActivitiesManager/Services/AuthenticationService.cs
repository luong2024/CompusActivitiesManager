using System.Net.Http.Json;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    /// <summary>
    /// Triển khai dịch vụ xác thực người dùng và quản lý phiên làm việc (RBAC).
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService<User> _userService;
        private readonly ILogger<AuthenticationService> _logger;

        private User? _currentUser;

        public User? CurrentUser => _currentUser;

        public Role CurrentRole => _currentUser?.Role ?? Role.Guest;

        public bool IsAdmin => _currentUser?.Role == Role.Admin;

        public bool IsManager => _currentUser?.Role == Role.Manager;

        public bool IsAuthenticated => _currentUser != null && _currentUser.IsActive;

        public event EventHandler? CurrentUserChanged;

        public AuthenticationService(IUserService<User> userService, ILogger<AuthenticationService> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return false;

                using var httpClient = new System.Net.Http.HttpClient(HttpHelper.GetInsecureHandler());
                var baseUrl = DeviceInfo.Platform == DevicePlatform.Android 
                    ? "https://10.0.2.2:7258/api/v1/auth/login" 
                    : "https://localhost:7258/api/v1/auth/login";

                var email = username.Trim();
                if (!email.Contains('@'))
                {
                    email = $"{email}@campus.edu.vn";
                }

                var loginRequest = new { Email = email, Password = password.Trim() };
                var response = await httpClient.PostAsJsonAsync(baseUrl, loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                    };
                    
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<Models.AuthLoginResponse>>(options);

                    if (result != null && result.Success && result.Data != null)
                    {
                        var loginData = result.Data;
                        Role parsedRole = Role.User;
                        if (string.Equals(loginData.Role, "Lecturer", StringComparison.OrdinalIgnoreCase))
                        {
                            parsedRole = Role.Manager;
                        }
                        else if (Enum.TryParse<Role>(loginData.Role, true, out var r))
                        {
                            parsedRole = r;
                        }

                        var user = new User
                        {
                            Id = loginData.Id,
                            Username = username.Trim(),
                            FullName = loginData.FullName,
                            Email = loginData.Email,
                            Role = parsedRole,
                            IsActive = loginData.IsActive
                        };

                        _currentUser = user;
                        _logger.LogInformation("Đăng nhập thành công qua API: {Email} với vai trò {Role}", user.Email, user.Role);
                        CurrentUserChanged?.Invoke(this, EventArgs.Empty);

                        try
                        {
                            await SecureStorage.Default.SetAsync("api_auth_token", loginData.Token);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "SecureStorage failed, falling back to Preferences");
                            Preferences.Default.Set("api_auth_token", loginData.Token);
                        }

                        return true;
                    }
                }
                else
                {
                    _logger.LogWarning("Đăng nhập thất bại qua API, mã lỗi: {StatusCode}", response.StatusCode);
                    return false;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thực hiện đăng nhập qua API");
                return false;
            }
        }

        public async Task<bool> SwitchUserAsync(string username)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAsync(username);
                if (user != null)
                {
                    _currentUser = user;
                    CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chuyển đổi người dùng sang {Username}", username);
                return false;
            }
        }

        public void Logout()
        {
            _currentUser = null;
            SecureStorage.Default.Remove("api_auth_token");
            Preferences.Default.Remove("api_auth_token");
            _logger.LogInformation("Người dùng đã đăng xuất");
            CurrentUserChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CheckPermission(Role requiredRole)
        {
            if (_currentUser == null)
                return requiredRole == Role.Guest;

            return _currentUser.Role switch
            {
                Role.Admin => true,
                Role.Manager => requiredRole is Role.Manager or Role.User or Role.Guest,
                Role.User => requiredRole is Role.User or Role.Guest,
                Role.Guest => requiredRole == Role.Guest,
                _ => false
            };
        }

        public async Task RefreshCurrentUserAsync()
        {
            try
            {
                if (_currentUser == null || string.IsNullOrEmpty(_currentUser.Id))
                    return;

                var refreshedUser = await _userService.GetUserByIdAsync(_currentUser.Id);
                if (refreshedUser != null)
                {
                    _currentUser = refreshedUser;
                    _logger.LogInformation("Đã làm mới phiên làm việc cho {Username}, vai trò mới: {Role}", _currentUser.Username, _currentUser.Role);
                    CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi làm mới phiên người dùng");
            }
        }
    }
}
