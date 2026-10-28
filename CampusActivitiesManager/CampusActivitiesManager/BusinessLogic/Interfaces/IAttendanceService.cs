using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.BusinessLogic.Services
{
    public interface IAttendanceService
    {
        Task<(bool Success, string Message, AttendanceRecord? Record)> CheckInWithQrAsync(int eventId, string studentCode, string qrPayload);
        Task<(bool Success, string Message, AttendanceRecord? Record)> CheckInWithPhotoAsync(int eventId, string studentCode, string photoBase64, string? notes);
        Task<(bool Success, string Message, AttendanceRecord? Record)> MarkManualAttendanceAsync(int eventId, string studentCode, string status, int points, string notes);
        Task<List<AttendanceRecord>> GetEventAttendanceAsync(int eventId);
        Task<List<AttendanceRecord>> GetStudentAttendanceHistoryAsync(string studentCode);
        Task<string> ExportAttendanceCsvAsync(int eventId);
        Task<List<CampusNotificationDto>> GetNotificationsAsync(string? studentCode = null, string? className = null);
        Task AddNotificationAsync(CampusNotificationDto notification);
        Task MarkNotificationAsReadAsync(string notificationId);
        Task MarkAllNotificationsAsReadAsync(string? studentCode = null);
        Task<(bool Success, string Message)> ReviewAttendanceAsync(string recordId, bool isApproved, string reviewerName, string? notes = null, int? customPointsDelta = null);
        Task<List<AttendanceRecord>> GetPendingAttendanceReviewsAsync(int? eventId = null);
        Task<List<AttendanceRecord>> GetAllAttendanceRecordsAsync();
    }
}
