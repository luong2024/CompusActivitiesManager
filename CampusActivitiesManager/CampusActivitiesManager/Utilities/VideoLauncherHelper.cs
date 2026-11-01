using Microsoft.Maui.ApplicationModel;

namespace CampusActivitiesManager.Utilities
{
    /// <summary>
    /// Tiện ích hỗ trợ mở video an toàn trên đa nền tảng (Android, Windows, iOS)
    /// Tự động xử lý URL thiếu https://, video YouTube, video mp4 trên web và phát hiện file video cục bộ
    /// </summary>
    public static class VideoLauncherHelper
    {
        public static async Task OpenVideoAsync(string? rawUrl, string eventName = "Hoạt động")
        {
            if (string.IsNullOrWhiteSpace(rawUrl))
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Thông báo", "Hoạt động này chưa có liên kết video đính kèm.", "Đóng");
                }
                return;
            }

            var url = rawUrl.Trim();

            // 1. Kiểm tra nếu là đường dẫn file cục bộ trên thiết bị (local file path)
            bool isLocalPath = url.StartsWith("/") ||
                               url.StartsWith("\\") ||
                               url.StartsWith("file:", StringComparison.OrdinalIgnoreCase) ||
                               url.StartsWith("content:", StringComparison.OrdinalIgnoreCase) ||
                               (url.Length > 2 && url[1] == ':' && (url[2] == '\\' || url[2] == '/'));

            if (isLocalPath)
            {
                // Nếu file thực sự tồn tại trên máy hiện tại
                if (File.Exists(url))
                {
                    try
                    {
                        await Launcher.Default.OpenAsync(new OpenFileRequest
                        {
                            File = new ReadOnlyFile(url),
                            Title = $"Video: {eventName}"
                        });
                        return;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi mở file video cục bộ: {ex.Message}");
                    }
                }

                // File cục bộ nhưng KHÔNG tồn tại trên máy này (do tạo từ máy khác hoặc file đã xóa)
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert(
                        "Video cục bộ 📁",
                        $"Video này được đính kèm từ bộ nhớ máy của người tạo sự kiện ({url}). Tệp này không nằm trên thiết bị hiện tại của bạn.\n\n💡 Để tất cả sinh viên trên mọi máy đều xem được, Ban tổ chức nên dán đường dẫn YouTube hoặc liên kết video trực tuyến công khai.",
                        "Đã hiểu");
                }
                return;
            }

            // 2. Chuẩn hóa URL cho video trực tuyến (YouTube, MP4 web...)
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = "https://" + url;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Thông báo", $"Đường dẫn video không hợp lệ:\n{rawUrl}", "Đóng");
                }
                return;
            }

            // 3. Thử mở bằng Browser ngoài (mở thẳng App YouTube hoặc trình duyệt mặc định)
            try
            {
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.External);
                return;
            }
            catch (Exception exExternal)
            {
                System.Diagnostics.Debug.WriteLine($"Không thể mở Browser ngoài: {exExternal.Message}");
            }

            // 4. Thử mở bằng Trình duyệt tích hợp (Chrome Custom Tabs / In-App Browser)
            try
            {
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                return;
            }
            catch (Exception exSystem)
            {
                System.Diagnostics.Debug.WriteLine($"Không thể mở In-App Browser: {exSystem.Message}");
            }

            // 5. Thử mở bằng Launcher mặc định
            try
            {
                await Launcher.Default.OpenAsync(uri);
                return;
            }
            catch (Exception exLauncher)
            {
                System.Diagnostics.Debug.WriteLine($"Không thể mở Launcher: {exLauncher.Message}");
            }

            // 6. Nếu tất cả đều không có app xử lý (thiết bị tối giản không có trình duyệt)
            if (Shell.Current != null)
            {
                bool copy = await Shell.Current.DisplayAlert(
                    "Video giới thiệu",
                    $"Không tìm thấy ứng dụng xem video tương thích trên máy.\n\nĐường dẫn video:\n{url}",
                    "Sao chép liên kết", "Đóng");
                if (copy)
                {
                    await Clipboard.Default.SetTextAsync(url);
                }
            }
        }
    }
}
