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
        public int TrainingPointsAwarded { get; set; } = 10;

        [JsonPropertyName("pointsDeducted")]
        public int PointsDeducted { get; set; } = 5;

        /// <summary>
        /// "PendingReview" (Chờ duyệt ảnh), "Approved" (Đã duyệt), "Rejected" (Từ chối)
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = "Approved";

        [JsonPropertyName("reviewerName")]
        public string? ReviewerName { get; set; }

        [JsonPropertyName("reviewNotes")]
        public string? ReviewNotes { get; set; }

        [JsonIgnore]
        public bool IsPendingReview => Status == "PendingReview";

        [JsonIgnore]
        public bool HasPhoto => !string.IsNullOrWhiteSpace(PhotoUrl);

        [JsonIgnore]
        public ImageSource? PhotoImageSource
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PhotoUrl)) return null;
                try
                {
                    if (PhotoUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase) || PhotoUrl.EndsWith(".png") || PhotoUrl.EndsWith(".jpg"))
                    {
                        return PhotoUrl;
                    }
                    var bytes = Convert.FromBase64String(PhotoUrl);
                    return ImageSource.FromStream(() => new MemoryStream(bytes));
                }
                catch
                {
                    return null;
                }
            }
        }

        // UI Presentation Properties
        [JsonIgnore]
        public string FormattedCheckInTime => CheckInTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        [JsonIgnore]
        public string MethodIcon => Method switch
        {
            "Photo" => "📸",
            "QR" => "🔗",
            "Manual" => "✍️",
            _ => "🖼️"
        };

        [JsonIgnore]
        public string MethodDisplay => Method switch
        {
            "Photo" => "Ảnh minh chứng camera",
            "QR" => "Đường dẫn sự kiện",
            "Manual" => "Duyệt thủ công",
            _ => Method
        };

        [JsonIgnore]
        public string PointsDisplay => Status switch
        {
            "Approved" => $"+{TrainingPointsAwarded} ĐRL",
            "Rejected" => $"-{PointsDeducted} ĐRL",
            _ => "Chờ duyệt..."
        };

        [JsonIgnore]
        public Color StatusColor => Status switch
        {
            "Approved" => Color.FromArgb("#16A34A"),
            "PendingReview" => Color.FromArgb("#D97706"),
            "Rejected" => Color.FromArgb("#DC2626"),
            _ => Color.FromArgb("#6B7280")
        };

        [JsonIgnore]
        public Color StatusBadgeBg => Status switch
        {
            "Approved" => Color.FromArgb("#DCFCE7"),
            "PendingReview" => Color.FromArgb("#FEF3C7"),
            "Rejected" => Color.FromArgb("#FEE2E2"),
            _ => Color.FromArgb("#F3F4F6")
        };

        [JsonIgnore]
        public string StatusDisplay => Status switch
        {
            "Approved" => "Đã duyệt (+Điểm) ✅",
            "PendingReview" => "Chờ duyệt ảnh ⏳",
            "Rejected" => "Bị từ chối (-Điểm) ❌",
            _ => Status
        };
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

        [JsonPropertyName("targetUsername")]
        public string TargetUsername { get; set; } = string.Empty;

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("isRead")]
        public bool IsRead { get; set; }

        [JsonIgnore]
        public bool IsUnread => !IsRead;

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
