using CampusActivitiesManager.Api.Models;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CampusActivitiesManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly FirebaseAuth _firebaseAuth;

        public AccountsController()
        {
            // Initialize FirebaseAuth if FirebaseApp is already configured
            _firebaseAuth = FirebaseAuth.DefaultInstance;
        }

        // POST api/accounts
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Validation failed", Errors = ModelState });
            }

            try
            {
                var userArgs = new UserRecordArgs
                {
                    Email = request.Email,
                    Password = request.Password,
                    DisplayName = request.DisplayName
                };

                UserRecord userRecord = await _firebaseAuth.CreateUserAsync(userArgs);

                return CreatedAtAction(nameof(GetAccount), new { uid = userRecord.Uid }, 
                    new { Message = "Account created successfully", Uid = userRecord.Uid });
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(new { Message = "Failed to create account in Firebase", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred", Details = ex.Message });
            }
        }

        // PUT api/accounts/{uid}
        [HttpPut("{uid}")]
        public async Task<IActionResult> UpdateAccount(string uid, [FromBody] UpdateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Validation failed", Errors = ModelState });
            }

            try
            {
                // First verify if user exists
                try
                {
                    await _firebaseAuth.GetUserAsync(uid);
                }
                catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
                {
                    return NotFound(new { Message = "Account not found" });
                }

                var userArgs = new UserRecordArgs
                {
                    Uid = uid
                };

                if (!string.IsNullOrEmpty(request.DisplayName))
                {
                    userArgs.DisplayName = request.DisplayName;
                }

                if (!string.IsNullOrEmpty(request.Password))
                {
                    userArgs.Password = request.Password;
                }

                if (!string.IsNullOrEmpty(request.PhoneNumber))
                {
                    userArgs.PhoneNumber = request.PhoneNumber;
                }

                UserRecord updatedUser = await _firebaseAuth.UpdateUserAsync(userArgs);

                return Ok(new { Message = "Account updated successfully", Uid = updatedUser.Uid });
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(new { Message = "Failed to update account in Firebase", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred", Details = ex.Message });
            }
        }

        // GET api/accounts/{uid}
        // Helper endpoint for CreatedAtAction
        [HttpGet("{uid}")]
        public async Task<IActionResult> GetAccount(string uid)
        {
            try
            {
                UserRecord userRecord = await _firebaseAuth.GetUserAsync(uid);
                return Ok(new
                {
                    Uid = userRecord.Uid,
                    Email = userRecord.Email,
                    DisplayName = userRecord.DisplayName
                });
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                return NotFound(new { Message = "Account not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred", Details = ex.Message });
            }
        }
    }
}
