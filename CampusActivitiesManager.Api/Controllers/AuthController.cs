using CampusActivitiesManager.Api.Models;
using CampusActivitiesManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IFirebaseAccountService _accountService;

        public AuthController(IFirebaseAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// AC 35.1.5: Account bị lock không thể đăng nhập (trả về 403 Forbidden).
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return CreateValidationErrorResponse();
            }

            try
            {
                var user = await _accountService.GetAccountByEmailAsync(request.Email);
                if (user == null)
                {
                    return Unauthorized(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = 401,
                        Error = "UNAUTHORIZED",
                        Message = "Invalid email or password"
                    });
                }

                // AC 35.1.5: Kiểm tra nếu tài khoản đang bị khóa
                if (user.IsDisabled || !user.IsActive)
                {
                    return StatusCode(403, new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = 403,
                        Error = "ACCOUNT_LOCKED",
                        Message = "Account is locked. Please contact administrator."
                    });
                }

                var loginResult = await _accountService.AuthenticateAsync(request.Email, request.Password);
                if (loginResult == null)
                {
                    return Unauthorized(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = 401,
                        Error = "UNAUTHORIZED",
                        Message = "Invalid email or password"
                    });
                }

                return Ok(new ApiResponse<LoginResponse>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Login successful",
                    Data = loginResult
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 500,
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
                StatusCode = 400,
                Error = "BAD_REQUEST",
                Message = "Validation failed",
                Errors = errors
            };

            return BadRequest(response);
        }
    }
}
