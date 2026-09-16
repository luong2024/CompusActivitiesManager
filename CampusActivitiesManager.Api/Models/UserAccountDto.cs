namespace CampusActivitiesManager.Api.Models
{
    public class UserAccountDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Student";
        public string? PhoneNumber { get; set; }
        public string? StudentCode { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsActive { get; set; } = true;
        public string CreatedAt { get; set; } = string.Empty;
        public string? UpdatedAt { get; set; }
    }
}
