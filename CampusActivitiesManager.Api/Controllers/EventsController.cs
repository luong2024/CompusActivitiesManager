using CampusActivitiesManager.Api.Models;
using CampusActivitiesManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Controllers
{
    [Route("api/v1/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IFirebaseEventService _eventService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IFirebaseEventService eventService, ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] int limit = 20, [FromQuery] int? afterId = null, [FromQuery] bool includePending = false)
        {
            try
            {
                var events = await _eventService.GetAllEventsAsync(limit, afterId, includePending);
                return Ok(new ApiResponse<List<EventDto>>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = events,
                    Message = "Lấy danh sách sự kiện từ Firebase thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting events from Firebase");
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
        public async Task<IActionResult> GetEventById(int id)
        {
            var evt = await _eventService.GetEventByIdAsync(id);
            if (evt == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = "Không tìm thấy sự kiện trên Firebase"
                });
            }

            return Ok(new ApiResponse<EventDto>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = evt,
                Message = "Thành công"
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newEvent = await _eventService.CreateEventAsync(request);

            return CreatedAtAction(nameof(GetEventById), new { id = newEvent.ID }, new ApiResponse<EventDto>
            {
                Success = true,
                StatusCode = StatusCodes.Status201Created,
                Data = newEvent,
                Message = "Tạo sự kiện trên Firebase thành công"
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventRequest request)
        {
            var updated = await _eventService.UpdateEventAsync(id, request);
            if (updated == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = "Không tìm thấy sự kiện trên Firebase"
                });
            }

            return Ok(new ApiResponse<EventDto>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = updated,
                Message = "Cập nhật sự kiện trên Firebase thành công"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var deleted = await _eventService.DeleteEventAsync(id);
            if (!deleted)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = "Không tìm thấy sự kiện trên Firebase"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = null,
                Message = "Xóa sự kiện trên Firebase thành công"
            });
        }

        [HttpPost("{id}/approve")]
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveEvent(int id, [FromBody] EventApprovalRequest? request)
        {
            string approvedBy = request?.ApprovedBy ?? "Admin";
            var updated = await _eventService.ApproveEventAsync(id, true, approvedBy, request?.Reason);
            if (updated == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = "Không tìm thấy sự kiện trên Firebase"
                });
            }

            return Ok(new ApiResponse<EventDto>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = updated,
                Message = "Duyệt sự kiện thành công"
            });
        }

        [HttpPost("{id}/reject")]
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectEvent(int id, [FromBody] EventApprovalRequest? request)
        {
            string approvedBy = request?.ApprovedBy ?? "Admin";
            var updated = await _eventService.ApproveEventAsync(id, false, approvedBy, request?.Reason);
            if (updated == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = "Không tìm thấy sự kiện trên Firebase"
                });
            }

            return Ok(new ApiResponse<EventDto>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = updated,
                Message = "Đã từ chối phê duyệt sự kiện"
            });
        }
    }
}
