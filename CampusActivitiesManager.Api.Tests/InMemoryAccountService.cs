using CampusActivitiesManager.Api.Models;
using CampusActivitiesManager.Api.Services;
using System.Collections.Concurrent;

namespace CampusActivitiesManager.Api.Tests
{
    /// <summary>
    /// Dịch vụ tài khoản In-Memory phục vụ kiểm thử tích hợp tự động và nạp sẵn mock data (DoD6).
    /// </summary>
    public class InMemoryAccountService : IFirebaseAccountService
    {
        private readonly ConcurrentDictionary<string, UserAccountDto> _accounts = new();
        private readonly ConcurrentDictionary<string, string> _passwords = new();

        public ConcurrentDictionary<string, UserAccountDto> Database => _accounts;

        public InMemoryAccountService()
        {
            SeedMockData();
        }

        /// <summary>
        /// DoD6: Khởi tạo tối thiểu 5 tài khoản mẫu với đầy đủ các vai trò (Admin, Manager, Lecturer, Student)
        /// </summary>
        private void SeedMockData()
        {
            var mockAccounts = new List<(UserAccountDto Account, string Password)>
            {
                (
                    new UserAccountDto
                    {
                        Id = "mock_admin_01",
                        Email = "admin@campus.edu",
                        FullName = "Quản trị viên Hệ thống",
                        Role = "Admin",
                        PhoneNumber = "0901234567",
                        StudentCode = null,
                        IsDisabled = false,
                        IsActive = true,
                        CreatedAt = "2026-01-01T00:00:00Z"
                    },
                    "Admin@123456"
                ),
                (
                    new UserAccountDto
                    {
                        Id = "mock_manager_02",
                        Email = "manager@campus.edu",
                        FullName = "Ban Tổ chức Sự kiện",
                        Role = "Manager",
                        PhoneNumber = "0902345678",
                        StudentCode = null,
                        IsDisabled = false,
                        IsActive = true,
                        CreatedAt = "2026-01-02T00:00:00Z"
                    },
                    "Manager@123456"
                ),
                (
                    new UserAccountDto
                    {
                        Id = "mock_lecturer_03",
                        Email = "lecturer@campus.edu",
                        FullName = "TS. Trần Văn B",
                        Role = "Lecturer",
                        PhoneNumber = "0903456789",
                        StudentCode = null,
                        IsDisabled = false,
                        IsActive = true,
                        CreatedAt = "2026-01-03T00:00:00Z"
                    },
                    "Lecturer@123456"
                ),
                (
                    new UserAccountDto
                    {
                        Id = "mock_student_04",
                        Email = "student1@campus.edu",
                        FullName = "Nguyễn Văn An",
                        Role = "Student",
                        PhoneNumber = "0904567890",
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
                        PhoneNumber = "0905678901",
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
            }
        }

        public Task<UserAccountDto> CreateAccountAsync(CreateAccountRequest request)
        {
            if (_accounts.Values.Any(a => a.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Email is already registered");
            }

            string id = "test_user_" + Guid.NewGuid().ToString("N")[..8];
            var account = new UserAccountDto
            {
                Id = id,
                Email = request.Email,
                FullName = request.FullName,
                Role = request.Role,
                PhoneNumber = request.PhoneNumber,
                StudentCode = request.StudentCode,
                IsDisabled = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            _accounts[id] = account;
            _passwords[request.Email.ToLowerInvariant()] = request.Password;

            return Task.FromResult(account);
        }

        public Task<UserAccountDto?> GetAccountByIdAsync(string id)
        {
            _accounts.TryGetValue(id, out var account);
            return Task.FromResult(account);
        }

        public Task<UserAccountDto?> GetAccountByEmailAsync(string email)
        {
            var account = _accounts.Values.FirstOrDefault(a => a.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(account);
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
            var account = _accounts.Values.FirstOrDefault(a => a.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (account == null)
            {
                return Task.FromResult<LoginResponse?>(null);
            }

            if (_passwords.TryGetValue(email.ToLowerInvariant(), out var expectedPassword) && expectedPassword != password)
            {
                return Task.FromResult<LoginResponse?>(null);
            }

            return Task.FromResult<LoginResponse?>(new LoginResponse
            {
                Id = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                Role = account.Role,
                Token = "mock_jwt_token_" + account.Id,
                IsActive = account.IsActive && !account.IsDisabled
            });
        }
    }
}
