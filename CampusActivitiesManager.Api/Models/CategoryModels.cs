using System.ComponentModel.DataAnnotations;

namespace CampusActivitiesManager.Api.Models
{
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;
        public string Color { get; set; } = "#FF0000";
    }

    public class UpdateCategoryRequest
    {
        public string? Title { get; set; }
        public string? Color { get; set; }
    }
}
