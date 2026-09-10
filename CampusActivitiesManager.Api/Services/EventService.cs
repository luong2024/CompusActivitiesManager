using CampusActivitiesManager.Api.Models;

namespace CampusActivitiesManager.Api.Services
{
    public class EventService : IEventService
    {
        private readonly List<CategoryDto> _categories = [];
        private readonly List<EventDto> _events = [];
        private readonly object _lock = new();

        public EventService()
        {
            SeedInitialData();
        }

        private void SeedInitialData()
        {
            // Seed Categories
            _categories.AddRange(new[]
            {
                new CategoryDto
                {
                    Id = 1,
                    Name = "Hội thảo & Học thuật",
                    Description = "Hội thảo chuyên đề, tọa đàm khoa học và định hướng nghiên cứu",
                    Color = "#3068DF",
                    Icon = "school"
                },
                new CategoryDto
                {
                    Id = 2,
                    Name = "Văn hóa & Nghệ thuật",
                    Description = "Giao lưu văn nghệ, đêm nhạc acoustic, kịch nghệ và lễ hội văn hóa",
                    Color = "#8800FF",
                    Icon = "palette"
                },
                new CategoryDto
                {
                    Id = 3,
                    Name = "Thể dục & Thể thao",
                    Description = "Giải bóng đá, cầu lông, giải chạy việt dã và rèn luyện thể chất",
                    Color = "#FF3300",
                    Icon = "fitness"
                },
                new CategoryDto
                {
                    Id = 4,
                    Name = "Tình nguyện & Xã hội",
                    Description = "Chiến dịch Mùa hè xanh, hiến máu nhân đạo và các hoạt động vì cộng đồng",
                    Color = "#28A745",
                    Icon = "volunteer"
                }
            });

            // Seed Events: Contains events with Category (AC2) and events without Category (AC3)
            _events.AddRange(new[]
            {
                new EventDto
                {
                    Id = 1,
                    Title = "Hội thảo Ứng dụng Trí tuệ Nhân tạo & AI trong Học tập 2026",
                    Description = "Chia sẻ kiến thức thực tế về Generative AI, ứng dụng vào đề tài nghiên cứu khoa học sinh viên.",
                    Location = "Hội trường lớn A1",
                    StartDate = DateTime.UtcNow.AddDays(3).AddHours(8),
                    EndDate = DateTime.UtcNow.AddDays(3).AddHours(12),
                    Status = "Upcoming",
                    MaxParticipants = 250,
                    CurrentParticipants = 145,
                    BannerUrl = "https://images.unsplash.com/photo-1540575467063-178a50c2df87",
                    CategoryId = 1,
                    Category = _categories.FirstOrDefault(c => c.Id == 1)
                },
                new EventDto
                {
                    Id = 2,
                    Title = "Đêm Nhạc Hội Tân Sinh Viên - Spring Melody 2026",
                    Description = "Chương trình ca múa nhạc chào đón sinh viên khóa mới với sự tham gia của các ban nhạc sinh viên.",
                    Location = "Sân khấu ngoài trời - Khuôn viên Trung tâm",
                    StartDate = DateTime.UtcNow.AddDays(5).AddHours(19),
                    EndDate = DateTime.UtcNow.AddDays(5).AddHours(22),
                    Status = "Upcoming",
                    MaxParticipants = 800,
                    CurrentParticipants = 620,
                    BannerUrl = "https://images.unsplash.com/photo-1514525253161-7a46d19cd819",
                    CategoryId = 2,
                    Category = _categories.FirstOrDefault(c => c.Id == 2)
                },
                new EventDto
                {
                    Id = 3,
                    Title = "Giải Bóng Đá Sinh Viên Tranh Cúp Campus Cup 2026",
                    Description = "Giải đấu thường niên giữa các khoa viện toàn trường.",
                    Location = "Sân vận động Ký túc xá",
                    StartDate = DateTime.UtcNow.AddDays(10).AddHours(7),
                    EndDate = DateTime.UtcNow.AddDays(12).AddHours(17),
                    Status = "Upcoming",
                    MaxParticipants = 320,
                    CurrentParticipants = 180,
                    BannerUrl = "https://images.unsplash.com/photo-1574629810360-7efbbe195018",
                    CategoryId = 3,
                    Category = _categories.FirstOrDefault(c => c.Id == 3)
                },
                new EventDto
                {
                    Id = 4,
                    Title = "Ngày Hội Hiến Máu Tình Nguyện - Giọt Hồng Kết Nối",
                    Description = "Chương trình hiến máu nhân đạo do Đoàn Thanh niên & Hội Sinh viên phối hợp tổ chức.",
                    Location = "Sảnh tầng 1 - Nhà hiệu bộ",
                    StartDate = DateTime.UtcNow.AddDays(2).AddHours(7).AddMinutes(30),
                    EndDate = DateTime.UtcNow.AddDays(2).AddHours(16).AddMinutes(30),
                    Status = "Upcoming",
                    MaxParticipants = 400,
                    CurrentParticipants = 310,
                    BannerUrl = "https://images.unsplash.com/photo-1615461066841-6116e61058f4",
                    CategoryId = 4,
                    Category = _categories.FirstOrDefault(c => c.Id == 4)
                },
                // AC3: Event KHÔNG CÓ Category vẫn được xử lý trơn tru theo business rule
                new EventDto
                {
                    Id = 5,
                    Title = "Ngày Hội Sách & Không Gian Văn Hóa Đọc Tự Do",
                    Description = "Sự kiện trưng bày và trao đổi sách tự do dành cho toàn thể sinh viên, không phân nhóm danh mục.",
                    Location = "Hành lang Thư viện Trung tâm",
                    StartDate = DateTime.UtcNow.AddDays(7).AddHours(9),
                    EndDate = DateTime.UtcNow.AddDays(8).AddHours(17),
                    Status = "Upcoming",
                    MaxParticipants = 500,
                    CurrentParticipants = 95,
                    BannerUrl = "https://images.unsplash.com/photo-1512820790803-83ca734da794",
                    CategoryId = null,
                    Category = null
                },
                // AC3: Event KHÔNG CÓ Category thứ 2
                new EventDto
                {
                    Id = 6,
                    Title = "Tập Huấn Kỹ Năng Thoát Hiểm & Phòng Cháy Chữa Cháy",
                    Description = "Buổi hướng dẫn thực hành kỹ năng an toàn và xử lý tình huống khẩn cấp cho cư dân nội trú.",
                    Location = "Sân sau Tòa nhà Ký túc xá B3",
                    StartDate = DateTime.UtcNow.AddDays(14).AddHours(14),
                    EndDate = DateTime.UtcNow.AddDays(14).AddHours(17),
                    Status = "Upcoming",
                    MaxParticipants = 200,
                    CurrentParticipants = 78,
                    BannerUrl = "https://images.unsplash.com/photo-1520607162513-77705c0f0d4a",
                    CategoryId = null,
                    Category = null
                }
            });
        }

