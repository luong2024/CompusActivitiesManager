using System.ComponentModel.DataAnnotations;

namespace CampusActivitiesManager.Api.Models
{
    public class CreateAccountRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "DisplayName is required")]
        public string DisplayName { get; set; } = null!;
    }
}
