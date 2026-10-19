using System.Text.Json.Serialization;

namespace CampusActivitiesManager.Models
{
    public class QrCheckInRequest
    {
        [JsonPropertyName("eventId")]
        public int EventId { get; set; }

        [JsonPropertyName("studentCode")]
        public string StudentCode { get; set; } = string.Empty;

        [JsonPropertyName("qrTicketCode")]
        public string QrTicketCode { get; set; } = string.Empty;

        [JsonPropertyName("deviceTimestamp")]
        public DateTime? DeviceTimestamp { get; set; } = DateTime.UtcNow;
    }

    public class PhotoCheckInRequest
    {
        [JsonPropertyName("eventId")]
        public int EventId { get; set; }

        [JsonPropertyName("studentCode")]
        public string StudentCode { get; set; } = string.Empty;

        [JsonPropertyName("photoBase64")]
        public string? PhotoBase64 { get; set; }

        [JsonPropertyName("photoUrl")]
        public string? PhotoUrl { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("deviceTimestamp")]
        public DateTime? DeviceTimestamp { get; set; } = DateTime.UtcNow;
    }

    public class ManualAttendanceRequest
    {
        [JsonPropertyName("eventId")]
        public int EventId { get; set; }

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
        public string Id { get; set; } = string.Empty;

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
        public string Status { get; set; } = "Present";

        // UI Presentation Properties
        [JsonIgnore]
        public string FormattedCheckInTime => CheckInTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        [JsonIgnore]
        public string MethodIcon => Method switch
        {
            "QR" => "📷",
            "Photo" => "🖼️",
            "Manual" => "✍️",
            _ => "✅"
        };

        [JsonIgnore]
        public string MethodDisplay => Method switch
        {
            "QR" => "Quét mã QR",
            "Photo" => "Ảnh minh chứng",
            "Manual" => "Thủ công (BTC)",
            _ => Method
        };

        [JsonIgnore]
        public string PointsDisplay => $"+{TrainingPointsAwarded} ĐRL";

        [JsonIgnore]
        public Color StatusColor => Status == "Present" ? Color.FromArgb("#16A34A") : Color.FromArgb("#DC2626");

        [JsonIgnore]
        public Color StatusBadgeBg => Status == "Present" ? Color.FromArgb("#DCFCE7") : Color.FromArgb("#FEE2E2");

        [JsonIgnore]
        public string StatusDisplay => Status == "Present" ? "Đã tham gia" : "Vắng mặt";
    }

    public class CampusNotificationDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = "Info";

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("isRead")]
        public bool IsRead { get; set; }

        [JsonIgnore]
        public string FormattedTime => CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        [JsonIgnore]
        public string TypeIcon => Type switch
        {
            "Points" => "🏆",
            "Reminder" => "⏰",
            "Success" => "🎫",
            "Urgent" => "⚠️",
            _ => "🔔"
        };

        [JsonIgnore]
        public Color AccentColor => Type switch
        {
            "Points" => Color.FromArgb("#D97706"), // Amber/Gold
            "Reminder" => Color.FromArgb("#2563EB"), // Primary Blue
            "Success" => Color.FromArgb("#16A34A"), // Green
            "Urgent" => Color.FromArgb("#DC2626"), // Red
            _ => Color.FromArgb("#4F46E5") // Indigo
        };
    }
}
