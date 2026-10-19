using System.Net.Http.Json;
using System.Text.Json;
using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    public class EventService : IEventService
    {
        private readonly HttpClient _httpClient;
        private readonly ModalErrorHandler _errorHandler;
        private readonly ILogger<EventService> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

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
                ImageUrl = "https://images.unsplash.com/photo-1517245386807-bb43f82c33c4?auto=format&fit=crop&w=800&q=80"
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
                StartDate = DateTime.Now.AddDays(8).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(8).ToString("dd/MM/yyyy"),
                MaxParticipants = 120,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                ApprovedBy = "Admin"
            },
            new()
            {
                ID = 5,
                Name = "Đêm nhạc Acoustic Chào Tân Sinh viên",
                Description = "Giao lưu âm nhạc và văn nghệ đặc sắc giữa các câu lạc bộ toàn trường",
                Icon = "🎸",
                CategoryID = 5,
                Category = _categories[4],
                Location = "Sảnh Giảng đường C",
                StartDate = DateTime.Now.AddDays(12).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(12).ToString("dd/MM/yyyy"),
                MaxParticipants = 400,
                Status = "Sắp diễn ra",
                IsApproved = true,
                ApprovalStatus = "Approved",
                CreatedByRole = "Admin",
                CreatedByName = "Quản trị viên Hệ thống",
                ApprovedBy = "Admin"
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
                CreatedByName = "Ban Tổ chức Sự kiện"
            }
        };

        public EventService(ModalErrorHandler errorHandler, ILogger<EventService> logger)
        {
            _httpClient = HttpHelper.CreateApiClient();
            _errorHandler = errorHandler;
            _logger = logger;
        }

        public async Task<List<CampusEvent>> GetEventsAsync(bool includePending = false)
        {
            try
            {
                string url = includePending ? $"{Constants.EventsApiUrl}?includePending=true" : Constants.EventsApiUrl;
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<CampusEvent>>>(JsonOptions);
                    if (result != null && result.Success && result.Data != null && result.Data.Count > 0)
                    {
                        return result.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API get events failed, using local in-app event store");
            }

            return includePending ? _localEvents.ToList() : _localEvents.Where(e => e.IsApproved).ToList();
        }

        public async Task<CampusEvent?> GetEventByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{Constants.EventsApiUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<CampusEvent>>(JsonOptions);
                    if (result != null && result.Success && result.Data != null)
                    {
                        return result.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API get event by id {Id} failed", id);
            }

            return _localEvents.FirstOrDefault(e => e.ID == id);
        }

        public async Task<bool> CreateEventAsync(CampusEvent evt)
        {
            try
            {
                var payload = new
                {
                    name = evt.Name,
                    description = evt.Description,
                    icon = string.IsNullOrWhiteSpace(evt.Icon) ? "📅" : evt.Icon,
                    categoryId = evt.CategoryID,
                    location = evt.Location,
                    startDate = evt.StartDate,
                    endDate = evt.EndDate,
                    maxParticipants = evt.MaxParticipants,
                    status = evt.Status,
                    isApproved = evt.IsApproved,
                    approvalStatus = evt.ApprovalStatus,
                    createdByRole = evt.CreatedByRole,
                    createdByName = evt.CreatedByName,
                    imageUrl = evt.ImageUrl,
                    videoUrl = evt.VideoUrl
                };

                var response = await _httpClient.PostAsJsonAsync(Constants.EventsApiUrl, payload);
                if (response.IsSuccessStatusCode) return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API create event failed, applying locally");
            }

            int newId = _localEvents.Any() ? _localEvents.Max(e => e.ID) + 1 : 1;
            evt.ID = newId;
            evt.Category = _categories.FirstOrDefault(c => c.ID == evt.CategoryID) ?? _categories[0];
            _localEvents.Add(evt);
            return true;
        }

        public async Task<bool> UpdateEventAsync(CampusEvent evt)
        {
            try
            {
                var payload = new
                {
                    name = evt.Name,
                    description = evt.Description,
                    icon = evt.Icon,
                    categoryId = evt.CategoryID,
                    location = evt.Location,
                    startDate = evt.StartDate,
                    endDate = evt.EndDate,
                    maxParticipants = evt.MaxParticipants,
                    status = evt.Status,
                    isApproved = evt.IsApproved,
                    approvalStatus = evt.ApprovalStatus,
                    imageUrl = evt.ImageUrl,
                    videoUrl = evt.VideoUrl
                };

                var response = await _httpClient.PutAsJsonAsync($"{Constants.EventsApiUrl}/{evt.ID}", payload);
                if (response.IsSuccessStatusCode) return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API update event failed, applying locally");
            }

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
                return true;
            }
            return false;
        }

        public async Task<bool> ApproveEventAsync(int id, bool approve, string? approvedBy = "Admin", string? reason = null)
        {
            try
            {
                string endpoint = approve ? "approve" : "reject";
                var payload = new
                {
                    isApproved = approve,
                    approvedBy = approvedBy ?? "Admin",
                    reason = reason
                };

                var response = await _httpClient.PostAsJsonAsync($"{Constants.EventsApiUrl}/{id}/{endpoint}", payload);
                if (response.IsSuccessStatusCode) return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API approve event failed, applying locally");
            }

            var local = _localEvents.FirstOrDefault(e => e.ID == id);
            if (local != null)
            {
                local.IsApproved = approve;
                local.ApprovalStatus = approve ? "Approved" : "Rejected";
                local.ApprovedBy = approvedBy;
                local.RejectionReason = reason;
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{Constants.EventsApiUrl}/{id}");
                if (response.IsSuccessStatusCode) return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API delete event failed, applying locally");
            }

            var local = _localEvents.FirstOrDefault(e => e.ID == id);
            if (local != null)
            {
                _localEvents.Remove(local);
                return true;
            }
            return false;
        }

        public async Task<List<EventCategoryDto>> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(Constants.CategoriesApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<EventCategoryDto>>>(JsonOptions);
                    if (result != null && result.Success && result.Data != null && result.Data.Count > 0)
                    {
                        return result.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API get categories failed, using default in-app list");
            }

            return _categories.ToList();
        }
    }
}
