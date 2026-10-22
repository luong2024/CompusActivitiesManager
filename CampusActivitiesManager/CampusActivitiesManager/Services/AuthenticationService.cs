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

        public bool IsLecturer => _currentUser?.Role == Role.Lecturer;

        public bool IsSecretary => _currentUser?.Role == Role.Secretary || _currentUser?.IsSecretary == true;

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

            // Tìm trong store người dùng trước để lấy đầy đủ điểm rèn luyện và lớp học
            var localUser = await _userService.GetUserByUsernameAsync(cleanUser);
            if (localUser != null && localUser.IsActive)
            {
                _currentUser = localUser;
                try
                {
                    await SecureStorage.Default.SetAsync(Constants.AuthTokenKey, "local_token_" + Guid.NewGuid().ToString("N"));
                }
                catch { }

                _logger.LogInformation("Authenticated session started for {Username} ({Role})", localUser.Username, localUser.Role);
                CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }

            // Fallback nếu người dùng gõ tên viết tắt
            if (cleanUser == "admin" || cleanUser == "admin@campus.edu.vn" || cleanUser == "admin@campus.edu")
            {
                _currentUser = (await _userService.GetUserByIdAsync("admin-sys-01")) ?? new User
                {
                    Id = "admin-sys-01",
                    Username = "admin",
                    FullName = "Quản Trị Viên Hệ Thống",
                    Email = "admin@campus.edu.vn",
                    Role = Role.Admin,
                    ClassName = "Ban Quản Trị",
                    IsActive = true
                };
                CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            else if (cleanUser == "manager" || cleanUser == "manager@campus.edu.vn")
            {
                _currentUser = (await _userService.GetUserByIdAsync("manager-sys-02")) ?? new User
                {
                    Id = "manager-sys-02",
                    Username = "manager",
                    FullName = "ThS. Nguyễn Văn Quản",
                    Email = "manager@campus.edu.vn",
                    Role = Role.Manager,
                    ClassName = "Khoa CNTT",
                    AssignedClass = "D20CNTT1",
                    IsActive = true
                };
                CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            else if (cleanUser == "giangvien" || cleanUser == "teacher" || cleanUser == "gv" || cleanUser == "giangvien@campus.edu.vn" || cleanUser == "teacher@campus.edu.vn")
            {
                _currentUser = (await _userService.GetUserByIdAsync("lecturer-sys-05")) ?? new User
                {
                    Id = "lecturer-sys-05",
                    Username = "giangvien",
                    FullName = "TS. Trần Văn Minh",
                    Email = "giangvien@campus.edu.vn",
                    Role = Role.Lecturer,
                    ClassName = "Khoa CNTT",
                    AssignedClass = "D21CNTT1",
                    IsActive = true
                };
                CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            else if (cleanUser == "bithu" || cleanUser == "bithu@campus.edu.vn")
            {
                _currentUser = (await _userService.GetUserByIdAsync("secretary-sys-03")) ?? new User
                {
                    Id = "secretary-sys-03",
                    Username = "bithu",
                    FullName = "Lê Hoàng Bí Thư",
                    Email = "bithu@campus.edu.vn",
                    Role = Role.Secretary,
                    IsSecretary = true,
                    ClassName = "D20CNTT1",
                    IsActive = true
                };
                CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            else if (cleanUser == "cuong.na@campus.edu.vn" || cleanUser == "b20dccn001" || cleanUser == "student")
            {
                _currentUser = (await _userService.GetUserByIdAsync("student-sys-04")) ?? new User
                {
                    Id = "student-sys-04",
                    Username = "B20DCCN001",
                    FullName = "Nguyễn An Cương",
                    Email = "cuong.na@campus.edu.vn",
                    Role = Role.User,
                    ClassName = "D20CNTT1",
                    IsActive = true,
                    TrainingPoints = 0
                };
                CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                return true;
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

            if (_currentUser.Role == Role.Admin) return true;

            return _currentUser.Role switch
            {
                Role.Manager => requiredRole is Role.Manager or Role.Lecturer or Role.Secretary or Role.User or Role.Guest,
                Role.Lecturer => requiredRole is Role.Lecturer or Role.User or Role.Guest,
                Role.Secretary => requiredRole is Role.Secretary or Role.User or Role.Guest,
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
