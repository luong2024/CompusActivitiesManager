using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CampusActivitiesManager.Api.Models
{
    public class QrCheckInRequest
    {
        [Required(ErrorMessage = "EventId là bắt buộc.")]
        [JsonPropertyName("eventId")]
        public int EventId { get; set; }

        [Required(ErrorMessage = "StudentCode là bắt buộc.")]
        [JsonPropertyName("studentCode")]
        public string StudentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã vé QR là bắt buộc.")]
        [JsonPropertyName("qrTicketCode")]
        public string QrTicketCode { get; set; } = string.Empty;

        [JsonPropertyName("deviceTimestamp")]
        public DateTime? DeviceTimestamp { get; set; } = DateTime.UtcNow;
    }

    public class PhotoCheckInRequest
    {
        [Required(ErrorMessage = "EventId là bắt buộc.")]
        [JsonPropertyName("eventId")]
        public int EventId { get; set; }

        [Required(ErrorMessage = "StudentCode là bắt buộc.")]
        [JsonPropertyName("studentCode")]
        public string StudentCode { get; set; } = string.Empty;

        [JsonPropertyName("photoBase64")]
        public string? PhotoBase64 { get; set; }

        [JsonPropertyName("photoUrl")]
        public string? PhotoUrl { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; } = string.Empty;

        [JsonPropertyName("deviceTimestamp")]
        public DateTime? DeviceTimestamp { get; set; } = DateTime.UtcNow;
    }

    public class ManualAttendanceRequest
    {
        [Required(ErrorMessage = "EventId là bắt buộc.")]
        [JsonPropertyName("eventId")]
        public int EventId { get; set; }

        [Required(ErrorMessage = "StudentCode là bắt buộc.")]
        [JsonPropertyName("studentCode")]
        public string StudentCode { get; set; } = string.Empty;

        [JsonPropertyName("notes")]
        public string Notes { get; set; } = "Điểm danh thủ công bởi Ban tổ chức";

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Present";

        [JsonPropertyName("trainingPoints")]
        public int TrainingPoints { get; set; } = 5;
    }

    public class AttendanceRecord
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        [JsonPropertyName("eventId")]
        public int EventId { get; set; }

        [JsonPropertyName("eventName")]
        public string EventName { get; set; } = string.Empty;

        [JsonPropertyName("studentCode")]
        public string StudentCode { get; set; } = string.Empty;

        [JsonPropertyName("studentName")]
        public string StudentName { get; set; } = string.Empty;

        [JsonPropertyName("checkInTime")]
        public DateTime CheckInTime { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("method")]
        public string Method { get; set; } = "QR"; // "QR", "Photo", "Manual"

        [JsonPropertyName("photoUrl")]
        public string? PhotoUrl { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; } = string.Empty;

        [JsonPropertyName("trainingPointsAwarded")]
        public int TrainingPointsAwarded { get; set; } = 5;

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Present"; // "Present", "PendingApproval", "Late"
    }

    public class AttendanceSummaryDto
    {
        [JsonPropertyName("eventId")]
        public int EventId { get; set; }

        [JsonPropertyName("eventName")]
        public string EventName { get; set; } = string.Empty;

        [JsonPropertyName("totalRegistered")]
        public int TotalRegistered { get; set; }

        [JsonPropertyName("totalAttended")]
        public int TotalAttended { get; set; }

        [JsonPropertyName("attendanceRatePercentage")]
        public double AttendanceRatePercentage => TotalRegistered > 0
            ? Math.Round((double)TotalAttended / TotalRegistered * 100, 1)
            : 0;

        [JsonPropertyName("records")]
        public List<AttendanceRecord> Records { get; set; } = [];
    }

    public class CampusNotificationDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = "Info"; // "Info", "Success", "Reminder", "Points"

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("isRead")]
        public bool IsRead { get; set; } = false;

        [JsonPropertyName("actionUrl")]
        public string? ActionUrl { get; set; }
    }
}
