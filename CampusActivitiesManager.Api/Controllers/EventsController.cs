using CampusActivitiesManager.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Controllers
{
    [Route("api/v1/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private static List<EventCategoryDto> _categories = new List<EventCategoryDto>
        {
            new EventCategoryDto { ID = 1, Title = "Học thuật", Color = "#2563EB" },
            new EventCategoryDto { ID = 2, Title = "Thể thao", Color = "#059669" },
            new EventCategoryDto { ID = 3, Title = "Tình nguyện", Color = "#DC2626" },
            new EventCategoryDto { ID = 4, Title = "Kỹ năng", Color = "#7C3AED" }
        };

        private static List<EventDto> _events = new List<EventDto>
        {
            new EventDto { ID = 1, Name = "Hội thảo Công nghệ Thông tin 2026", Description = "Cập nhật xu hướng AI và Blockchain", Icon = "💻", Category = _categories[0] },
            new EventDto { ID = 2, Name = "Giải Bóng đá Sinh viên Toàn trường", Description = "Khai mạc giải đấu thể thao lớn nhất năm", Icon = "⚽", Category = _categories[1] },
            new EventDto { ID = 3, Name = "Mùa Hè Xanh - Tình nguyện Hè", Description = "Xây dựng nông thôn mới và dạy học", Icon = "🌿", Category = _categories[2] },
            new EventDto { ID = 4, Name = "Workshop Kỹ năng Thuyết trình", Description = "Phát triển kỹ năng giao tiếp và tự tin", Icon = "🗣️", Category = _categories[3] }
        };

        [HttpGet]
        public IActionResult GetEvents()
        {
            return Ok(new ApiResponse<List<EventDto>>
            {
                Success = true,
                StatusCode = 200,
                Data = _events,
                Message = "Lấy danh sách sự kiện thành công"
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetEventById(int id)
        {
            var evt = _events.FirstOrDefault(e => e.ID == id);
            if (evt == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Error = "NOT_FOUND",
                    Message = "Không tìm thấy sự kiện"
                });
            }

            return Ok(new ApiResponse<EventDto>
            {
                Success = true,
                StatusCode = 200,
                Data = evt,
                Message = "Thành công"
            });
        }

        [HttpPost]
        public IActionResult CreateEvent([FromBody] CreateEventRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newId = _events.Any() ? _events.Max(e => e.ID) + 1 : 1;
            var category = _categories.FirstOrDefault(c => c.ID == request.CategoryId);

            var newEvent = new EventDto
            {
                ID = newId,
                Name = request.Name,
                Description = request.Description,
                Icon = request.Icon,
                Category = category
            };

            _events.Add(newEvent);

            return CreatedAtAction(nameof(GetEventById), new { id = newId }, new ApiResponse<EventDto>
            {
                Success = true,
                StatusCode = 201,
                Data = newEvent,
                Message = "Tạo thành công"
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEvent(int id, [FromBody] UpdateEventRequest request)
        {
            var evt = _events.FirstOrDefault(e => e.ID == id);
            if (evt == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Error = "NOT_FOUND",
                    Message = "Không tìm thấy sự kiện"
                });
            }

            if (request.Name != null) evt.Name = request.Name;
            if (request.Description != null) evt.Description = request.Description;
            if (request.Icon != null) evt.Icon = request.Icon;
            if (request.CategoryId.HasValue)
            {
                evt.Category = _categories.FirstOrDefault(c => c.ID == request.CategoryId.Value);
            }

            return Ok(new ApiResponse<EventDto>
            {
                Success = true,
                StatusCode = 200,
                Data = evt,
                Message = "Cập nhật thành công"
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEvent(int id)
        {
            var evt = _events.FirstOrDefault(e => e.ID == id);
            if (evt == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Error = "NOT_FOUND",
                    Message = "Không tìm thấy sự kiện"
                });
            }

            _events.Remove(evt);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                StatusCode = 200,
                Data = null,
                Message = "Xóa thành công"
            });
        }
    }
}
