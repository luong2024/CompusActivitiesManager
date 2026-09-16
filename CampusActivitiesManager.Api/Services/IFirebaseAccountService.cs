using CampusActivitiesManager.Api.Models;

namespace CampusActivitiesManager.Api.Services
{
    public interface IFirebaseAccountService
    {
        Task<UserAccountDto> CreateAccountAsync(CreateAccountRequest request);
        Task<UserAccountDto?> GetAccountByIdAsync(string id);
        Task<UserAccountDto?> GetAccountByEmailAsync(string email);
        Task<List<UserAccountDto>> GetAllAccountsAsync();
        Task<UserAccountDto> UpdateAccountAsync(string id, UpdateAccountRequest request);
        Task<AccountStatusResponse> SetAccountLockStatusAsync(string id, bool isLocked);
        Task<LoginResponse?> AuthenticateAsync(string email, string password);
    }
}
