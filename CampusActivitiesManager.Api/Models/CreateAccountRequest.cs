using System.ComponentModel.DataAnnotations;

namespace CampusActivitiesManager.Api.Models
{
    public class CreateAccountRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 chars")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&^#\-_+=<>~`|/]).{8,}$", 
            ErrorMessage = "Password must contain uppercase, lowercase, numeric, and special characters.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Full name cannot be blank")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Full name cannot be blank")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression(@"^(Admin|Manager|User|Guest|Lecturer|Student)$", ErrorMessage = "Invalid user role specified")]
        public string Role { get; set; } = null!;

        public string? PhoneNumber { get; set; }
        
        public string? StudentCode { get; set; }
    }
}
