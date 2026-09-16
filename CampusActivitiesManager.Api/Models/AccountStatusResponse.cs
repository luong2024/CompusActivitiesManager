namespace CampusActivitiesManager.Api.Models
{
    public class AccountStatusResponse
    {
        public string Id { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public string Message { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;
    }
}
