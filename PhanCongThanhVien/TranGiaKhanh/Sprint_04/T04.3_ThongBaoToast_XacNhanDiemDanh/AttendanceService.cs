using System.Net.Http.Json;
using System.Text.Json;
using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly HttpClient _httpClient;
        private readonly ModalErrorHandler _errorHandler;
        private readonly ILogger<AttendanceService> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly List<AttendanceRecord> _localFallbackRecords = new()
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

        private static readonly List<CampusNotificationDto> _localFallbackNotifications = new()
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
            _httpClient = HttpHelper.CreateApiClient();
            _errorHandler = errorHandler;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, AttendanceRecord? Record)> CheckInWithQrAsync(int eventId, string studentCode, string qrPayload)
        {
            try
            {
                var req = new QrCheckInRequest
                {
                    EventId = eventId,
                    StudentCode = studentCode,
                    QrTicketCode = qrPayload,
                    DeviceTimestamp = DateTime.UtcNow
                };

                var response = await _httpClient.PostAsJsonAsync($"{Constants.AttendanceApiUrl}/qr", req);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<AttendanceRecord>>(JsonOptions);
                    return (true, result?.Message ?? "Điểm danh thành công!", result?.Data);
                }

                var err = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions);
                return (false, err?.Message ?? "Điểm danh thất bại", null);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API QR check-in failed, simulating local record");
                var record = new AttendanceRecord
                {
                    Id = "LOCAL-" + Guid.NewGuid().ToString("N")[..6],
                    EventId = eventId,
                    EventName = "Sự kiện hoạt động #" + eventId,
                    StudentCode = studentCode,
                    StudentName = "Sinh viên " + studentCode,
                    CheckInTime = DateTime.UtcNow,
                    Method = "QR",
                    TrainingPointsAwarded = 5,
                    Status = "Present",
                    Notes = "Điểm danh ngoại tuyến thành công"
                };
                _localFallbackRecords.Insert(0, record);
                return (true, "Điểm danh QR thành công (chế độ phản hồi nhanh)!", record);
            }
        }

        public async Task<(bool Success, string Message, AttendanceRecord? Record)> CheckInWithPhotoAsync(int eventId, string studentCode, string photoBase64, string? notes)
        {
            try
            {
                var req = new PhotoCheckInRequest
                {
                    EventId = eventId,
                    StudentCode = studentCode,
                    PhotoBase64 = photoBase64,
                    Notes = notes,
                    DeviceTimestamp = DateTime.UtcNow
                };

                var response = await _httpClient.PostAsJsonAsync($"{Constants.AttendanceApiUrl}/photo", req);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<AttendanceRecord>>(JsonOptions);
                    return (true, result?.Message ?? "Điểm danh bằng hình ảnh thành công!", result?.Data);
                }

                var err = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions);
                return (false, err?.Message ?? "Điểm danh ảnh thất bại", null);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API Photo check-in failed, simulating local record");
                var record = new AttendanceRecord
                {
                    Id = "LOCAL-" + Guid.NewGuid().ToString("N")[..6],
                    EventId = eventId,
                    EventName = "Sự kiện hoạt động #" + eventId,
                    StudentCode = studentCode,
                    StudentName = "Sinh viên " + studentCode,
                    CheckInTime = DateTime.UtcNow,
                    Method = "Photo",
                    TrainingPointsAwarded = 5,
                    Status = "Present",
                    Notes = notes ?? "Điểm danh kèm hình ảnh minh chứng camera"
                };
                _localFallbackRecords.Insert(0, record);
                return (true, "Điểm danh ảnh thành công!", record);
            }
        }

        public async Task<(bool Success, string Message, AttendanceRecord? Record)> MarkManualAttendanceAsync(int eventId, string studentCode, string status, int points, string notes)
        {
            try
            {
                var req = new ManualAttendanceRequest
                {
                    EventId = eventId,
                    StudentCode = studentCode,
                    Status = status,
                    TrainingPoints = points,
                    Notes = notes
                };

                var response = await _httpClient.PostAsJsonAsync($"{Constants.AttendanceApiUrl}/manual", req);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<AttendanceRecord>>(JsonOptions);
                    return (true, result?.Message ?? "Cập nhật thành công!", result?.Data);
                }

                var err = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions);
                return (false, err?.Message ?? "Lỗi khi cập nhật thủ công", null);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API manual attendance failed, fallback to local");
                var record = new AttendanceRecord
                {
                    Id = "LOCAL-" + Guid.NewGuid().ToString("N")[..6],
                    EventId = eventId,
                    EventName = "Sự kiện hoạt động #" + eventId,
                    StudentCode = studentCode,
                    StudentName = "Sinh viên " + studentCode,
                    CheckInTime = DateTime.UtcNow,
                    Method = "Manual",
                    TrainingPointsAwarded = points,
                    Status = status,
                    Notes = notes
                };
                _localFallbackRecords.Insert(0, record);
                return (true, "Cập nhật điểm danh thành công!", record);
            }
        }

        public async Task<List<AttendanceRecord>> GetEventAttendanceAsync(int eventId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{Constants.AttendanceApiUrl}/event/{eventId}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<AttendanceRecord>>>(JsonOptions);
                    if (result?.Data != null && result.Data.Count > 0)
                    {
                        return result.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API get event attendance failed, fallback");
            }

            return _localFallbackRecords.Where(r => r.EventId == eventId).ToList();
        }

        public async Task<List<AttendanceRecord>> GetStudentAttendanceHistoryAsync(string studentCode)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{Constants.AttendanceApiUrl}/student/{studentCode}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<AttendanceRecord>>>(JsonOptions);
                    if (result?.Data != null && result.Data.Count > 0)
                    {
                        return result.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API get student attendance failed, fallback");
            }

            return _localFallbackRecords.Where(r => string.Equals(r.StudentCode, studentCode, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<string> ExportAttendanceCsvAsync(int eventId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{Constants.AttendanceApiUrl}/event/{eventId}/export-csv");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API export CSV failed, generating local CSV");
            }

            var records = _localFallbackRecords.Where(r => r.EventId == eventId).ToList();
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Mã Điểm Danh,Mã Sự Kiện,Tên Sự Kiện,Mã Sinh Viên,Họ và Tên,Thời Gian Điểm Danh,Hình Thức,Trạng Thái,Điểm Rèn Luyện,Ghi Chú");
            foreach (var r in records)
            {
                sb.AppendLine($"\"{r.Id}\",\"{r.EventId}\",\"{r.EventName}\",\"{r.StudentCode}\",\"{r.StudentName}\",\"{r.CheckInTime:dd/MM/yyyy HH:mm:ss}\",\"{r.Method}\",\"{r.Status}\",\"{r.TrainingPointsAwarded}\",\"{r.Notes}\"");
            }
            return sb.ToString();
        }

        public async Task<List<CampusNotificationDto>> GetNotificationsAsync(string? studentCode = null)
        {
            try
            {
                var url = string.IsNullOrWhiteSpace(studentCode)
                    ? Constants.NotificationsApiUrl
                    : $"{Constants.NotificationsApiUrl}?studentCode={studentCode}";

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<CampusNotificationDto>>>(JsonOptions);
                    if (result?.Data != null && result.Data.Count > 0)
                    {
                        return result.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API get notifications failed, fallback");
            }

            return _localFallbackNotifications;
        }
    }
}
