using System.Security.Claims;
using CampusActivitiesManager.Api.Models;
using CampusActivitiesManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Models
{
    public class UpdateProfileRequest
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
    }
}

namespace CampusActivitiesManager.Api.Controllers
{
    [Route("api/v1/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IFirebaseAccountService _accountService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IFirebaseAccountService accountService, ILogger<ProfileController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        private async Task<UserAccountDto?> ResolveTargetUserAsync(string? id = null, string? email = null)
        {
            id ??= User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? Request.Headers["X-User-Id"].FirstOrDefault();
            email ??= User.FindFirst(ClaimTypes.Email)?.Value
                ?? Request.Headers["X-User-Email"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(id))
            {
                var byId = await _accountService.GetAccountByIdAsync(id);
                if (byId != null) return byId;
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var byEmail = await _accountService.GetAccountByEmailAsync(email);
                if (byEmail != null) return byEmail;
            }

            var allUsers = await _accountService.GetAllAccountsAsync();
            return allUsers.FirstOrDefault();
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile([FromQuery] string? id = null, [FromQuery] string? email = null)
        {
            try
            {
                var profile = await ResolveTargetUserAsync(id, email);
                if (profile == null)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = "NOT_FOUND",
                        Message = "Không tìm thấy hồ sơ người dùng trên Firebase"
                    });
                }

                return Ok(new ApiResponse<UserAccountDto>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = profile,
                    Message = "Lấy thông tin cá nhân từ Firebase thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile from Firebase");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, [FromQuery] string? id = null, [FromQuery] string? email = null)
        {
            try
            {
                var targetUser = await ResolveTargetUserAsync(request.Id ?? id, request.Email ?? email);
                if (targetUser == null)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = "NOT_FOUND",
                        Message = "Không tìm thấy hồ sơ người dùng trên Firebase"
                    });
                }

                var updatedProfile = await _accountService.UpdateAccountAsync(targetUser.Id, new UpdateAccountRequest
                {
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    AvatarUrl = request.AvatarUrl,
                    Role = targetUser.Role
                });

                return Ok(new ApiResponse<UserAccountDto>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = updatedProfile,
                    Message = "Cập nhật thông tin cá nhân lên Firebase thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile on Firebase");
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
