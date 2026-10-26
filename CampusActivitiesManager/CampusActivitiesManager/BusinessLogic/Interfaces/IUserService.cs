using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.BusinessLogic.Services
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
        Task<bool> AddClassAsync(ClassRoom classRoom);
        Task<bool> AssignLecturerToClassAsync(string className, string lecturerUsername, string lecturerName);
        Task<bool> DeleteClassAsync(string className);
        Task<List<User>> GetLecturersAsync();
        Task SeedDefaultUsersAsync();
    }

    public interface IUserService : IUserService<User>
    {
    }
}
