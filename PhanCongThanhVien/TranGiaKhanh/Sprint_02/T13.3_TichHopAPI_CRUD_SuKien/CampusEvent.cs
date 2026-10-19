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

        // UI Helpers
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
