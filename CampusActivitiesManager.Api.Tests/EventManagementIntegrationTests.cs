using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CampusActivitiesManager.Api.Models;
using Xunit;

namespace CampusActivitiesManager.Api.Tests
{
    public class EventManagementIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public EventManagementIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task T38_1_GetEvents_ShouldReturnOk_AndIncludeCategoryInfo()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/events");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<EventDto>>>(_jsonOptions);
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);

            // AC2: Mỗi event phải có thông tin Category tương ứng
            foreach (var evt in result.Data)
            {
                Assert.True(evt.ID > 0);
                Assert.False(string.IsNullOrWhiteSpace(evt.Name));
                Assert.NotNull(evt.Category);
                Assert.False(string.IsNullOrWhiteSpace(evt.Category.Title));
            }
        }

        [Fact]
        public async Task T38_1_GetEventById_ExistingId_ShouldReturnEvent()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/events/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<EventDto>>(_jsonOptions);
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.ID);
            Assert.NotNull(result.Data.Category);
        }

        [Fact]
        public async Task T38_1_GetEventById_NonExistingId_ShouldReturn404NotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/events/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task US13_CreateEvent_ValidRequest_ShouldReturn201Created()
        {
            // Arrange
            var createRequest = new CreateEventRequest
            {
                Name = "Workshop Flutter & MAUI 2026",
                Description = "Trải nghiệm phát triển ứng dụng di động đa nền tảng hiện đại",
                Icon = "📱",
                CategoryId = 1,
                Location = "Phòng Lab 402",
                StartDate = "15/11/2026",
                EndDate = "15/11/2026",
                MaxParticipants = 80,
                Status = "Sắp diễn ra"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/events", createRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<EventDto>>(_jsonOptions);
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Workshop Flutter & MAUI 2026", result.Data.Name);
            Assert.Equal("Phòng Lab 402", result.Data.Location);
        }

        [Fact]
        public async Task US13_UpdateEvent_ValidRequest_ShouldReturn200Ok()
        {
            // Arrange
            var updateRequest = new UpdateEventRequest
            {
                Name = "Hội thảo AI & Công nghệ 2026 (Đã cập nhật)",
                Description = "Mở rộng quy mô tiếp đón thêm 100 sinh viên",
                Location = "Hội trường Lớn B1"
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/v1/events/1", updateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<EventDto>>(_jsonOptions);
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Hội thảo AI & Công nghệ 2026 (Đã cập nhật)", result.Data.Name);
            Assert.Equal("Hội trường Lớn B1", result.Data.Location);
        }

        [Fact]
        public async Task US13_DeleteEvent_ValidId_ShouldReturn200Ok()
        {
            // Arrange: create an event to delete
            var createRequest = new CreateEventRequest
            {
                Name = "Sự kiện chuẩn bị xóa",
                Description = "Sự kiện thử nghiệm",
                CategoryId = 2
            };
            var createResponse = await _client.PostAsJsonAsync("/api/v1/events", createRequest);
            var createdResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<EventDto>>(_jsonOptions);
            int idToDelete = createdResult!.Data!.ID;

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/v1/events/{idToDelete}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

            // Verify it no longer exists
            var getAgain = await _client.GetAsync($"/api/v1/events/{idToDelete}");
            Assert.Equal(HttpStatusCode.NotFound, getAgain.StatusCode);
        }

        [Fact]
        public async Task US33_GetCategories_ShouldReturnOkWithCategories()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/categories");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<EventCategoryDto>>>(_jsonOptions);
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Count >= 5);
        }

        [Fact]
        public async Task US28_ApproveAndRejectEvent_ShouldUpdateApprovalStatus()
        {
            // Create a pending event as a manager
            var newEventReq = new CreateEventRequest
            {
                Name = "Sự kiện cần duyệt",
                Description = "Mô tả sự kiện do Manager đăng",
                CreatedByRole = "Manager",
                CreatedByName = "Ban tổ chức CLB",
                IsApproved = false,
                ApprovalStatus = "Pending"
            };

            var createRes = await _client.PostAsJsonAsync("/api/v1/events", newEventReq);
            Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);
            var created = await createRes.Content.ReadFromJsonAsync<ApiResponse<EventDto>>(_jsonOptions);
            int eventId = created!.Data!.ID;

            // 1. Without includePending, it should NOT appear on public list
            var publicRes = await _client.GetAsync("/api/v1/events?includePending=false");
            var publicEvents = await publicRes.Content.ReadFromJsonAsync<ApiResponse<List<EventDto>>>(_jsonOptions);
            Assert.DoesNotContain(publicEvents!.Data!, e => e.ID == eventId);

            // 2. With includePending=true, it should appear on admin list
            var adminRes = await _client.GetAsync("/api/v1/events?includePending=true");
            var adminEvents = await adminRes.Content.ReadFromJsonAsync<ApiResponse<List<EventDto>>>(_jsonOptions);
            Assert.Contains(adminEvents!.Data!, e => e.ID == eventId);

            // 3. Approve event
            var approveRes = await _client.PostAsJsonAsync($"/api/v1/events/{eventId}/approve", new EventApprovalRequest
            {
                IsApproved = true,
                ApprovedBy = "Admin Tổng"
            });
            Assert.Equal(HttpStatusCode.OK, approveRes.StatusCode);
            var approved = await approveRes.Content.ReadFromJsonAsync<ApiResponse<EventDto>>(_jsonOptions);
            Assert.True(approved!.Data!.IsApproved);
            Assert.Equal("Approved", approved.Data.ApprovalStatus);

            // 4. Now it should appear on public list
            var publicAfterApprove = await _client.GetAsync("/api/v1/events?includePending=false");
            var publicAfter = await publicAfterApprove.Content.ReadFromJsonAsync<ApiResponse<List<EventDto>>>(_jsonOptions);
            Assert.Contains(publicAfter!.Data!, e => e.ID == eventId);

            // 5. Reject event
            var rejectRes = await _client.PostAsJsonAsync($"/api/v1/events/{eventId}/reject", new EventApprovalRequest
            {
                IsApproved = false,
                Reason = "Thông tin chưa đầy đủ"
            });
            Assert.Equal(HttpStatusCode.OK, rejectRes.StatusCode);
            var rejected = await rejectRes.Content.ReadFromJsonAsync<ApiResponse<EventDto>>(_jsonOptions);
            Assert.False(rejected!.Data!.IsApproved);
            Assert.Equal("Rejected", rejected.Data.ApprovalStatus);
        }
    }
}
