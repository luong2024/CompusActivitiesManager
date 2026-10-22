using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    public class UserService : IUserService, IDataStore<User>
    {
        private readonly ModalErrorHandler _errorHandler;
        private readonly ILogger<UserService> _logger;

        private static readonly List<ClassRoom> _classes = new()
        {
            new ClassRoom
            {
                ClassName = "D20CNTT1",
                Department = "Khoa Công nghệ Thông tin",
                AdvisorName = "ThS. Nguyễn Văn Quản",
                SecretaryName = "Lê Hoàng Bí Thư",
                AcademicYear = "K20 (2020 - 2025)",
                StudentCount = 45,
                Description = "Lớp Kỹ thuật Phần mềm & Trí tuệ Nhân tạo D20"
            },
            new ClassRoom
            {
                ClassName = "D21CNTT1",
                Department = "Khoa Công nghệ Thông tin",
                AdvisorName = "TS. Trần Văn Minh",
                SecretaryName = "Trần Thị Mai",
                AcademicYear = "K21 (2021 - 2026)",
                StudentCount = 50,
                Description = "Lớp Hệ thống Thông tin & An toàn Mạng D21"
            },
            new ClassRoom
            {
                ClassName = "D22QTKD1",
                Department = "Khoa Quản trị Kinh doanh",
                AdvisorName = "ThS. Vũ Thị Hương",
                SecretaryName = "Hoàng Kim Ngân",
                AcademicYear = "K22 (2022 - 2027)",
                StudentCount = 52,
                Description = "Lớp Quản trị Doanh nghiệp & Marketing D22"
            }
        };

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
                ClassName = "Ban Quản Trị",
                IsActive = true,
                TrainingPoints = 0
            },
            new User
            {
                Id = "manager-sys-02",
                Username = "manager",
                FullName = "ThS. Nguyễn Văn Quản",
                Email = "manager@campus.edu.vn",
                PhoneNumber = "0902345678",
                Role = Role.Manager,
                ClassName = "Khoa CNTT",
                AssignedClass = "D20CNTT1",
                IsActive = true,
                TrainingPoints = 0
            },
            new User
            {
                Id = "lecturer-sys-05",
                Username = "giangvien",
                FullName = "TS. Trần Văn Minh",
                Email = "giangvien@campus.edu.vn",
                PhoneNumber = "0906789012",
                Role = Role.Lecturer,
                ClassName = "Khoa CNTT",
                AssignedClass = "D21CNTT1",
                IsActive = true,
                TrainingPoints = 0
            },
            new User
            {
                Id = "secretary-sys-03",
                Username = "bithu",
                FullName = "Lê Hoàng Bí Thư",
                Email = "bithu@campus.edu.vn",
                PhoneNumber = "0903456789",
                Role = Role.Secretary,
                IsSecretary = true,
                ClassName = "D20CNTT1",
                IsActive = true,
                TrainingPoints = 0
            },
            new User
            {
                Id = "student-sys-04",
                Username = "B20DCCN001",
                FullName = "Nguyễn An Cương",
                Email = "cuong.na@campus.edu.vn",
                PhoneNumber = "0904567890",
                Role = Role.User,
                ClassName = "D20CNTT1",
                IsActive = true,
                TrainingPoints = 0
            },
            new User
            {
                Id = "student-sys-05",
                Username = "B20DCCN002",
                FullName = "Phạm Minh Hoàng",
                Email = "hoang.pm@campus.edu.vn",
                PhoneNumber = "0905678901",
                Role = Role.User,
                ClassName = "D20CNTT1",
                IsActive = true,
                TrainingPoints = 0
            },
            new User
            {
                Id = "student-sys-06",
                Username = "B21DCCN045",
                FullName = "Trần Thị Mai",
                Email = "student2@campus.edu",
                PhoneNumber = "0905678902",
                Role = Role.User,
                ClassName = "D21CNTT1",
                IsActive = true,
                TrainingPoints = 0
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

        public async Task<List<User>> GetStudentsByClassAsync(string className)
        {
            await Task.CompletedTask;
            if (string.IsNullOrWhiteSpace(className) || className == "Tất cả")
            {
                return _localUsers.Where(u => u.Role == Role.User || u.Role == Role.Secretary).ToList();
            }

            return _localUsers.Where(u =>
                (u.Role == Role.User || u.Role == Role.Secretary) &&
                string.Equals(u.ClassName, className.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<bool> AdjustTrainingPointsAsync(string studentCodeOrId, int pointsDelta)
        {
            await Task.CompletedTask;
            var user = _localUsers.FirstOrDefault(u =>
                u.Id.Equals(studentCodeOrId, StringComparison.OrdinalIgnoreCase) ||
                u.Username.Equals(studentCodeOrId, StringComparison.OrdinalIgnoreCase));

            if (user != null)
            {
                user.TrainingPoints = Math.Max(0, user.TrainingPoints + pointsDelta);
                _logger.LogInformation("Adjusted training points for {Username} by {Delta}. New points: {Points}", user.Username, pointsDelta, user.TrainingPoints);
                return true;
            }
            return false;
        }

        public async Task<List<ClassRoom>> GetClassesAsync()
        {
            await Task.CompletedTask;
            return _classes.ToList();
        }

        public async Task<bool> UpdateUserRoleAsync(string id, Role newRole)
        {
            await Task.CompletedTask;
            var local = _localUsers.FirstOrDefault(u => u.Id == id);
            if (local != null)
            {
                local.Role = newRole;
                local.IsSecretary = (newRole == Role.Secretary);
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
                existing.ClassName = user.ClassName;
                existing.IsSecretary = user.IsSecretary;
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
                local.ClassName = user.ClassName;
                local.IsSecretary = user.IsSecretary;
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
}
