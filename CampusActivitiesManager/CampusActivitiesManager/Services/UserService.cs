using System.Text.Json;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    public class UserService : IUserService, IDataStore<User>
    {
        private readonly ModalErrorHandler _errorHandler;
        private readonly ILogger<UserService> _logger;

        private static readonly string UsersFilePath = Path.Combine(FileSystem.AppDataDirectory, "campus_users.json");
        private static readonly string ClassesFilePath = Path.Combine(FileSystem.AppDataDirectory, "campus_classes.json");
        private static readonly SemaphoreSlim _fileLock = new(1, 1);
        private static bool _isLoadedFromFile = false;
        private static bool _isClassesLoaded = false;

        private static readonly List<ClassRoom> _classes = new()
        {
            new ClassRoom
            {
                ClassName = "D20CNTT1",
                Department = "Khoa Công nghệ Thông tin",
                AdvisorName = "ThS. Nguyễn Văn Quản",
                AdvisorUsername = "manager",
                SecretaryName = "Lê Hoàng Bí Thư",
                SecretaryUsername = "bithu",
                AcademicYear = "K20 (2020 - 2025)",
                StudentCount = 45,
                Description = "Lớp Kỹ thuật Phần mềm & Trí tuệ Nhân tạo D20"
            },
            new ClassRoom
            {
                ClassName = "D21CNTT1",
                Department = "Khoa Công nghệ Thông tin",
                AdvisorName = "TS. Trần Văn Minh",
                AdvisorUsername = "giangvien",
                SecretaryName = "Trần Thị Mai",
                SecretaryUsername = "B21DCCN045",
                AcademicYear = "K21 (2021 - 2026)",
                StudentCount = 50,
                Description = "Lớp Hệ thống Thông tin & An toàn Mạng D21"
            },
            new ClassRoom
            {
                ClassName = "D22QTKD1",
                Department = "Khoa Quản trị Kinh doanh",
                AdvisorName = "TS. Trần Văn Minh",
                AdvisorUsername = "giangvien",
                SecretaryName = "Hoàng Kim Ngân",
                SecretaryUsername = "B22DCCN001",
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

        private static async Task EnsureLoadedAsync()
        {
            if (_isLoadedFromFile) return;

            await _fileLock.WaitAsync();
            try
            {
                if (_isLoadedFromFile) return;

                if (File.Exists(UsersFilePath))
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(UsersFilePath);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            var list = JsonSerializer.Deserialize<List<User>>(json);
                            if (list != null && list.Count > 0)
                            {
                                _localUsers.Clear();
                                _localUsers.AddRange(list);
                            }
                        }
                    }
                    catch { }
                }
                else
                {
                    await SaveUsersToDiskInternalAsync();
                }

                _isLoadedFromFile = true;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        private static async Task SaveUsersToDiskInternalAsync()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_localUsers, options);
                await File.WriteAllTextAsync(UsersFilePath, json);
            }
            catch { }
        }

        public async Task SaveUsersToDiskAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                await SaveUsersToDiskInternalAsync();
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task<List<User>> GetUsersListAsync()
        {
            await EnsureLoadedAsync();
            return _localUsers.ToList();
        }

        public async Task<List<User>> GetItemsAsync() => await GetUsersListAsync();

        public async Task<User?> GetUserByIdAsync(string id)
        {
            await EnsureLoadedAsync();
            return _localUsers.FirstOrDefault(u => u.Id == id);
        }

        public async Task<User?> GetItemAsync(string id) => await GetUserByIdAsync(id);

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            await EnsureLoadedAsync();
            var clean = username.Trim().ToLower();
            return _localUsers.FirstOrDefault(u =>
                u.Email.ToLower() == clean ||
                u.Username.ToLower() == clean ||
                (clean.Contains('@') && clean.StartsWith(u.Username.ToLower() + "@")));
        }

        public async Task<List<User>> GetStudentsByClassAsync(string className)
        {
            await EnsureLoadedAsync();
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
            await EnsureLoadedAsync();
            var user = _localUsers.FirstOrDefault(u =>
                u.Id.Equals(studentCodeOrId, StringComparison.OrdinalIgnoreCase) ||
                u.Username.Equals(studentCodeOrId, StringComparison.OrdinalIgnoreCase));

            if (user != null)
            {
                user.TrainingPoints = Math.Max(0, user.TrainingPoints + pointsDelta);
                _logger.LogInformation("Adjusted training points for {Username} by {Delta}. New points: {Points}", user.Username, pointsDelta, user.TrainingPoints);
                await SaveUsersToDiskAsync();
                return true;
            }
            return false;
        }

        private static async Task EnsureClassesLoadedAsync()
        {
            if (_isClassesLoaded) return;
            try
            {
                if (File.Exists(ClassesFilePath))
                {
                    var json = await File.ReadAllTextAsync(ClassesFilePath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var list = JsonSerializer.Deserialize<List<ClassRoom>>(json);
                        if (list != null && list.Count > 0)
                        {
                            _classes.Clear();
                            _classes.AddRange(list);
                        }
                    }
                }
                else
                {
                    await SaveClassesToDiskInternalAsync();
                }
                _isClassesLoaded = true;
            }
            catch { }
        }

        private static async Task SaveClassesToDiskInternalAsync()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_classes, options);
                await File.WriteAllTextAsync(ClassesFilePath, json);
            }
            catch { }
        }

        public async Task<List<ClassRoom>> GetClassesAsync()
        {
            await EnsureClassesLoadedAsync();
            return _classes.ToList();
        }

        public async Task<bool> AddClassAsync(ClassRoom classRoom)
        {
            if (classRoom == null || string.IsNullOrWhiteSpace(classRoom.ClassName)) return false;
            await EnsureClassesLoadedAsync();
            var existing = _classes.FirstOrDefault(c => string.Equals(c.ClassName, classRoom.ClassName.Trim(), StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Department = classRoom.Department;
                existing.AdvisorName = classRoom.AdvisorName;
                existing.AdvisorUsername = classRoom.AdvisorUsername;
                existing.SecretaryName = classRoom.SecretaryName;
                existing.SecretaryUsername = classRoom.SecretaryUsername;
                existing.AcademicYear = classRoom.AcademicYear;
                existing.Description = classRoom.Description;
            }
            else
            {
                _classes.Add(classRoom);
            }

            if (!string.IsNullOrWhiteSpace(classRoom.AdvisorUsername))
            {
                var lecturer = _localUsers.FirstOrDefault(u => string.Equals(u.Username, classRoom.AdvisorUsername, StringComparison.OrdinalIgnoreCase));
                if (lecturer != null)
                {
                    lecturer.AddManagedClass(classRoom.ClassName);
                    await SaveUsersToDiskAsync();
                }
            }

            await SaveClassesToDiskInternalAsync();
            return true;
        }

        public async Task<bool> AssignLecturerToClassAsync(string className, string lecturerUsername, string lecturerName)
        {
            await EnsureClassesLoadedAsync();
            var targetClass = _classes.FirstOrDefault(c => string.Equals(c.ClassName, className.Trim(), StringComparison.OrdinalIgnoreCase));
            if (targetClass == null) return false;

            targetClass.AdvisorUsername = lecturerUsername;
            targetClass.AdvisorName = lecturerName;

            var lecturer = _localUsers.FirstOrDefault(u => string.Equals(u.Username, lecturerUsername, StringComparison.OrdinalIgnoreCase));
            if (lecturer != null)
            {
                lecturer.AddManagedClass(className);
                await SaveUsersToDiskAsync();
            }

            await SaveClassesToDiskInternalAsync();
            return true;
        }

        public async Task<bool> DeleteClassAsync(string className)
        {
            await EnsureClassesLoadedAsync();
            var target = _classes.FirstOrDefault(c => string.Equals(c.ClassName, className.Trim(), StringComparison.OrdinalIgnoreCase));
            if (target != null)
            {
                _classes.Remove(target);
                await SaveClassesToDiskInternalAsync();
                return true;
            }
            return false;
        }

        public async Task<List<User>> GetLecturersAsync()
        {
            await EnsureLoadedAsync();
            return _localUsers.Where(u => u.Role == Role.Lecturer || u.Role == Role.Manager).ToList();
        }

        public async Task<bool> UpdateUserRoleAsync(string id, Role newRole)
        {
            await EnsureLoadedAsync();
            var local = _localUsers.FirstOrDefault(u => u.Id == id);
            if (local != null)
            {
                local.Role = newRole;
                local.IsSecretary = (newRole == Role.Secretary);
                _logger.LogInformation("Updated role for user {Id} to {Role}", id, newRole);
                await SaveUsersToDiskAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            await EnsureLoadedAsync();
            var local = _localUsers.FirstOrDefault(u => u.Id == id);
            if (local != null)
            {
                _localUsers.Remove(local);
                _logger.LogInformation("Deleted user {Id}", id);
                await SaveUsersToDiskAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteItemAsync(string id) => await DeleteUserAsync(id);

        public async Task<int> SaveUserAsync(User user)
        {
            await EnsureLoadedAsync();
            if (string.IsNullOrEmpty(user.Id))
            {
                user.Id = "user_" + Guid.NewGuid().ToString("N")[..8];
            }

            var existing = _localUsers.FirstOrDefault(u => u.Id == user.Id || u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.FullName = user.FullName;
                existing.Email = user.Email;
                existing.PhoneNumber = user.PhoneNumber;
                existing.Role = user.Role;
                existing.ClassName = user.ClassName;
                existing.IsSecretary = user.IsSecretary;
                existing.IsActive = user.IsActive;
                existing.TrainingPoints = user.TrainingPoints;
            }
            else
            {
                _localUsers.Add(user);
            }
            _logger.LogInformation("Saved user {Username} ({Id})", user.Username, user.Id);
            await SaveUsersToDiskAsync();
            return 1;
        }

        public async Task<int> SaveItemAsync(User item) => await SaveUserAsync(item);

        public async Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
        {
            await EnsureLoadedAsync();
            var local = _localUsers.FirstOrDefault(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
            if (local != null)
            {
                local.FullName = request.FullName;
                local.PhoneNumber = request.PhoneNumber;
                await SaveUsersToDiskAsync();
                return true;
            }
            return true;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            await EnsureLoadedAsync();
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
                local.TrainingPoints = user.TrainingPoints;
                await SaveUsersToDiskAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateItemAsync(User item) => await UpdateUserAsync(item);

        public async Task<bool> ToggleUserStatusAsync(string id)
        {
            await EnsureLoadedAsync();
            var local = _localUsers.FirstOrDefault(u => u.Id == id);
            if (local != null)
            {
                local.IsActive = !local.IsActive;
                _logger.LogInformation("Toggled active status for user {Id} to {Status}", id, local.IsActive);
                await SaveUsersToDiskAsync();
                return true;
            }
            return false;
        }

        public async Task SeedDefaultUsersAsync()
        {
            await EnsureLoadedAsync();
        }
    }
}
