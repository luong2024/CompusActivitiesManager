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
                _logger.LogWarning(ex, "API get events failed, falling back to local defaults");
            }

            var defaults = GetDefaultEvents();
            return includePending ? defaults : defaults.Where(e => e.IsApproved).ToList();
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

            var defaults = GetDefaultEvents();
            return defaults.FirstOrDefault(e => e.ID == id);
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
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event via API");
                _errorHandler.HandleError(ex);
                return false;
            }
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
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event via API");
                _errorHandler.HandleError(ex);
                return false;
            }
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
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving/rejecting event via API");
                _errorHandler.HandleError(ex);
                return false;
            }
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{Constants.EventsApiUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event via API");
                _errorHandler.HandleError(ex);
                return false;
            }
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
                _logger.LogWarning(ex, "API get categories failed, using default list");
            }

            return GetDefaultCategories();
        }

        private List<EventCategoryDto> GetDefaultCategories()
        {
            return new List<EventCategoryDto>
            {
                new() { ID = 1, Title = "Học thuật", Color = "#2563EB" },
                new() { ID = 2, Title = "Thể thao", Color = "#10B981" },
                new() { ID = 3, Title = "Tình nguyện", Color = "#EF4444" },
                new() { ID = 4, Title = "Kỹ năng mềm", Color = "#7C3AED" },
                new() { ID = 5, Title = "Văn hóa - Nghệ thuật", Color = "#0284C7" }
            };
        }

        private List<CampusEvent> GetDefaultEvents()
        {
            var cats = GetDefaultCategories().ToDictionary(c => c.ID, c => c);
            return new List<CampusEvent>
            {
                new()
                {
                    ID = 1,
                    Name = "Hội thảo AI & Công nghệ Thông tin 2026",
                    Description = "Cập nhật xu hướng Generative AI và Cloud Computing cho sinh viên toàn trường",
                    Icon = "💻",
                    CategoryID = 1,
                    Category = cats[1],
                    Location = "Hội trường A2",
                    StartDate = "20/10/2026",
                    EndDate = "20/10/2026",
                    MaxParticipants = 200,
                    Status = "Sắp diễn ra"
                },
                new()
                {
                    ID = 2,
                    Name = "Giải Bóng đá Sinh viên Toàn trường",
                    Description = "Khai mạc giải đấu thể thao thường niên tranh cúp vô địch tại Sân vận động",
                    Icon = "⚽",
                    CategoryID = 2,
                    Category = cats[2],
                    Location = "Sân Vận Động Trường",
                    StartDate = "25/10/2026",
                    EndDate = "30/10/2026",
                    MaxParticipants = 350,
                    Status = "Sắp diễn ra"
                },
                new()
                {
                    ID = 3,
                    Name = "Chiến dịch Tình nguyện Mùa Hè Xanh",
                    Description = "Hoạt động tình nguyện hỗ trợ cộng đồng, xây dựng sân chơi và dạy học vùng cao",
                    Icon = "🌿",
                    CategoryID = 3,
                    Category = cats[3],
                    Location = "Tỉnh Hòa Bình",
                    StartDate = "05/11/2026",
                    EndDate = "15/11/2026",
                    MaxParticipants = 80,
                    Status = "Sắp diễn ra"
                },
                new()
                {
                    ID = 4,
                    Name = "Workshop Kỹ năng Thuyết trình & Phỏng vấn",
                    Description = "Phát triển kỹ năng giao tiếp tự tin và chinh phục nhà tuyển dụng công nghệ",
                    Icon = "🗣️",
                    CategoryID = 4,
                    Category = cats[4],
                    Location = "Phòng Hội thảo 302-B1",
                    StartDate = "12/11/2026",
                    EndDate = "12/11/2026",
                    MaxParticipants = 120,
                    Status = "Sắp diễn ra"
                },
                new()
                {
                    ID = 5,
                    Name = "Đêm nhạc Acoustic Chào Tân Sinh viên",
                    Description = "Giao lưu âm nhạc và văn nghệ đặc sắc giữa các câu lạc bộ toàn trường",
                    Icon = "🎸",
                    CategoryID = 5,
                    Category = cats[5],
                    Location = "Sảnh Giảng đường C",
                    StartDate = "18/11/2026",
                    EndDate = "18/11/2026",
                    MaxParticipants = 400,
                    Status = "Sắp diễn ra"
                }
            };
        }
    }
}
