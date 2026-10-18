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

        [JsonPropertyName("maxParticipants")]
        public int MaxParticipants { get; set; } = 150;

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Sắp diễn ra";

        // Admin Approval Workflow
        [JsonPropertyName("isApproved")]
        public bool IsApproved { get; set; } = true;

        [JsonPropertyName("approvalStatus")]
        public string ApprovalStatus { get; set; } = "Approved"; // "Approved", "Pending", "Rejected"

        [JsonPropertyName("createdByRole")]
        public string CreatedByRole { get; set; } = "Admin";

        [JsonPropertyName("createdByName")]
        public string CreatedByName { get; set; } = string.Empty;

        [JsonPropertyName("approvedBy")]
        public string? ApprovedBy { get; set; }

        [JsonPropertyName("rejectionReason")]
        public string? RejectionReason { get; set; }

        // Media properties: Photos and Video
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [JsonPropertyName("imageUrls")]
        public List<string> ImageUrls { get; set; } = new();

        [JsonPropertyName("videoUrl")]
        public string VideoUrl { get; set; } = string.Empty;

        // UI Helpers
        [JsonIgnore]
        public string ApprovalStatusDisplay => ApprovalStatus switch
        {
            "Approved" => "Đã duyệt ✅",
            "Pending" => "Chờ phê duyệt ⏳",
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
            "Học thuật" => "#DBEAFE",
            "Thể thao" => "#D1FAE5",
            "Tình nguyện" => "#FEE2E2",
            "Kỹ năng mềm" => "#F3E8FF",
            "Văn hóa - Nghệ thuật" => "#BAE6FD",
            _ => "#E0E7FF"
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
        public string TimeLocationDisplay => $"📍 {Location}  •  📅 {(string.IsNullOrEmpty(StartDate) ? "Sắp thông báo" : StartDate)}";

        [JsonIgnore]
        public string ParticipantsDisplay => $"👥 Tối đa {MaxParticipants} sinh viên";

        [JsonIgnore]
        public bool IsRegistered { get; set; } = false;

        [JsonIgnore]
        public string RegisterButtonText => IsRegistered ? "🎫 Xem Vé QR" : "Đăng Ký Tham Gia";

        [JsonIgnore]
        public int TrainingPointsReward => CategoryTitle switch
        {
            "Học thuật" => 5,
            "Tình nguyện" => 10,
            "Thể thao" => 5,
            "Kỹ năng mềm" => 4,
            _ => 3
        };

        [JsonIgnore]
        public string TrainingPointsRewardDisplay => $"+{TrainingPointsReward} ĐRL";

        public override string ToString() => Name;
    }
}
