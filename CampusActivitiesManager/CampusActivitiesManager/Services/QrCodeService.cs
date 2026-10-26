using QRCoder;

namespace CampusActivitiesManager.Services
{
    public class QrCodeService : IQrCodeService
    {
        public byte[] GenerateQrCodePngBytes(string payload, int pixelsPerModule = 20)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                payload = "CAMPUS-ACTIVITIES-MANAGER";
            }

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule);
        }

        public ImageSource GenerateQrCodeImageSource(string payload, int pixelsPerModule = 20)
        {
            var bytes = GenerateQrCodePngBytes(payload, pixelsPerModule);
            return ImageSource.FromStream(() => new MemoryStream(bytes));
        }

        public string GenerateTicketPayload(int eventId, string studentCode, string eventName)
        {
            var normalizedCode = string.IsNullOrWhiteSpace(studentCode) ? "STUDENT" : studentCode.Trim().ToUpperInvariant();
            var issueDate = DateTime.UtcNow.ToString("yyyyMMdd");
            // Payload format: CAMPUS|EVT:{eventId}|STU:{studentCode}|DATE:{issueDate}|HASH:{shortHash}
            var raw = $"{eventId}:{normalizedCode}:{issueDate}";
            var hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(raw)))[..8];
            return $"CAMPUS|EVT:{eventId}|STU:{normalizedCode}|DATE:{issueDate}|SIG:{hash}";
        }
    }
}
