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

    public class CreateEventRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        public string Icon { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
    }

    public class UpdateEventRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public int? CategoryId { get; set; }
    }
}
