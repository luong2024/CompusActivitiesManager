using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.Services
{
    public interface IUserService<T> where T : class
    {
        Task<List<T>> GetUsersListAsync();
        Task<T?> GetUserByIdAsync(string id);
        Task<T?> GetUserByUsernameAsync(string username);
        Task<int> SaveUserAsync(T user);
        Task<bool> UpdateUserAsync(T user);
        Task<bool> UpdateUserRoleAsync(string id, Role newRole);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> ToggleUserStatusAsync(string id);
        Task SeedDefaultUsersAsync();
    }

    public interface IUserService : IUserService<User>
    {
    }
}
