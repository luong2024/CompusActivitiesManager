using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    public class UserService : IUserService, IDataStore<User>
    {
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
            _errorHandler = errorHandler;
            _logger = logger;
        }

        public async Task<List<User>> GetUsersListAsync()
        {
            await Task.CompletedTask;
            return _localUsers.ToList();
        }

        public async Task<List<User>> GetItemsAsync() => await GetUsersListAsync();

        public async Task<User?> GetUserByIdAsync(string id)
        {
            await Task.CompletedTask;
            return _localUsers.FirstOrDefault(u => u.Id == id);
        }

        public async Task<User?> GetItemAsync(string id) => await GetUserByIdAsync(id);

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            await Task.CompletedTask;
            var clean = username.Trim().ToLower();
            return _localUsers.FirstOrDefault(u =>
                u.Email.ToLower() == clean ||
                u.Username.ToLower() == clean ||
                (clean.Contains('@') && clean.StartsWith(u.Username.ToLower() + "@")));
        }

        public async Task<bool> UpdateUserRoleAsync(string id, Role newRole)
        {
            await Task.CompletedTask;
            var local = _localUsers.FirstOrDefault(u => u.Id == id);
            if (local != null)
            {
                local.Role = newRole;
                _logger.LogInformation("Updated role for user {Id} to {Role}", id, newRole);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            await Task.CompletedTask;
            var local = _localUsers.FirstOrDefault(u => u.Id == id);
            if (local != null)
            {
                _localUsers.Remove(local);
                _logger.LogInformation("Deleted user {Id}", id);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteItemAsync(string id) => await DeleteUserAsync(id);

        public async Task<int> SaveUserAsync(User user)
        {
            await Task.CompletedTask;
            if (string.IsNullOrEmpty(user.Id))
            {
                user.Id = "user_" + Guid.NewGuid().ToString("N")[..8];
            }

            var existing = _localUsers.FirstOrDefault(u => u.Id == user.Id);
            if (existing != null)
            {
                existing.FullName = user.FullName;
                existing.Email = user.Email;
                existing.PhoneNumber = user.PhoneNumber;
                existing.Role = user.Role;
                existing.IsActive = user.IsActive;
            }
            else
            {
                _localUsers.Add(user);
            }
            _logger.LogInformation("Saved user {Username} ({Id})", user.Username, user.Id);
            return 1;
        }

        public async Task<int> SaveItemAsync(User item) => await SaveUserAsync(item);

        public async Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
        {
            await Task.CompletedTask;
            var local = _localUsers.FirstOrDefault(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
            if (local != null)
            {
                local.FullName = request.FullName;
                local.PhoneNumber = request.PhoneNumber;
                return true;
            }
            return true;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            await Task.CompletedTask;
            var local = _localUsers.FirstOrDefault(u => u.Id == user.Id);
            if (local != null)
            {
                local.FullName = user.FullName;
                local.PhoneNumber = user.PhoneNumber;
                local.Role = user.Role;
                local.Email = user.Email;
                local.IsActive = user.IsActive;
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateItemAsync(User item) => await UpdateUserAsync(item);

        public async Task<bool> ToggleUserStatusAsync(string id)
        {
            await Task.CompletedTask;
            var local = _localUsers.FirstOrDefault(u => u.Id == id);
            if (local != null)
            {
                local.IsActive = !local.IsActive;
                _logger.LogInformation("Toggled active status for user {Id} to {Status}", id, local.IsActive);
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
