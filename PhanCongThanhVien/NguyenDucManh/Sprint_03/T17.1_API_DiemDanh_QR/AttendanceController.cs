using System.Text;
using CampusActivitiesManager.Api.Models;
using CampusActivitiesManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Controllers
{
    [Route("api/v1/attendance")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IFirebaseAttendanceService _attendanceService;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(IFirebaseAttendanceService attendanceService, ILogger<AttendanceController> logger)
        {
            _attendanceService = attendanceService;
            _logger = logger;
        }

        [HttpPost("qr")]
        public async Task<IActionResult> CheckInWithQr([FromBody] QrCheckInRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.QrTicketCode) || string.IsNullOrWhiteSpace(request.StudentCode))
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = "INVALID_REQUEST",
                    Message = "Dữ liệu QR Code và mã sinh viên không được để trống"
                });
            }

            try
            {
                var record = await _attendanceService.CheckInWithQrAsync(request);
                return Ok(new ApiResponse<AttendanceRecord>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = record,
                    Message = "Điểm danh qua mã QR thành công"
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Error = "ALREADY_CHECKED_IN",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking in with QR");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        [HttpPost("photo")]
        public async Task<IActionResult> CheckInWithPhoto([FromBody] PhotoCheckInRequest request)
        {
            if (request.EventId <= 0 || string.IsNullOrWhiteSpace(request.StudentCode) || (string.IsNullOrWhiteSpace(request.PhotoBase64) && string.IsNullOrWhiteSpace(request.PhotoUrl)))
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = "INVALID_REQUEST",
                    Message = "Dữ liệu ảnh điểm danh, ID sự kiện và mã sinh viên không được để trống"
                });
            }

            try
            {
                var record = await _attendanceService.CheckInWithPhotoAsync(request);
                return Ok(new ApiResponse<AttendanceRecord>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = record,
                    Message = "Điểm danh bằng hình ảnh thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking in with photo");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        [HttpPost("manual")]
        public async Task<IActionResult> MarkManualAttendance([FromBody] ManualAttendanceRequest request)
        {
            if (request.EventId <= 0 || string.IsNullOrWhiteSpace(request.StudentCode))
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = "INVALID_REQUEST",
                    Message = "EventId và StudentCode là bắt buộc"
                });
            }

            try
            {
                var record = await _attendanceService.MarkManualAttendanceAsync(request);
                return Ok(new ApiResponse<AttendanceRecord>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = record,
                    Message = "Cập nhật điểm danh thủ công thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking manual attendance");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetEventAttendance(int eventId)
        {
            try
            {
                var records = await _attendanceService.GetEventAttendanceAsync(eventId);
                return Ok(new ApiResponse<List<AttendanceRecord>>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = records,
                    Message = $"Lấy danh sách điểm danh sự kiện {eventId} thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting event attendance");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        [HttpGet("student/{studentCode}")]
        public async Task<IActionResult> GetStudentAttendance(string studentCode)
        {
            try
            {
                var records = await _attendanceService.GetStudentAttendanceHistoryAsync(studentCode);
                return Ok(new ApiResponse<List<AttendanceRecord>>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = records,
                    Message = $"Lấy lịch sử tham gia của sinh viên {studentCode} thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student attendance");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        [HttpGet("event/{eventId}/export-csv")]
        public async Task<IActionResult> ExportAttendanceCsv(int eventId)
        {
            try
            {
                var csv = await _attendanceService.ExportAttendanceCsvAsync(eventId);
                // Prepend UTF-8 BOM so Excel opens Vietnamese characters correctly
                var preamble = Encoding.UTF8.GetPreamble();
                var csvBytes = Encoding.UTF8.GetBytes(csv);
                var fullBytes = new byte[preamble.Length + csvBytes.Length];
                Buffer.BlockCopy(preamble, 0, fullBytes, 0, preamble.Length);
                Buffer.BlockCopy(csvBytes, 0, fullBytes, preamble.Length, csvBytes.Length);

                return File(fullBytes, "text/csv; charset=utf-8", $"DanhSach_DiemDanh_SuKien_{eventId}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting CSV");
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
