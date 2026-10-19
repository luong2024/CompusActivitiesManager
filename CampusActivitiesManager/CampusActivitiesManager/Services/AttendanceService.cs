using System.Text;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ModalErrorHandler _errorHandler;
        private readonly ILogger<AttendanceService> _logger;

        private static readonly List<AttendanceRecord> _localRecords = new()
        {
            new AttendanceRecord
            {
                Id = "ATT-001",
                EventId = 1,
                EventName = "Hội thảo Công nghệ AI & Tương lai Lập trình viên 2026",
                StudentCode = "B20DCCN001",
                StudentName = "Nguyễn An Cương",
                CheckInTime = DateTime.UtcNow.AddDays(-2),
                Method = "QR",
                TrainingPointsAwarded = 5,
                Status = "Present",
                Notes = "Quét mã QR tại cửa hội trường"
            },
            new AttendanceRecord
            {
                Id = "ATT-002",
                EventId = 2,
                EventName = "Giải Bóng đá Sinh viên Toàn trường 2026",
                StudentCode = "B20DCCN001",
                StudentName = "Nguyễn An Cương",
                CheckInTime = DateTime.UtcNow.AddDays(-5),
                Method = "Photo",
                TrainingPointsAwarded = 5,
                Status = "Present",
                Notes = "Điểm danh kèm ảnh tham dự"
            }
        };

        private static readonly List<CampusNotificationDto> _localNotifications = new()
        {
            new CampusNotificationDto
            {
                Id = "NOTIF-01",
                Title = "Điểm danh thành công 🎉",
                Message = "Bạn đã hoàn tất điểm danh QR cho Hội thảo AI 2026 và được cộng +5 điểm rèn luyện.",
                Type = "Points",
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                IsRead = false
            },
            new CampusNotificationDto
            {
                Id = "NOTIF-02",
                Title = "Nhắc nhở sự kiện sắp bắt đầu ⏰",
                Message = "Sự kiện 'Giải Bóng đá Sinh viên' sẽ bắt đầu lúc 08:00 sáng mai tại Sân vận động trường.",
                Type = "Reminder",
                CreatedAt = DateTime.UtcNow.AddHours(-6),
                IsRead = false
            },
            new CampusNotificationDto
            {
                Id = "NOTIF-03",
                Title = "Vé điện tử QR đã sẵn sàng 🎫",
                Message = "Mã QR tham dự đã được cấp sẵn trong mục Vé QR của sự kiện.",
                Type = "Success",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                IsRead = true
            }
        };

        public AttendanceService(ModalErrorHandler errorHandler, ILogger<AttendanceService> logger)
        {
            _errorHandler = errorHandler;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, AttendanceRecord? Record)> CheckInWithQrAsync(int eventId, string studentCode, string qrPayload)
        {
            await Task.CompletedTask;

            if (string.IsNullOrWhiteSpace(qrPayload))
            {
                return (false, "Mã QR không hợp lệ hoặc rỗng.", null);
            }

            var record = new AttendanceRecord
            {
                Id = "ATT-QR-" + Guid.NewGuid().ToString("N")[..6].ToUpper(),
                EventId = eventId,
                EventName = "Sự kiện hoạt động #" + eventId,
                StudentCode = string.IsNullOrWhiteSpace(studentCode) ? "B20DCCN001" : studentCode,
                StudentName = "Sinh viên " + studentCode,
                CheckInTime = DateTime.UtcNow,
                Method = "QR",
                TrainingPointsAwarded = 5,
                Status = "Present",
                Notes = "Quét mã QR trực tiếp trên ứng dụng"
            };

            _localRecords.Insert(0, record);

            _localNotifications.Insert(0, new CampusNotificationDto
            {
                Id = "NOTIF-" + Guid.NewGuid().ToString("N")[..4],
                Title = "Điểm danh QR thành công 🎉",
                Message = $"Bạn đã điểm danh thành công sự kiện #{eventId} và nhận 5 điểm rèn luyện.",
                Type = "Points",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });

            _logger.LogInformation("QR Check-in successful for event {EventId}, student {StudentCode}", eventId, studentCode);
            return (true, "Điểm danh QR thành công! (+5 Điểm rèn luyện)", record);
        }

        public async Task<(bool Success, string Message, AttendanceRecord? Record)> CheckInWithPhotoAsync(int eventId, string studentCode, string photoBase64, string? notes)
        {
            await Task.CompletedTask;

            var record = new AttendanceRecord
            {
                Id = "ATT-IMG-" + Guid.NewGuid().ToString("N")[..6].ToUpper(),
                EventId = eventId,
                EventName = "Sự kiện hoạt động #" + eventId,
                StudentCode = string.IsNullOrWhiteSpace(studentCode) ? "B20DCCN001" : studentCode,
                StudentName = "Sinh viên " + studentCode,
                CheckInTime = DateTime.UtcNow,
                Method = "Photo",
                TrainingPointsAwarded = 5,
                Status = "Present",
                Notes = notes ?? "Điểm danh hình ảnh minh chứng camera",
                PhotoUrl = string.IsNullOrWhiteSpace(photoBase64) ? null : photoBase64
            };

            _localRecords.Insert(0, record);

            _localNotifications.Insert(0, new CampusNotificationDto
            {
                Id = "NOTIF-" + Guid.NewGuid().ToString("N")[..4],
                Title = "Điểm danh ảnh thành công 📸",
                Message = $"Minh chứng ảnh đã được ghi nhận cho sự kiện #{eventId}.",
                Type = "Success",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });

            _logger.LogInformation("Photo Check-in successful for event {EventId}, student {StudentCode}", eventId, studentCode);
            return (true, "Điểm danh kèm hình ảnh thành công!", record);
        }

        public async Task<(bool Success, string Message, AttendanceRecord? Record)> MarkManualAttendanceAsync(int eventId, string studentCode, string status, int points, string notes)
        {
            await Task.CompletedTask;

            var record = new AttendanceRecord
            {
                Id = "ATT-MAN-" + Guid.NewGuid().ToString("N")[..6].ToUpper(),
                EventId = eventId,
                EventName = "Sự kiện hoạt động #" + eventId,
                StudentCode = studentCode,
                StudentName = "Sinh viên " + studentCode,
                CheckInTime = DateTime.UtcNow,
                Method = "Manual",
                TrainingPointsAwarded = points,
                Status = string.IsNullOrWhiteSpace(status) ? "Present" : status,
                Notes = notes
            };

            _localRecords.Insert(0, record);
            _logger.LogInformation("Manual attendance recorded for event {EventId}, student {StudentCode}", eventId, studentCode);
            return (true, "Cập nhật điểm danh thành công!", record);
        }

        public async Task<List<AttendanceRecord>> GetEventAttendanceAsync(int eventId)
        {
            await Task.CompletedTask;
            return _localRecords.Where(r => r.EventId == eventId).ToList();
        }

        public async Task<List<AttendanceRecord>> GetStudentAttendanceHistoryAsync(string studentCode)
        {
            await Task.CompletedTask;
            if (string.IsNullOrWhiteSpace(studentCode))
                return _localRecords.ToList();

            return _localRecords.Where(r =>
                string.Equals(r.StudentCode, studentCode, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<string> ExportAttendanceCsvAsync(int eventId)
        {
            await Task.CompletedTask;
            var list = _localRecords.Where(r => r.EventId == eventId).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("ID,EventID,StudentCode,StudentName,CheckInTime,Method,Status,TrainingPoints,Notes");

            foreach (var r in list)
            {
                sb.AppendLine($"\"{r.Id}\",{r.EventId},\"{r.StudentCode}\",\"{r.StudentName}\",\"{r.CheckInTime:yyyy-MM-dd HH:mm:ss}\",\"{r.Method}\",\"{r.Status}\",{r.TrainingPointsAwarded},\"{r.Notes}\"");
            }

            return sb.ToString();
        }

        public async Task<List<CampusNotificationDto>> GetNotificationsAsync(string? studentCode = null)
        {
            await Task.CompletedTask;
            return _localNotifications.ToList();
        }
    }
}
