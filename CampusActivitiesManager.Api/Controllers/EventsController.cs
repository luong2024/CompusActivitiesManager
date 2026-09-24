using CampusActivitiesManager.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Controllers
{
    [Route("api/v1/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetEvents()
        {
            var categories = new List<EventCategoryDto>
            {
                new EventCategoryDto { ID = 1, Title = "Học thuật", Color = "#2563EB" },
                new EventCategoryDto { ID = 2, Title = "Thể thao", Color = "#059669" },
                new EventCategoryDto { ID = 3, Title = "Tình nguyện", Color = "#DC2626" },
                new EventCategoryDto { ID = 4, Title = "Kỹ năng", Color = "#7C3AED" }
            };

            var events = new List<EventDto>
            {
                new EventDto { ID = 1, Name = "Hội thảo Công nghệ Thông tin 2026", Description = "Cập nhật xu hướng AI và Blockchain", Icon = "💻", Category = categories[0] },
                new EventDto { ID = 2, Name = "Giải Bóng đá Sinh viên Toàn trường", Description = "Khai mạc giải đấu thể thao lớn nhất năm", Icon = "⚽", Category = categories[1] },
                new EventDto { ID = 3, Name = "Mùa Hè Xanh - Tình nguyện Hè", Description = "Xây dựng nông thôn mới và dạy học", Icon = "🌿", Category = categories[2] },
                new EventDto { ID = 4, Name = "Workshop Kỹ năng Thuyết trình", Description = "Phát triển kỹ năng giao tiếp và tự tin", Icon = "🗣️", Category = categories[3] }
            };

            return Ok(new ApiResponse<List<EventDto>>
            {
                Success = true,
                StatusCode = 200,
                Data = events,
                Message = "Lấy danh sách sự kiện thành công"
            });
        }
    }
}
