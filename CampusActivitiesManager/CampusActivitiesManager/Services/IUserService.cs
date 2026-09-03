using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.Services
{
    /// <summary>
    /// Generic Interface cho User Service theo đặc tả Task.md và Tools.md
    /// </summary>
    /// <typeparam name="T">Kiểu thực thể User</typeparam>
    public interface IUserService<T> where T : class
    {
        Task<List<T>> GetUsersListAsync();
        Task<T?> GetUserByIdAsync(string id);
        Task<T?> GetUserByUsernameAsync(string username);
        Task<bool> UpdateUserRoleAsync(string id, Role newRole);
        Task<bool> DeleteUserAsync(string id);
        Task<int> SaveUserAsync(T user);
        Task<bool> UpdateUserAsync(T user);
        Task<bool> ToggleUserStatusAsync(string id);
        Task SeedDefaultUsersAsync();
    }

    /// <summary>
    /// Interface dịch vụ người dùng chuyên biệt cho model User.
    /// </summary>
    public interface IUserService : IUserService<User>
    {
    }
}
