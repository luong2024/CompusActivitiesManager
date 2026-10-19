using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.Services
{
    public interface IUserService<T> where T : class
    {
        Task<List<T>> GetUsersListAsync();
        Task<T?> GetUserByIdAsync(string id);
        Task<T?> GetUserByUsernameAsync(string username);
        Task<List<T>> GetStudentsByClassAsync(string className);
        Task<int> SaveUserAsync(T user);
        Task<bool> UpdateUserAsync(T user);
        Task<bool> UpdateProfileAsync(UpdateProfileRequest request);
        Task<bool> UpdateUserRoleAsync(string id, Role newRole);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> ToggleUserStatusAsync(string id);
        Task<bool> AdjustTrainingPointsAsync(string studentCodeOrId, int pointsDelta);
        Task<List<ClassRoom>> GetClassesAsync();
        Task SeedDefaultUsersAsync();
    }

    public interface IUserService : IUserService<User>
    {
    }
}
