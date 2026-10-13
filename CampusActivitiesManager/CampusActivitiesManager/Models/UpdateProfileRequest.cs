namespace CampusActivitiesManager.Models
{
    public class UpdateProfileRequest
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }
}
