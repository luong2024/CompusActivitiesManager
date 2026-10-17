using System.Text.Json.Serialization;

namespace CampusActivitiesManager.Models
{
    public class CampusPost
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("excerpt")]
        public string Excerpt { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = "Tin tức"; // "Tin tức", "Gương sáng", "Hướng dẫn", "Hoạt động"

        [JsonPropertyName("author")]
        public string Author { get; set; } = "Ban Truyền thông Đoàn trường";

        [JsonPropertyName("publishDate")]
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("coverImageUrl")]
        public string CoverImageUrl { get; set; } = "https://picsum.photos/600/320";

        [JsonPropertyName("readTimeMinutes")]
        public int ReadTimeMinutes { get; set; } = 3;

        [JsonPropertyName("likesCount")]
        public int LikesCount { get; set; } = 42;

        [JsonPropertyName("isLiked")]
        public bool IsLiked { get; set; }

        [JsonIgnore]
        public string FormattedDate => PublishDate.ToString("dd/MM/yyyy");

        [JsonIgnore]
        public string CategoryBg => Category switch
        {
            "Tin tức" => "#EFF6FF",
            "Gương sáng" => "#FEF3C7",
            "Hướng dẫn" => "#ECFDF5",
            "Hoạt động" => "#FAF5FF",
            _ => "#EFF6FF"
        };

        [JsonIgnore]
        public Color CategoryTextColor => Category switch
        {
            "Tin tức" => Color.FromArgb("#2563EB"),
            "Gương sáng" => Color.FromArgb("#D97706"),
            "Hướng dẫn" => Color.FromArgb("#059669"),
            "Hoạt động" => Color.FromArgb("#7C3AED"),
            _ => Color.FromArgb("#2563EB")
        };
    }
}
