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
        Task<(bool Success, string Message)> AdjustTrainingPointsWithAuditAsync(string studentCode, int delta, string reason, string operatorUsername, string operatorName);
        Task<List<PointAuditRecord>> GetPointAuditsForStudentAsync(string studentCode);
        Task<(bool Success, string Message)> ChangePasswordAsync(string username, string currentPassword, string newPassword);
        Task<(bool Success, string Message, string AssignedDefaultPassword)> RequestPasswordResetAsync(string usernameOrEmail, string note);
        Task<List<PasswordResetRequest>> GetPendingPasswordResetRequestsAsync();
        Task<(bool Success, string Message, string ResetPassword)> ResolvePasswordResetAsync(string requestId, string adminUsername);
        Task<(bool Success, string Message, string ResetPassword)> ResetUserPasswordToDefaultAsync(string username, string adminUsername);
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
