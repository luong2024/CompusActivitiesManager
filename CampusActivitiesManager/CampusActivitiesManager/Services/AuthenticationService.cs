using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
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
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            var cleanUser = username.Trim().ToLower();

            // 1. Quản trị viên (Admin)
            if (cleanUser == "admin" || cleanUser == "admin@campus.edu.vn" || cleanUser == "admin@campus.edu")
            {
                _currentUser = new User
                {
                    Id = "admin-sys-01",
                    Username = "admin",
                    FullName = "Quản Trị Viên Hệ Thống (Admin)",
                    Email = "admin@campus.edu.vn",
                    Role = Role.Admin,
                    IsActive = true
                };
                try
                {
                    await SecureStorage.Default.SetAsync(Constants.AuthTokenKey, "local_admin_token_" + Guid.NewGuid().ToString("N"));
                }
                catch { }

                _logger.LogInformation("Authenticated session started for Admin");
                CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            // 2. Ban Quản Lý Hoạt Động (Manager)
            else if (cleanUser == "manager" || cleanUser == "manager@campus.edu.vn" || cleanUser == "manager@campus.edu")
            {
                _currentUser = new User
                {
                    Id = "manager-sys-02",
                    Username = "manager",
                    FullName = "Ban Quản Lý Hoạt Động (Manager)",
                    Email = "manager@campus.edu.vn",
                    Role = Role.Manager,
                    IsActive = true
                };
                try
                {
                    await SecureStorage.Default.SetAsync(Constants.AuthTokenKey, "local_manager_token_" + Guid.NewGuid().ToString("N"));
                }
                catch { }

                _logger.LogInformation("Authenticated session started for Manager");
                CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            // 3. Sinh viên mẫu (Student)
            else if (cleanUser == "cuong.na@campus.edu.vn" || cleanUser == "b20dccn001" || cleanUser == "b20dccn001@campus.edu.vn" || cleanUser == "student" || cleanUser == "student1@campus.edu")
            {
                _currentUser = new User
                {
                    Id = "student-sys-04",
                    Username = "B20DCCN001",
                    FullName = "Nguyễn An Cương",
                    Email = "cuong.na@campus.edu.vn",
                    Role = Role.User,
                    IsActive = true
                };
                try
                {
                    await SecureStorage.Default.SetAsync(Constants.AuthTokenKey, "local_student_token_" + Guid.NewGuid().ToString("N"));
                }
                catch { }

                _logger.LogInformation("Authenticated session started for Student");
                CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            // 4. Tài khoản lưu trong danh sách người dùng nội bộ
            else
            {
                try
                {
                    var localUser = await _userService.GetUserByUsernameAsync(username.Trim());
                    if (localUser != null && localUser.IsActive)
                    {
                        _currentUser = localUser;
                        try
                        {
                            await SecureStorage.Default.SetAsync(Constants.AuthTokenKey, "local_user_token_" + Guid.NewGuid().ToString("N"));
                        }
                        catch { }

                        _logger.LogInformation("Authenticated session started for {Username}", localUser.Username);
                        CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error checking user store for {Username}", username);
                }
            }

            return false;
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
                _logger.LogError(ex, "Error switching current user");
                return false;
            }
        }

        public void Logout()
        {
            _currentUser = null;
            try
            {
                SecureStorage.Default.Remove(Constants.AuthTokenKey);
            }
            catch { }
            _logger.LogInformation("User session ended");
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
                    CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing current user session");
            }
            await Task.CompletedTask;
        }
    }
}
