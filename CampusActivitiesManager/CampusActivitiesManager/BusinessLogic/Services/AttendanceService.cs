using CampusActivitiesManager.BusinessLogic.Interfaces;
using System.Text;
using System.Text.Json;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.BusinessLogic.Services
{
    public class AttendanceService : IAttendanceService
    {
        private static readonly string AttendanceFilePath = Path.Combine(FileSystem.AppDataDirectory, "campus_attendance.json");
        private static readonly string NotificationsFilePath = Path.Combine(FileSystem.AppDataDirectory, "campus_notifications.json");

        private static readonly SemaphoreSlim _fileLock = new(1, 1);
        private static bool _isLoadedFromFile = false;

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
                Status = "Approved",
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
                Status = "Approved",
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
                TargetUsername = "B20DCCN001",
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                IsRead = false
            },
            new CampusNotificationDto
            {
                Id = "NOTIF-02",
                Title = "Nhắc nhở sự kiện sắp bắt đầu ⏰",
                Message = "Sự kiện 'Giải Bóng đá Sinh viên' sẽ bắt đầu lúc 08:00 sáng mai tại Sân vận động trường.",
                Type = "Reminder",
                TargetUsername = "ALL",
                CreatedAt = DateTime.UtcNow.AddHours(-6),
                IsRead = false
            },
            new CampusNotificationDto
            {
                Id = "NOTIF-03",
                Title = "Vé điện tử QR đã sẵn sàng 🎫",
                Message = "Mã QR tham dự đã được cấp sẵn trong mục Vé QR của sự kiện.",
                Type = "Success",
                TargetUsername = "B20DCCN001",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                IsRead = true
            }
        };

        private readonly IUserService _userService;
        private readonly IEventService _eventService;
        private readonly ModalErrorHandler _errorHandler;
        private readonly ILogger<AttendanceService> _logger;
        private readonly IFirebaseSyncService? _firebaseSync;

        public AttendanceService(
            IUserService userService,
            IEventService eventService,
            ModalErrorHandler errorHandler,
            ILogger<AttendanceService> logger,
            IFirebaseSyncService? firebaseSync = null)
        {
            _userService = userService;
            _eventService = eventService;
            _errorHandler = errorHandler;
            _logger = logger;
            _firebaseSync = firebaseSync;
        }

        private async Task EnsureLoadedAsync()
        {
            if (_isLoadedFromFile) return;

            await _fileLock.WaitAsync();
            try
            {
                if (_isLoadedFromFile) return;

                // 1. Tải điểm danh
                if (File.Exists(AttendanceFilePath))
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(AttendanceFilePath);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            var list = JsonSerializer.Deserialize<List<AttendanceRecord>>(json);
                            if (list != null && list.Count > 0)
                            {
                                _localRecords.Clear();
                                _localRecords.AddRange(list);
                            }
                        }
                    }
                    catch { }
                }
                else
                {
                    if (_firebaseSync != null && _firebaseSync.IsConfigured)
                    {
                        try
                        {
                            var cloud = await _firebaseSync.PullAttendanceAsync();
                            if (cloud != null && cloud.Count > 0)
                            {
                                _localRecords.Clear();
                                _localRecords.AddRange(cloud);
                            }
                        }
                        catch { }
                    }
                    await SaveRecordsToDiskInternalAsync();
                }

                // 2. Tải thông báo
                if (File.Exists(NotificationsFilePath))
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(NotificationsFilePath);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            var list = JsonSerializer.Deserialize<List<CampusNotificationDto>>(json);
                            if (list != null && list.Count > 0)
                            {
                                _localNotifications.Clear();
                                _localNotifications.AddRange(list);
                            }
                        }
                    }
                    catch { }
                }
                else if (_firebaseSync != null && _firebaseSync.IsConfigured)
                {
                    try
                    {
                        var cloudNotifs = await _firebaseSync.PullNotificationsAsync();
                        if (cloudNotifs != null && cloudNotifs.Count > 0)
                        {
                            _localNotifications.Clear();
                            _localNotifications.AddRange(cloudNotifs);
                        }
                    }
                    catch { }
                }

                _isLoadedFromFile = true;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        private static async Task SaveRecordsToDiskInternalAsync()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_localRecords, options);
                await File.WriteAllTextAsync(AttendanceFilePath, json);

                var jsonNotifs = JsonSerializer.Serialize(_localNotifications, options);
                await File.WriteAllTextAsync(NotificationsFilePath, jsonNotifs);
            }
            catch { }
        }

        public async Task SaveRecordsToDiskAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                await SaveRecordsToDiskInternalAsync();
            }
            finally
            {
                _fileLock.Release();
            }

            if (_firebaseSync != null && _firebaseSync.IsConfigured)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _firebaseSync.PushAttendanceAsync(_localRecords);
                        await _firebaseSync.PushNotificationsAsync(_localNotifications);
                    }
                    catch { }
                });
            }
        }

        public async Task<(bool Success, string Message, AttendanceRecord? Record)> CheckInWithQrAsync(int eventId, string studentCode, string qrPayload)
        {
            await EnsureLoadedAsync();

            if (string.IsNullOrWhiteSpace(qrPayload))
            {
                return (false, "Mã QR không hợp lệ hoặc rỗng.", null);
            }

            var evt = await _eventService.GetEventByIdAsync(eventId);
            if (evt == null)
            {
                return (false, "Không tìm thấy thông tin sự kiện để điểm danh.", null);
            }

            if (!evt.IsApproved || evt.ApprovalStatus != "Approved")
            {
                return (false, "Hoạt động này đang chờ Quản trị viên duyệt, chưa mở chức năng điểm danh.", null);
            }

            if (evt.IsEventEnded)
            {
                return (false, $"Sự kiện đã kết thúc vào lúc {evt.EndTime} ngày {evt.EndDate}. Hệ thống đã khóa điểm danh sau thời gian kết thúc!", null);
            }

            if (evt.IsEventNotStarted)
            {
                return (false, $"Sự kiện chưa bắt đầu (diễn ra từ {evt.StartTime} đến {evt.EndTime} ngày {evt.StartDate}). Vui lòng quay lại trong khung giờ tổ chức!", null);
            }

            var cleanStudent = string.IsNullOrWhiteSpace(studentCode) ? "B20DCCN001" : studentCode.Trim().ToUpperInvariant();

            // Kiểm tra trùng lặp điểm danh
            var existing = _localRecords.FirstOrDefault(r =>
                r.EventId == eventId &&
                string.Equals(r.StudentCode, cleanStudent, StringComparison.OrdinalIgnoreCase) &&
                (r.Status == "Approved" || r.Status == "Present" || r.Status == "PendingReview"));

            if (existing != null)
            {
                return (false, "Sinh viên đã được điểm danh sự kiện này trước đó!", existing);
            }

            int reward = evt.PointsReward > 0 ? evt.PointsReward : 10;
            string evtTitle = evt.Title ?? ("Sự kiện hoạt động #" + eventId);

            var record = new AttendanceRecord
            {
                Id = "ATT-QR-" + Guid.NewGuid().ToString("N")[..6].ToUpper(),
                EventId = eventId,
                EventName = evtTitle,
                StudentCode = cleanStudent,
                StudentName = "Sinh viên " + cleanStudent,
                CheckInTime = DateTime.UtcNow,
                Method = "QR",
                TrainingPointsAwarded = reward,
                Status = "Approved",
                Notes = "Điểm danh qua mã QR sự kiện"
            };

            _localRecords.Insert(0, record);

            // Cộng điểm rèn luyện trực tiếp cho sinh viên
            await _userService.AdjustTrainingPointsAsync(cleanStudent, reward);

            await AddNotificationAsync(new CampusNotificationDto
            {
                Id = "NOTIF-" + Guid.NewGuid().ToString("N")[..6].ToUpper(),
                Title = "Điểm danh QR thành công 🎉",
                Message = $"Bạn đã điểm danh thành công cho sự kiện '{evtTitle}' và được cộng +{reward} ĐRL.",
                Type = "Points",
                TargetUsername = cleanStudent,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });

            await SaveRecordsToDiskAsync();
            _logger.LogInformation("QR check-in completed for event {EventId}, student {StudentCode}. Awarded +{Points} points", eventId, cleanStudent, reward);
            return (true, $"Điểm danh thành công! Bạn nhận được +{reward} điểm rèn luyện.", record);
        }

        public async Task<(bool Success, string Message, AttendanceRecord? Record)> CheckInWithPhotoAsync(int eventId, string studentCode, string photoBase64, string? notes)
        {
            await EnsureLoadedAsync();

            var evt = await _eventService.GetEventByIdAsync(eventId);
            if (evt == null)
            {
                return (false, "Không tìm thấy thông tin sự kiện để điểm danh.", null);
            }

            if (!evt.IsApproved)
            {
                return (false, "Hoạt động này đang chờ Quản trị viên duyệt, chưa mở chức năng điểm danh.", null);
            }

            if (evt.IsEventEnded)
            {
                return (false, $"Sự kiện đã kết thúc vào lúc {evt.EndTime} ngày {evt.EndDate}. Hệ thống đã khóa điểm danh sau thời gian kết thúc!", null);
            }

            if (evt.IsEventNotStarted)
            {
                return (false, $"Sự kiện chưa bắt đầu (diễn ra từ {evt.StartTime} đến {evt.EndTime} ngày {evt.StartDate}). Vui lòng quay lại trong khung giờ tổ chức!", null);
            }

            string evtTitle = evt.Title ?? ("Sự kiện hoạt động #" + eventId);
            int reward = evt.PointsReward;
            int penalty = evt.PointsPenalty;
            string cleanStudent = string.IsNullOrWhiteSpace(studentCode) ? "B20DCCN001" : studentCode.Trim().ToUpperInvariant();

            var record = new AttendanceRecord
            {
                Id = "ATT-IMG-" + Guid.NewGuid().ToString("N")[..6].ToUpper(),
                EventId = eventId,
                EventName = evtTitle,
                StudentCode = cleanStudent,
                StudentName = "Sinh viên " + cleanStudent,
                CheckInTime = DateTime.UtcNow,
                Method = "Photo",
                TrainingPointsAwarded = reward,
                PointsDeducted = penalty,
                Status = "PendingReview", // Chờ Admin hoặc Quản lý sự kiện duyệt!
                Notes = notes ?? "Điểm danh hình ảnh minh chứng camera",
                PhotoUrl = string.IsNullOrWhiteSpace(photoBase64) ? null : photoBase64
            };

            _localRecords.Insert(0, record);

            await AddNotificationAsync(new CampusNotificationDto
            {
                Id = "NOTIF-" + Guid.NewGuid().ToString("N")[..6].ToUpper(),
                Title = "Gửi minh chứng ảnh thành công 📸",
                Message = $"Minh chứng ảnh cho '{evtTitle}' đang chờ Ban tổ chức hoặc Admin phê duyệt.",
                Type = "Pending",
                TargetUsername = cleanStudent,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });

            await SaveRecordsToDiskAsync();
            _logger.LogInformation("Photo Check-in submitted for event {EventId}, student {StudentCode}. Status: PendingReview", eventId, cleanStudent);
            return (true, "Gửi minh chứng ảnh thành công! Đang chờ duyệt điểm danh (+ " + reward + " ĐRL nếu duyệt, - " + penalty + " ĐRL nếu từ chối).", record);
        }

        public async Task<(bool Success, string Message)> ReviewAttendanceAsync(string recordId, bool isApproved, string reviewerName, string? notes = null, int? customPointsDelta = null)
        {
            await EnsureLoadedAsync();

            var record = _localRecords.FirstOrDefault(r => r.Id.Equals(recordId, StringComparison.OrdinalIgnoreCase));
            if (record == null)
            {
                return (false, "Không tìm thấy bản ghi điểm danh này.");
            }

            var evt = await _eventService.GetEventByIdAsync(record.EventId);

            // Cho phép người duyệt cấu hình hoặc override điểm thưởng / điểm trừ thủ công
            int reward = customPointsDelta.HasValue && customPointsDelta.Value > 0
                ? customPointsDelta.Value
                : (evt?.PointsReward ?? (record.TrainingPointsAwarded > 0 ? record.TrainingPointsAwarded : 10));

            int penalty = customPointsDelta.HasValue
                ? Math.Abs(customPointsDelta.Value)
                : (evt?.PointsPenalty ?? (record.PointsDeducted > 0 ? record.PointsDeducted : 5));

            record.ReviewerName = reviewerName;
            record.ReviewNotes = notes;

            if (isApproved)
            {
                record.Status = "Approved";
                record.TrainingPointsAwarded = reward;
                record.PointsDeducted = 0;

                // Cộng điểm rèn luyện cho sinh viên
                await _userService.AdjustTrainingPointsAsync(record.StudentCode, reward);

                await AddNotificationAsync(new CampusNotificationDto
                {
                    Id = "NOTIF-" + Guid.NewGuid().ToString("N")[..6].ToUpper(),
                    Title = "Điểm danh đã được duyệt! 🎉",
                    Message = $"Minh chứng cho '{record.EventName}' đã được duyệt bởi {reviewerName}. Bạn được cộng +{reward} ĐRL.",
                    Type = "Points",
                    TargetUsername = record.StudentCode,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });

                await SaveRecordsToDiskAsync();
                _logger.LogInformation("Attendance {RecordId} approved by {Reviewer}. Awarded +{Points} to {Student}", recordId, reviewerName, reward, record.StudentCode);
                return (true, $"Đã duyệt điểm danh! Cộng +{reward} ĐRL cho sinh viên {record.StudentCode}.");
            }
            else
            {
                record.Status = "Rejected";
                record.TrainingPointsAwarded = 0;
                record.PointsDeducted = penalty;

                // Trừ điểm rèn luyện nếu từ chối
                await _userService.AdjustTrainingPointsAsync(record.StudentCode, -penalty);

                await AddNotificationAsync(new CampusNotificationDto
                {
                    Id = "NOTIF-" + Guid.NewGuid().ToString("N")[..6].ToUpper(),
                    Title = "Điểm danh bị từ chối ⚠️",
                    Message = $"Minh chứng cho '{record.EventName}' không hợp lệ (Duyệt bởi {reviewerName}). Bạn bị trừ -{penalty} ĐRL.",
                    Type = "Warning",
                    TargetUsername = record.StudentCode,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });

                await SaveRecordsToDiskAsync();
                _logger.LogInformation("Attendance {RecordId} rejected by {Reviewer}. Deducted -{Points} from {Student}", recordId, reviewerName, penalty, record.StudentCode);
                return (true, $"Đã từ chối điểm danh. Trừ -{penalty} ĐRL của sinh viên {record.StudentCode}.");
            }
        }

        public async Task<List<AttendanceRecord>> GetPendingAttendanceReviewsAsync(int? eventId = null)
        {
            await EnsureLoadedAsync();
            var query = _localRecords.Where(r => r.Status == "PendingReview");
            if (eventId.HasValue && eventId.Value > 0)
            {
                query = query.Where(r => r.EventId == eventId.Value);
            }
            return query.ToList();
        }

        public async Task<List<AttendanceRecord>> GetAllAttendanceRecordsAsync()
        {
            await EnsureLoadedAsync();
            return _localRecords.ToList();
        }

        public async Task<(bool Success, string Message, AttendanceRecord? Record)> MarkManualAttendanceAsync(int eventId, string studentCode, string status, int points, string notes)
        {
            await EnsureLoadedAsync();

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
                Status = "Approved",
                Notes = notes
            };

            _localRecords.Insert(0, record);
            await _userService.AdjustTrainingPointsAsync(studentCode, points);
            await SaveRecordsToDiskAsync();

            _logger.LogInformation("Manual attendance recorded for event {EventId}, student {StudentCode}", eventId, studentCode);
            return (true, "Cập nhật điểm danh thành công!", record);
        }

        public async Task<List<AttendanceRecord>> GetEventAttendanceAsync(int eventId)
        {
            await EnsureLoadedAsync();
            return _localRecords.Where(r => r.EventId == eventId).ToList();
        }

        public async Task<List<AttendanceRecord>> GetStudentAttendanceHistoryAsync(string studentCode)
        {
            await EnsureLoadedAsync();
            if (string.IsNullOrWhiteSpace(studentCode))
                return _localRecords.ToList();

            return _localRecords.Where(r =>
                string.Equals(r.StudentCode, studentCode, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<string> ExportAttendanceCsvAsync(int eventId)
        {
            await EnsureLoadedAsync();
            var list = _localRecords.Where(r => r.EventId == eventId).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("ID,EventID,StudentCode,StudentName,CheckInTime,Method,Status,TrainingPoints,Notes");

            foreach (var r in list)
            {
                sb.AppendLine($"\"{r.Id}\",{r.EventId},\"{r.StudentCode}\",\"{r.StudentName}\",\"{r.CheckInTime:yyyy-MM-dd HH:mm:ss}\",\"{r.Method}\",\"{r.Status}\",{r.TrainingPointsAwarded},\"{r.Notes}\"");
            }

            return sb.ToString();
        }

        // ==================== THÔNG BÁO (NOTIFICATIONS) ====================

        public async Task<List<CampusNotificationDto>> GetNotificationsAsync(string? studentCode = null)
        {
            await EnsureLoadedAsync();
            if (string.IsNullOrWhiteSpace(studentCode) || studentCode == "ALL")
                return _localNotifications.OrderByDescending(n => n.CreatedAt).ToList();

            return _localNotifications.Where(n =>
                string.IsNullOrWhiteSpace(n.TargetUsername) ||
                n.TargetUsername == "ALL" ||
                string.Equals(n.TargetUsername, studentCode, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public async Task AddNotificationAsync(CampusNotificationDto notification)
        {
            await EnsureLoadedAsync();
            if (notification == null) return;

            if (string.IsNullOrWhiteSpace(notification.Id))
            {
                notification.Id = "NOTIF-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
            }

            _localNotifications.Insert(0, notification);
            await SaveRecordsToDiskAsync();
        }

        public async Task MarkNotificationAsReadAsync(string notificationId)
        {
            await EnsureLoadedAsync();
            var notif = _localNotifications.FirstOrDefault(n => n.Id.Equals(notificationId, StringComparison.OrdinalIgnoreCase));
            if (notif != null)
            {
                notif.IsRead = true;
                await SaveRecordsToDiskAsync();
            }
        }

        public async Task MarkAllNotificationsAsReadAsync(string? studentCode = null)
        {
            await EnsureLoadedAsync();
            foreach (var n in _localNotifications)
            {
                if (string.IsNullOrWhiteSpace(studentCode) ||
                    string.IsNullOrWhiteSpace(n.TargetUsername) ||
                    n.TargetUsername == "ALL" ||
                    string.Equals(n.TargetUsername, studentCode, StringComparison.OrdinalIgnoreCase))
                {
                    n.IsRead = true;
                }
            }
            await SaveRecordsToDiskAsync();
        }
    }
}
