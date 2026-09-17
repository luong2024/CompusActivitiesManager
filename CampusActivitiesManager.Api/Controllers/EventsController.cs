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
                new EventCategoryDto { ID = 1, Title = "H?c thu?t", Color = "#2563EB" },
                new EventCategoryDto { ID = 2, Title = "Th? thao", Color = "#059669" },
                new EventCategoryDto { ID = 3, Title = "Tình nguy?n", Color = "#DC2626" },
                new EventCategoryDto { ID = 4, Title = "K? nang", Color = "#7C3AED" }
            };

            var events = new List<EventDto>
            {
                new EventDto { ID = 1, Name = "H?i th?o Công ngh? Thông tin 2026", Description = "C?p nh?t xu hu?ng AI và Blockchain", Icon = "??", Category = categories[0] },
                new EventDto { ID = 2, Name = "Gi?i Bóng dá Sinh viên Toàn tru?ng", Description = "Khai m?c gi?i d?u th? thao l?n nh?t nam", Icon = "?", Category = categories[1] },
                new EventDto { ID = 3, Name = "Mùa Hè Xanh - Tình nguy?n Hè", Description = "Xây d?ng nông thôn m?i và d?y h?c", Icon = "??", Category = categories[2] },
                new EventDto { ID = 4, Name = "Workshop K? nang Thuy?t trình", Description = "Phát tri?n k? nang giao ti?p và t? tin", Icon = "??", Category = categories[3] }
            };

            return Ok(new ApiResponse<List<EventDto>>
            {
                Success = true,
                StatusCode = 200,
                Data = events,
                Message = "L?y danh sách s? ki?n thành công"
            });
        }
    }
}
