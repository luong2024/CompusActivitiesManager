using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CampusActivitiesManager.Api.Models;
using Xunit;

namespace CampusActivitiesManager.Api.Tests
{
    public class AttendanceIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public AttendanceIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Fact]
        public async Task QrCheckIn_WithValidData_ReturnsSuccessAndAttendanceRecord()
        {
            // Arrange
            var request = new QrCheckInRequest
            {
                EventId = 2,
                StudentCode = "B22DCCN999",
                QrTicketCode = "CAMPUS-EVT2-B22DCCN999-SECURE"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/attendance/qr", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AttendanceRecord>>(content, _jsonOptions);

            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal("B22DCCN999", apiResponse.Data.StudentCode);
            Assert.Equal("QR", apiResponse.Data.Method);
            Assert.Equal("Present", apiResponse.Data.Status);
        }

        [Fact]
        public async Task QrCheckIn_EmptyQr_ReturnsBadRequest()
        {
            // Arrange
            var request = new QrCheckInRequest
            {
                EventId = 1,
                StudentCode = "B20DCCN001",
                QrTicketCode = ""
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/attendance/qr", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PhotoCheckIn_WithValidPhoto_ReturnsSuccess()
        {
            // Arrange
            var request = new PhotoCheckInRequest
            {
                EventId = 3,
                StudentCode = "B22DCCN888",
                PhotoBase64 = "data:image/jpeg;base64,/9j/4AAQSkZJRg==",
                Notes = "Điểm danh hoạt động trồng cây"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/attendance/photo", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AttendanceRecord>>(content, _jsonOptions);

            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal("Photo", apiResponse.Data.Method);
            Assert.Contains("trồng cây", apiResponse.Data.Notes);
        }

        [Fact]
        public async Task ManualAttendance_ValidData_ReturnsSuccess()
        {
            // Arrange
            var request = new ManualAttendanceRequest
            {
                EventId = 1,
                StudentCode = "B22DCCN777",
                Status = "Present",
                TrainingPoints = 6,
                Notes = "Cộng điểm rèn luyện ban tổ chức"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/attendance/manual", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AttendanceRecord>>(content, _jsonOptions);

            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Success);
            Assert.Equal(6, apiResponse.Data!.TrainingPointsAwarded);
        }

        [Fact]
        public async Task GetStudentAttendanceHistory_ReturnsListWithTrainingPoints()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/attendance/student/B20DCCN001");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<AttendanceRecord>>>(content, _jsonOptions);

            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Success);
            Assert.NotEmpty(apiResponse.Data!);
            Assert.All(apiResponse.Data!, r => Assert.Equal("B20DCCN001", r.StudentCode));
        }

        [Fact]
        public async Task ExportAttendanceCsv_ReturnsCsvFileWithUtf8BOM()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/attendance/event/1/export-csv");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/csv; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.True(bytes.Length > 3);
            // Verify UTF-8 BOM
            Assert.Equal(0xEF, bytes[0]);
            Assert.Equal(0xBB, bytes[1]);
            Assert.Equal(0xBF, bytes[2]);
        }

        [Fact]
        public async Task GetNotifications_ReturnsNotificationList()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/notifications");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<CampusNotificationDto>>>(content, _jsonOptions);

            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Success);
            Assert.NotEmpty(apiResponse.Data!);
        }
    }
}
