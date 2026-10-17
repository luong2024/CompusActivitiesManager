namespace CampusActivitiesManager.Api.Models
{
    public class LoginResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
