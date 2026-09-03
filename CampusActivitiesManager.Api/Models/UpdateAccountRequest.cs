using System.ComponentModel.DataAnnotations;

namespace CampusActivitiesManager.Api.Models
{
    public class UpdateAccountRequest
    {
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Full name cannot be blank")]
        public string? FullName { get; set; }
        
        public string? PhoneNumber { get; set; }
        
        public string? AvatarUrl { get; set; }

        [RegularExpression(@"^(Admin|Manager|User|Guest|Lecturer|Student)$", ErrorMessage = "Invalid user role specified")]
        public string? Role { get; set; }
    }
}
