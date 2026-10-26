using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.BusinessLogic.Interfaces
{
    public interface IFirebaseSyncService
    {
        string DatabaseUrl { get; set; }
        bool IsConfigured { get; }

        Task<List<CampusEvent>?> PullEventsAsync();
        Task<bool> PushEventsAsync(List<CampusEvent> events);

        Task<List<User>?> PullUsersAsync();
        Task<bool> PushUsersAsync(List<User> users);

        Task<List<ClassRoom>?> PullClassesAsync();
        Task<bool> PushClassesAsync(List<ClassRoom> classes);

        Task<List<AttendanceRecord>?> PullAttendanceAsync();
        Task<bool> PushAttendanceAsync(List<AttendanceRecord> records);

        Task<bool> TestConnectionAsync();
    }
}
