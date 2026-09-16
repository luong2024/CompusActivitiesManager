using CampusActivitiesManager.Api.Models;
using CampusActivitiesManager.Api.Services;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IFirebaseAccountService _accountService;

        public AccountsController(IFirebaseAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return CreateValidationErrorResponse();
            }

            try
            {
                var user = await _accountService.CreateAccountAsync(request);

                var responseData = new
                {
                    id = user.Id,
                    email = user.Email,
                    fullName = user.FullName,
                    role = user.Role,
                    createdAt = user.CreatedAt
                };

                return StatusCode(201, new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "Account created successfully",
                    Data = responseData
                });
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
            {
                return StatusCode(409, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 409,
                    Error = "CONFLICT",
                    Message = "Email is already registered"
                });
            }
            catch (FirebaseAuthException ex)
            {
                return StatusCode(400, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 400,
                    Error = "BAD_REQUEST",
                    Message = ex.Message
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

        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            try
            {
                var users = await _accountService.GetAllAccountsAsync();
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Accounts retrieved successfully",
                    Data = users
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

        [HttpGet("{id}")]
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
                        StatusCode = 404,
                        Error = "NOT_FOUND",
                        Message = $"Account with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Account retrieved successfully",
                    Data = user
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

        [HttpPut("{id}")]
        [HttpPatch("{id}")]
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
                        StatusCode = 404,
                        Error = "NOT_FOUND",
                        Message = $"Account with ID {id} not found"
                    });
                }

                var updatedUser = await _accountService.UpdateAccountAsync(id, request);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = 200,
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
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Error = "NOT_FOUND",
                    Message = $"Account with ID {id} not found"
                });
            }
            catch (FirebaseAuthException ex)
            {
                return StatusCode(400, new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 400,
                    Error = "BAD_REQUEST",
                    Message = ex.Message
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

        /// <summary>
        /// AC 35.1.1 & AC 35.1.3 & AC 35.1.4: Lock account endpoint
        /// </summary>
        [HttpPost("{id}/lock")]
        public async Task<IActionResult> LockAccount(string id)
        {
            try
            {
                var existingUser = await _accountService.GetAccountByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Error = "NOT_FOUND",
                        Message = $"Account with ID {id} not found"
                    });
                }

                var response = await _accountService.SetAccountLockStatusAsync(id, true);

                return Ok(new ApiResponse<AccountStatusResponse>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Account locked successfully",
                    Data = response
                });
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Error = "NOT_FOUND",
                    Message = $"Account with ID {id} not found"
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

        /// <summary>
        /// AC 35.1.2 & AC 35.1.3 & AC 35.1.4: Unlock account endpoint
        /// </summary>
        [HttpPost("{id}/unlock")]
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
                        StatusCode = 404,
                        Error = "NOT_FOUND",
                        Message = $"Account with ID {id} not found"
                    });
                }

                var response = await _accountService.SetAccountLockStatusAsync(id, false);

                return Ok(new ApiResponse<AccountStatusResponse>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Account unlocked successfully",
                    Data = response
                });
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Error = "NOT_FOUND",
                    Message = $"Account with ID {id} not found"
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

        /// <summary>
        /// Toggle status endpoint for backward compatibility
        /// </summary>
        [HttpPost("{id}/toggle-status")]
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
                        StatusCode = 404,
                        Error = "NOT_FOUND",
                        Message = $"Account with ID {id} not found"
                    });
                }

                bool targetLockState = existingUser.IsActive; // if currently active, lock it; if locked, unlock it
                var response = await _accountService.SetAccountLockStatusAsync(id, targetLockState);

                return Ok(new ApiResponse<AccountStatusResponse>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = response.Message,
                    Data = response
                });
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Error = "NOT_FOUND",
                    Message = $"Account with ID {id} not found"
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
                    Field = char.ToLowerInvariant(ms.Key[0]) + ms.Key.Substring(1), // camelCase
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
