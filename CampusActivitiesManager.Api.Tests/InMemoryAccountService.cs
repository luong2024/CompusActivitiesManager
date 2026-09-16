using CampusActivitiesManager.Api.Models;
using CampusActivitiesManager.Api.Services;
using System.Collections.Concurrent;

namespace CampusActivitiesManager.Api.Tests
{
    public class InMemoryAccountService : IFirebaseAccountService
    {
        private readonly ConcurrentDictionary<string, UserAccountDto> _accounts = new();
        private readonly ConcurrentDictionary<string, string> _passwords = new();

        public ConcurrentDictionary<string, UserAccountDto> Database => _accounts;

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