        public Task<IEnumerable<EventDto>> GetEventsAsync(
            int? categoryId = null,
            bool? hasCategory = null,
            string? search = null,
            string? status = null)
        {
            lock (_lock)
            {
                var query = _events.AsEnumerable();

                // Lọc theo CategoryId nếu có
                if (categoryId.HasValue)
                {
                    query = query.Where(e => e.CategoryId == categoryId.Value);
                }

                // Lọc theo cờ hasCategory: true = chỉ lấy event có category, false = chỉ lấy event không có category
                if (hasCategory.HasValue)
                {
                    if (hasCategory.Value)
                    {
                        query = query.Where(e => e.CategoryId.HasValue && e.Category != null);
                    }
                    else
                    {
                        query = query.Where(e => !e.CategoryId.HasValue || e.Category == null);
                    }
                }

                // Tìm kiếm theo từ khóa (Title, Description, Location, Category Name)
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.Trim();
                    query = query.Where(e =>
                        e.Title.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                        e.Description.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                        e.Location.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                        (e.Category != null && e.Category.Name.Contains(s, StringComparison.OrdinalIgnoreCase)));
                }

                // Lọc theo Status nếu có
                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(e => string.Equals(e.Status, status.Trim(), StringComparison.OrdinalIgnoreCase));
                }

                // Đảm bảo dữ liệu Category đồng bộ an toàn:
                // Nếu CategoryId có giá trị thì nạp CategoryDto tương ứng; nếu không có (null) thì giữ Category = null
                var result = query.Select(e =>
                {
                    var copy = new EventDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        Location = e.Location,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        Status = e.Status,
                        MaxParticipants = e.MaxParticipants,
                        CurrentParticipants = e.CurrentParticipants,
                        BannerUrl = e.BannerUrl,
                        CategoryId = e.CategoryId,
                        Category = e.CategoryId.HasValue ? _categories.FirstOrDefault(c => c.Id == e.CategoryId.Value) : null,
                        CreatedAt = e.CreatedAt
                    };
                    return copy;
                }).OrderBy(e => e.Id).ToList();

                // AC4: Nếu không có Event phù hợp, trả về danh sách rỗng []
                return Task.FromResult<IEnumerable<EventDto>>(result);
            }
        }

        public Task<EventDto?> GetEventByIdAsync(int id)
        {
            lock (_lock)
            {
                var evt = _events.FirstOrDefault(e => e.Id == id);
                if (evt == null)
                {
                    return Task.FromResult<EventDto?>(null);
                }

                var copy = new EventDto
                {
                    Id = evt.Id,
                    Title = evt.Title,
                    Description = evt.Description,
                    Location = evt.Location,
                    StartDate = evt.StartDate,
                    EndDate = evt.EndDate,
                    Status = evt.Status,
                    MaxParticipants = evt.MaxParticipants,
                    CurrentParticipants = evt.CurrentParticipants,
                    BannerUrl = evt.BannerUrl,
                    CategoryId = evt.CategoryId,
                    Category = evt.CategoryId.HasValue ? _categories.FirstOrDefault(c => c.Id == evt.CategoryId.Value) : null,
                    CreatedAt = evt.CreatedAt
                };

                return Task.FromResult<EventDto?>(copy);
            }
        }

        public Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            lock (_lock)
            {
                return Task.FromResult<IEnumerable<CategoryDto>>(_categories.ToList());
            }
        }

        public Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            lock (_lock)
            {
                var cat = _categories.FirstOrDefault(c => c.Id == id);
                return Task.FromResult(cat);
            }
        }
    }
}
