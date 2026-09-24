using CampusActivitiesManager.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private static List<EventCategoryDto> _categories = new List<EventCategoryDto>
        {
            new EventCategoryDto { ID = 1, Title = "Hoc thuat", Color = "#2563EB" },
            new EventCategoryDto { ID = 2, Title = "The thao", Color = "#059669" },
            new EventCategoryDto { ID = 3, Title = "Tinh nguyen", Color = "#DC2626" },
            new EventCategoryDto { ID = 4, Title = "Ky nang", Color = "#7C3AED" }
        };

        [HttpGet]
        public IActionResult GetCategories()
        {
            return Ok(new ApiResponse<List<EventCategoryDto>>
            {
                Success = true,
                StatusCode = 200,
                Data = _categories,
                Message = "Lay danh sach danh muc thanh cong"
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var cat = _categories.FirstOrDefault(c => c.ID == id);
            if (cat == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Error = "NOT_FOUND",
                    Message = "Khong tim thay danh muc"
                });
            }

            return Ok(new ApiResponse<EventCategoryDto>
            {
                Success = true,
                StatusCode = 200,
                Data = cat,
                Message = "Thanh cong"
            });
        }

        [HttpPost]
        public IActionResult CreateCategory([FromBody] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newId = _categories.Any() ? _categories.Max(c => c.ID) + 1 : 1;
            var newCat = new EventCategoryDto
            {
                ID = newId,
                Title = request.Title,
                Color = request.Color
            };

            _categories.Add(newCat);

            return CreatedAtAction(nameof(GetCategoryById), new { id = newId }, new ApiResponse<EventCategoryDto>
            {
                Success = true,
                StatusCode = 201,
                Data = newCat,
                Message = "Tao danh muc thanh cong"
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
        {
            var cat = _categories.FirstOrDefault(c => c.ID == id);
            if (cat == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Error = "NOT_FOUND",
                    Message = "Khong tim thay danh muc"
                });
            }

            if (request.Title != null) cat.Title = request.Title;
            if (request.Color != null) cat.Color = request.Color;

            return Ok(new ApiResponse<EventCategoryDto>
            {
                Success = true,
                StatusCode = 200,
                Data = cat,
                Message = "Cap nhat danh muc thanh cong"
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var cat = _categories.FirstOrDefault(c => c.ID == id);
            if (cat == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Error = "NOT_FOUND",
                    Message = "Khong tim thay danh muc"
                });
            }

            _categories.Remove(cat);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                StatusCode = 200,
                Data = null,
                Message = "Xoa danh muc thanh cong"
            });
        }
    }
}
