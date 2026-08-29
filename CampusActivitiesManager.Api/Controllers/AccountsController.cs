using CampusActivitiesManager.Api.Models;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AccountsController : ControllerBase
    {
        private readonly FirebaseAuth _firebaseAuth;
        private readonly FirestoreDb? _firestoreDb;

        public AccountsController()
        {
            _firebaseAuth = FirebaseAuth.DefaultInstance;
            
            try 
            {
                // In a real app, inject this or get the project ID dynamically
                // Currently defaulting to a dummy project ID if environment variable is missing
                string projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT") ?? "campusacmanage";
                _firestoreDb = FirestoreDb.Create(projectId);
            }
            catch
            {
                _firestoreDb = null; // Proceeding without Firestore if not configured properly yet
            }
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
                var userArgs = new UserRecordArgs
                {
                    Email = request.Email,
                    Password = request.Password,
                    DisplayName = request.FullName,
                    PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber
                };

                UserRecord userRecord = await _firebaseAuth.CreateUserAsync(userArgs);

                // Store extended attributes in Firestore
                if (_firestoreDb != null)
                {
                    DocumentReference docRef = _firestoreDb.Collection("users").Document(userRecord.Uid);
                    await docRef.SetAsync(new
                    {
                        email = request.Email,
                        fullName = request.FullName,
                        role = request.Role,
                        phoneNumber = request.PhoneNumber,
                        studentCode = request.StudentCode,
                        createdAt = DateTime.UtcNow
                    });
                }

                var responseData = new
                {
                    id = userRecord.Uid,
                    email = request.Email,
                    fullName = request.FullName,
                    role = request.Role,
                    createdAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
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
                UserRecord? existingUser = null;
                try
                {
                    existingUser = await _firebaseAuth.GetUserAsync(id);
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

                var userArgs = new UserRecordArgs
                {
                    Uid = id
                };

                if (!string.IsNullOrEmpty(request.FullName))
                {
                    userArgs.DisplayName = request.FullName;
                }

                if (!string.IsNullOrEmpty(request.PhoneNumber))
                {
                    userArgs.PhoneNumber = request.PhoneNumber;
                }

                UserRecord updatedUser = await _firebaseAuth.UpdateUserAsync(userArgs);

                // Update Firestore
                if (_firestoreDb != null)
                {
                    DocumentReference docRef = _firestoreDb.Collection("users").Document(id);
                    var updates = new Dictionary<string, object>();
                    
                    if (!string.IsNullOrEmpty(request.FullName)) updates["fullName"] = request.FullName;
                    if (!string.IsNullOrEmpty(request.PhoneNumber)) updates["phoneNumber"] = request.PhoneNumber;
                    if (!string.IsNullOrEmpty(request.AvatarUrl)) updates["avatarUrl"] = request.AvatarUrl;
                    if (!string.IsNullOrEmpty(request.Role)) updates["role"] = request.Role;
                    
                    updates["updatedAt"] = DateTime.UtcNow;

                    await docRef.SetAsync(updates, SetOptions.MergeAll);
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Account updated successfully",
                    Data = new
                    {
                        id = updatedUser.Uid,
                        email = updatedUser.Email,
                        fullName = request.FullName ?? existingUser.DisplayName,
                        role = request.Role ?? "Student", // In real app, fetch existing role from Firestore
                        updatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    }
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
