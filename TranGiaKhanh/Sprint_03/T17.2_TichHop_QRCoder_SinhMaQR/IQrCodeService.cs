namespace CampusActivitiesManager.Services
{
    public interface IQrCodeService
    {
        /// <summary>
        /// Tạo hình ảnh mã QR thực tế từ chuỗi payload bất kỳ
        /// </summary>
        byte[] GenerateQrCodePngBytes(string payload, int pixelsPerModule = 20);

        /// <summary>
        /// Trả về ImageSource cho MAUI View hiển thị mã QR trực tiếp
        /// </summary>
        ImageSource GenerateQrCodeImageSource(string payload, int pixelsPerModule = 20);

        /// <summary>
        /// Tạo mã định danh vé QR tiêu chuẩn bảo mật cho sinh viên và sự kiện
        /// </summary>
        string GenerateTicketPayload(int eventId, string studentCode, string eventName);
    }
}
