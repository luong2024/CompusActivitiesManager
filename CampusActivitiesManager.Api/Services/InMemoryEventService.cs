using CampusActivitiesManager.Api.Models;

namespace CampusActivitiesManager.Api.Services
{
    /// <summary>
    /// Dịch vụ sự kiện In-Memory hoạt động độc lập không cần Firebase Firestore credentials,
    /// phục vụ kiểm thử và cho phép người ngoài clone repo chạy ngay lập tức.
    /// </summary>
    public class InMemoryEventService : IFirebaseEventService
    {
        public List<EventCategoryDto> Categories { get; } = new()
        {
            new() { ID = 1, Title = "Học thuật", Color = "#2563EB" },
            new() { ID = 2, Title = "Thể thao", Color = "#10B981" },
            new() { ID = 3, Title = "Tình nguyện", Color = "#EF4444" },
            new() { ID = 4, Title = "Kỹ năng mềm", Color = "#F59E0B" },
            new() { ID = 5, Title = "Văn hóa - Nghệ thuật", Color = "#0EA5E9" }
        };

        public List<EventDto> Events { get; } = new()
        {
            new()
            {
                ID = 1,
                Name = "Hội thảo Công nghệ AI & Tương lai Lập trình viên 2026",
                Description = "Cập nhật xu hướng AI, Machine Learning và Cloud Computing cho sinh viên toàn trường",
                Icon = "💻",
                CategoryID = 1,
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
                VideoUrl = ""
            },
            new()
            {
                ID = 2,
                Name = "Giải Bóng đá Sinh viên Toàn trường 2026",
                Description = "Khai mạc giải đấu thể thao lớn nhất năm học tại sân vận động trường",
                Icon = "⚽",
                CategoryID = 2,
                Location = "Sân vận động trường",
                StartDate = DateTime.Now.AddDays(5).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(10).ToString("dd/MM/yyyy"),
                MaxParticipants = 300,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                ApprovedBy = "Admin",
                ImageUrl = "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=800&q=80",
                VideoUrl = ""
            },
            new()
            {
                ID = 3,
                Name = "Chiến dịch Tình nguyện Mùa Hè Xanh 2026",
                Description = "Hoạt động tình nguyện hỗ trợ cộng đồng và dạy học vùng cao tại Hòa Bình",
                Icon = "🌿",
                CategoryID = 3,
                Location = "Xã Thống Nhất, Hòa Bình",
                StartDate = DateTime.Now.AddDays(15).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(25).ToString("dd/MM/yyyy"),
                MaxParticipants = 80,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                ApprovedBy = "Admin",
                ImageUrl = "https://images.unsplash.com/photo-1559027615-cd4628902d4a?auto=format&fit=crop&w=800&q=80",
                VideoUrl = ""
            },
            new()
            {
                ID = 4,
                Name = "Workshop Kỹ năng Thuyết trình & Phỏng vấn Tuyển dụng",
                Description = "Phát triển kỹ năng giao tiếp và chinh phục nhà tuyển dụng hàng đầu",
                Icon = "🗣️",
                CategoryID = 4,
                Location = "Phòng Hội thảo C102",
                StartDate = DateTime.Now.AddDays(8).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(8).ToString("dd/MM/yyyy"),
                MaxParticipants = 120,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                ApprovedBy = "Admin",
                ImageUrl = "",
                VideoUrl = ""
            },
            new()
            {
                ID = 5,
                Name = "Đêm nhạc Acoustic Chào Tân Sinh viên K26",
                Description = "Giao lưu âm nhạc giữa các câu lạc bộ nghệ thuật trong toàn trường",
                Icon = "🎸",
                CategoryID = 5,
                Location = "Sân khấu Ngoài trời Khu B",
                StartDate = DateTime.Now.AddDays(12).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(12).ToString("dd/MM/yyyy"),
                MaxParticipants = 250,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                ApprovedBy = "Admin",
                ImageUrl = "",
                VideoUrl = ""
            },
            new()
            {
                ID = 6,
                Name = "Tọa đàm Khởi nghiệp Sinh viên Đổi mới Sáng tạo 2026",
                Description = "Cơ hội nhận vốn đầu tư thiên thần cho các dự án khởi nghiệp công nghệ của sinh viên",
                Icon = "💡",
                CategoryID = 1,
                Location = "Hội trường tầng 5 Thư viện",
                StartDate = DateTime.Now.AddDays(20).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(20).ToString("dd/MM/yyyy"),
                MaxParticipants = 150,
                Status = "Chờ phê duyệt",
                IsApproved = false,
                ApprovalStatus = "Pending",
                CreatedByRole = "Manager",
                CreatedByName = "Ban Tổ chức Sự kiện",
                ApprovedBy = null,
                ImageUrl = "",
                VideoUrl = ""
            }
        };

        public InMemoryEventService()
        {
            foreach (var evt in Events)
            {
                evt.Category = Categories.FirstOrDefault(c => c.ID == evt.CategoryID);
            }
        }

        public Task<List<EventDto>> GetAllEventsAsync(int limit = 20, int? afterId = null, bool includePending = false)
        {
            var query = Events.AsEnumerable();
            if (!includePending)
            {
                query = query.Where(e => e.IsApproved);
            }
            if (afterId.HasValue)
            {
                query = query.Where(e => e.ID > afterId.Value);
            }
            int safeLimit = limit <= 0 ? 20 : Math.Min(limit, 100);
            var res = query.OrderBy(e => e.ID).Take(safeLimit).ToList();
            return Task.FromResult(res);
        }

        public Task<EventDto?> GetEventByIdAsync(int id)
        {
            var evt = Events.FirstOrDefault(e => e.ID == id);
            return Task.FromResult(evt);
        }

