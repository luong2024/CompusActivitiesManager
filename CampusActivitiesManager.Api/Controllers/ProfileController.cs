using CampusActivitiesManager.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Models
{
    public class UpdateProfileRequest
    {
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
        // Mock data for current user profile
        private static UserAccountDto _currentProfile = new UserAccountDto
        {
            Id = "mock-user-id",
            Email = "student@campus.edu",
            FullName = "Nguyen Duc Manh",
            Role = "Student",
            PhoneNumber = "0912345678",
            AvatarUrl = "https://example.com/avatar.png"
        };

        [HttpGet]
        public IActionResult GetProfile()
        {
            return Ok(new ApiResponse<UserAccountDto>
            {
                Success = true,
                StatusCode = 200,
                Data = _currentProfile,
                Message = "Lay thong tin ca nhan thanh cong"
            });
        }

        [HttpPut]
        public IActionResult UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            if (request.FullName != null) _currentProfile.FullName = request.FullName;
            if (request.PhoneNumber != null) _currentProfile.PhoneNumber = request.PhoneNumber;
            if (request.AvatarUrl != null) _currentProfile.AvatarUrl = request.AvatarUrl;

            return Ok(new ApiResponse<UserAccountDto>
            {
                Success = true,
                StatusCode = 200,
                Data = _currentProfile,
                Message = "Cap nhat thong tin ca nhan thanh cong"
            });
        }
    }
}
