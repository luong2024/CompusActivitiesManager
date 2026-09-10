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

        public override string ToString() => $"{Name}";
    }

    public class ProjectsJson
    {
        public List<Project> Projects { get; set; } = [];
    }
}