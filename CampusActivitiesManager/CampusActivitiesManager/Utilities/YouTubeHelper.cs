using System.Text.RegularExpressions;

namespace CampusActivitiesManager.Utilities
{
    /// <summary>
    /// Tiện ích hỗ trợ nhận diện, trích xuất mã video, tạo ảnh thu nhỏ (thumbnail) 
    /// và mã nhúng phát trực tiếp cho YouTube
    /// </summary>
    public static class YouTubeHelper
    {
        private static readonly Regex YouTubeRegex = new(
            @"(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|watch\?v=|watch\?.+&v=|shorts\/))([\w-]{11})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Trích xuất mã định danh video YouTube (11 ký tự)
        /// </summary>
        public static bool TryExtractVideoId(string? url, out string videoId)
        {
            videoId = string.Empty;
            if (string.IsNullOrWhiteSpace(url)) return false;

            var match = YouTubeRegex.Match(url.Trim());
            if (match.Success && match.Groups.Count > 1)
            {
                videoId = match.Groups[1].Value;
                return true;
            }

            return false;
        }

        public static string? ExtractVideoId(string? url)
        {
            return TryExtractVideoId(url, out var id) ? id : null;
        }

        public static bool IsYouTubeUrl(string? url)
        {
            return TryExtractVideoId(url, out _);
        }

        /// <summary>
        /// Lấy link ảnh thu nhỏ chất lượng cao tự động từ YouTube (không cần upload ảnh)
        /// </summary>
        public static string GetThumbnailUrl(string videoIdOrUrl, bool highRes = true)
        {
            string id = TryExtractVideoId(videoIdOrUrl, out var extracted) ? extracted : videoIdOrUrl.Trim();
            if (string.IsNullOrWhiteSpace(id) || id.Length != 11) return string.Empty;

            return highRes
                ? $"https://img.youtube.com/vi/{id}/hqdefault.jpg"
                : $"https://img.youtube.com/vi/{id}/default.jpg";
        }

        /// <summary>
        /// Tạo link chuẩn xem trên YouTube
        /// </summary>
        public static string GetWatchUrl(string videoIdOrUrl)
        {
            string id = TryExtractVideoId(videoIdOrUrl, out var extracted) ? extracted : videoIdOrUrl.Trim();
            return !string.IsNullOrWhiteSpace(id) ? $"https://www.youtube.com/watch?v={id}" : (videoIdOrUrl ?? "");
        }

        /// <summary>
        /// Sinh mã HTML5 iframe responsive tối ưu để phát video mượt mà trong WebView MAUI
        /// </summary>
        public static string BuildEmbedHtml(string videoId, string title = "Video Hoạt Động")
        {
            return $@"<!DOCTYPE html>
<html>
<head>
<meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'>
<style>
* {{ margin: 0; padding: 0; box-sizing: border-box; }}
html, body {{ background-color: #000000; width: 100%; height: 100%; overflow: hidden; display: flex; align-items: center; justify-content: center; }}
iframe {{ width: 100%; height: 100%; border: none; }}
</style>
</head>
<body>
<iframe src='https://www.youtube.com/embed/{videoId}?autoplay=1&playsinline=1&rel=0&modestbranding=1'
        title='{title}'
        frameborder='0'
        allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture'
        allowfullscreen></iframe>
</body>
</html>";
        }
    }
}
