using CampusActivitiesManager.Api.Models;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CampusActivitiesManager.Api.Services
{
    /// <summary>
    /// Dịch vụ tài khoản In-Memory hoạt động độc lập không cần Firebase credentials,
    /// phục vụ kiểm thử và cho phép người ngoài clone repo chạy ngay lập tức.
    /// </summary>
    public class InMemoryAccountService : IFirebaseAccountService
    {
        private readonly ConcurrentDictionary<string, UserAccountDto> _accounts = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, string> _passwords = new(StringComparer.OrdinalIgnoreCase);
        private readonly IConfiguration? _configuration;

        public ConcurrentDictionary<string, UserAccountDto> Database => _accounts;

        public InMemoryAccountService(IConfiguration? configuration = null)
        {
            _configuration = configuration;
            SeedMockData();
        }

        private void SeedMockData()
        {
            var mockAccounts = new List<(UserAccountDto Account, string Password)>
            {
                (
                    new UserAccountDto
                    {
                        Id = "admin-sys-01",
                        Email = "admin@campus.edu.vn",
                        FullName = "Quản trị viên Hệ thống",
                        Role = "Admin",
                        PhoneNumber = "0901234567",
                        StudentCode = "ADMIN01",
                        IsDisabled = false,
                        IsActive = true,
                        CreatedAt = "2026-01-01T00:00:00Z"
                    },
                    "Admin@123456"
                ),
                (
                    new UserAccountDto
                    {
                        Id = "manager-sys-02",
                        Email = "manager@campus.edu.vn",
                        FullName = "Ban Tổ chức Sự kiện",
                        Role = "Manager",
                        PhoneNumber = "0902345678",
                        StudentCode = "MANAGER01",
                        IsDisabled = false,
                        IsActive = true,
                        CreatedAt = "2026-01-02T00:00:00Z"
                    },
                    "Manager@123456"
                ),
                (
                    new UserAccountDto
                    {
                        Id = "lecturer-sys-03",
                        Email = "lecturer@campus.edu.vn",
                        FullName = "TS. Trần Văn B",
                        Role = "Lecturer",
                        PhoneNumber = "0903456789",
                        StudentCode = "GV001",
                        IsDisabled = false,
                        IsActive = true,
                        CreatedAt = "2026-01-03T00:00:00Z"
                    },
                    "Lecturer@123456"
                ),
                (
                    new UserAccountDto
                    {
                        Id = "student-sys-04",
                        Email = "cuong.na@campus.edu.vn",
                        FullName = "Nguyễn An Cương",
                        Role = "Student",
                        PhoneNumber = "0904567890",
                        StudentCode = "B20DCCN001",
                        IsDisabled = false,
                        IsActive = true,
                        CreatedAt = "2026-01-04T00:00:00Z"
                    },
                    "Student@123456"
                ),
                (
                    new UserAccountDto
                    {
                        Id = "mock_student_04",
                        Email = "student1@campus.edu",
                        FullName = "Nguyễn Văn An",
                        Role = "Student",
                        PhoneNumber = "0905678901",
                        StudentCode = "B26DCCN001",
                        IsDisabled = false,
                        IsActive = true,
                        CreatedAt = "2026-01-04T00:00:00Z"
                    },
                    "Student@123456"
                ),
                (
                    new UserAccountDto
                    {
                        Id = "mock_student_05",
                        Email = "student2@campus.edu",
                        FullName = "Lê Thị Mai",
                        Role = "Student",
                        PhoneNumber = "0905678902",
                        StudentCode = "B26DCCN002",
                        IsDisabled = false,
                        IsActive = true,
                        CreatedAt = "2026-01-05T00:00:00Z"
                    },
                    "Student@123456"
                ),
                (
                    new UserAccountDto
                    {
                        Id = "mock_locked_06",
                        Email = "locked_student@campus.edu",
                        FullName = "Phạm Văn Khóa",
                        Role = "Student",
                        PhoneNumber = "0906789012",
                        StudentCode = "B26DCCN003",
                        IsDisabled = true,
                        IsActive = false,
                        CreatedAt = "2026-01-06T00:00:00Z"
                    },
                    "Student@123456"
                )
            };

            foreach (var (account, password) in mockAccounts)
            {
                _accounts[account.Id] = account;
                _passwords[account.Email.ToLowerInvariant()] = password;
                if (!string.IsNullOrEmpty(account.StudentCode))
                {
                    _passwords[$"{account.StudentCode.ToLowerInvariant()}@campus.edu.vn"] = password;
                    _passwords[account.StudentCode.ToLowerInvariant()] = password;
                }
            }

            // Also seed alias for admin@campus.edu
            _passwords["admin@campus.edu"] = "Admin@123456";
            _passwords["admin"] = "Admin@123456";
            _passwords["manager@campus.edu"] = "Manager@123456";
            _passwords["manager"] = "Manager@123456";
        }

        public Task<UserAccountDto> CreateAccountAsync(CreateAccountRequest request)
        {
            if (_accounts.Values.Any(a => a.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Email is already registered");
            }

            string id = "user_" + Guid.NewGuid().ToString("N")[..8];
            string email = request.Email.Trim();
            string studentCode = request.StudentCode ?? (email.Contains('@') ? email.Split('@')[0] : email);

            var account = new UserAccountDto
            {
                Id = id,
                Email = email,
                FullName = request.FullName,
                Role = string.IsNullOrWhiteSpace(request.Role) ? "Student" : request.Role,
                PhoneNumber = request.PhoneNumber,
                StudentCode = studentCode,
                IsDisabled = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            _accounts[id] = account;
            _passwords[email.ToLowerInvariant()] = request.Password;
            if (!string.IsNullOrEmpty(studentCode))
            {
                _passwords[studentCode.ToLowerInvariant()] = request.Password;
            }

            return Task.FromResult(account);
        }

        public Task<UserAccountDto?> GetAccountByIdAsync(string id)
        {
            _accounts.TryGetValue(id, out var account);
            return Task.FromResult(account);
        }

        public Task<UserAccountDto?> GetAccountByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return Task.FromResult<UserAccountDto?>(null);

            string clean = email.Trim().ToLowerInvariant();

            // Direct email match
            var account = _accounts.Values.FirstOrDefault(a => a.Email.Equals(clean, StringComparison.OrdinalIgnoreCase));
            if (account != null) return Task.FromResult<UserAccountDto?>(account);

            // StudentCode match or prefix match
            account = _accounts.Values.FirstOrDefault(a =>
                (!string.IsNullOrEmpty(a.StudentCode) && a.StudentCode.Equals(clean, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(a.StudentCode) && clean.StartsWith(a.StudentCode.ToLowerInvariant() + "@")) ||
                clean.StartsWith(a.Email.Split('@')[0].ToLowerInvariant() + "@"));

            if (account != null) return Task.FromResult<UserAccountDto?>(account);

            // Convenient aliases
            if (clean == "admin" || clean == "admin@campus.edu")
            {
                return Task.FromResult(_accounts.Values.FirstOrDefault(a => a.Role == "Admin"));
            }
            if (clean == "manager" || clean == "manager@campus.edu")
            {
                return Task.FromResult(_accounts.Values.FirstOrDefault(a => a.Role == "Manager"));
            }

            return Task.FromResult<UserAccountDto?>(null);
        }

        public Task<List<UserAccountDto>> GetAllAccountsAsync()
        {
            return Task.FromResult(_accounts.Values.ToList());
        }

        public Task<UserAccountDto> UpdateAccountAsync(string id, UpdateAccountRequest request)
        {
            if (!_accounts.TryGetValue(id, out var account))
            {
                throw new KeyNotFoundException($"Account with ID {id} not found");
            }

            if (!string.IsNullOrEmpty(request.FullName)) account.FullName = request.FullName;
            if (!string.IsNullOrEmpty(request.PhoneNumber)) account.PhoneNumber = request.PhoneNumber;
            if (!string.IsNullOrEmpty(request.AvatarUrl)) account.AvatarUrl = request.AvatarUrl;
            if (!string.IsNullOrEmpty(request.Role)) account.Role = request.Role;
            account.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            return Task.FromResult(account);
        }

        public Task<AccountStatusResponse> SetAccountLockStatusAsync(string id, bool isLocked)
        {
            if (!_accounts.TryGetValue(id, out var account))
            {
                throw new KeyNotFoundException($"Account with ID {id} not found");
            }

            account.IsDisabled = isLocked;
            account.IsActive = !isLocked;
            account.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            return Task.FromResult(new AccountStatusResponse
            {
                Id = id,
                IsLocked = isLocked,
                IsActive = !isLocked,
                Message = isLocked ? "Account locked successfully" : "Account unlocked successfully",
                UpdatedAt = account.UpdatedAt
            });
        }

        public Task<LoginResponse?> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email)) return Task.FromResult<LoginResponse?>(null);

            var accountTask = GetAccountByEmailAsync(email);
            accountTask.Wait();
            var account = accountTask.Result;

            if (account == null)
            {
                return Task.FromResult<LoginResponse?>(null);
            }

            string cleanEmail = email.Trim().ToLowerInvariant();
            if (_passwords.TryGetValue(cleanEmail, out var expected) && expected != password)
            {
                // Accept universal testing passwords if in mock mode
                if (password != "Admin@123456" && password != "Manager@123456" && password != "Student@123456" && password != "123456")
                {
                    return Task.FromResult<LoginResponse?>(null);
                }
            }

            string token = GenerateJwtToken(account);

            return Task.FromResult<LoginResponse?>(new LoginResponse
            {
                Id = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                Role = account.Role,
                Token = token,
                IsActive = account.IsActive && !account.IsDisabled
            });
        }

        public Task<bool> DeleteAccountAsync(string id)
        {
            if (_accounts.TryRemove(id, out var removed))
            {
                _passwords.TryRemove(removed.Email.ToLowerInvariant(), out _);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<UserAccountDto> EnsureAdminAccountAsync()
        {
            var admin = _accounts.Values.FirstOrDefault(a => a.Role == "Admin");
            return Task.FromResult(admin ?? new UserAccountDto
            {
                Id = "admin-sys-01",
                Email = "admin@campus.edu.vn",
                FullName = "Quản trị viên Hệ thống",
                Role = "Admin",
                IsActive = true,
                IsDisabled = false
            });
        }

        private string GenerateJwtToken(UserAccountDto user)
        {
            string issuer = _configuration?["Jwt:Issuer"] ?? "CampusActivitiesManager.Api";
            string audience = _configuration?["Jwt:Audience"] ?? "CampusActivitiesManager.Client";
            string rawKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                ?? _configuration?["Jwt:SecretKey"]
                ?? "CampusActivitiesManager_DefaultDevSecretKey_2026!#";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(rawKey.PadRight(32, '_')));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("role", user.Role),
                new Claim("isActive", (user.IsActive && !user.IsDisabled).ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
