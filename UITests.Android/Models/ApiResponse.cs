using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CampusActivitiesManager.Models;

namespace UITests.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    public enum AccountStatus
    {
        DangHoc = 0,
        BaoLuu = 1,
        BiKhoa = 2
    }

    public enum AccountRole
    {
        SinhVien = 0,
        LopTruong = 1,
        GiangVien = 2,
        BanChuNhiem = 3,
        Admin = 4
    }

    public class AccountFilterCriteria
    {
        public string SearchText { get; set; } = string.Empty;
        public AccountStatus? SelectedStatus { get; set; }
        public string SelectedClass { get; set; } = string.Empty;
        public string SelectedAcademicYear { get; set; } = string.Empty;
        public AccountRole? SelectedRole { get; set; }

        public int ActiveFilterCount
        {
            get
            {
                int count = 0;
                if (!string.IsNullOrWhiteSpace(SelectedClass) && SelectedClass != "Tất cả") count++;
                if (!string.IsNullOrWhiteSpace(SelectedAcademicYear) && SelectedAcademicYear != "Tất cả") count++;
                if (SelectedRole.HasValue) count++;
                return count;
            }
        }

        public bool HasAdvancedFilters => ActiveFilterCount > 0;

        public void Reset()
        {
            SearchText = string.Empty;
            SelectedStatus = null;
            SelectedClass = string.Empty;
            SelectedAcademicYear = string.Empty;
            SelectedRole = null;
        }

        public AccountFilterCriteria Clone()
        {
            return new AccountFilterCriteria
            {
                SearchText = SearchText,
                SelectedStatus = SelectedStatus,
                SelectedClass = SelectedClass,
                SelectedAcademicYear = SelectedAcademicYear,
                SelectedRole = SelectedRole
            };
        }
    }

    public class AccountTestService
    {
        private readonly List<User> _users = new()
        {
            new() { Id = "admin-01", Username = "admin", FullName = "Quản Trị Viên Hệ Thống", Email = "admin@campus.edu.vn", Role = Role.Admin, IsActive = true, PasswordHash = HashPassword("Admin@123456") },
            new() { Id = "student-01", Username = "B20DCCN001", FullName = "Nguyễn An Cương", Email = "cuong.na@campus.edu.vn", Role = Role.User, IsActive = true, PasswordHash = HashPassword("Student@123456") },
            new() { Id = "student-02", Username = "UST0504", FullName = "Vũ Tiến Đạt", Email = "dat.vt@campus.edu.vn", Role = Role.User, IsActive = true, PasswordHash = HashPassword("Dat@123456") }
        };

        private readonly Dictionary<string, int> _failedAttempts = new(StringComparer.OrdinalIgnoreCase);

        public static string HashPassword(string plainPassword)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(plainPassword),
                salt,
                iterations: 10000,
                HashAlgorithmName.SHA256,
                outputLength: 32);

            return $"$PBKDF2-SHA256$10000${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string plainPassword, string storedHash)
        {
            var parts = storedHash.Split('$', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4 || parts[0] != "PBKDF2-SHA256") return false;
            int iterations = int.Parse(parts[1]);
            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] expectedHash = Convert.FromBase64String(parts[3]);
            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(plainPassword), salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }

        public static int CalculateResponsiveColumnSpan(double width)
        {
            if (width < 600) return 1;
            if (width < 1000) return 2;
            return 3;
        }

        public static string FormatCategoryTagText(string? rawTitle)
        {
            if (string.IsNullOrWhiteSpace(rawTitle)) return "Chưa phân loại";
            var trimmed = rawTitle.Trim();
            return trimmed.Length <= 15 ? trimmed : trimmed[..12].TrimEnd() + "...";
        }

        public Task<ApiResponse<User>> LoginAsync(string usernameOrEmail, string password)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
            {
                return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 400, Message = "Vui lòng nhập đầy đủ thông tin" });
            }

            var key = usernameOrEmail.Trim();
            if (_failedAttempts.TryGetValue(key, out int count) && count >= 5)
            {
                return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 403, Message = "ACCOUNT_LOCKED" });
            }

            var user = _users.FirstOrDefault(u =>
                string.Equals(u.Username, key, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u.Email, key, StringComparison.OrdinalIgnoreCase));

            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                _failedAttempts[key] = count + 1;
                if (_failedAttempts[key] >= 5)
                {
                    if (user != null) user.IsActive = false;
                    return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 403, Message = "ACCOUNT_LOCKED" });
                }
                return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 401, Message = "Tài khoản hoặc mật khẩu không chính xác" });
            }

            if (!user.IsActive)
            {
                return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 403, Message = "Tài khoản của bạn đã bị khóa" });
            }

            _failedAttempts.Remove(key);
            return Task.FromResult(new ApiResponse<User> { Success = true, StatusCode = 200, Message = "Đăng nhập thành công.", Data = user });
        }

        public Task<ApiResponse<User>> RegisterAsync(string fullName, string email, string password, string confirmPassword, bool acceptedTerms)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 400, Message = "Vui lòng nhập đầy đủ thông tin" });

            if (!acceptedTerms)
                return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 400, Message = "Vui lòng đồng ý với Điều khoản" });

            if (!Regex.IsMatch(email.Trim(), @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$"))
                return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 400, Message = "Email không hợp lệ" });

            if (password != confirmPassword)
                return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 400, Message = "Mật khẩu xác nhận không khớp" });

            if (_users.Any(u => string.Equals(u.Email, email.Trim(), StringComparison.OrdinalIgnoreCase)))
                return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 409, Message = "Email đã tồn tại" });

            var newUser = new User
            {
                Id = "user-" + Guid.NewGuid().ToString("N")[..8],
                FullName = fullName.Trim(),
                Email = email.Trim(),
                Username = email.Trim().Split('@')[0],
                PasswordHash = HashPassword(password),
                Role = Role.User,
                IsActive = true
            };
            _users.Add(newUser);
            return Task.FromResult(new ApiResponse<User> { Success = true, StatusCode = 201, Data = newUser });
        }

        public Task<ApiResponse<User>> SetLockStateAsync(User actor, string targetId, bool lockAccount)
        {
            if (actor.Role != Role.Admin)
                return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 403, Message = "403 Forbidden" });

            if (lockAccount && actor.Id == targetId)
                return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 400, Message = "Admin không thể tự khóa tài khoản của chính mình" });

            var target = _users.FirstOrDefault(u => u.Id == targetId);
            if (target == null)
                return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 404, Message = "Not Found" });

            target.IsActive = !lockAccount;
            return Task.FromResult(new ApiResponse<User> { Success = true, StatusCode = 200, Data = target });
        }

        public Task<ApiResponse<User>> AssignRoleAsync(User actor, string targetId, Role newRole)
        {
            if (actor.Role != Role.Admin)
                return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 403, Message = "403 Forbidden" });

            var target = _users.FirstOrDefault(u => u.Id == targetId);
            if (target == null)
                return Task.FromResult(new ApiResponse<User> { Success = false, StatusCode = 404, Message = "Not Found" });

            target.Role = newRole;
            return Task.FromResult(new ApiResponse<User> { Success = true, StatusCode = 200, Data = target });
        }
    }
}
