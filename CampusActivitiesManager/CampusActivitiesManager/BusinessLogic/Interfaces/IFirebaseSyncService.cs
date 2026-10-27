using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.BusinessLogic.Interfaces
{
    public interface IFirebaseSyncService
    {
        string DatabaseUrl { get; set; }
        bool IsConfigured { get; }

        Task<List<CampusEvent>?> PullEventsAsync();
        Task<bool> PushEventsAsync(List<CampusEvent> events);
        Task<bool> PushEventAsync(CampusEvent evt);
        Task<bool> DeleteEventAsync(int eventId);

        Task<List<User>?> PullUsersAsync();
        Task<bool> PushUsersAsync(List<User> users);
        Task<bool> PushUserAsync(User user);

        Task<List<ClassRoom>?> PullClassesAsync();
        Task<bool> PushClassesAsync(List<ClassRoom> classes);

        Task<List<AttendanceRecord>?> PullAttendanceAsync();
        Task<bool> PushAttendanceAsync(List<AttendanceRecord> records);

        Task<List<PasswordResetRequest>?> PullPasswordResetRequestsAsync();
        Task<bool> PushPasswordResetRequestsAsync(List<PasswordResetRequest> requests);

        Task<List<CampusNotificationDto>?> PullNotificationsAsync();
        Task<bool> PushNotificationsAsync(List<CampusNotificationDto> notifications);

        Task<List<PointAuditRecord>?> PullPointAuditsAsync();
        Task<bool> PushPointAuditsAsync(List<PointAuditRecord> audits);

        Task<bool> TestConnectionAsync();
    }
}
