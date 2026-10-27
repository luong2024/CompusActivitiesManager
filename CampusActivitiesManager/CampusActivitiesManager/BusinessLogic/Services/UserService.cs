using CampusActivitiesManager.BusinessLogic.Interfaces;
using System.Text.Json;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.BusinessLogic.Services
{
    public class UserService : IUserService, IDataStore<User>
    {
        private readonly ModalErrorHandler _errorHandler;
        private readonly ILogger<UserService> _logger;

        private static readonly string UsersFilePath = Path.Combine(FileSystem.AppDataDirectory, "campus_users.json");
        private static readonly string ClassesFilePath = Path.Combine(FileSystem.AppDataDirectory, "campus_classes.json");
        private static readonly string PasswordResetsFilePath = Path.Combine(FileSystem.AppDataDirectory, "campus_password_resets.json");
        private static readonly string PointAuditsFilePath = Path.Combine(FileSystem.AppDataDirectory, "campus_point_audits.json");

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
                Password = "Admin@123",
                PasswordHash = "Admin@123",
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
                Password = "Manager@123",
                PasswordHash = "Manager@123",
                ClassName = "Khoa CNTT",
                AssignedClass = "D20CNTT1",
                ManagedClasses = "D20CNTT1",
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
                Password = "Gv@123",
                PasswordHash = "Gv@123",
                ClassName = "Khoa CNTT",
                AssignedClass = "D21CNTT1",
                ManagedClasses = "D21CNTT1, D22QTKD1",
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
                Password = "Bithu@123",
                PasswordHash = "Bithu@123",
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
                Password = "Student@123",
                PasswordHash = "Student@123",
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
                Password = "Student@123",
                PasswordHash = "Student@123",
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
                Password = "Student@123",
                PasswordHash = "Student@123",
                ClassName = "D21CNTT1",
                IsActive = true,
                TrainingPoints = 0
            }
        };

        private static readonly List<PasswordResetRequest> _localResetRequests = new();
        private static readonly List<PointAuditRecord> _localPointAudits = new();

        private readonly IFirebaseSyncService? _firebaseSync;

        public UserService(ModalErrorHandler errorHandler, ILogger<UserService> logger, IFirebaseSyncService? firebaseSync = null)
        {
            _errorHandler = errorHandler;
            _logger = logger;
            _firebaseSync = firebaseSync;
        }

        private async Task EnsureLoadedAsync()
        {
            if (_isLoadedFromFile) return;

            await _fileLock.WaitAsync();
            try
            {
                if (_isLoadedFromFile) return;

                // 1. Tải danh sách người dùng
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
                    // Fresh install: Khôi phục từ Firebase Cloud Database
                    bool cloudRestored = false;
                    if (_firebaseSync != null && _firebaseSync.IsConfigured)
                    {
                        try
                        {
                            var cloudUsers = await _firebaseSync.PullUsersAsync();
                            if (cloudUsers != null && cloudUsers.Count > 0)
                            {
                                _localUsers.Clear();
                                _localUsers.AddRange(cloudUsers);
                                cloudRestored = true;
                            }
                        }
                        catch { }
                    }

                    await SaveUsersToDiskInternalAsync();
                    if (!cloudRestored && _firebaseSync != null && _firebaseSync.IsConfigured)
                    {
                        _ = Task.Run(async () => await _firebaseSync.PushUsersAsync(_localUsers));
                    }
                }

                // Đảm bảo mật khẩu không bị trống
                foreach (var u in _localUsers)
                {
                    if (string.IsNullOrWhiteSpace(u.Password))
                    {
                        u.Password = !string.IsNullOrWhiteSpace(u.PasswordHash) ? u.PasswordHash : u.Role.GetDefaultPassword();
                        u.PasswordHash = u.Password;
                    }
                }

                // 2. Tải danh sách yêu cầu reset mật khẩu
                if (File.Exists(PasswordResetsFilePath))
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(PasswordResetsFilePath);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            var list = JsonSerializer.Deserialize<List<PasswordResetRequest>>(json);
                            if (list != null)
                            {
                                _localResetRequests.Clear();
                                _localResetRequests.AddRange(list);
                            }
                        }
                    }
                    catch { }
                }
                else if (_firebaseSync != null && _firebaseSync.IsConfigured)
                {
                    try
                    {
                        var cloudResets = await _firebaseSync.PullPasswordResetRequestsAsync();
                        if (cloudResets != null && cloudResets.Count > 0)
                        {
                            _localResetRequests.Clear();
                            _localResetRequests.AddRange(cloudResets);
                        }
                    }
                    catch { }
                }

                // 3. Tải nhật ký cộng/trừ điểm rèn luyện
                if (File.Exists(PointAuditsFilePath))
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(PointAuditsFilePath);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            var list = JsonSerializer.Deserialize<List<PointAuditRecord>>(json);
                            if (list != null)
                            {
                                _localPointAudits.Clear();
                                _localPointAudits.AddRange(list);
                            }
                        }
                    }
                    catch { }
                }
                else if (_firebaseSync != null && _firebaseSync.IsConfigured)
                {
                    try
                    {
                        var cloudAudits = await _firebaseSync.PullPointAuditsAsync();
                        if (cloudAudits != null && cloudAudits.Count > 0)
                        {
                            _localPointAudits.Clear();
                            _localPointAudits.AddRange(cloudAudits);
                        }
                    }
                    catch { }
                }

                _isLoadedFromFile = true;
                TriggerBackgroundCloudSync();
            }
            finally
            {
                _fileLock.Release();
            }
        }

        private void TriggerBackgroundCloudSync()
        {
            if (_firebaseSync == null || !_firebaseSync.IsConfigured) return;

            _ = Task.Run(async () =>
            {
                try
                {
                    // 1. Kéo người dùng mới từ Cloud Firestore
                    var cloudUsers = await _firebaseSync.PullUsersAsync();
                    if (cloudUsers != null && cloudUsers.Count > 0)
                    {
                        bool hasNew = false;
                        foreach (var cu in cloudUsers)
                        {
                            var existing = _localUsers.FirstOrDefault(u => u.Id == cu.Id || u.Email.Equals(cu.Email, StringComparison.OrdinalIgnoreCase));
                            if (existing == null)
                            {
                                _localUsers.Add(cu);
                                hasNew = true;
                            }
                        }

                        if (hasNew)
                        {
                            await SaveUsersToDiskAsync();
                        }
                    }

                    // 2. Đẩy toàn bộ người dùng cục bộ lên Firebase Authentication & Cloud Firestore
                    await _firebaseSync.PushUsersAsync(_localUsers);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Lỗi đồng bộ tự động 2 chiều người dùng lúc khởi động");
                }
            });
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

            // Tự động đồng bộ ngầm lên Firebase Cloud Database mà không bắt người dùng bấm
            if (_firebaseSync != null && _firebaseSync.IsConfigured)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _firebaseSync.PushUsersAsync(_localUsers);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Lỗi tự động đồng bộ người dùng lên Firebase Cloud");
                    }
                });
            }
        }

        private async Task SavePasswordResetsToDiskAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_localResetRequests, options);
                await File.WriteAllTextAsync(PasswordResetsFilePath, json);
            }
            catch { }
            finally
            {
                _fileLock.Release();
            }

            if (_firebaseSync != null && _firebaseSync.IsConfigured)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _firebaseSync.PushPasswordResetRequestsAsync(_localResetRequests);
                    }
                    catch { }
                });
            }
        }

        private async Task SavePointAuditsToDiskAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_localPointAudits, options);
                await File.WriteAllTextAsync(PointAuditsFilePath, json);
            }
            catch { }
            finally
            {
                _fileLock.Release();
            }

            if (_firebaseSync != null && _firebaseSync.IsConfigured)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _firebaseSync.PushPointAuditsAsync(_localPointAudits);
                    }
                    catch { }
                });
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
            var clean = username.Trim().ToLowerInvariant();
            return _localUsers.FirstOrDefault(u =>
                u.Email.ToLowerInvariant() == clean ||
                u.Username.ToLowerInvariant() == clean ||
                (clean.Contains('@') && clean.StartsWith(u.Username.ToLowerInvariant() + "@")));
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

        public async Task<(bool Success, string Message)> AdjustTrainingPointsWithAuditAsync(string studentCode, int delta, string reason, string operatorUsername, string operatorName)
        {
            await EnsureLoadedAsync();
            var user = _localUsers.FirstOrDefault(u =>
                u.Username.Equals(studentCode, StringComparison.OrdinalIgnoreCase) ||
                u.Id.Equals(studentCode, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                return (false, "Không tìm thấy thông tin sinh viên trong hệ thống.");

            int oldPts = user.TrainingPoints;
            user.TrainingPoints = Math.Max(0, user.TrainingPoints + delta);

            var audit = new PointAuditRecord
            {
                Id = "AUD-" + Guid.NewGuid().ToString("N")[..6].ToUpper(),
                StudentCode = user.Username,
                StudentName = user.FullName,
                Delta = delta,
                Reason = string.IsNullOrWhiteSpace(reason) ? (delta >= 0 ? "Cộng điểm rèn luyện" : "Trừ điểm vi phạm") : reason.Trim(),
                OperatorUsername = operatorUsername,
                OperatorName = operatorName,
                Timestamp = DateTime.UtcNow
            };

            _localPointAudits.Insert(0, audit);
            await SaveUsersToDiskAsync();
            await SavePointAuditsToDiskAsync();

            string action = delta >= 0 ? "cộng" : "trừ";
            _logger.LogInformation("Adjusted training points for {Username} by {Delta}. Old: {Old}, New: {New}. Reason: {Reason}", user.Username, delta, oldPts, user.TrainingPoints, reason);
            return (true, $"Đã {action} {Math.Abs(delta)} ĐRL cho {user.FullName}. ĐRL hiện tại: {user.TrainingPoints} ĐRL.");
        }

        public async Task<List<PointAuditRecord>> GetPointAuditsForStudentAsync(string studentCode)
        {
            await EnsureLoadedAsync();
            if (string.IsNullOrWhiteSpace(studentCode) || studentCode == "ALL")
                return _localPointAudits.ToList();

            return _localPointAudits.Where(a => string.Equals(a.StudentCode, studentCode, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // ==================== ĐỔI MẬT KHẨU & QUÊN MẬT KHẨU ====================

        public async Task<(bool Success, string Message)> ChangePasswordAsync(string username, string currentPassword, string newPassword)
        {
            await EnsureLoadedAsync();
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
                return (false, "Không tìm thấy thông tin người dùng.");

            string currentEffective = user.EffectivePassword;
            if (!string.Equals(currentEffective, currentPassword?.Trim()))
            {
                return (false, "Mật khẩu hiện tại không chính xác. Vui lòng thử lại!");
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Trim().Length < 6)
            {
                return (false, "Mật khẩu mới phải có tối thiểu 6 ký tự.");
            }

            user.Password = newPassword.Trim();
            user.PasswordHash = newPassword.Trim();
            await SaveUsersToDiskAsync();

            _logger.LogInformation("Password changed successfully for {Username}", user.Username);
            return (true, "Đổi mật khẩu tài khoản thành công!");
        }

        public async Task<(bool Success, string Message, string AssignedDefaultPassword)> RequestPasswordResetAsync(string usernameOrEmail, string note)
        {
            await EnsureLoadedAsync();
            if (string.IsNullOrWhiteSpace(usernameOrEmail))
                return (false, "Vui lòng nhập tên đăng nhập, email hoặc mã sinh viên.", string.Empty);

            var user = await GetUserByUsernameAsync(usernameOrEmail.Trim());
            if (user == null)
            {
                return (false, "Không tìm thấy tài khoản với thông tin đã nhập.", string.Empty);
            }

            string defaultPassword = user.Role.GetDefaultPassword();

            // Kiểm tra xem đã có yêu cầu Pending chưa
            var existing = _localResetRequests.FirstOrDefault(r =>
                r.IsPending && string.Equals(r.Username, user.Username, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                return (true, $"Yêu cầu đặt lại mật khẩu của bạn đã được gửi trước đó và đang chờ Admin xử lý. Mật khẩu sau khi reset sẽ là: {defaultPassword}", defaultPassword);
            }

            var req = new PasswordResetRequest
            {
                Id = "RST-" + Guid.NewGuid().ToString("N")[..6].ToUpper(),
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                RequestedAt = DateTime.UtcNow,
                Status = "Pending",
                DefaultPasswordAssigned = defaultPassword,
                Notes = string.IsNullOrWhiteSpace(note) ? "Báo quên mật khẩu từ màn hình đăng nhập" : note
            };

            _localResetRequests.Insert(0, req);
            await SavePasswordResetsToDiskAsync();
            _logger.LogInformation("Password reset request submitted for {Username} ({Role})", user.Username, user.Role);
            return (true, $"Đã gửi yêu cầu đặt lại mật khẩu tới Ban Quản Trị (Admin). Mật khẩu mặc định sau khi được duyệt là: {defaultPassword}", defaultPassword);
        }

        public async Task<List<PasswordResetRequest>> GetPendingPasswordResetRequestsAsync()
        {
            await EnsureLoadedAsync();
            return _localResetRequests.Where(r => r.IsPending).OrderByDescending(r => r.RequestedAt).ToList();
        }

        public async Task<(bool Success, string Message, string ResetPassword)> ResolvePasswordResetAsync(string requestId, string adminUsername)
        {
            await EnsureLoadedAsync();
            var req = _localResetRequests.FirstOrDefault(r => r.Id.Equals(requestId, StringComparison.OrdinalIgnoreCase));
            if (req == null)
                return (false, "Không tìm thấy yêu cầu đặt lại mật khẩu này.", string.Empty);

            var user = await GetUserByUsernameAsync(req.Username);
            if (user == null)
                return (false, "Không tìm thấy tài khoản người dùng tương ứng.", string.Empty);

            string defaultPassword = user.Role.GetDefaultPassword();
            user.Password = defaultPassword;
            user.PasswordHash = defaultPassword;

            req.Status = "Resolved";
            req.ResolvedAt = DateTime.UtcNow;
            req.ResolvedBy = adminUsername;
            req.DefaultPasswordAssigned = defaultPassword;

            await SaveUsersToDiskAsync();
            await SavePasswordResetsToDiskAsync();
            _logger.LogInformation("Reset request {RequestId} resolved by {Admin}. User {Username} reset to {DefaultPw}", requestId, adminUsername, user.Username, defaultPassword);
            return (true, $"Đã đặt lại mật khẩu cho {user.FullName} về mặc định ({defaultPassword}) thành công!", defaultPassword);
        }

        public async Task<(bool Success, string Message, string ResetPassword)> ResetUserPasswordToDefaultAsync(string username, string adminUsername)
        {
            await EnsureLoadedAsync();
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
                return (false, "Không tìm thấy tài khoản người dùng.", string.Empty);

            string defaultPassword = user.Role.GetDefaultPassword();
            user.Password = defaultPassword;
            user.PasswordHash = defaultPassword;

            foreach (var r in _localResetRequests.Where(r => r.IsPending && string.Equals(r.Username, user.Username, StringComparison.OrdinalIgnoreCase)))
            {
                r.Status = "Resolved";
                r.ResolvedAt = DateTime.UtcNow;
                r.ResolvedBy = adminUsername;
                r.DefaultPasswordAssigned = defaultPassword;
            }

            await SaveUsersToDiskAsync();
            await SavePasswordResetsToDiskAsync();
            _logger.LogInformation("Password for {Username} directly reset to {DefaultPassword} by Admin {Admin}", user.Username, defaultPassword, adminUsername);
            return (true, $"Đã đặt lại mật khẩu cho {user.FullName} về mặc định ({defaultPassword}) thành công!", defaultPassword);
        }

        // ==================== LỚP HỌC & GIẢNG VIÊN ====================

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

            if (_firebaseSync != null && _firebaseSync.IsConfigured)
            {
                _ = Task.Run(async () => await _firebaseSync.PushClassesAsync(_classes));
            }

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

            if (_firebaseSync != null && _firebaseSync.IsConfigured)
            {
                _ = Task.Run(async () => await _firebaseSync.PushClassesAsync(_classes));
            }

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
                if (_firebaseSync != null && _firebaseSync.IsConfigured)
                {
                    _ = Task.Run(async () => await _firebaseSync.PushClassesAsync(_classes));
                }
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
                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    existing.Password = user.Password;
                    existing.PasswordHash = user.Password;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    user.Password = user.Role.GetDefaultPassword();
                    user.PasswordHash = user.Password;
                }
                _localUsers.Add(user);
            }
            _logger.LogInformation("Saved user {Username} ({Id})", user.Username, user.Id);
            if (_firebaseSync != null && _firebaseSync.IsConfigured)
            {
                try
                {
                    await _firebaseSync.PushUserAsync(user);
                    _logger.LogInformation("Pushed user {Email} directly to Firebase Authentication & Cloud Firestore", user.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Lỗi đẩy trực tiếp người dùng lên Firebase");
                }
            }
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
                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    local.Password = user.Password;
                    local.PasswordHash = user.Password;
                }
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
