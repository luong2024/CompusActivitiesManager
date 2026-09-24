using System.ComponentModel.DataAnnotations;

namespace CampusActivitiesManager.Api.Models
{
    public class EventCategoryDto {
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Color { get; set; } = "#FF0000";
    }

    public class EventDto {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public EventCategoryDto? Category { get; set; }
    }

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
