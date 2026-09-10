using CampusActivitiesManager.Api.Models;
using CampusActivitiesManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventService eventService, ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        /// <summary>
        /// GET /api/v1/events or /api/events
        /// AC1: Lấy danh sách Event thành công.
        /// AC2: Mỗi Event trong response có thông tin Category tương ứng.
        /// AC3: Event không có Category vẫn được xử lý đúng theo business rule.
        /// AC4: Trả về danh sách rỗng nếu không có Event phù hợp.
        /// AC5: HTTP status 200 và response format đúng quy định (ApiResponse).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetEvents(
            [FromQuery] int? categoryId = null,
            [FromQuery] bool? hasCategory = null,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            try
            {
                // Validate Query Parameters (AC5)
                if (categoryId.HasValue && categoryId.Value < 0)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = 400,
                        Error = "BAD_REQUEST",
                        Message = "CategoryId must be a positive integer or null.",
                        Errors = new List<ApiErrorDetail>
                        {
                            new() { Field = "categoryId", Message = "CategoryId cannot be negative." }
                        }
                    });
                }

                var events = await _eventService.GetEventsAsync(categoryId, hasCategory, search, status);

                // AC1, AC4, AC5: Trả về HTTP 200 OK kèm envelope ApiResponse
                return Ok(new ApiResponse<IEnumerable<EventDto>>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = events.Any() ? "Events retrieved successfully" : "No events matched the criteria",
                    Data = events
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving events list");
                return StatusCode(500, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 500,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// GET /api/v1/events/{id} or /api/events/{id}
        /// Lấy chi tiết một sự kiện theo ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = 400,
                        Error = "BAD_REQUEST",
                        Message = "Event ID must be a positive integer.",
                        Errors = new List<ApiErrorDetail>
                        {
                            new() { Field = "id", Message = "ID must be greater than 0." }
                        }
                    });
                }

                var evt = await _eventService.GetEventByIdAsync(id);
                if (evt == null)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Error = "NOT_FOUND",
                        Message = $"Event with ID {id} not found."
                    });
                }

                return Ok(new ApiResponse<EventDto>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Event retrieved successfully",
                    Data = evt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving event with ID {Id}", id);
                return StatusCode(500, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 500,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// GET /api/v1/events/categories or /api/events/categories
        /// Lấy danh sách các danh mục sự kiện hiện có
        /// </summary>
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _eventService.GetCategoriesAsync();
                return Ok(new ApiResponse<IEnumerable<CategoryDto>>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Categories retrieved successfully",
                    Data = categories
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving event categories");
                return StatusCode(500, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 500,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }
    }
}
