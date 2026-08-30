using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.Services
{
    /// <summary>
    /// Giao diện quản lý xác thực, phiên làm việc (Session) và kiểm tra quyền truy cập (RBAC Guard).
    /// </summary>
    public interface IAuthenticationService
    {
        User? CurrentUser { get; }
        Role CurrentRole { get; }
        bool IsAdmin { get; }
        bool IsManager { get; }
        bool IsAuthenticated { get; }

        event EventHandler? CurrentUserChanged;

        Task<bool> LoginAsync(string username, string password);
        Task<bool> SwitchUserAsync(string username);
        void Logout();
        bool CheckPermission(Role requiredRole);
        Task RefreshCurrentUserAsync();
    }
}
