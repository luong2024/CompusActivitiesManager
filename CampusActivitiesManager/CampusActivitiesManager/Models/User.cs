using System.Text.Json.Serialization;

namespace CampusActivitiesManager.Models
{
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Student = "Student";

        public static readonly List<string> AllRoles = [Admin, Manager, Student];
    }

    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = UserRoles.Student;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        [JsonIgnore]
        public bool IsAdmin => Role == UserRoles.Admin;

        [JsonIgnore]
        public bool IsManager => Role == UserRoles.Manager;

        [JsonIgnore]
        public bool IsStudent => Role == UserRoles.Student;

        [JsonIgnore]
        public string RoleDisplayName => Role switch
        {
            UserRoles.Admin => "Quản trị viên (Admin)",
            UserRoles.Manager => "Quản lý hoạt động (Manager)",
            UserRoles.Student => "Sinh viên (Student)",
            _ => Role
        };

        [JsonIgnore]
        public string RoleBadgeColor => Role switch
        {
            UserRoles.Admin => "#FF3366", // Đỏ tím nổi bật cho Admin
            UserRoles.Manager => "#3068DF", // Xanh dương cho Manager
            UserRoles.Student => "#107C41", // Xanh lá cho Student
            _ => "#6E6E6E"
        };

        [JsonIgnore]
        public Brush RoleBadgeBrush => new SolidColorBrush(Microsoft.Maui.Graphics.Color.FromArgb(RoleBadgeColor));

        [JsonIgnore]
        public string AvatarInitials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullName))
                    return Username.Length > 0 ? Username[..1].ToUpperInvariant() : "U";

                var parts = FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                    return parts[0][..1].ToUpperInvariant();

                return $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();
            }
        }

        public override string ToString() => $"{FullName} ({Role})";
    }
}
