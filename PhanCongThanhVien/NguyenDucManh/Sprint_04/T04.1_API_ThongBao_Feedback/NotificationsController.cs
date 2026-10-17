using CampusActivitiesManager.Api.Models;
using CampusActivitiesManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Controllers
{
    [Route("api/v1/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IFirebaseAttendanceService _attendanceService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(IFirebaseAttendanceService attendanceService, ILogger<NotificationsController> logger)
        {
            _attendanceService = attendanceService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] string? studentCode = null)
        {
            try
            {
                var notifications = await _attendanceService.GetNotificationsAsync(studentCode);
                return Ok(new ApiResponse<List<CampusNotificationDto>>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = notifications,
                    Message = "Lấy danh sách thông báo thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campus notifications");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }
    }
}
