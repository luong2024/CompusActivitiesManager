using CampusActivitiesManager.Api.Models;
using CampusActivitiesManager.Api.Services;

namespace CampusActivitiesManager.Api.Tests
{
    public class InMemoryEventService : IFirebaseEventService
    {
        public List<EventCategoryDto> Categories { get; } = new()
        {
            new() { ID = 1, Title = "Học thuật", Color = "#2563EB" },
            new() { ID = 2, Title = "Thể thao", Color = "#10B981" },
            new() { ID = 3, Title = "Tình nguyện", Color = "#EF4444" },
            new() { ID = 4, Title = "Kỹ năng mềm", Color = "#7C3AED" },
            new() { ID = 5, Title = "Văn hóa - Nghệ thuật", Color = "#0284C7" }
        };

        public List<EventDto> Events { get; } = new()
        {
            new() { ID = 1, Name = "Hội thảo AI 2026", Description = "Hội thảo về AI", Icon = "💻", CategoryID = 1, Location = "Hội trường A2", StartDate = "20/10/2026", EndDate = "20/10/2026", MaxParticipants = 200, Status = "Sắp diễn ra" },
            new() { ID = 2, Name = "Giải Bóng đá Sinh viên", Description = "Giải đấu thể thao", Icon = "⚽", CategoryID = 2, Location = "Sân vận động", StartDate = "25/10/2026", EndDate = "30/10/2026", MaxParticipants = 300, Status = "Sắp diễn ra" },
            new() { ID = 3, Name = "Chiến dịch Mùa Hè Xanh", Description = "Hoạt động thiện nguyện", Icon = "🌿", CategoryID = 3, Location = "Hòa Bình", StartDate = "05/11/2026", EndDate = "15/11/2026", MaxParticipants = 80, Status = "Sắp diễn ra" }
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
            var res = query.OrderBy(e => e.ID).Take(limit).ToList();
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
                Location = request.Location ?? "Hội trường A2",
                StartDate = request.StartDate ?? "20/10/2026",
                EndDate = request.EndDate ?? "20/10/2026",
                MaxParticipants = request.MaxParticipants ?? 150,
                Status = request.Status ?? "Sắp diễn ra",
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
