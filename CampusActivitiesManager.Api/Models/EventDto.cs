namespace CampusActivitiesManager.Api.Models
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Alias for Title to match MAUI Project.Name convention
        /// </summary>
        public string Name
        {
            get => Title;
            set => Title = value;
        }

        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Upcoming";
        public int MaxParticipants { get; set; } = 100;
        public int CurrentParticipants { get; set; } = 0;
        public string? BannerUrl { get; set; }

        /// <summary>
        /// Foreign key to Category. Nullable: Event may have no category (AC3).
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Detailed Category information. Nullable: Event may have no category (AC2 & AC3).
        /// </summary>
        public CategoryDto? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
