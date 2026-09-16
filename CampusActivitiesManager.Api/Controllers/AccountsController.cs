using CampusActivitiesManager.Api.Models;
using CampusActivitiesManager.Api.Services;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusActivitiesManager.Api.Controllers
{
    /// <summary>
    /// API Quản lý Tài khoản người dùng: Tạo mới, Cập nhật, Khóa/Mở khóa tài khoản (US35 - T35.1)
    /// </summary>
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IFirebaseAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IFirebaseAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        /// <summary>
        /// AC1: API tạo account thành công khi dữ liệu đầu vào hợp lệ.
        /// </summary>
        /// <param name="request">Thông tin tài khoản mới</param>
        /// <returns>HTTP 201 Created và dữ liệu tài khoản</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return CreateValidationErrorResponse();
            }

            try
            {
                _logger.LogInformation("Creating account for email: {Email}", request.Email);
                var user = await _accountService.CreateAccountAsync(request);

                var responseData = new
                {
                    id = user.Id,
                    email = user.Email,
                    fullName = user.FullName,
                    role = user.Role,
                    createdAt = user.CreatedAt
                };

                return StatusCode(StatusCodes.Status201Created, new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Account created successfully",
                    Data = responseData
                });
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
            {
                _logger.LogWarning("Create account conflict: Email already registered ({Email})", request.Email);
                return StatusCode(StatusCodes.Status409Conflict, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Error = "CONFLICT",
                    Message = "Email is already registered"
                });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already registered", StringComparison.OrdinalIgnoreCase) || ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Create account conflict: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status409Conflict, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Error = "CONFLICT",
                    Message = "Email is already registered"
                });
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogError(ex, "FirebaseAuthException creating account for {Email}", request.Email);
                return StatusCode(StatusCodes.Status400BadRequest, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = "BAD_REQUEST",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception creating account for {Email}", request.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy toàn bộ danh sách tài khoản
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAccounts()
        {
            try
            {
                var users = await _accountService.GetAllAccountsAsync();
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Accounts retrieved successfully",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving accounts");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy chi tiết thông tin một tài khoản qua ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAccountById(string id)
        {
            try
            {
                var user = await _accountService.GetAccountByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = "NOT_FOUND",
                        Message = $"Account with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Account retrieved successfully",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving account {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// AC3: Cập nhật thông tin account đang tồn tại.
        /// </summary>
        [HttpPut("{id}")]
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAccount(string id, [FromBody] UpdateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return CreateValidationErrorResponse();
            }

            try
            {
                var existingUser = await _accountService.GetAccountByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = "NOT_FOUND",
                        Message = $"Account with ID {id} not found"
                    });
                }

                var updatedUser = await _accountService.UpdateAccountAsync(id, request);
                _logger.LogInformation("Account {Id} updated successfully", id);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Account updated successfully",
                    Data = new
                    {
                        id = updatedUser.Id,
                        email = updatedUser.Email,
                        fullName = updatedUser.FullName,
                        role = updatedUser.Role,
                        updatedAt = updatedUser.UpdatedAt
                    }
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = $"Account with ID {id} not found"
                });
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = $"Account with ID {id} not found"
                });
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogError(ex, "FirebaseAuthException updating account {Id}", id);
                return StatusCode(StatusCodes.Status400BadRequest, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = "BAD_REQUEST",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception updating account {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// AC 35.1.1 & AC 35.1.3 & AC 35.1.4: API lock account thành công với account tồn tại.
        /// Bảo mật: Admin không thể tự khóa tài khoản của chính mình.
        /// </summary>
        [HttpPost("{id}/lock")]
        [ProducesResponseType(typeof(ApiResponse<AccountStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LockAccount(string id)
        {
            try
            {
                // Kiểm tra ràng buộc bảo mật: Không cho phép tự khóa tài khoản của chính mình
                string? currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(currentUserId) && currentUserId.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Admin self-lock attempt prevented for UID {Id}", id);
                    return BadRequest(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Error = "OPERATION_NOT_ALLOWED",
                        Message = "Admin cannot lock their own account"
                    });
                }

                var existingUser = await _accountService.GetAccountByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = "NOT_FOUND",
                        Message = $"Account with ID {id} not found"
                    });
                }

                var response = await _accountService.SetAccountLockStatusAsync(id, true);
                _logger.LogInformation("Account {Id} locked successfully", id);

                return Ok(new ApiResponse<AccountStatusResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Account locked successfully",
                    Data = response
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = $"Account with ID {id} not found"
                });
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = $"Account with ID {id} not found"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking account {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// AC 35.1.2 & AC 35.1.3 & AC 35.1.4: API unlock account thành công với account đang bị khóa.
        /// </summary>
        [HttpPost("{id}/unlock")]
        [ProducesResponseType(typeof(ApiResponse<AccountStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnlockAccount(string id)
        {
            try
            {
                var existingUser = await _accountService.GetAccountByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = "NOT_FOUND",
                        Message = $"Account with ID {id} not found"
                    });
                }

                var response = await _accountService.SetAccountLockStatusAsync(id, false);
                _logger.LogInformation("Account {Id} unlocked successfully", id);

                return Ok(new ApiResponse<AccountStatusResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Account unlocked successfully",
                    Data = response
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = $"Account with ID {id} not found"
                });
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = $"Account with ID {id} not found"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking account {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "INTERNAL_SERVER_ERROR",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Đổi trạng thái khóa/mở khóa linh hoạt (Toggle status)
        /// </summary>
        [HttpPost("{id}/toggle-status")]
        [ProducesResponseType(typeof(ApiResponse<AccountStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleAccountStatus(string id)
        {
            try
            {
                var existingUser = await _accountService.GetAccountByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = "NOT_FOUND",
                        Message = $"Account with ID {id} not found"
                    });
                }

                bool targetLockState = existingUser.IsActive; // Nếu đang active thì chuyển sang lock, nếu đang locked thì chuyển sang unlock
                var response = await _accountService.SetAccountLockStatusAsync(id, targetLockState);
                _logger.LogInformation("Account {Id} status toggled: isLocked={IsLocked}", id, targetLockState);

                return Ok(new ApiResponse<AccountStatusResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = response.Message,
                    Data = response
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = $"Account with ID {id} not found"
                });
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "NOT_FOUND",
                    Message = $"Account with ID {id} not found"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling account status {Id}", id);
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
