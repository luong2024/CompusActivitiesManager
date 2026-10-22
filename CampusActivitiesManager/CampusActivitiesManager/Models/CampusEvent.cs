using System.Text.Json.Serialization;

namespace CampusActivitiesManager.Models
{
    public class EventCategoryDto
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("color")]
        public string Color { get; set; } = "#2563EB";
    }

    /// <summary>
    /// Đơn đăng ký tham gia sự kiện của người dùng
    /// </summary>
    public class EventRegistration
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

        [JsonPropertyName("eventId")]
        public int EventId { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("studentCode")]
        public string StudentCode { get; set; } = string.Empty;

        [JsonPropertyName("studentName")]
        public string StudentName { get; set; } = string.Empty;

        [JsonPropertyName("className")]
        public string ClassName { get; set; } = "D20CNTT1";

        [JsonPropertyName("registeredAt")]
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// "Approved" (Đã được duyệt tham gia), "Pending" (Chờ duyệt tham gia), "Rejected" (Bị từ chối), "Cancelled" (Đã hủy)
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = "Approved";

        [JsonIgnore]
        public string StatusDisplay => Status switch
        {
            "Approved" => "Đã duyệt tham gia ✅",
            "Pending" => "Chờ duyệt tham gia ⏳",
            "Rejected" => "Bị từ chối ❌",
            "Cancelled" => "Đã hủy 🚫",
            _ => Status
        };

        [JsonIgnore]
        public string StatusColor => Status switch
        {
            "Approved" => "#10B981",
            "Pending" => "#F59E0B",
            "Rejected" => "#EF4444",
            "Cancelled" => "#6B7280",
            _ => "#6B7280"
        };

        [JsonIgnore]
        public bool CanBeApproved => Status == "Pending";
    }

    public class CampusEvent
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("icon")]
        public string Icon { get; set; } = "📅";

        [JsonPropertyName("categoryID")]
        public int CategoryID { get; set; } = 1;

        [JsonPropertyName("category")]
        public EventCategoryDto? Category { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; } = "Hội trường A2";

        [JsonPropertyName("startDate")]
        public string StartDate { get; set; } = string.Empty;

        [JsonPropertyName("endDate")]
        public string EndDate { get; set; } = string.Empty;

        [JsonPropertyName("startTime")]
        public string StartTime { get; set; } = "08:00";

        [JsonPropertyName("endTime")]
        public string EndTime { get; set; } = "11:30";

        [JsonPropertyName("maxParticipants")]
        public int MaxParticipants { get; set; } = 150;

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Sắp diễn ra";

        // Admin Approval Workflow: Mọi bài đăng phải qua Admin duyệt
        [JsonPropertyName("isApproved")]
        public bool IsApproved { get; set; } = false;

        [JsonPropertyName("approvalStatus")]
        public string ApprovalStatus { get; set; } = "Pending"; // "Approved", "Pending", "Rejected"

        [JsonPropertyName("createdByRole")]
        public string CreatedByRole { get; set; } = "Manager";

        [JsonPropertyName("createdByName")]
        public string CreatedByName { get; set; } = string.Empty;

        [JsonPropertyName("createdByUsername")]
        public string CreatedByUsername { get; set; } = string.Empty;

        [JsonPropertyName("approvedBy")]
        public string? ApprovedBy { get; set; }

        [JsonPropertyName("rejectionReason")]
        public string? RejectionReason { get; set; }

        // Phân quyền theo Lớp học & Phạm vi
        [JsonPropertyName("scope")]
        public string Scope { get; set; } = "Public"; // "Public" (Toàn trường) hoặc "ClassOnly" (Nội bộ lớp)

        [JsonPropertyName("targetClass")]
        public string TargetClass { get; set; } = "Tất cả"; // e.g. "D20CNTT1", "D21CNTT1", hoặc "Tất cả"

        // Chế độ đăng ký tham gia (Tự động hoặc Cần ban tổ chức duyệt)
        [JsonPropertyName("registrationType")]
        public string RegistrationType { get; set; } = "Auto"; // "Auto" (Tự động) hoặc "RequiresApproval" (Cần duyệt)

        // Cơ chế cộng / trừ điểm rèn luyện do ban tổ chức thiết lập
        [JsonPropertyName("pointsReward")]
        public int PointsReward { get; set; } = 10; // Điểm cộng nếu điểm danh thành công

        [JsonPropertyName("pointsPenalty")]
        public int PointsPenalty { get; set; } = 5; // Điểm trừ nếu vắng / không điểm danh

        // Trạng thái đã đăng ký của user hiện tại (phục vụ hiển thị UI và binding)
        [JsonIgnore]
        public bool IsRegistered { get; set; } = false;

        // Danh sách đăng ký tham gia
        [JsonPropertyName("registrations")]
        public List<EventRegistration> Registrations { get; set; } = new();

        // Media properties: Photos and Video
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [JsonPropertyName("imageUrls")]
        public List<string> ImageUrls { get; set; } = new();

        [JsonPropertyName("videoUrl")]
        public string VideoUrl { get; set; } = string.Empty;

        // UI Helpers
        [JsonIgnore]
        public bool IsPendingApproval => string.Equals(ApprovalStatus, "Pending", StringComparison.OrdinalIgnoreCase);

        [JsonIgnore]
        public bool CanAdminApprove { get; set; } = false;

        [JsonIgnore]
        public bool CanUserManage { get; set; } = false;

        [JsonIgnore]
        public DateTime StartDateTime => ParseDateTime(StartDate, StartTime, isEnd: false);

        [JsonIgnore]
        public DateTime EndDateTime => ParseDateTime(EndDate, EndTime, isEnd: true);

        [JsonIgnore]
        public bool IsEventEnded => DateTime.Now > EndDateTime;

        [JsonIgnore]
        public bool IsEventNotStarted => DateTime.Now < StartDateTime;

        [JsonIgnore]
        public bool IsAttendanceOpen => !IsEventEnded && !IsEventNotStarted;

        [JsonIgnore]
        public bool CanCheckIn => IsAttendanceOpen && IsApproved;

        [JsonIgnore]
        public string AttendanceTimeWindowDisplay => $"⏰ {StartTime} - {EndTime} | 📅 {(string.IsNullOrEmpty(StartDate) ? "Sắp thông báo" : StartDate)}";

        [JsonIgnore]
        public string AttendanceStatusBadge
        {
            get
            {
                if (IsEventEnded) return "Hết hạn điểm danh ⛔";
                if (IsEventNotStarted) return "Chưa mở điểm danh ⏳";
                return "Đang mở điểm danh 🟢";
            }
        }

        [JsonIgnore]
        public string AttendanceStatusBg
        {
            get
            {
                if (IsEventEnded) return "#FEE2E2";
                if (IsEventNotStarted) return "#FEF3C7";
                return "#DCFCE7";
            }
        }

        [JsonIgnore]
        public string AttendanceStatusTextColor
        {
            get
            {
                if (IsEventEnded) return "#DC2626";
                if (IsEventNotStarted) return "#D97706";
                return "#15803D";
            }
        }

        private static DateTime ParseDateTime(string dateStr, string timeStr, bool isEnd)
        {
            DateTime date = DateTime.Today;
            if (!string.IsNullOrWhiteSpace(dateStr))
            {
                string[] dateFormats = new[] { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy" };
                if (!DateTime.TryParseExact(dateStr.Trim(), dateFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    if (!DateTime.TryParse(dateStr.Trim(), out date))
                    {
                        date = DateTime.Today;
                    }
                }
            }

            TimeSpan time = isEnd ? new TimeSpan(21, 30, 0) : new TimeSpan(8, 0, 0);
            if (!string.IsNullOrWhiteSpace(timeStr))
            {
                string[] timeFormats = new[] { "hh\\:mm", "h\\:mm", "HH\\:mm", "H\\:mm", "hh\\:mm\\:ss", "HH\\:mm\\:ss" };
                if (TimeSpan.TryParseExact(timeStr.Trim(), timeFormats, System.Globalization.CultureInfo.InvariantCulture, out var parsedTime))
                {
                    time = parsedTime;
                }
                else if (TimeSpan.TryParse(timeStr.Trim(), out var fallbackTime))
                {
                    time = fallbackTime;
                }
            }

            return date.Date.Add(time);
        }

        [JsonIgnore]
        public string ApprovalStatusDisplay => ApprovalStatus switch
        {
            "Approved" => "Đã duyệt ✅",
            "Pending" => "Chờ Admin duyệt ⏳",
            "Rejected" => "Bị từ chối ❌",
            _ => "Đã duyệt ✅"
        };

        [JsonIgnore]
        public string ApprovalBadgeBg => ApprovalStatus switch
        {
            "Approved" => "#ECFDF5",
            "Pending" => "#FEF3C7",
            "Rejected" => "#FEE2E2",
            _ => "#ECFDF5"
        };

        [JsonIgnore]
        public string ApprovalBadgeTextColor => ApprovalStatus switch
        {
            "Approved" => "#059669",
            "Pending" => "#D97706",
            "Rejected" => "#DC2626",
            _ => "#059669"
        };

        [JsonIgnore]
        public string ScopeBadgeText => Scope == "ClassOnly"
            ? $"Lớp {TargetClass} 🔒"
            : "Toàn trường 🌐";

        [JsonIgnore]
        public string ScopeBadgeBg => Scope == "ClassOnly" ? "#F5F3FF" : "#EFF6FF";

        [JsonIgnore]
        public string ScopeBadgeTextColor => Scope == "ClassOnly" ? "#7C3AED" : "#2563EB";

        [JsonIgnore]
        public string RegistrationTypeDisplay => RegistrationType == "RequiresApproval"
            ? "Cần duyệt tham gia 🛡️"
            : "Tự động tham gia ⚡";

        [JsonIgnore]
        public string PointsRewardDisplay => $"+{PointsReward} ĐRL";

        [JsonIgnore]
        public string PointsPenaltyDisplay => $"-{PointsPenalty} ĐRL";

        [JsonIgnore]
        public bool HasVideo => !string.IsNullOrWhiteSpace(VideoUrl);

        [JsonIgnore]
        public bool HasImage => !string.IsNullOrWhiteSpace(ImageUrl) || (ImageUrls != null && ImageUrls.Count > 0);

        [JsonIgnore]
        public string PrimaryImageUrl => !string.IsNullOrWhiteSpace(ImageUrl)
            ? ImageUrl
            : (ImageUrls != null && ImageUrls.Count > 0 ? ImageUrls[0] : "https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&q=80");

        [JsonIgnore]
        public string CategoryTitle => !string.IsNullOrWhiteSpace(Category?.Title) ? Category.Title : "Học thuật";

        [JsonIgnore]
        public string CategoryTagBg => CategoryTitle switch
        {
            "Học thuật" => "#EFF6FF",
            "Thể thao" => "#ECFDF5",
            "Tình nguyện" => "#FEF2F2",
            "Kỹ năng mềm" => "#FAF5FF",
            "Văn hóa - Nghệ thuật" => "#F0F9FF",
            _ => "#EEF2FF"
        };

        [JsonIgnore]
        public string CategoryTagTextColor => CategoryTitle switch
        {
            "Học thuật" => "#2563EB",
            "Thể thao" => "#059669",
            "Tình nguyện" => "#DC2626",
            "Kỹ năng mềm" => "#7C3AED",
            "Văn hóa - Nghệ thuật" => "#0284C7",
            _ => "#4F46E5"
        };

        [JsonIgnore]
        public string CategoryTagBorderColor => CategoryTitle switch
        {
            "Học thuật" => "#BFDBFE",
            "Thể thao" => "#A7F3D0",
            "Tình nguyện" => "#FECACA",
            "Kỹ năng mềm" => "#E9D5FF",
            "Văn hóa - Nghệ thuật" => "#BAE6FD",
            _ => "#C7D2FE"
        };

        [JsonIgnore]
        public string StatusColor => Status switch
        {
            "Đang diễn ra" => "#10B981",
            "Sắp diễn ra" => "#2563EB",
            "Đã kết thúc" => "#6B7280",
            _ => "#2563EB"
        };

        [JsonIgnore]
        public string StatusBg => Status switch
        {
            "Đang diễn ra" => "#ECFDF5",
            "Sắp diễn ra" => "#EFF6FF",
            "Đã kết thúc" => "#F3F4F6",
            _ => "#EFF6FF"
        };

        [JsonIgnore]
        public string TimeLocationDisplay => $"📍 {Location}  •  ⏰ {StartTime}-{EndTime} ({(string.IsNullOrEmpty(StartDate) ? "Sắp thông báo" : StartDate)})";

        [JsonIgnore]
        public int RegisteredCount => Registrations?.Count(r => r.Status == "Approved" || r.Status == "Pending") ?? 0;

        [JsonIgnore]
        public string ParticipantsDisplay => $"👥 Đã đăng ký: {RegisteredCount}/{MaxParticipants}";

        [JsonIgnore]
        public string Title { get => Name; set => Name = value; }

        [JsonIgnore]
        public int TrainingPointsReward { get => PointsReward; set => PointsReward = value; }

        [JsonPropertyName("organizerName")]
        public string OrganizerName { get; set; } = "Ban Tổ Chức";

        [JsonIgnore]
        public string ScopeBadge => ScopeBadgeText;

        [JsonIgnore]
        public string ApprovalBadgeText => ApprovalStatusDisplay;

        [JsonIgnore]
        public Color ApprovalBadgeColor => ApprovalStatus switch
        {
            "Approved" => Color.FromArgb("#10B981"),
            "Pending" => Color.FromArgb("#F59E0B"),
            "Rejected" => Color.FromArgb("#EF4444"),
            _ => Color.FromArgb("#10B981")
        };

        [JsonIgnore]
        public string DateRangeFormatted => $"Từ {StartDate} đến {EndDate}";

        [JsonIgnore]
        public string TrainingPointsRewardDisplay => $"+{PointsReward} ĐRL";

        public EventRegistration? GetUserRegistration(string studentCodeOrId)
        {
            if (string.IsNullOrWhiteSpace(studentCodeOrId) || Registrations == null) return null;
            return Registrations.FirstOrDefault(r =>
                r.UserId.Equals(studentCodeOrId, StringComparison.OrdinalIgnoreCase) ||
                r.StudentCode.Equals(studentCodeOrId, StringComparison.OrdinalIgnoreCase));
        }

        public bool CanUserCancelRegistration(string studentCodeOrId)
        {
            var reg = GetUserRegistration(studentCodeOrId);
            return reg != null && reg.Status == "Pending";
        }

        public override string ToString() => Name;
    }

    public static class EventScope
    {
        public const string Public = "Public";
        public const string ClassOnly = "ClassOnly";
    }

    public static class EventRegistrationType
    {
        public const string Auto = "Auto";
        public const string RequiresApproval = "RequiresApproval";
    }
}
