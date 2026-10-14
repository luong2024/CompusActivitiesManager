using CampusActivitiesManager.Api.Models;
using CampusActivitiesManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IFirebaseEventService _eventService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(IFirebaseEventService eventService, ILogger<CategoriesController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _eventService.GetAllCategoriesAsync();
                return Ok(new ApiResponse<List<EventCategoryDto>>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = categories,
                    Message = "Lấy danh sách danh mục từ Firebase thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories from Firebase");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var cat = await _eventService.GetCategoryByIdAsync(id);
            if (cat == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = "Không tìm thấy danh mục trên Firebase"
                });
            }

            return Ok(new ApiResponse<EventCategoryDto>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = cat,
                Message = "Thành công"
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newCat = await _eventService.CreateCategoryAsync(request);

            return CreatedAtAction(nameof(GetCategoryById), new { id = newCat.ID }, new ApiResponse<EventCategoryDto>
            {
                Success = true,
                StatusCode = StatusCodes.Status201Created,
                Data = newCat,
                Message = "Tạo danh mục trên Firebase thành công"
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
        {
            var updated = await _eventService.UpdateCategoryAsync(id, request);
            if (updated == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = "Không tìm thấy danh mục trên Firebase"
                });
            }

            return Ok(new ApiResponse<EventCategoryDto>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = updated,
                Message = "Cập nhật danh mục trên Firebase thành công"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleted = await _eventService.DeleteCategoryAsync(id);
            if (!deleted)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = "Không tìm thấy danh mục trên Firebase"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = null,
                Message = "Xóa danh mục trên Firebase thành công"
            });
        }
    }
}
