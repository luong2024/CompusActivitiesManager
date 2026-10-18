using System.ComponentModel.DataAnnotations;

namespace CampusActivitiesManager.Api.Models
{
    public class EventCategoryDto
    {
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Color { get; set; } = "#2563EB";
    }

    public class EventDto
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = "📅";
        public int CategoryID { get; set; }
        public EventCategoryDto? Category { get; set; }
        public string Location { get; set; } = "Hội trường A2";
        public string StartDate { get; set; } = DateTime.Now.AddDays(3).ToString("dd/MM/yyyy");
        public string EndDate { get; set; } = DateTime.Now.AddDays(3).ToString("dd/MM/yyyy");
        public int MaxParticipants { get; set; } = 150;
        public string Status { get; set; } = "Sắp diễn ra";

        // US 13 & US 28: Admin approval workflow
        public bool IsApproved { get; set; } = true;
        public string ApprovalStatus { get; set; } = "Approved"; // "Approved", "Pending", "Rejected"
        public string CreatedByRole { get; set; } = "Admin";
        public string CreatedByName { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public string? RejectionReason { get; set; }

        // Media support: Photos & Video
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
        public string VideoUrl { get; set; } = string.Empty;
    }

    public class CreateEventRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        public string Icon { get; set; } = "📅";

        public int? CategoryId { get; set; }
        public string? Location { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? MaxParticipants { get; set; }
        public string? Status { get; set; }

        public bool? IsApproved { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? CreatedByRole { get; set; }
        public string? CreatedByName { get; set; }

        public string? ImageUrl { get; set; }
        public List<string>? ImageUrls { get; set; }
        public string? VideoUrl { get; set; }
    }

    public class UpdateEventRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public int? CategoryId { get; set; }
        public string? Location { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? MaxParticipants { get; set; }
        public string? Status { get; set; }

        public bool? IsApproved { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? ApprovedBy { get; set; }
        public string? RejectionReason { get; set; }

        public string? ImageUrl { get; set; }
        public List<string>? ImageUrls { get; set; }
        public string? VideoUrl { get; set; }
    }

    public class EventApprovalRequest
    {
        public bool IsApproved { get; set; } = true;
        public string? ApprovedBy { get; set; } = "Admin";
        public string? Reason { get; set; }
    }

    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;
        public string Color { get; set; } = "#2563EB";
    }

    public class UpdateCategoryRequest
    {
        public string? Title { get; set; }
        public string? Color { get; set; }
    }
}
