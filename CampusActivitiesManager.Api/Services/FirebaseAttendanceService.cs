using System.Collections.Concurrent;
using System.Text;
using CampusActivitiesManager.Api.Models;

namespace CampusActivitiesManager.Api.Services
{
    public class FirebaseAttendanceService : IFirebaseAttendanceService
    {
        private static readonly ConcurrentBag<AttendanceRecord> _records = new()
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
                Notes = "Quét mã QR thành công tại cửa hội trường A2"
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
                Notes = "Điểm danh bằng ảnh tham dự sân bóng D2"
            },
            new AttendanceRecord
            {
                Id = "ATT-003",
                EventId = 1,
                EventName = "Hội thảo Công nghệ AI & Tương lai Lập trình viên 2026",
                StudentCode = "B21DCCN045",
                StudentName = "Trần Thị Mai",
                CheckInTime = DateTime.UtcNow.AddDays(-2).AddMinutes(15),
                Method = "QR",
                TrainingPointsAwarded = 5,
                Status = "Present",
                Notes = "Check-in QR"
            },
            new AttendanceRecord
            {
                Id = "ATT-004",
                EventId = 3,
                EventName = "Chiến dịch Tình nguyện Mùa Hè Xanh 2026",
                StudentCode = "B20DCCN001",
                StudentName = "Nguyễn An Cương",
                CheckInTime = DateTime.UtcNow.AddDays(-10),
                Method = "Manual",
                TrainingPointsAwarded = 8,
                Status = "Present",
                Notes = "Ban tổ chức điểm danh theo danh sách đoàn đội"
            }
        };

        private static readonly ConcurrentBag<CampusNotificationDto> _notifications = new()
        {
            new CampusNotificationDto
            {
                Id = "NOTIF-01",
                Title = "Điểm danh thành công 🎉",
                Message = "Bạn đã được ghi nhận tham gia 'Hội thảo AI 2026' và được cộng +5 điểm rèn luyện.",
                Type = "Points",
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                IsRead = false
            },
            new CampusNotificationDto
            {
                Id = "NOTIF-02",
                Title = "Nhắc nhở sự kiện sắp diễn ra ⏰",
                Message = "Sự kiện 'Giải Bóng đá Sinh viên' sẽ bắt đầu lúc 08:00 ngày mai tại Sân vận động trường.",
                Type = "Reminder",
                CreatedAt = DateTime.UtcNow.AddHours(-6),
                IsRead = false
            },
            new CampusNotificationDto
            {
                Id = "NOTIF-03",
                Title = "Vé QR điện tử của bạn đã sẵn sàng 🎫",
                Message = "Mã QR tham dự đã được phát hành cho sự kiện Workshop Kỹ Năng Mềm.",
                Type = "Success",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                IsRead = true
            }
        };

        private readonly ILogger<FirebaseAttendanceService>? _logger;

        public FirebaseAttendanceService(ILogger<FirebaseAttendanceService>? logger = null)
        {
            _logger = logger;
        }

        public Task<AttendanceRecord> CheckInWithQrAsync(QrCheckInRequest request)
        {
            // Kiểm tra trùng lặp: nếu sinh viên đã điểm danh sự kiện này rồi thì trả về bản ghi cũ
            var existing = _records.FirstOrDefault(r => r.EventId == request.EventId &&
                                                        string.Equals(r.StudentCode, request.StudentCode, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                _logger?.LogInformation("Sinh viên {Code} đã điểm danh sự kiện {EventId} trước đó", request.StudentCode, request.EventId);
                return Task.FromResult(existing);
            }

            var record = new AttendanceRecord
            {
                Id = "ATT-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(),
                EventId = request.EventId,
                EventName = GetEventName(request.EventId),
                StudentCode = request.StudentCode.Trim().ToUpperInvariant(),
                StudentName = GetStudentName(request.StudentCode),
                CheckInTime = request.DeviceTimestamp ?? DateTime.UtcNow,
                Method = "QR",
                Notes = $"Quét vé QR: {request.QrTicketCode}",
                TrainingPointsAwarded = 5,
                Status = "Present"
            };

            _records.Add(record);
            AddPointsNotification(record);
            return Task.FromResult(record);
        }

        public Task<AttendanceRecord> CheckInWithPhotoAsync(PhotoCheckInRequest request)
        {
            var existing = _records.FirstOrDefault(r => r.EventId == request.EventId &&
                                                        string.Equals(r.StudentCode, request.StudentCode, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                // Cập nhật ảnh minh chứng nếu có
                if (!string.IsNullOrEmpty(request.PhotoUrl))
                {
                    existing.PhotoUrl = request.PhotoUrl;
                }
                return Task.FromResult(existing);
            }

            var record = new AttendanceRecord
            {
                Id = "ATT-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(),
                EventId = request.EventId,
                EventName = GetEventName(request.EventId),
                StudentCode = request.StudentCode.Trim().ToUpperInvariant(),
                StudentName = GetStudentName(request.StudentCode),
                CheckInTime = request.DeviceTimestamp ?? DateTime.UtcNow,
                Method = "Photo",
                PhotoUrl = request.PhotoUrl,
                Notes = string.IsNullOrWhiteSpace(request.Notes) ? "Điểm danh kèm hình ảnh minh chứng camera" : request.Notes,
                TrainingPointsAwarded = 5,
                Status = "Present"
            };

            _records.Add(record);
            AddPointsNotification(record);
            return Task.FromResult(record);
        }

        public Task<AttendanceRecord> MarkManualAttendanceAsync(ManualAttendanceRequest request)
        {
            var existing = _records.FirstOrDefault(r => r.EventId == request.EventId &&
                                                        string.Equals(r.StudentCode, request.StudentCode, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Status = request.Status;
                existing.Notes = request.Notes;
                return Task.FromResult(existing);
            }

            var record = new AttendanceRecord
            {
                Id = "ATT-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(),
                EventId = request.EventId,
                EventName = GetEventName(request.EventId),
                StudentCode = request.StudentCode.Trim().ToUpperInvariant(),
                StudentName = GetStudentName(request.StudentCode),
                CheckInTime = DateTime.UtcNow,
                Method = "Manual",
                Notes = request.Notes,
                TrainingPointsAwarded = request.TrainingPoints > 0 ? request.TrainingPoints : 5,
                Status = request.Status
            };

            _records.Add(record);
            return Task.FromResult(record);
        }

        public Task<List<AttendanceRecord>> GetEventAttendanceAsync(int eventId)
        {
            var list = _records.Where(r => r.EventId == eventId)
                               .OrderByDescending(r => r.CheckInTime)
                               .ToList();
            return Task.FromResult(list);
        }

        public Task<List<AttendanceRecord>> GetStudentAttendanceHistoryAsync(string studentCode)
        {
            var code = studentCode.Trim();
            var list = _records.Where(r => string.Equals(r.StudentCode, code, StringComparison.OrdinalIgnoreCase))
                               .OrderByDescending(r => r.CheckInTime)
                               .ToList();
            return Task.FromResult(list);
        }

        public Task<string> ExportAttendanceCsvAsync(int eventId)
        {
            var list = _records.Where(r => r.EventId == eventId).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Mã Điểm Danh,Mã Sự Kiện,Tên Sự Kiện,Mã Sinh Viên,Họ và Tên,Thời Gian Điểm Danh,Hình Thức,Trạng Thái,Điểm Rèn Luyện,Ghi Chú");

            foreach (var r in list)
            {
                sb.AppendLine($"\"{r.Id}\",\"{r.EventId}\",\"{r.EventName}\",\"{r.StudentCode}\",\"{r.StudentName}\",\"{r.CheckInTime:dd/MM/yyyy HH:mm:ss}\",\"{r.Method}\",\"{r.Status}\",\"{r.TrainingPointsAwarded}\",\"{r.Notes}\"");
            }

            return Task.FromResult(sb.ToString());
        }

        public Task<List<CampusNotificationDto>> GetNotificationsAsync(string? studentCode = null)
        {
            return Task.FromResult(_notifications.OrderByDescending(n => n.CreatedAt).ToList());
        }

        private static string GetEventName(int eventId) => eventId switch
        {
            1 => "Hội thảo Công nghệ AI & Tương lai Lập trình viên 2026",
            2 => "Giải Bóng đá Sinh viên Toàn trường 2026",
            3 => "Chiến dịch Tình nguyện Mùa Hè Xanh 2026",
            4 => "Workshop Kỹ năng Thuyết trình & Lãnh đạo",
            _ => $"Sự kiện học đường #{eventId}"
        };

        private static string GetStudentName(string code) => code.ToUpperInvariant() switch
        {
            "B20DCCN001" => "Nguyễn An Cương",
            "B21DCCN045" => "Trần Thị Mai",
            "ADMIN01" => "Quản Trị Viên",
            _ => "Sinh viên " + code.ToUpperInvariant()
        };

        private static void AddPointsNotification(AttendanceRecord record)
        {
            _notifications.Add(new CampusNotificationDto
            {
                Id = "NOTIF-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant(),
                Title = "Điểm danh thành công 🎉",
                Message = $"Bạn đã hoàn tất điểm danh [{record.Method}] cho '{record.EventName}' và được cộng +{record.TrainingPointsAwarded} điểm rèn luyện.",
                Type = "Points",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });
        }
    }
}
