using CampusActivitiesManager.Api.Controllers;
using CampusActivitiesManager.Api.Models;
using CampusActivitiesManager.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CampusActivitiesManager.Api.Tests
{
    public class EventsControllerTests
    {
        private readonly Mock<IEventService> _mockEventService;
        private readonly NullLogger<EventsController> _logger;
        private readonly EventsController _controller;

        public EventsControllerTests()
        {
            _mockEventService = new Mock<IEventService>();
            _logger = new NullLogger<EventsController>();
            _controller = new EventsController(_mockEventService.Object, _logger);
        }

        #region AC1: API trả về danh sách Event thành công

        [Fact]
        public async Task GetEvents_ReturnsOkResult_WithListOfEvents()
        {
            // Arrange (AC1)
            var sampleEvents = GetSampleEvents();
            _mockEventService
                .Setup(s => s.GetEventsAsync(null, null, null, null))
                .ReturnsAsync(sampleEvents);

            // Act
            var result = await _controller.GetEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<EventDto>>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(200, apiResponse.StatusCode);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(sampleEvents.Count, apiResponse.Data.Count());
        }

        #endregion

        #region AC2: Mỗi Event trong response có thông tin Category tương ứng

        [Fact]
        public async Task GetEvents_EventsWithCategory_IncludesCorrespondingCategoryInformation()
        {
            // Arrange (AC2)
            var sampleEvents = GetSampleEvents();
            _mockEventService
                .Setup(s => s.GetEventsAsync(null, null, null, null))
                .ReturnsAsync(sampleEvents);

            // Act
            var result = await _controller.GetEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<EventDto>>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);

            var eventsWithCategory = apiResponse.Data.Where(e => e.CategoryId.HasValue).ToList();
            Assert.NotEmpty(eventsWithCategory);

            foreach (var evt in eventsWithCategory)
            {
                Assert.NotNull(evt.Category);
                Assert.Equal(evt.CategoryId, evt.Category.Id);
                Assert.False(string.IsNullOrWhiteSpace(evt.Category.Name));
                Assert.False(string.IsNullOrWhiteSpace(evt.Category.Color));
                Assert.False(string.IsNullOrWhiteSpace(evt.Category.Description));
            }
        }

        #endregion

        #region AC3: Event không có Category vẫn được xử lý đúng theo business rule

        [Fact]
        public async Task GetEvents_EventsWithoutCategory_HandledCorrectlyAccordingToBusinessRule()
        {
            // Arrange (AC3)
            var sampleEvents = GetSampleEvents();
            _mockEventService
                .Setup(s => s.GetEventsAsync(null, null, null, null))
                .ReturnsAsync(sampleEvents);

            // Act
            var result = await _controller.GetEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<EventDto>>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);

            // Business rule: Event without category must have CategoryId == null and Category == null
            var eventsWithoutCategory = apiResponse.Data.Where(e => !e.CategoryId.HasValue).ToList();
            Assert.NotEmpty(eventsWithoutCategory);

            foreach (var evt in eventsWithoutCategory)
            {
                Assert.Null(evt.CategoryId);
                Assert.Null(evt.Category);
                Assert.False(string.IsNullOrWhiteSpace(evt.Title));
                Assert.False(string.IsNullOrWhiteSpace(evt.Location));
            }
        }

        [Fact]
        public async Task GetEvents_FilterHasCategoryFalse_ReturnsOnlyEventsWithoutCategory()
        {
            // Arrange (AC3)
            var uncategorizedEvents = GetSampleEvents().Where(e => !e.CategoryId.HasValue).ToList();
            _mockEventService
                .Setup(s => s.GetEventsAsync(null, false, null, null))
                .ReturnsAsync(uncategorizedEvents);

            // Act
            var result = await _controller.GetEvents(hasCategory: false);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<EventDto>>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
            Assert.All(apiResponse.Data, e =>
            {
                Assert.Null(e.CategoryId);
                Assert.Null(e.Category);
            });
        }

        #endregion

        #region AC4: API trả về danh sách rỗng nếu không có Event phù hợp

        [Fact]
        public async Task GetEvents_WhenNoMatchingEvents_ReturnsOkWithEmptyList()
        {
            // Arrange (AC4)
            int nonExistentCategoryId = 9999;
            _mockEventService
                .Setup(s => s.GetEventsAsync(nonExistentCategoryId, null, null, null))
                .ReturnsAsync(new List<EventDto>());

            // Act
            var result = await _controller.GetEvents(categoryId: nonExistentCategoryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<EventDto>>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(200, apiResponse.StatusCode);
            Assert.NotNull(apiResponse.Data);
            Assert.Empty(apiResponse.Data);
        }

        #endregion

        #region AC5: API trả về HTTP status và response format đúng quy định

        [Fact]
        public async Task GetEvents_ReturnsStandardApiResponseFormat_WithHttpStatus200()
        {
            // Arrange (AC5)
            _mockEventService
                .Setup(s => s.GetEventsAsync(null, null, null, null))
                .ReturnsAsync(GetSampleEvents());

            // Act
            var result = await _controller.GetEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var response = Assert.IsType<ApiResponse<IEnumerable<EventDto>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(200, response.StatusCode);
            Assert.False(string.IsNullOrWhiteSpace(response.Message));
            Assert.NotNull(response.Data);
        }

        [Fact]
        public async Task GetEvents_WithNegativeCategoryId_ReturnsBadRequestWithApiErrorResponse()
        {
            // Arrange (AC5 - Validation Error)
            int invalidCategoryId = -1;

            // Act
            var result = await _controller.GetEvents(categoryId: invalidCategoryId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);

            var errorResponse = Assert.IsType<ApiErrorResponse>(badRequestResult.Value);
            Assert.False(errorResponse.Success);
            Assert.Equal(400, errorResponse.StatusCode);
            Assert.Equal("BAD_REQUEST", errorResponse.Error);
            Assert.NotNull(errorResponse.Errors);
            Assert.Contains(errorResponse.Errors, err => err.Field == "categoryId");
        }

        [Fact]
        public async Task GetEventById_WhenNotFound_ReturnsNotFoundWithApiErrorResponse()
        {
            // Arrange (AC5 - Not Found)
            int nonExistentId = 999;
            _mockEventService
                .Setup(s => s.GetEventByIdAsync(nonExistentId))
                .ReturnsAsync((EventDto?)null);

            // Act
            var result = await _controller.GetEventById(nonExistentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);

            var errorResponse = Assert.IsType<ApiErrorResponse>(notFoundResult.Value);
            Assert.False(errorResponse.Success);
            Assert.Equal(404, errorResponse.StatusCode);
            Assert.Equal("NOT_FOUND", errorResponse.Error);
        }

        #endregion

        #region AC6: API được kiểm thử với Event có và không có Category

        [Fact]
        public async Task GetEventById_WithCategory_ReturnsEventWithCompleteCategoryInfo()
        {
            // Arrange (AC6 - Event with Category)
            var eventWithCategory = new EventDto
            {
                Id = 1,
                Title = "Hội thảo AI 2026",
                Location = "Hội trường A1",
                CategoryId = 1,
                Category = new CategoryDto
                {
                    Id = 1,
                    Name = "Hội thảo & Học thuật",
                    Description = "Khoa học & nghiên cứu",
                    Color = "#3068DF"
                }
            };
            _mockEventService
                .Setup(s => s.GetEventByIdAsync(1))
                .ReturnsAsync(eventWithCategory);

            // Act
            var result = await _controller.GetEventById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<EventDto>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
            Assert.NotNull(apiResponse.Data.Category);
            Assert.Equal(1, apiResponse.Data.CategoryId);
            Assert.Equal("Hội thảo & Học thuật", apiResponse.Data.Category.Name);
        }

        [Fact]
        public async Task GetEventById_WithoutCategory_ReturnsEventWithNullCategory_WithoutError()
        {
            // Arrange (AC6 - Event without Category)
            var eventWithoutCategory = new EventDto
            {
                Id = 5,
                Title = "Ngày hội Sách Tự do",
                Location = "Hành lang Thư viện",
                CategoryId = null,
                Category = null
            };
            _mockEventService
                .Setup(s => s.GetEventByIdAsync(5))
                .ReturnsAsync(eventWithoutCategory);

            // Act
            var result = await _controller.GetEventById(5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<EventDto>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
            Assert.Null(apiResponse.Data.CategoryId);
            Assert.Null(apiResponse.Data.Category);
            Assert.Equal("Ngày hội Sách Tự do", apiResponse.Data.Title);
        }

        [Fact]
        public async Task IntegrationTest_EventService_DefaultDataset_ContainsBothCategorizedAndUncategorizedEvents()
        {
            // Integration-style test directly testing real EventService (AC2, AC3, AC6)
            var service = new EventService();

            var allEvents = (await service.GetEventsAsync()).ToList();

            // Verify dataset has both types of events
            Assert.True(allEvents.Any(e => e.CategoryId.HasValue && e.Category != null), "Must contain events with category");
            Assert.True(allEvents.Any(e => !e.CategoryId.HasValue && e.Category == null), "Must contain events without category");

            // Verify filtering by category
            var categorizedOnly = (await service.GetEventsAsync(categoryId: 1)).ToList();
            Assert.NotEmpty(categorizedOnly);
            Assert.All(categorizedOnly, e => Assert.Equal(1, e.CategoryId));

            // Verify empty result on non-existent category
            var emptyResult = (await service.GetEventsAsync(categoryId: 9999)).ToList();
            Assert.Empty(emptyResult);
        }

        #endregion

        #region Helper Data Generator

        private static List<EventDto> GetSampleEvents()
        {
            var categoryAcademic = new CategoryDto
            {
                Id = 1,
                Name = "Hội thảo & Học thuật",
                Description = "Hội thảo chuyên đề khoa học",
                Color = "#3068DF",
                Icon = "school"
            };

            var categorySports = new CategoryDto
            {
                Id = 3,
                Name = "Thể dục & Thể thao",
                Description = "Giải bóng đá và rèn luyện",
                Color = "#FF3300",
                Icon = "fitness"
            };

            return new List<EventDto>
            {
                // Event có Category (AC2)
                new EventDto
                {
                    Id = 1,
                    Title = "Hội thảo AI 2026",
                    Description = "Ứng dụng AI",
                    Location = "Hội trường A1",
                    StartDate = DateTime.UtcNow.AddDays(3),
                    EndDate = DateTime.UtcNow.AddDays(3).AddHours(4),
                    Status = "Upcoming",
                    CategoryId = 1,
                    Category = categoryAcademic
                },
                // Event có Category (AC2)
                new EventDto
                {
                    Id = 2,
                    Title = "Giải Bóng Đá Campus Cup",
                    Description = "Bóng đá sinh viên",
                    Location = "Sân vận động",
                    StartDate = DateTime.UtcNow.AddDays(7),
                    EndDate = DateTime.UtcNow.AddDays(10),
                    Status = "Upcoming",
                    CategoryId = 3,
                    Category = categorySports
                },
                // Event KHÔNG CÓ Category (AC3)
                new EventDto
                {
                    Id = 3,
                    Title = "Ngày Hội Sách Tự Do",
                    Description = "Trao đổi sách",
                    Location = "Sảnh Thư viện",
                    StartDate = DateTime.UtcNow.AddDays(5),
                    EndDate = DateTime.UtcNow.AddDays(6),
                    Status = "Upcoming",
                    CategoryId = null,
                    Category = null
                },
                // Event KHÔNG CÓ Category (AC3)
                new EventDto
                {
                    Id = 4,
                    Title = "Tập huấn PCCC & Kỹ Năng Thoát Hiểm",
                    Description = "An toàn ký túc xá",
                    Location = "Ký túc xá B3",
                    StartDate = DateTime.UtcNow.AddDays(8),
                    EndDate = DateTime.UtcNow.AddDays(8).AddHours(3),
                    Status = "Upcoming",
                    CategoryId = null,
                    Category = null
                }
            };
        }

        #endregion
    }
}
