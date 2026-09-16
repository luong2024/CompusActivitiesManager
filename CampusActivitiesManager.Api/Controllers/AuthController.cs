using CampusActivitiesManager.Api.Models;
using CampusActivitiesManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Controllers
{
    /// <summary>
    /// API Xác thực người dùng và Quản lý Đăng nhập (US37, AC 35.1.5)
    /// </summary>
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IFirebaseAccountService _accountService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IFirebaseAccountService accountService, ILogger<AuthController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        /// <summary>
        /// AC 35.1.5: Đăng nhập vào hệ thống. Account bị lock không thể đăng nhập (trả về HTTP 403 Forbidden).
        /// </summary>
        /// <param name="request">Email và Mật khẩu đăng nhập</param>
        /// <returns>JWT Token và thông tin tài khoản nếu thành công</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return CreateValidationErrorResponse();
            }

            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", request.Email);

                var user = await _accountService.GetAccountByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found for email {Email}", request.Email);
                    return Unauthorized(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Error = "UNAUTHORIZED",
                        Message = "Invalid email or password"
                    });
                }

                // AC 35.1.5: Kiểm tra nếu tài khoản đang bị khóa
                if (user.IsDisabled || !user.IsActive)
                {
                    _logger.LogWarning("Login rejected: Account is locked for email {Email} (UID: {Id})", request.Email, user.Id);
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status403Forbidden,
                        Error = "ACCOUNT_LOCKED",
                        Message = "Account is locked. Please contact administrator."
                    });
                }

                var loginResult = await _accountService.AuthenticateAsync(request.Email, request.Password);
                if (loginResult == null)
                {
                    _logger.LogWarning("Login failed: Invalid credentials for email {Email}", request.Email);
                    return Unauthorized(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Error = "UNAUTHORIZED",
                        Message = "Invalid email or password"
                    });
                }

                _logger.LogInformation("User {Email} logged in successfully", request.Email);
                return Ok(new ApiResponse<LoginResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Login successful",
                    Data = loginResult
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception during login for {Email}", request.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        private IActionResult CreateValidationErrorResponse()
        {
            var errors = ModelState
                .Where(ms => ms.Value!.Errors.Count > 0)
                .Select(ms => new ApiErrorDetail
                {
                    Field = char.ToLowerInvariant(ms.Key[0]) + ms.Key.Substring(1),
                    Message = ms.Value!.Errors.First().ErrorMessage
                })
                .ToList();

            var response = new ApiErrorResponse
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Error = "BAD_REQUEST",
                Message = "Validation failed",
                Errors = errors
            };

            return BadRequest(response);
        }
    }
}
