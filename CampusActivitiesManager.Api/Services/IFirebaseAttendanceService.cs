using CampusActivitiesManager.Api.Models;

namespace CampusActivitiesManager.Api.Services
{
    public interface IFirebaseAttendanceService
    {
        Task<AttendanceRecord> CheckInWithQrAsync(QrCheckInRequest request);
        Task<AttendanceRecord> CheckInWithPhotoAsync(PhotoCheckInRequest request);
        Task<AttendanceRecord> MarkManualAttendanceAsync(ManualAttendanceRequest request);
        Task<List<AttendanceRecord>> GetEventAttendanceAsync(int eventId);
        Task<List<AttendanceRecord>> GetStudentAttendanceHistoryAsync(string studentCode);
        Task<string> ExportAttendanceCsvAsync(int eventId);
        Task<List<CampusNotificationDto>> GetNotificationsAsync(string? studentCode = null);
    }
}
