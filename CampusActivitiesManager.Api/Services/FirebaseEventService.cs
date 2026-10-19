using CampusActivitiesManager.Api.Models;
using Google.Cloud.Firestore;

namespace CampusActivitiesManager.Api.Services
{
    public class FirebaseEventService : IFirebaseEventService
    {
        private readonly FirestoreDb? _firestoreDb;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FirebaseEventService> _logger;

        public FirebaseEventService(IConfiguration configuration, ILogger<FirebaseEventService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            try
            {
                string? credPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")?.Trim('"', ' ');
                if (string.IsNullOrEmpty(credPath) || !File.Exists(credPath))
                {
                    if (File.Exists(@"D:\keys\campusacmanage-df24bcb494c7.json"))
                    {
                        credPath = @"D:\keys\campusacmanage-df24bcb494c7.json";
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credPath);
                    }
                    else
                    {
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", null);
                        credPath = null;
                    }
                }

                string projectId = _configuration["Firebase:ProjectId"]
                    ?? Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT")?.Trim('"', ' ')
                    ?? "campusacmanage";
                _firestoreDb = FirestoreDb.Create(projectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not initialize FirestoreDb in FirebaseEventService");
                _firestoreDb = null;
            }
        }

        private async Task EnsureSeedDataAsync()
        {
            if (_firestoreDb == null) return;

            try
            {
                var catRef = _firestoreDb.Collection("categories");
                var catSnap = await catRef.Limit(1).GetSnapshotAsync();
                if (catSnap.Count == 0)
                {
                    var defaultCategories = new List<EventCategoryDto>
                    {
                        new() { ID = 1, Title = "Học thuật", Color = "#2563EB" },
                        new() { ID = 2, Title = "Thể thao", Color = "#10B981" },
                        new() { ID = 3, Title = "Tình nguyện", Color = "#EF4444" },
                        new() { ID = 4, Title = "Kỹ năng mềm", Color = "#F59E0B" },
                        new() { ID = 5, Title = "Văn hóa - Nghệ thuật", Color = "#0EA5E9" }
                    };

                    foreach (var cat in defaultCategories)
                    {
                        await catRef.Document(cat.ID.ToString()).SetAsync(new Dictionary<string, object>
                        {
                            ["id"] = cat.ID,
                            ["title"] = cat.Title,
                            ["color"] = cat.Color
                        });
                    }
                }

                var evtRef = _firestoreDb.Collection("events");
                var evtSnap = await evtRef.Limit(1).GetSnapshotAsync();
                if (evtSnap.Count == 0)
                {
                    var defaultEvents = new List<EventDto>
                    {
                        new() { ID = 1, Name = "Hội thảo Công nghệ Thông tin 2026", Description = "Cập nhật xu hướng AI và Cloud Computing cho sinh viên toàn trường", Icon = "💻", CategoryID = 1 },
                        new() { ID = 2, Name = "Giải Bóng đá Sinh viên Toàn trường", Description = "Khai mạc giải đấu thể thao lớn nhất năm học tại sân vận động", Icon = "⚽", CategoryID = 2 },
                        new() { ID = 3, Name = "Chiến dịch Mùa Hè Xanh 2026", Description = "Hoạt động tình nguyện hỗ trợ cộng đồng và dạy học vùng cao", Icon = "🌿", CategoryID = 3 },
                        new() { ID = 4, Name = "Workshop Kỹ năng Thuyết trình & Phỏng vấn", Description = "Phát triển kỹ năng giao tiếp và chinh phục nhà tuyển dụng", Icon = "🗣️", CategoryID = 4 },
                        new() { ID = 5, Name = "Đêm nhạc Acoustic Chào Tân Sinh viên", Description = "Giao lưu âm nhạc giữa các câu lạc bộ nghệ thuật trong trường", Icon = "🎸", CategoryID = 5 }
                    };

                    foreach (var evt in defaultEvents)
                    {
                        await evtRef.Document(evt.ID.ToString()).SetAsync(new Dictionary<string, object>
                        {
                            ["id"] = evt.ID,
                            ["name"] = evt.Name,
                            ["description"] = evt.Description,
                            ["icon"] = evt.Icon,
                            ["categoryId"] = evt.CategoryID
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error seeding initial categories/events in Firestore");
            }
        }

        public async Task<List<EventCategoryDto>> GetAllCategoriesAsync()
        {
            if (_firestoreDb == null) return new List<EventCategoryDto>();

            await EnsureSeedDataAsync();
            var categories = new List<EventCategoryDto>();
            var snapshot = await _firestoreDb.Collection("categories").GetSnapshotAsync();
            foreach (var doc in snapshot.Documents)
            {
                var dict = doc.ToDictionary();
                int id = dict.TryGetValue("id", out var idVal) ? Convert.ToInt32(idVal) : (int.TryParse(doc.Id, out var parsed) ? parsed : 0);
                categories.Add(new EventCategoryDto
                {
                    ID = id,
                    Title = dict.TryGetValue("title", out var t) ? t?.ToString() ?? "" : "",
                    Color = dict.TryGetValue("color", out var c) ? c?.ToString() ?? "#2563EB" : "#2563EB"
                });
            }
            return categories.OrderBy(c => c.ID).ToList();
        }

        public async Task<EventCategoryDto?> GetCategoryByIdAsync(int id)
        {
            if (_firestoreDb == null) return null;

            await EnsureSeedDataAsync();
            var doc = await _firestoreDb.Collection("categories").Document(id.ToString()).GetSnapshotAsync();
            if (!doc.Exists) return null;

            var dict = doc.ToDictionary();
            return new EventCategoryDto
            {
                ID = id,
                Title = dict.TryGetValue("title", out var t) ? t?.ToString() ?? "" : "",
                Color = dict.TryGetValue("color", out var c) ? c?.ToString() ?? "#2563EB" : "#2563EB"
            };
        }

        public async Task<EventCategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
        {
            var categories = await GetAllCategoriesAsync();
            int newId = categories.Any() ? categories.Max(c => c.ID) + 1 : 1;

            var newCat = new EventCategoryDto
            {
                ID = newId,
                Title = request.Title,
                Color = string.IsNullOrWhiteSpace(request.Color) ? "#2563EB" : request.Color
            };

            if (_firestoreDb != null)
            {
                await _firestoreDb.Collection("categories").Document(newId.ToString()).SetAsync(new Dictionary<string, object>
                {
                    ["id"] = newCat.ID,
                    ["title"] = newCat.Title,
                    ["color"] = newCat.Color
                });
            }

            return newCat;
        }

        public async Task<EventCategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
        {
            var existing = await GetCategoryByIdAsync(id);
            if (existing == null) return null;

            if (!string.IsNullOrWhiteSpace(request.Title)) existing.Title = request.Title;
            if (!string.IsNullOrWhiteSpace(request.Color)) existing.Color = request.Color;

            if (_firestoreDb != null)
            {
                await _firestoreDb.Collection("categories").Document(id.ToString()).SetAsync(new Dictionary<string, object>
                {
                    ["id"] = existing.ID,
                    ["title"] = existing.Title,
                    ["color"] = existing.Color
                }, SetOptions.MergeAll);
            }

            return existing;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var existing = await GetCategoryByIdAsync(id);
            if (existing == null) return false;

            if (_firestoreDb != null)
            {
                await _firestoreDb.Collection("categories").Document(id.ToString()).DeleteAsync();
            }

            return true;
        }

        public async Task<List<EventDto>> GetAllEventsAsync(int limit = 20, int? afterId = null, bool includePending = false)
        {
            if (_firestoreDb == null) return new List<EventDto>();

            await EnsureSeedDataAsync();
            var categories = await GetAllCategoriesAsync();
            var catMap = categories.ToDictionary(c => c.ID, c => c);

            int safeLimit = limit <= 0 ? 20 : Math.Min(limit, 100);
            Query query = _firestoreDb.Collection("events").OrderBy("id");
            if (afterId.HasValue)
            {
                query = query.StartAfter(afterId.Value);
            }
            query = query.Limit(safeLimit);

            var events = new List<EventDto>();
            var snapshot = await query.GetSnapshotAsync();
            foreach (var doc in snapshot.Documents)
            {
                var dict = doc.ToDictionary();
                int id = dict.TryGetValue("id", out var idVal) ? Convert.ToInt32(idVal) : (int.TryParse(doc.Id, out var parsed) ? parsed : 0);
                int catId = dict.TryGetValue("categoryId", out var cVal) && cVal != null ? Convert.ToInt32(cVal) : 1;
                catMap.TryGetValue(catId, out var catDto);

                bool isApproved = dict.TryGetValue("isApproved", out var appVal) && appVal != null ? Convert.ToBoolean(appVal) : true;
                string approvalStatus = dict.TryGetValue("approvalStatus", out var statVal) && statVal != null ? statVal.ToString()! : (isApproved ? "Approved" : "Pending");

                if (!includePending && !isApproved)
                {
                    continue;
                }

                events.Add(new EventDto
                {
                    ID = id,
                    Name = dict.TryGetValue("name", out var n) ? n?.ToString() ?? "" : "",
                    Description = dict.TryGetValue("description", out var d) ? d?.ToString() ?? "" : "",
                    Icon = dict.TryGetValue("icon", out var ic) ? ic?.ToString() ?? "📅" : "📅",
                    CategoryID = catId,
                    Category = catDto,
                    Location = dict.TryGetValue("location", out var loc) ? loc?.ToString() ?? "Hội trường A2" : "Hội trường A2",
                    StartDate = dict.TryGetValue("startDate", out var sd) ? sd?.ToString() ?? DateTime.Now.AddDays(3).ToString("dd/MM/yyyy") : DateTime.Now.AddDays(3).ToString("dd/MM/yyyy"),
                    EndDate = dict.TryGetValue("endDate", out var ed) ? ed?.ToString() ?? DateTime.Now.AddDays(3).ToString("dd/MM/yyyy") : DateTime.Now.AddDays(3).ToString("dd/MM/yyyy"),
                    MaxParticipants = dict.TryGetValue("maxParticipants", out var mp) && mp != null ? Convert.ToInt32(mp) : 150,
                    Status = dict.TryGetValue("status", out var st) ? st?.ToString() ?? "Sắp diễn ra" : "Sắp diễn ra",
                    IsApproved = isApproved,
                    ApprovalStatus = approvalStatus,
                    CreatedByRole = dict.TryGetValue("createdByRole", out var rVal) ? rVal?.ToString() ?? "Admin" : "Admin",
                    CreatedByName = dict.TryGetValue("createdByName", out var cName) ? cName?.ToString() ?? "" : "",
                    ApprovedBy = dict.TryGetValue("approvedBy", out var apBy) ? apBy?.ToString() : null,
                    ImageUrl = dict.TryGetValue("imageUrl", out var img) ? img?.ToString() ?? "" : "",
                    VideoUrl = dict.TryGetValue("videoUrl", out var vid) ? vid?.ToString() ?? "" : ""
                });
            }

            return events.OrderBy(e => e.ID).ToList();
        }

        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
            if (_firestoreDb == null) return null;

            await EnsureSeedDataAsync();
            var doc = await _firestoreDb.Collection("events").Document(id.ToString()).GetSnapshotAsync();
            if (!doc.Exists) return null;

            var dict = doc.ToDictionary();
            int catId = dict.TryGetValue("categoryId", out var cVal) && cVal != null ? Convert.ToInt32(cVal) : 1;
            var category = await GetCategoryByIdAsync(catId);

            bool isApproved = dict.TryGetValue("isApproved", out var appVal) && appVal != null ? Convert.ToBoolean(appVal) : true;
            string approvalStatus = dict.TryGetValue("approvalStatus", out var statVal) && statVal != null ? statVal.ToString()! : (isApproved ? "Approved" : "Pending");

            return new EventDto
            {
                ID = id,
                Name = dict.TryGetValue("name", out var n) ? n?.ToString() ?? "" : "",
                Description = dict.TryGetValue("description", out var d) ? d?.ToString() ?? "" : "",
                Icon = dict.TryGetValue("icon", out var ic) ? ic?.ToString() ?? "📅" : "📅",
                CategoryID = catId,
                Category = category,
                Location = dict.TryGetValue("location", out var loc) ? loc?.ToString() ?? "Hội trường A2" : "Hội trường A2",
                StartDate = dict.TryGetValue("startDate", out var sd) ? sd?.ToString() ?? DateTime.Now.AddDays(3).ToString("dd/MM/yyyy") : DateTime.Now.AddDays(3).ToString("dd/MM/yyyy"),
                EndDate = dict.TryGetValue("endDate", out var ed) ? ed?.ToString() ?? DateTime.Now.AddDays(3).ToString("dd/MM/yyyy") : DateTime.Now.AddDays(3).ToString("dd/MM/yyyy"),
                MaxParticipants = dict.TryGetValue("maxParticipants", out var mp) && mp != null ? Convert.ToInt32(mp) : 150,
                Status = dict.TryGetValue("status", out var st) ? st?.ToString() ?? "Sắp diễn ra" : "Sắp diễn ra",
                IsApproved = isApproved,
                ApprovalStatus = approvalStatus,
                CreatedByRole = dict.TryGetValue("createdByRole", out var rVal) ? rVal?.ToString() ?? "Admin" : "Admin",
                CreatedByName = dict.TryGetValue("createdByName", out var cName) ? cName?.ToString() ?? "" : "",
                ApprovedBy = dict.TryGetValue("approvedBy", out var apBy) ? apBy?.ToString() : null,
                ImageUrl = dict.TryGetValue("imageUrl", out var img) ? img?.ToString() ?? "" : "",
                VideoUrl = dict.TryGetValue("videoUrl", out var vid) ? vid?.ToString() ?? "" : ""
            };
        }

        public async Task<EventDto> CreateEventAsync(CreateEventRequest request)
        {
            var events = await GetAllEventsAsync(100, null, true);
            int newId = events.Any() ? events.Max(e => e.ID) + 1 : 1;
            int catId = request.CategoryId ?? 1;
            var category = await GetCategoryByIdAsync(catId);

            bool isApproved = request.IsApproved ?? (request.CreatedByRole == "Admin");
            string approvalStatus = request.ApprovalStatus ?? (isApproved ? "Approved" : "Pending");

            var newEvent = new EventDto
            {
                ID = newId,
                Name = request.Name,
                Description = request.Description,
                Icon = string.IsNullOrWhiteSpace(request.Icon) ? "📅" : request.Icon,
                CategoryID = catId,
                Category = category,
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

            if (_firestoreDb != null)
            {
                await _firestoreDb.Collection("events").Document(newId.ToString()).SetAsync(new Dictionary<string, object>
                {
                    ["id"] = newEvent.ID,
                    ["name"] = newEvent.Name,
                    ["description"] = newEvent.Description,
                    ["icon"] = newEvent.Icon,
                    ["categoryId"] = newEvent.CategoryID,
                    ["location"] = newEvent.Location,
                    ["startDate"] = newEvent.StartDate,
                    ["endDate"] = newEvent.EndDate,
                    ["maxParticipants"] = newEvent.MaxParticipants,
                    ["status"] = newEvent.Status,
                    ["isApproved"] = newEvent.IsApproved,
                    ["approvalStatus"] = newEvent.ApprovalStatus,
                    ["createdByRole"] = newEvent.CreatedByRole,
                    ["createdByName"] = newEvent.CreatedByName,
                    ["imageUrl"] = newEvent.ImageUrl,
                    ["videoUrl"] = newEvent.VideoUrl
                });
            }

            return newEvent;
        }

        public async Task<EventDto?> UpdateEventAsync(int id, UpdateEventRequest request)
        {
            var existing = await GetEventByIdAsync(id);
            if (existing == null) return null;

            if (!string.IsNullOrWhiteSpace(request.Name)) existing.Name = request.Name;
            if (request.Description != null) existing.Description = request.Description;
            if (!string.IsNullOrWhiteSpace(request.Icon)) existing.Icon = request.Icon;
            if (request.CategoryId.HasValue)
            {
                existing.CategoryID = request.CategoryId.Value;
                existing.Category = await GetCategoryByIdAsync(existing.CategoryID);
            }
            if (!string.IsNullOrWhiteSpace(request.Location)) existing.Location = request.Location;
            if (!string.IsNullOrWhiteSpace(request.StartDate)) existing.StartDate = request.StartDate;
            if (!string.IsNullOrWhiteSpace(request.EndDate)) existing.EndDate = request.EndDate;
            if (request.MaxParticipants.HasValue) existing.MaxParticipants = request.MaxParticipants.Value;
            if (!string.IsNullOrWhiteSpace(request.Status)) existing.Status = request.Status;
            if (request.IsApproved.HasValue) existing.IsApproved = request.IsApproved.Value;
            if (!string.IsNullOrWhiteSpace(request.ApprovalStatus)) existing.ApprovalStatus = request.ApprovalStatus;
            if (request.ApprovedBy != null) existing.ApprovedBy = request.ApprovedBy;
            if (request.ImageUrl != null) existing.ImageUrl = request.ImageUrl;
            if (request.VideoUrl != null) existing.VideoUrl = request.VideoUrl;

            if (_firestoreDb != null)
            {
                await _firestoreDb.Collection("events").Document(id.ToString()).SetAsync(new Dictionary<string, object>
                {
                    ["id"] = existing.ID,
                    ["name"] = existing.Name,
                    ["description"] = existing.Description,
                    ["icon"] = existing.Icon,
                    ["categoryId"] = existing.CategoryID,
                    ["location"] = existing.Location,
                    ["startDate"] = existing.StartDate,
                    ["endDate"] = existing.EndDate,
                    ["maxParticipants"] = existing.MaxParticipants,
                    ["status"] = existing.Status,
                    ["isApproved"] = existing.IsApproved,
                    ["approvalStatus"] = existing.ApprovalStatus,
                    ["approvedBy"] = existing.ApprovedBy ?? "",
                    ["imageUrl"] = existing.ImageUrl,
                    ["videoUrl"] = existing.VideoUrl
                }, SetOptions.MergeAll);
            }

            return existing;
        }

        public async Task<EventDto?> ApproveEventAsync(int id, bool approve, string? approvedBy = "Admin", string? reason = null)
        {
            var existing = await GetEventByIdAsync(id);
            if (existing == null) return null;

            existing.IsApproved = approve;
            existing.ApprovalStatus = approve ? "Approved" : "Rejected";
            existing.ApprovedBy = approvedBy;
            existing.RejectionReason = reason;

            if (_firestoreDb != null)
            {
                await _firestoreDb.Collection("events").Document(id.ToString()).SetAsync(new Dictionary<string, object>
                {
                    ["isApproved"] = existing.IsApproved,
                    ["approvalStatus"] = existing.ApprovalStatus,
                    ["approvedBy"] = approvedBy ?? "Admin",
                    ["rejectionReason"] = reason ?? ""
                }, SetOptions.MergeAll);
            }

            return existing;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var existing = await GetEventByIdAsync(id);
            if (existing == null) return false;

            if (_firestoreDb != null)
            {
                await _firestoreDb.Collection("events").Document(id.ToString()).DeleteAsync();
            }

            return true;
        }
    }
}
