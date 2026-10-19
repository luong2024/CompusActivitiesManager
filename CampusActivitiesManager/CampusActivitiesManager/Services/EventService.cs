using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    public class EventService : IEventService
    {
        private readonly ModalErrorHandler _errorHandler;
        private readonly ILogger<EventService> _logger;

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
                Description = "Cập nhật xu hướng Generative AI và Cloud Computing cho sinh viên toàn trường",
                Icon = "💻",
                CategoryID = 1,
                Category = _categories[0],
                Location = "Hội trường A2",
                StartDate = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy"),
                MaxParticipants = 200,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                ApprovedBy = "Admin",
                ImageUrl = "https://images.unsplash.com/photo-1517245386807-bb43f82c33c4?auto=format&fit=crop&w=800&q=80",
                VideoUrl = "https://www.w3schools.com/html/mov_bbb.mp4"
            },
            new()
            {
                ID = 2,
                Name = "Giải Bóng đá Sinh viên Toàn trường",
                Description = "Khai mạc giải đấu thể thao thường niên tranh cúp vô địch tại Sân vận động",
                Icon = "⚽",
                CategoryID = 2,
                Category = _categories[1],
                Location = "Sân Vận Động Trường",
                StartDate = DateTime.Now.AddDays(5).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(10).ToString("dd/MM/yyyy"),
                MaxParticipants = 350,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                ApprovedBy = "Admin",
                ImageUrl = "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=800&q=80"
            },
            new()
            {
                ID = 3,
                Name = "Chiến dịch Tình nguyện Mùa Hè Xanh",
                Description = "Hoạt động tình nguyện hỗ trợ cộng đồng, xây dựng sân chơi và dạy học vùng cao",
                Icon = "🌿",
                CategoryID = 3,
                Category = _categories[2],
                Location = "Tỉnh Hòa Bình",
                StartDate = DateTime.Now.AddDays(15).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(25).ToString("dd/MM/yyyy"),
                MaxParticipants = 80,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                ApprovedBy = "Admin",
                ImageUrl = "https://images.unsplash.com/photo-1559027615-cd4628902d4a?auto=format&fit=crop&w=800&q=80"
            },
            new()
            {
                ID = 4,
                Name = "Workshop Kỹ năng Thuyết trình & Phỏng vấn",
                Description = "Phát triển kỹ năng giao tiếp tự tin và chinh phục nhà tuyển dụng công nghệ",
                Icon = "🗣️",
                CategoryID = 4,
                Category = _categories[3],
                Location = "Phòng Hội thảo 302-B1",
                StartDate = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy"),
                MaxParticipants = 100,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                ApprovedBy = "Admin",
                ImageUrl = "https://images.unsplash.com/photo-1475721027785-f74eccf877e2?auto=format&fit=crop&w=800&q=80"
            },
            new()
            {
                ID = 5,
                Name = "Đêm Nhạc Hội Chào Tân Sinh Viên",
                Description = "Chương trình biểu diễn nghệ thuật, giao lưu âm nhạc và kết nối các câu lạc bộ",
                Icon = "🎸",
                CategoryID = 5,
                Category = _categories[4],
                Location = "Quảng trường Trung tâm",
                StartDate = DateTime.Now.AddDays(12).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(12).ToString("dd/MM/yyyy"),
                MaxParticipants = 500,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                ApprovedBy = "Admin",
                ImageUrl = "https://images.unsplash.com/photo-1492684223066-81342ee5ff30?auto=format&fit=crop&w=800&q=80"
            },
            new()
            {
                ID = 6,
                Name = "Tọa đàm Khởi nghiệp Sinh viên Đổi mới Sáng tạo 2026",
                Description = "Cơ hội nhận vốn đầu tư thiên thần cho các dự án khởi nghiệp công nghệ của sinh viên",
                Icon = "💡",
                CategoryID = 1,
                Category = _categories[0],
                Location = "Hội trường tầng 5 Thư viện",
                StartDate = DateTime.Now.AddDays(20).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(20).ToString("dd/MM/yyyy"),
                MaxParticipants = 150,
                Status = "Chờ phê duyệt",
                IsApproved = false,
                ApprovalStatus = "Pending",
                CreatedByRole = "Manager",
                CreatedByName = "Ban Tổ chức Sự kiện",
                ImageUrl = "https://images.unsplash.com/photo-1556761175-5973dc0f32e7?auto=format&fit=crop&w=800&q=80"
            }
        };

        public EventService(ModalErrorHandler errorHandler, ILogger<EventService> logger)
        {
            _errorHandler = errorHandler;
            _logger = logger;
        }

        public async Task<List<CampusEvent>> GetEventsAsync(bool includePending = false)
        {
            await Task.CompletedTask;
            return includePending 
                ? _localEvents.ToList() 
                : _localEvents.Where(e => e.IsApproved).ToList();
        }

        public async Task<CampusEvent?> GetEventByIdAsync(int id)
        {
            await Task.CompletedTask;
            return _localEvents.FirstOrDefault(e => e.ID == id);
        }

        public async Task<bool> CreateEventAsync(CampusEvent evt)
        {
            await Task.CompletedTask;
            int newId = _localEvents.Any() ? _localEvents.Max(e => e.ID) + 1 : 1;
            evt.ID = newId;
            evt.Category = _categories.FirstOrDefault(c => c.ID == evt.CategoryID) ?? _categories[0];

            // Nếu người tạo là Manager thì cần duyệt qua Admin mới xuất hiện trên app
            if (string.Equals(evt.CreatedByRole, "Manager", StringComparison.OrdinalIgnoreCase))
            {
                evt.IsApproved = false;
                evt.ApprovalStatus = "Pending";
                evt.Status = "Chờ phê duyệt";
            }
            else
            {
                evt.IsApproved = true;
                evt.ApprovalStatus = "Approved";
                evt.Status = "Sắp diễn ra";
            }

            _localEvents.Insert(0, evt);
            _logger.LogInformation("Event '{Name}' created (ID: {Id}, Approved: {IsApproved})", evt.Name, evt.ID, evt.IsApproved);
            return true;
        }

        public async Task<bool> UpdateEventAsync(CampusEvent evt)
        {
            await Task.CompletedTask;
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
                local.MaxParticipants = evt.MaxParticipants;
                local.Status = evt.Status;
                local.IsApproved = evt.IsApproved;
                local.ApprovalStatus = evt.ApprovalStatus;
                local.ImageUrl = evt.ImageUrl;
                local.VideoUrl = evt.VideoUrl;
                _logger.LogInformation("Event {Id} updated", evt.ID);
                return true;
            }
            return false;
        }

        public async Task<bool> ApproveEventAsync(int id, bool approve, string? approvedBy = "Admin", string? reason = null)
        {
            await Task.CompletedTask;
            var local = _localEvents.FirstOrDefault(e => e.ID == id);
            if (local != null)
            {
                local.IsApproved = approve;
                local.ApprovalStatus = approve ? "Approved" : "Rejected";
                local.Status = approve ? "Sắp diễn ra" : "Đã từ chối";
                local.ApprovedBy = approvedBy ?? "Admin";
                local.RejectionReason = reason;
                _logger.LogInformation("Event {Id} approved state changed to {Status} by {Approver}", id, local.ApprovalStatus, approvedBy);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            await Task.CompletedTask;
            var local = _localEvents.FirstOrDefault(e => e.ID == id);
            if (local != null)
            {
                _localEvents.Remove(local);
                _logger.LogInformation("Event {Id} deleted", id);
                return true;
            }
            return false;
        }

        public async Task<List<EventCategoryDto>> GetCategoriesAsync()
        {
            await Task.CompletedTask;
            return _categories.ToList();
        }
    }
}
