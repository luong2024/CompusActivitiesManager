using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;

namespace CampusActivitiesManager.Utilities
{
    /// <summary>
    /// Tiện ích nén ảnh và chuyển đổi ảnh sang Base64 lưu trực tiếp lên Cloud Firestore
    /// Tự động thu nhỏ ảnh về kích thước chuẩn (720px) và tối ưu dung lượng (khoảng 25KB - 40KB)
    /// </summary>
    public static class ImageCompressor
    {
        public static async Task<string> CompressAndConvertToBase64Async(FileResult file, int maxDimension = 720, float quality = 0.75f)
        {
            if (file == null) return string.Empty;

            try
            {
                using var inStream = await file.OpenReadAsync();
                var image = PlatformImage.FromStream(inStream);
                if (image != null)
                {
                    var downsized = image.Downsize(maxDimension, true);
                    using var outStream = new MemoryStream();
                    downsized.Save(outStream, ImageFormat.Jpeg, quality);
                    var bytes = outStream.ToArray();
                    return "data:image/jpeg;base64," + Convert.ToBase64String(bytes);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error downsizing image: {ex.Message}");
            }

            // Fallback: Đọc trực tiếp dữ liệu nếu bộ nén đồ họa không hỗ trợ định dạng
            try
            {
                using var inStream = await file.OpenReadAsync();
                using var ms = new MemoryStream();
                await inStream.CopyToAsync(ms);
                return "data:image/jpeg;base64," + Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading image bytes: {ex.Message}");
                return string.Empty;
            }
        }

        public static ImageSource? ConvertToImageSource(string? imageUrlOrBase64)
        {
            if (string.IsNullOrWhiteSpace(imageUrlOrBase64)) return null;

            var str = imageUrlOrBase64.Trim();

            if (str.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    int commaIdx = str.IndexOf(',');
                    string base64 = commaIdx >= 0 ? str.Substring(commaIdx + 1) : str;
                    byte[] bytes = Convert.FromBase64String(base64);
                    return ImageSource.FromStream(() => new MemoryStream(bytes));
                }
                catch { }
            }

            if (str.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                str.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                if (Uri.TryCreate(str, UriKind.Absolute, out var uri))
                {
                    return ImageSource.FromUri(uri);
                }
            }

            if (File.Exists(str))
            {
                return ImageSource.FromFile(str);
            }

            return null;
        }
    }
}
