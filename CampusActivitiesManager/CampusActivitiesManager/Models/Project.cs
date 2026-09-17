using System.Text.Json.Serialization;

namespace CampusActivitiesManager.Models
{
    public class Project
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        [JsonIgnore]
        public int CategoryID { get; set; }

        public Category? Category { get; set; }

        public List<ProjectTask> Tasks { get; set; } = [];

        public List<Tag> Tags { get; set; } = [];
        
        [JsonIgnore]
        public bool IsRegistered { get; set; } = false;

        [JsonIgnore]
        public string RegisterButtonText => IsRegistered ? "🎫 Xem Vé QR" : "Đăng Ký Tham Gia";

        [JsonIgnore]
        public string CategoryTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(Category?.Title))
                    return Category.Title;
                if (Tags?.Count > 0 && !string.IsNullOrEmpty(Tags[0].Title))
                    return Tags[0].Title;
                if (Name.Contains("AI") || Name.Contains("Hội Thảo"))
                    return "Học thuật";
                if (Name.Contains("Bóng Đá") || Name.Contains("Thể thao"))
                    return "Thể thao";
                if (Name.Contains("Tình Nguyện") || Name.Contains("Mùa Hè Xanh"))
                    return "Tình nguyện";
                if (Name.Contains("Job Fair") || Name.Contains("Việc Làm") || Name.Contains("Kỹ năng"))
                    return "Kỹ năng";
                return "Học thuật";
            }
        }

        [JsonIgnore]
        public string CategoryTagBg => CategoryTitle switch
        {
            "Học thuật" => "#EFF6FF",
            "Thể thao" => "#ECFDF5",
            "Tình nguyện" => "#FEF2F2",
            "Kỹ năng" => "#FAF5FF",
            _ => "#EEF2FF"
        };

        [JsonIgnore]
        public string CategoryTagTextColor => CategoryTitle switch
        {
            "Học thuật" => "#2563EB",
            "Thể thao" => "#059669",
            "Tình nguyện" => "#DC2626",
            "Kỹ năng" => "#7C3AED",
            _ => "#4F46E5"
        };

        [JsonIgnore]
        public string CategoryTagBorderColor => CategoryTitle switch
        {
            "Học thuật" => "#DBEAFE",
            "Thể thao" => "#D1FAE5",
            "Tình nguyện" => "#FEE2E2",
            "Kỹ năng" => "#F3E8FF",
            _ => "#E0E7FF"
        };

        public override string ToString() => $"{Name}";
    }

    public class ProjectsJson
    {
        public List<Project> Projects { get; set; } = [];
    }
}