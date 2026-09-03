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
                if (string.IsNullOrWhiteSpace(username))
                    return false;

                var user = await _userService.GetUserByUsernameAsync(username.Trim());
                if (user == null)
                {
                    _logger.LogWarning("Đăng nhập thất bại: Không tìm thấy tài khoản {Username}", username);
                    return false;
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Đăng nhập thất bại: Tài khoản {Username} đã bị khóa", username);
                    return false;
                }

                // Kiểm tra mật khẩu (hỗ trợ pass 123 hoặc đúng với passwordHash)
                if (!string.IsNullOrEmpty(user.PasswordHash) && !string.IsNullOrEmpty(password) && user.PasswordHash != password && password != "123")
                {
                    _logger.LogWarning("Đăng nhập thất bại: Sai mật khẩu cho {Username}", username);
                    return false;
                }

                _currentUser = user;
                _logger.LogInformation("Đăng nhập thành công: {Username} với vai trò {Role}", user.Username, user.Role);
                CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thực hiện đăng nhập");
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
