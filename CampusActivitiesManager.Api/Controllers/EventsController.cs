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
            new EventCategoryDto { ID = 1, Title = "Hoc thuat", Color = "#2563EB" },
            new EventCategoryDto { ID = 2, Title = "The thao", Color = "#059669" },
            new EventCategoryDto { ID = 3, Title = "Tinh nguyen", Color = "#DC2626" },
            new EventCategoryDto { ID = 4, Title = "Ky nang", Color = "#7C3AED" }
        };

        private static List<EventDto> _events = new List<EventDto>
        {
            new EventDto { ID = 1, Name = "Hoi thao Cong nghe Thong tin 2026", Description = "Cap nhat xu huong AI", Icon = "IT", Category = _categories[0] },
            new EventDto { ID = 2, Name = "Giai Bong da Sinh vien", Description = "Khai mac giai dau", Icon = "S", Category = _categories[1] }
        };

        [HttpGet]
        public IActionResult GetEvents()
        {
            return Ok(new ApiResponse<List<EventDto>>
            {
                Success = true,
                StatusCode = 200,
                Data = _events,
                Message = "Lay danh sach su kien thanh cong"
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
                    Message = "Khong tim thay su kien"
                });
            }

            return Ok(new ApiResponse<EventDto>
            {
                Success = true,
                StatusCode = 200,
                Data = evt,
                Message = "Thanh cong"
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
                Message = "Tao thanh cong"
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
                    Message = "Khong tim thay su kien"
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
                Message = "Cap nhat thanh cong"
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
                    Message = "Khong tim thay su kien"
                });
            }

            _events.Remove(evt);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                StatusCode = 200,
                Data = null,
                Message = "Xoa thanh cong"
            });
        }
    }
}
