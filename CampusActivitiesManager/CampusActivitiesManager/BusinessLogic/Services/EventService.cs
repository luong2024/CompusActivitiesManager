using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.BusinessLogic.Services
{
    public class EventService : IEventService
    {
        private readonly ModalErrorHandler _errorHandler;
        private readonly ILogger<EventService> _logger;

        private static bool _isInitialized = false;
        private static readonly SemaphoreSlim _fileLock = new(1, 1);
        private static string EventsFilePath => Path.Combine(FileSystem.AppDataDirectory, "campus_events.json");

        private static readonly List<EventCategoryDto> _categories = new()
        {
            new() { ID = 1, Title = "Học thuật", Color = "#2563EB" },
            new() { ID = 2, Title = "Thể thao", Color = "#10B981" },
            new() { ID = 3, Title = "Tình nguyện", Color = "#EF4444" },
            new() { ID = 4, Title = "Kỹ năng mềm", Color = "#7C3AED" },
            new() { ID = 5, Title = "Văn hóa - Nghệ thuật", Color = "#0284C7" }
        };

        private static readonly List<CampusEvent> _localEvents = new()
        {
            new()
            {
                ID = 1,
                Name = "Hội thảo AI & Công nghệ Thông tin 2026",
                Description = "Cập nhật xu hướng Generative AI, LLM và Cloud Computing. Sinh viên tham gia đầy đủ được cấp chứng nhận và cộng điểm rèn luyện.",
                Icon = "💻",
                CategoryID = 1,
                Category = _categories[0],
                Location = "Hội trường A2",
                StartDate = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy"),
                StartTime = "08:00",
                EndTime = "11:30",
                MaxParticipants = 200,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                CreatedByUsername = "admin",
                ApprovedBy = "Admin",
                Scope = "Public",
                TargetClass = "Tất cả",
                RegistrationType = "Auto",
                PointsReward = 10,
                PointsPenalty = 5,
                ImageUrl = "https://images.unsplash.com/photo-1517245386807-bb43f82c33c4?auto=format&fit=crop&w=800&q=80",
                VideoUrl = "https://www.w3schools.com/html/mov_bbb.mp4"
            },
            new()
            {
                ID = 2,
                Name = "Giải Bóng đá Sinh viên Toàn trường",
                Description = "Khai mạc giải đấu thể thao thường niên tranh cúp vô địch tại Sân vận động trường. Yêu cầu vận động viên có mặt đúng giờ.",
                Icon = "⚽",
                CategoryID = 2,
                Category = _categories[1],
                Location = "Sân Vận Động Trường",
                StartDate = DateTime.Now.AddDays(5).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(10).ToString("dd/MM/yyyy"),
                StartTime = "07:30",
                EndTime = "17:00",
                MaxParticipants = 350,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                CreatedByUsername = "admin",
                ApprovedBy = "Admin",
                Scope = "Public",
                TargetClass = "Tất cả",
                RegistrationType = "RequiresApproval",
                PointsReward = 10,
                PointsPenalty = 5,
                ImageUrl = "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=800&q=80"
            },
            new()
            {
                ID = 3,
                Name = "Chiến dịch Tình nguyện Mùa Hè Xanh",
                Description = "Hoạt động tình nguyện hỗ trợ cộng đồng, xây dựng sân chơi và dạy học vùng cao tại Tỉnh Hòa Bình.",
                Icon = "🌿",
                CategoryID = 3,
                Category = _categories[2],
                Location = "Tỉnh Hòa Bình",
                StartDate = DateTime.Now.AddDays(15).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(25).ToString("dd/MM/yyyy"),
                StartTime = "06:30",
                EndTime = "18:00",
                MaxParticipants = 80,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                CreatedByUsername = "admin",
                ApprovedBy = "Admin",
                Scope = "Public",
                TargetClass = "Tất cả",
                RegistrationType = "RequiresApproval",
                PointsReward = 15,
                PointsPenalty = 5,
                ImageUrl = "https://images.unsplash.com/photo-1559027615-cd4628902d4a?auto=format&fit=crop&w=800&q=80"
            },
            new()
            {
                ID = 4,
                Name = "Sinh hoạt Chi đoàn & Bầu Ban Cán sự Lớp D20CNTT1",
                Description = "Họp lớp định kỳ đánh giá rèn luyện học kỳ và triển khai kế hoạch thực tập tốt nghiệp. Hoạt động nội bộ lớp D20CNTT1.",
                Icon = "👥",
                CategoryID = 4,
                Category = _categories[3],
                Location = "Phòng học 402 Nhà A3",
                StartDate = DateTime.Now.AddDays(3).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(3).ToString("dd/MM/yyyy"),
                StartTime = "19:00",
                EndTime = "21:00",
                MaxParticipants = 50,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Secretary",
                CreatedByName = "Lê Hoàng Bí Thư",
                CreatedByUsername = "bithu",
                ApprovedBy = "Admin",
                Scope = "ClassOnly",
                TargetClass = "D20CNTT1",
                RegistrationType = "Auto",
                PointsReward = 5,
                PointsPenalty = 3,
                ImageUrl = "https://images.unsplash.com/photo-1523240795612-9a054b0db644?auto=format&fit=crop&w=800&q=80"
            },
            new()
            {
                ID = 5,
                Name = "Tọa đàm Hướng nghiệp Chuyên ngành D20CNTT1",
                Description = "Gặp gỡ doanh nghiệp phần mềm và định hướng chuyên môn cho sinh viên lớp D20CNTT1 do Giảng viên chủ nhiệm tổ chức.",
                Icon = "🎯",
                CategoryID = 1,
                Category = _categories[0],
                Location = "Phòng Hội thảo 302-B1",
                StartDate = DateTime.Now.AddDays(8).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(8).ToString("dd/MM/yyyy"),
                StartTime = "14:00",
                EndTime = "16:30",
                MaxParticipants = 50,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Manager",
                CreatedByName = "ThS. Nguyễn Văn Quản",
                CreatedByUsername = "manager",
                ApprovedBy = "Admin",
                Scope = "ClassOnly",
                TargetClass = "D20CNTT1",
                RegistrationType = "Auto",
                PointsReward = 5,
                PointsPenalty = 2,
                ImageUrl = "https://images.unsplash.com/photo-1475721027785-f74eccf877e2?auto=format&fit=crop&w=800&q=80"
            },
            new()
            {
                ID = 6,
                Name = "Tọa đàm Khởi nghiệp Sinh viên Đổi mới Sáng tạo 2026",
                Description = "Cơ hội nhận vốn đầu tư thiên thần cho các dự án khởi nghiệp công nghệ của sinh viên. (Đang chờ Quản trị viên duyệt)",
                Icon = "💡",
                CategoryID = 1,
                Category = _categories[0],
                Location = "Hội trường tầng 5 Thư viện",
                StartDate = DateTime.Now.AddDays(20).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(20).ToString("dd/MM/yyyy"),
                StartTime = "08:30",
                EndTime = "11:30",
                MaxParticipants = 150,
                Status = "Chờ phê duyệt",
                IsApproved = false,
                ApprovalStatus = "Pending",
                CreatedByRole = "Manager",
                CreatedByName = "ThS. Nguyễn Văn Quản",
                CreatedByUsername = "manager",
                Scope = "Public",
                TargetClass = "Tất cả",
                RegistrationType = "RequiresApproval",
                PointsReward = 10,
                PointsPenalty = 5,
                ImageUrl = "https://images.unsplash.com/photo-1556761175-5973dc0f32e7?auto=format&fit=crop&w=800&q=80"
            },
            new()
            {
                ID = 7,
                Name = "Ngày Hội Hiến Máu Tình Nguyện Giọt Hồng PTIT 2026",
                Description = "Chương trình hiến máu nhân đạo cứu người đã diễn ra tuần trước. Sự kiện đã kết thúc, hệ thống đã khóa chức năng điểm danh nhận điểm.",
                Icon = "🩸",
                CategoryID = 3,
                Category = _categories[2],
                Location = "Sảnh Giảng đường A2",
                StartDate = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy"),
                StartTime = "07:30",
                EndTime = "11:30",
                MaxParticipants = 250,
                Status = "Đã kết thúc",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                CreatedByUsername = "admin",
                ApprovedBy = "Admin",
                Scope = "Public",
                TargetClass = "Tất cả",
                RegistrationType = "Auto",
                PointsReward = 15,
                PointsPenalty = 0,
                ImageUrl = "https://images.unsplash.com/photo-1615461066841-6116e61058f4?auto=format&fit=crop&w=800&q=80"
            },
            new()
            {
                ID = 8,
                Name = "Cuộc thi Lập trình Hackathon PTIT Innovation 2026",
                Description = "Thử thách 24 giờ xây dựng sản phẩm AI ứng dụng chuyển đổi số giáo dục. Đề xuất từ Câu lạc bộ IT, đang đợi Admin phê duyệt.",
                Icon = "🚀",
                CategoryID = 1,
                Category = _categories[0],
                Location = "Phòng Lab AI Tầng 4",
                StartDate = DateTime.Now.AddDays(12).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(13).ToString("dd/MM/yyyy"),
                StartTime = "08:00",
                EndTime = "18:00",
                MaxParticipants = 120,
                Status = "Chờ phê duyệt",
                IsApproved = false,
                ApprovalStatus = "Pending",
                CreatedByRole = "Manager",
                CreatedByName = "CLB Công Nghệ Sinh Viên",
                CreatedByUsername = "manager",
                Scope = "Public",
                TargetClass = "Tất cả",
                RegistrationType = "RequiresApproval",
                PointsReward = 20,
                PointsPenalty = 5,
                ImageUrl = "https://images.unsplash.com/photo-1504384308090-c894fdcc538d?auto=format&fit=crop&w=800&q=80"
            }
        };

        public EventService(ModalErrorHandler errorHandler, ILogger<EventService> logger)
        {
            _errorHandler = errorHandler;
            _logger = logger;
        }

        private async Task EnsureLoadedAsync()
        {
            if (_isInitialized) return;
            await _fileLock.WaitAsync();
            try
            {
                if (_isInitialized) return;

                if (File.Exists(EventsFilePath))
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(EventsFilePath);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            var loaded = JsonSerializer.Deserialize<List<CampusEvent>>(json);
                            if (loaded != null && loaded.Count > 0)
                            {
                                _localEvents.Clear();
                                _localEvents.AddRange(loaded);
                                _logger.LogInformation("Loaded {Count} events from persistent storage ({Path})", _localEvents.Count, EventsFilePath);
                                _isInitialized = true;
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to load events from disk. Initializing with default seed.");
                    }
                }

                // If file does not exist, save current seed to disk for future runs
                await SaveEventsToDiskInternalAsync();
                _isInitialized = true;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        private async Task SaveEventsToDiskAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                await SaveEventsToDiskInternalAsync();
            }
            finally
            {
                _fileLock.Release();
            }
        }

        private async Task SaveEventsToDiskInternalAsync()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_localEvents, options);
                var dir = Path.GetDirectoryName(EventsFilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                await File.WriteAllTextAsync(EventsFilePath, json);
                _logger.LogInformation("Saved {Count} events to disk ({Path})", _localEvents.Count, EventsFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist events to disk");
            }
        }

        public async Task<List<CampusEvent>> GetEventsAsync(bool includePending = false)
        {
            await EnsureLoadedAsync();
            return includePending 
                ? _localEvents.ToList() 
                : _localEvents.Where(e => e.IsApproved && e.ApprovalStatus == "Approved").ToList();
        }

        public async Task<List<CampusEvent>> GetEventsForUserAsync(User? user)
        {
            await EnsureLoadedAsync();
            if (user == null)
            {
                return _localEvents.Where(e => e.IsApproved && e.ApprovalStatus == "Approved" && e.Scope == "Public").ToList();
            }

            // Admin: xem được toàn bộ sự kiện (cả chờ duyệt, cả sự kiện các lớp)
            if (user.Role == Role.Admin)
            {
                return _localEvents.ToList();
            }

            // Manager: xem sự kiện do mình tạo (kể cả pending) + sự kiện công khai ĐÃ DUYỆT + sự kiện lớp được phân công ĐÃ DUYỆT
            if (user.Role == Role.Manager)
            {
                var myAssignedClass = user.AssignedClass ?? user.ClassName;
                return _localEvents.Where(e =>
                    string.Equals(e.CreatedByUsername, user.Username, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(e.CreatedByName, user.FullName, StringComparison.OrdinalIgnoreCase) ||
                    (e.IsApproved && e.ApprovalStatus == "Approved" && (e.Scope == "Public" || e.TargetClass == myAssignedClass))
                ).ToList();
            }

            // Giảng viên: xem sự kiện do mình tạo + sự kiện các lớp phụ trách ĐÃ DUYỆT + sự kiện công khai ĐÃ DUYỆT (1 giảng viên quản lý nhiều lớp)
            if (user.Role == Role.Lecturer)
            {
                return _localEvents.Where(e =>
                    string.Equals(e.CreatedByUsername, user.Username, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(e.CreatedByName, user.FullName, StringComparison.OrdinalIgnoreCase) ||
                    (e.IsApproved && e.ApprovalStatus == "Approved" && (e.Scope == "Public" || user.IsInChargeOfClass(e.TargetClass)))
                ).ToList();
            }

            // Bí thư lớp: xem sự kiện do mình tạo + sự kiện lớp mình ĐÃ DUYỆT + sự kiện công khai ĐÃ DUYỆT
            if (user.Role == Role.Secretary || user.IsSecretary)
            {
                return _localEvents.Where(e =>
                    string.Equals(e.CreatedByUsername, user.Username, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(e.CreatedByName, user.FullName, StringComparison.OrdinalIgnoreCase) ||
                    (e.IsApproved && e.ApprovalStatus == "Approved" && (e.Scope == "Public" || e.TargetClass == user.ClassName))
                ).ToList();
            }

            // Học sinh / Sinh viên thường: CHỈ XEM sự kiện công khai ĐÃ DUYỆT hoặc sự kiện lớp mình ĐÃ DUYỆT
            return _localEvents.Where(e =>
                e.IsApproved && e.ApprovalStatus == "Approved" &&
                (e.Scope == "Public" || string.Equals(e.TargetClass, user.ClassName, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        public async Task<CampusEvent?> GetEventByIdAsync(int id)
        {
            await EnsureLoadedAsync();
            return _localEvents.FirstOrDefault(e => e.ID == id);
        }

        public async Task<bool> CreateEventAsync(CampusEvent evt)
        {
            await EnsureLoadedAsync();
            int newId = _localEvents.Any() ? _localEvents.Max(e => e.ID) + 1 : 1;
            evt.ID = newId;
            evt.Category = _categories.FirstOrDefault(c => c.ID == evt.CategoryID) ?? _categories[0];

            // QUY TẮC BẮT BUỘC: Mọi bài đăng trước khi hiển thị trên app phải được quản lý và duyệt bởi Admin
            // Chỉ bài đăng do Admin trực tiếp tạo mới được tự động duyệt
            if (string.Equals(evt.CreatedByRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                evt.IsApproved = true;
                evt.ApprovalStatus = "Approved";
                evt.Status = "Sắp diễn ra";
                evt.ApprovedBy = "Admin";
            }
            else
            {
                evt.IsApproved = false;
                evt.ApprovalStatus = "Pending";
                evt.Status = "Chờ phê duyệt";
            }

            if (string.IsNullOrWhiteSpace(evt.StartTime)) evt.StartTime = "08:00";
            if (string.IsNullOrWhiteSpace(evt.EndTime)) evt.EndTime = "11:30";

            _localEvents.Insert(0, evt);
            await SaveEventsToDiskAsync();
            _logger.LogInformation("Event '{Name}' created (ID: {Id}, Scope: {Scope}, Approved: {IsApproved})", evt.Name, evt.ID, evt.Scope, evt.IsApproved);
            return true;
        }

        public async Task<bool> UpdateEventAsync(CampusEvent evt)
        {
            await EnsureLoadedAsync();
            var local = _localEvents.FirstOrDefault(e => e.ID == evt.ID);
            if (local != null)
            {
                local.Name = evt.Name;
                local.Description = evt.Description;
                local.Icon = evt.Icon;
                local.CategoryID = evt.CategoryID;
                local.Category = _categories.FirstOrDefault(c => c.ID == evt.CategoryID) ?? local.Category;
                local.Location = evt.Location;
                local.StartDate = evt.StartDate;
                local.EndDate = evt.EndDate;
                local.StartTime = evt.StartTime;
                local.EndTime = evt.EndTime;
                local.MaxParticipants = evt.MaxParticipants;
                local.Status = evt.Status;
                local.ImageUrl = evt.ImageUrl;
                local.VideoUrl = evt.VideoUrl;
                local.Scope = evt.Scope;
                local.TargetClass = evt.TargetClass;
                local.RegistrationType = evt.RegistrationType;
                local.PointsReward = evt.PointsReward;
                local.PointsPenalty = evt.PointsPenalty;
                await SaveEventsToDiskAsync();
                _logger.LogInformation("Event {Id} updated", evt.ID);
                return true;
            }
            return false;
        }

        public async Task<bool> ApproveEventAsync(int id, bool approve, string? approvedBy = "Admin", string? reason = null)
        {
            await EnsureLoadedAsync();
            var local = _localEvents.FirstOrDefault(e => e.ID == id);
            if (local != null)
            {
                local.IsApproved = approve;
                local.ApprovalStatus = approve ? "Approved" : "Rejected";
                local.Status = approve ? "Sắp diễn ra" : "Đã từ chối";
                local.ApprovedBy = approvedBy ?? "Admin";
                local.RejectionReason = reason;
                await SaveEventsToDiskAsync();
                _logger.LogInformation("Event {Id} approval status updated to {Status} by {Approver}", id, local.ApprovalStatus, approvedBy);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            await EnsureLoadedAsync();
            var local = _localEvents.FirstOrDefault(e => e.ID == id);
            if (local != null)
            {
                _localEvents.Remove(local);
                await SaveEventsToDiskAsync();
                _logger.LogInformation("Event {Id} deleted", id);
                return true;
            }
            return false;
        }

        public async Task<List<EventCategoryDto>> GetCategoriesAsync()
        {
            await EnsureLoadedAsync();
            return _categories.ToList();
        }

        public async Task<(bool Success, string Message, string Status)> RegisterUserForEventAsync(int eventId, User user)
        {
            await EnsureLoadedAsync();
            var evt = _localEvents.FirstOrDefault(e => e.ID == eventId);
            if (evt == null)
            {
                return (false, "Không tìm thấy hoạt động.", "NotFound");
            }

            if (!evt.IsApproved || evt.ApprovalStatus != "Approved")
            {
                return (false, "Hoạt động chưa được phê duyệt bởi Quản trị viên.", "NotApproved");
            }

            // Kiểm tra phân quyền phạm vi lớp
            if (evt.Scope == "ClassOnly" && !string.Equals(evt.TargetClass, user.ClassName, StringComparison.OrdinalIgnoreCase) && user.Role != Role.Admin)
            {
                return (false, $"Hoạt động này chỉ dành riêng cho sinh viên lớp {evt.TargetClass}.", "WrongClass");
            }

            // Kiểm tra đã đăng ký chưa
            var existing = evt.GetUserRegistration(user.Username);
            if (existing != null)
            {
                if (existing.Status == "Approved")
                    return (false, "Bạn đã được duyệt tham gia hoạt động này rồi.", "AlreadyApproved");
                if (existing.Status == "Pending")
                    return (false, "Đơn đăng ký của bạn đang chờ ban tổ chức phê duyệt.", "AlreadyPending");
            }

            string regStatus = evt.RegistrationType == "RequiresApproval" ? "Pending" : "Approved";

            var reg = new EventRegistration
            {
                Id = Guid.NewGuid().ToString("N")[..8],
                EventId = eventId,
                UserId = user.Id,
                StudentCode = user.Username,
                StudentName = user.FullName,
                ClassName = user.ClassName,
                RegisteredAt = DateTime.UtcNow,
                Status = regStatus
            };

            evt.Registrations.RemoveAll(r => r.StudentCode.Equals(user.Username, StringComparison.OrdinalIgnoreCase));
            evt.Registrations.Add(reg);
            evt.IsRegistered = true;

            await SaveEventsToDiskAsync();

            string msg = regStatus == "Approved"
                ? "Đăng ký thành công! Bạn đã được tự động duyệt tham gia."
                : "Đăng ký thành công! Đơn đăng ký đang chờ Ban tổ chức phê duyệt.";

            _logger.LogInformation("User {StudentCode} registered for event {EventId} with status {Status}", user.Username, eventId, regStatus);
            return (true, msg, regStatus);
        }

        public async Task<(bool Success, string Message)> CancelRegistrationAsync(int eventId, string studentCodeOrId)
        {
            await EnsureLoadedAsync();
            var evt = _localEvents.FirstOrDefault(e => e.ID == eventId);
            if (evt == null)
                return (false, "Không tìm thấy hoạt động.");

            var reg = evt.GetUserRegistration(studentCodeOrId);
            if (reg == null)
                return (false, "Bạn chưa đăng ký tham gia hoạt động này.");

            // Người dùng chỉ có thể hủy tham gia TRƯỚC KHI ĐƯỢC DUYỆT (tức là khi trạng thái còn Pending)
            if (reg.Status != "Pending")
            {
                return (false, "Đơn đăng ký của bạn đã được phê duyệt, không thể tự hủy tham gia.");
            }

            evt.Registrations.Remove(reg);
            evt.IsRegistered = false;
            await SaveEventsToDiskAsync();

            _logger.LogInformation("User {StudentCode} cancelled pending registration for event {EventId}", studentCodeOrId, eventId);
            return (true, "Đã hủy đăng ký tham gia thành công!");
        }

        public async Task<bool> ApproveRegistrationAsync(int eventId, string registrationId, bool approve)
        {
            await EnsureLoadedAsync();
            var evt = _localEvents.FirstOrDefault(e => e.ID == eventId);
            if (evt == null) return false;

            var reg = evt.Registrations.FirstOrDefault(r => r.Id == registrationId);
            if (reg != null)
            {
                reg.Status = approve ? "Approved" : "Rejected";
                await SaveEventsToDiskAsync();
                _logger.LogInformation("Registration {Id} for event {EventId} set to {Status}", registrationId, eventId, reg.Status);
                return true;
            }
            return false;
        }
    }
}
