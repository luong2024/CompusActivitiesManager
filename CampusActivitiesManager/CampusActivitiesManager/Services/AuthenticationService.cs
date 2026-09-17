using System.Net.Http.Json;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    /// <summary>
    /// Triển khai dịch vụ xác thực và quản lý phiên người dùng (Session Management và RBAC Guard).
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

            // Mặc định khởi tạo tài khoản Admin cho phiên làm việc ban đầu
            _ = InitializeDefaultSessionAsync();
        }

        private async Task InitializeDefaultSessionAsync()
        {
            try
            {
                var admin = await _userService.GetUserByUsernameAsync("admin");
                if (admin != null)
                {
                    _currentUser = admin;
                    CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi khởi tạo phiên làm việc mặc định");
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return false;

                // T28.4: K?t n?i giao di?n dang nh?p v?i API
                using var httpClient = new System.Net.Http.HttpClient();
                var baseUrl = DeviceInfo.Platform == DevicePlatform.Android 
                    ? "http://10.0.2.2:5073/api/v1/auth/login" 
                    : "http://localhost:5073/api/v1/auth/login";

                // Map username to email if it doesn't contain '@'
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
                        var user = new User
                        {
                            Id = loginData.Id,
                            Username = username.Trim(),
                            FullName = loginData.FullName,
                            Email = loginData.Email,
                            Role = Enum.TryParse<Role>(loginData.Role, true, out var r) ? r : Role.User,
                            IsActive = loginData.IsActive
                        };

                        _currentUser = user;
                        _logger.LogInformation("Dang nhap thanh cong qua API: {Email} voi vai tro {Role}", user.Email, user.Role);
                        CurrentUserChanged?.Invoke(this, EventArgs.Empty);

                        // L?u token vao SecureStorage (T28.3 & T28.4)
                        await SecureStorage.Default.SetAsync("api_auth_token", loginData.Token);

                        return true;
                    }
                }
                else
                {
                    _logger.LogWarning("Dang nhap that bai qua API, ma loi: {StatusCode}", response.StatusCode);
                    return false;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi thuc hien dang nhap qua API");
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
            _logger.LogInformation("Người dùng đã đăng xuất");
            CurrentUserChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Kiểm tra cấp bậc phân quyền hiện tại với quyền tối thiểu cần để thực thi tác vụ/truy cập trang.
        /// Cấp bậc: Admin (toàn quyền) >= Manager >= User >= Guest.
        /// </summary>
        public bool CheckPermission(Role requiredRole)
        {
            if (_currentUser == null)
                return requiredRole == Role.Guest;

            return _currentUser.Role switch
            {
                Role.Admin => true, // Admin luôn có toàn quyền
                Role.Manager => requiredRole is Role.Manager or Role.User or Role.Guest,
                Role.User => requiredRole is Role.User or Role.Guest,
                Role.Guest => requiredRole == Role.Guest,
                _ => false
            };
        }

        /// <summary>
        /// Đồng bộ lại thông tin tài khoản hiện tại từ CSDL (khi bị đổi quyền hoặc đổi thông tin).
        /// </summary>
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
