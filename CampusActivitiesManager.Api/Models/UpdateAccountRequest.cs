using System.ComponentModel.DataAnnotations;

namespace CampusActivitiesManager.Api.Models
{
    public class UpdateAccountRequest
    {
        public string? DisplayName { get; set; }
        
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string? Password { get; set; }
        
        public string? PhoneNumber { get; set; }
    }
}
