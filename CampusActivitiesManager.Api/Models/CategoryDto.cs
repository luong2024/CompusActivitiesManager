namespace CampusActivitiesManager.Api.Models
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Alias for Name to ensure compatibility with MAUI Category.Title model
        /// </summary>
        public string Title
        {
            get => Name;
            set => Name = value;
        }

        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = "#3068DF";
        public string? Icon { get; set; }
    }
}