        public Task<EventDto> CreateEventAsync(CreateEventRequest request)
        {
            int newId = Events.Any() ? Events.Max(e => e.ID) + 1 : 1;
            int catId = request.CategoryId ?? 1;
            var cat = Categories.FirstOrDefault(c => c.ID == catId);

            bool isApproved = request.IsApproved ?? (request.CreatedByRole == "Admin");
            string approvalStatus = request.ApprovalStatus ?? (isApproved ? "Approved" : "Pending");

            var newEvt = new EventDto
            {
                ID = newId,
                Name = request.Name,
                Description = request.Description,
                Icon = string.IsNullOrWhiteSpace(request.Icon) ? "📅" : request.Icon,
                CategoryID = catId,
                Category = cat,
                Location = string.IsNullOrWhiteSpace(request.Location) ? "Hội trường A2" : request.Location,
                StartDate = string.IsNullOrWhiteSpace(request.StartDate) ? DateTime.Now.AddDays(3).ToString("dd/MM/yyyy") : request.StartDate,
                EndDate = string.IsNullOrWhiteSpace(request.EndDate) ? DateTime.Now.AddDays(3).ToString("dd/MM/yyyy") : request.EndDate,
                MaxParticipants = request.MaxParticipants ?? 150,
                Status = string.IsNullOrWhiteSpace(request.Status) ? "Sắp diễn ra" : request.Status,
                IsApproved = isApproved,
                ApprovalStatus = approvalStatus,
                CreatedByRole = request.CreatedByRole ?? "Admin",
                CreatedByName = request.CreatedByName ?? "",
                ImageUrl = request.ImageUrl ?? "",
                VideoUrl = request.VideoUrl ?? ""
            };

            Events.Add(newEvt);
            return Task.FromResult(newEvt);
        }

        public Task<EventDto?> UpdateEventAsync(int id, UpdateEventRequest request)
        {
            var evt = Events.FirstOrDefault(e => e.ID == id);
            if (evt == null) return Task.FromResult<EventDto?>(null);

            if (!string.IsNullOrWhiteSpace(request.Name)) evt.Name = request.Name;
            if (request.Description != null) evt.Description = request.Description;
            if (!string.IsNullOrWhiteSpace(request.Icon)) evt.Icon = request.Icon;
            if (request.CategoryId.HasValue)
            {
                evt.CategoryID = request.CategoryId.Value;
                evt.Category = Categories.FirstOrDefault(c => c.ID == evt.CategoryID);
            }
            if (!string.IsNullOrWhiteSpace(request.Location)) evt.Location = request.Location;
            if (!string.IsNullOrWhiteSpace(request.StartDate)) evt.StartDate = request.StartDate;
            if (!string.IsNullOrWhiteSpace(request.EndDate)) evt.EndDate = request.EndDate;
            if (request.MaxParticipants.HasValue) evt.MaxParticipants = request.MaxParticipants.Value;
            if (!string.IsNullOrWhiteSpace(request.Status)) evt.Status = request.Status;
            if (request.IsApproved.HasValue) evt.IsApproved = request.IsApproved.Value;
            if (!string.IsNullOrWhiteSpace(request.ApprovalStatus)) evt.ApprovalStatus = request.ApprovalStatus;
            if (request.ApprovedBy != null) evt.ApprovedBy = request.ApprovedBy;
            if (request.ImageUrl != null) evt.ImageUrl = request.ImageUrl;
            if (request.VideoUrl != null) evt.VideoUrl = request.VideoUrl;

            return Task.FromResult<EventDto?>(evt);
        }

        public Task<EventDto?> ApproveEventAsync(int id, bool approve, string? approvedBy = "Admin", string? reason = null)
        {
            var evt = Events.FirstOrDefault(e => e.ID == id);
            if (evt == null) return Task.FromResult<EventDto?>(null);

            evt.IsApproved = approve;
            evt.ApprovalStatus = approve ? "Approved" : "Rejected";
            evt.ApprovedBy = approvedBy;
            evt.RejectionReason = reason;

            return Task.FromResult<EventDto?>(evt);
        }

        public Task<bool> DeleteEventAsync(int id)
        {
            var evt = Events.FirstOrDefault(e => e.ID == id);
            if (evt == null) return Task.FromResult(false);

            Events.Remove(evt);
            return Task.FromResult(true);
        }

        public Task<List<EventCategoryDto>> GetAllCategoriesAsync()
        {
            return Task.FromResult(Categories.ToList());
        }

        public Task<EventCategoryDto?> GetCategoryByIdAsync(int id)
        {
            var cat = Categories.FirstOrDefault(c => c.ID == id);
            return Task.FromResult(cat);
        }

        public Task<EventCategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
        {
            int newId = Categories.Any() ? Categories.Max(c => c.ID) + 1 : 1;
            var cat = new EventCategoryDto
            {
                ID = newId,
                Title = request.Title,
                Color = request.Color
            };
            Categories.Add(cat);
            return Task.FromResult(cat);
        }

        public Task<EventCategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
        {
            var cat = Categories.FirstOrDefault(c => c.ID == id);
            if (cat == null) return Task.FromResult<EventCategoryDto?>(null);

            if (!string.IsNullOrWhiteSpace(request.Title)) cat.Title = request.Title;
            if (!string.IsNullOrWhiteSpace(request.Color)) cat.Color = request.Color;

            return Task.FromResult<EventCategoryDto?>(cat);
        }

        public Task<bool> DeleteCategoryAsync(int id)
        {
            var cat = Categories.FirstOrDefault(c => c.ID == id);
            if (cat == null) return Task.FromResult(false);

            Categories.Remove(cat);
            return Task.FromResult(true);
        }
    }
}
