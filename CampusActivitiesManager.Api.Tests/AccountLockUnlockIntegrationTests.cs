using CampusActivitiesManager.Api.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace CampusActivitiesManager.Api.Tests
{
    public class AccountLockUnlockIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public AccountLockUnlockIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task<UserAccountDto> CreateTestAccountAsync(string emailPrefix)
        {
            var createRequest = new CreateAccountRequest
            {
                Email = $"{emailPrefix}_{Guid.NewGuid():N}@campus.edu",
                Password = "SecurePassword@2026",
                FullName = "Nguyen Van Test",
                Role = "Student",
                PhoneNumber = "0912345678"
            };

            var postResponse = await _client.PostAsJsonAsync("/api/v1/accounts", createRequest);
            postResponse.EnsureSuccessStatusCode();

            var apiResult = await postResponse.Content.ReadFromJsonAsync<ApiResponse<JsonElement>>(_jsonOptions);
            Assert.NotNull(apiResult);
            Assert.True(apiResult.Success);
            Assert.Equal(201, apiResult.StatusCode);

            string userId = apiResult.Data.GetProperty("id").GetString()!;
            return _factory.AccountService.Database[userId];
        }

        /// <summary>
        /// AC 35.1.1: API lock account thành công với account tồn tại.
        /// </summary>
        [Fact]
        public async Task AC_35_1_1_LockAccount_WithExistingAccount_ShouldReturn200AndLockSuccessfully()
        {
            // Arrange: Tạo tài khoản đang active
            var account = await CreateTestAccountAsync("lock_success");

            // Act: Gọi API khóa tài khoản
            var response = await _client.PostAsync($"/api/v1/accounts/{account.Id}/lock", null);

            // Assert: Trả về HTTP 200 OK và trạng thái đã khóa
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<AccountStatusResponse>>(_jsonOptions);
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Account locked successfully", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(account.Id, result.Data.Id);
            Assert.True(result.Data.IsLocked);
            Assert.False(result.Data.IsActive);
        }

        /// <summary>
        /// AC 35.1.2: API unlock account thành công với account đang bị khóa.
        /// </summary>
        [Fact]
        public async Task AC_35_1_2_UnlockAccount_WithLockedAccount_ShouldReturn200AndUnlockSuccessfully()
        {
            // Arrange: Tạo tài khoản rồi khóa lại trước
            var account = await CreateTestAccountAsync("unlock_success");
            var lockResponse = await _client.PostAsync($"/api/v1/accounts/{account.Id}/lock", null);
            Assert.Equal(HttpStatusCode.OK, lockResponse.StatusCode);

            // Act: Gọi API mở khóa tài khoản đang bị khóa
            var unlockResponse = await _client.PostAsync($"/api/v1/accounts/{account.Id}/unlock", null);

            // Assert: Trả về HTTP 200 OK và trạng thái đã được mở khóa
            Assert.Equal(HttpStatusCode.OK, unlockResponse.StatusCode);

            var result = await unlockResponse.Content.ReadFromJsonAsync<ApiResponse<AccountStatusResponse>>(_jsonOptions);
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Account unlocked successfully", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(account.Id, result.Data.Id);
            Assert.False(result.Data.IsLocked);
            Assert.True(result.Data.IsActive);
        }

        /// <summary>
        /// AC 35.1.3: Không thể lock/unlock account không tồn tại.
        /// </summary>
        [Fact]
        public async Task AC_35_1_3_LockOrUnlock_NonExistentAccount_ShouldReturn404NotFound()
        {
            string nonExistentId = "non_existent_account_99999";

            // 1. Thử khóa tài khoản không tồn tại
            var lockResponse = await _client.PostAsync($"/api/v1/accounts/{nonExistentId}/lock", null);
            Assert.Equal(HttpStatusCode.NotFound, lockResponse.StatusCode);

            var lockError = await lockResponse.Content.ReadFromJsonAsync<ApiErrorResponse>(_jsonOptions);
            Assert.NotNull(lockError);
            Assert.False(lockError.Success);
            Assert.Equal(404, lockError.StatusCode);
            Assert.Equal("NOT_FOUND", lockError.Error);
            Assert.Contains(nonExistentId, lockError.Message);

            // 2. Thử mở khóa tài khoản không tồn tại
            var unlockResponse = await _client.PostAsync($"/api/v1/accounts/{nonExistentId}/unlock", null);
            Assert.Equal(HttpStatusCode.NotFound, unlockResponse.StatusCode);

            var unlockError = await unlockResponse.Content.ReadFromJsonAsync<ApiErrorResponse>(_jsonOptions);
            Assert.NotNull(unlockError);
            Assert.False(unlockError.Success);
            Assert.Equal(404, unlockError.StatusCode);
            Assert.Equal("NOT_FOUND", unlockError.Error);
            Assert.Contains(nonExistentId, unlockError.Message);
        }

        /// <summary>
        /// AC 35.1.4: Trạng thái account trong database được cập nhật chính xác.
        /// </summary>
        [Fact]
        public async Task AC_35_1_4_DatabaseStatus_ShouldBeAccuratelyUpdated()
        {
            // Arrange
            var account = await CreateTestAccountAsync("db_status_check");
            string id = account.Id;

            // Kiểm tra trạng thái ban đầu trong CSDL
            var dbInitial = _factory.AccountService.Database[id];
            Assert.False(dbInitial.IsDisabled);
            Assert.True(dbInitial.IsActive);

            // Act 1: Lock account
            await _client.PostAsync($"/api/v1/accounts/{id}/lock", null);

            // Assert 1: Database phải phản ánh Disabled = true, IsActive = false
            var dbLocked = _factory.AccountService.Database[id];
            Assert.True(dbLocked.IsDisabled);
            Assert.False(dbLocked.IsActive);
            Assert.False(string.IsNullOrEmpty(dbLocked.UpdatedAt));

            // Act 2: Unlock account
            await _client.PostAsync($"/api/v1/accounts/{id}/unlock", null);

            // Assert 2: Database phải phản ánh Disabled = false, IsActive = true
            var dbUnlocked = _factory.AccountService.Database[id];
            Assert.False(dbUnlocked.IsDisabled);
            Assert.True(dbUnlocked.IsActive);
        }

        /// <summary>
        /// AC 35.1.5: Account bị lock không thể đăng nhập.
        /// </summary>
        [Fact]
        public async Task AC_35_1_5_LockedAccount_CannotLogin_ShouldReturn403Forbidden()
        {
            // Arrange: Tạo account và mật khẩu
            string email = $"locked_user_{Guid.NewGuid():N}@campus.edu";
            string password = "SecurePassword@2026";
            var createRequest = new CreateAccountRequest
            {
                Email = email,
                Password = password,
                FullName = "Locked User Demo",
                Role = "Student"
            };
            var createRes = await _client.PostAsJsonAsync("/api/v1/accounts", createRequest);
            createRes.EnsureSuccessStatusCode();

            var account = _factory.AccountService.Database.Values.First(u => u.Email == email);

            // Khóa tài khoản
            var lockRes = await _client.PostAsync($"/api/v1/accounts/{account.Id}/lock", null);
            Assert.Equal(HttpStatusCode.OK, lockRes.StatusCode);

            // Act: Thử đăng nhập với tài khoản bị khóa
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };
            var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

            // Assert: API từ chối đăng nhập với HTTP 403 Forbidden
            Assert.Equal(HttpStatusCode.Forbidden, loginResponse.StatusCode);

            var error = await loginResponse.Content.ReadFromJsonAsync<ApiErrorResponse>(_jsonOptions);
            Assert.NotNull(error);
            Assert.False(error.Success);
            Assert.Equal(403, error.StatusCode);
            Assert.Equal("ACCOUNT_LOCKED", error.Error);
            Assert.Equal("Account is locked. Please contact administrator.", error.Message);
        }

        /// <summary>
        /// AC 35.1.6: API trả đúng HTTP Status Code và error message.
        /// </summary>
        [Fact]
        public async Task AC_35_1_6_ApiReturns_CorrectHttpStatusCodesAndEnvelopeFormat()
        {
            // 1. Kiểm tra validation error (400 Bad Request) trên Login
            var invalidLogin = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest
            {
                Email = "invalid-email-format",
                Password = ""
            });
            Assert.Equal(HttpStatusCode.BadRequest, invalidLogin.StatusCode);
            var err400 = await invalidLogin.Content.ReadFromJsonAsync<ApiErrorResponse>(_jsonOptions);
            Assert.NotNull(err400);
            Assert.False(err400.Success);
            Assert.Equal(400, err400.StatusCode);
            Assert.NotNull(err400.Errors);
            Assert.NotEmpty(err400.Errors);

            // 2. Kiểm tra sai thông tin đăng nhập (401 Unauthorized)
            var unauthorizedLogin = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest
            {
                Email = "notfound@campus.edu",
                Password = "RandomPassword123!"
            });
            Assert.Equal(HttpStatusCode.Unauthorized, unauthorizedLogin.StatusCode);
            var err401 = await unauthorizedLogin.Content.ReadFromJsonAsync<ApiErrorResponse>(_jsonOptions);
            Assert.NotNull(err401);
            Assert.False(err401.Success);
            Assert.Equal(401, err401.StatusCode);
            Assert.Equal("UNAUTHORIZED", err401.Error);

            // 3. Kiểm tra Not Found (404)
            var notFoundRes = await _client.PostAsync("/api/v1/accounts/acc_99999/lock", null);
            Assert.Equal(HttpStatusCode.NotFound, notFoundRes.StatusCode);
            var err404 = await notFoundRes.Content.ReadFromJsonAsync<ApiErrorResponse>(_jsonOptions);
            Assert.NotNull(err404);
            Assert.False(err404.Success);
            Assert.Equal(404, err404.StatusCode);

            // 4. Kiểm tra Thành công (200 OK)
            var account = await CreateTestAccountAsync("status_code_test");
            var successRes = await _client.PostAsync($"/api/v1/accounts/{account.Id}/lock", null);
            Assert.Equal(HttpStatusCode.OK, successRes.StatusCode);
            var successEnvelope = await successRes.Content.ReadFromJsonAsync<ApiResponse<AccountStatusResponse>>(_jsonOptions);
            Assert.NotNull(successEnvelope);
            Assert.True(successEnvelope.Success);
            Assert.Equal(200, successEnvelope.StatusCode);
        }

        /// <summary>
        /// AC 35.1.7: Hoàn thành integration test toàn diện với Login API (End-to-End Flow).
        /// </summary>
        [Fact]
        public async Task AC_35_1_7_FullIntegrationWorkflow_Create_Login_Lock_RejectLogin_Unlock_LoginSuccess()
        {
            string email = $"e2e_user_{Guid.NewGuid():N}@campus.edu";
            string password = "SecurePassword@2026";

            // Bước 1: Tạo tài khoản mới thành công (HTTP 201)
            var createRequest = new CreateAccountRequest
            {
                Email = email,
                Password = password,
                FullName = "End To End Test User",
                Role = "Student"
            };
            var createRes = await _client.PostAsJsonAsync("/api/v1/accounts", createRequest);
            Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);

            var account = _factory.AccountService.Database.Values.First(u => u.Email == email);
            string userId = account.Id;

            // Bước 2: Đăng nhập thành công lần đầu khi tài khoản active (HTTP 200)
            var loginRequest = new LoginRequest { Email = email, Password = password };
            var login1Res = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
            Assert.Equal(HttpStatusCode.OK, login1Res.StatusCode);
            var login1Data = await login1Res.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>(_jsonOptions);
            Assert.NotNull(login1Data);
            Assert.True(login1Data.Success);
            Assert.NotNull(login1Data.Data?.Token);

            // Bước 3: Khóa tài khoản (HTTP 200)
            var lockRes = await _client.PostAsync($"/api/v1/accounts/{userId}/lock", null);
            Assert.Equal(HttpStatusCode.OK, lockRes.StatusCode);

            // Bước 4: Thử đăng nhập lại -> Phải bị từ chối với HTTP 403 Forbidden
            var loginLockedRes = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
            Assert.Equal(HttpStatusCode.Forbidden, loginLockedRes.StatusCode);
            var lockedErr = await loginLockedRes.Content.ReadFromJsonAsync<ApiErrorResponse>(_jsonOptions);
            Assert.NotNull(lockedErr);
            Assert.Equal("ACCOUNT_LOCKED", lockedErr.Error);

            // Bước 5: Mở khóa tài khoản (HTTP 200)
            var unlockRes = await _client.PostAsync($"/api/v1/accounts/{userId}/unlock", null);
            Assert.Equal(HttpStatusCode.OK, unlockRes.StatusCode);

            // Bước 6: Đăng nhập lại sau khi mở khóa -> Thành công (HTTP 200)
            var loginAfterUnlockRes = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
            Assert.Equal(HttpStatusCode.OK, loginAfterUnlockRes.StatusCode);
            var login2Data = await loginAfterUnlockRes.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>(_jsonOptions);
            Assert.NotNull(login2Data);
            Assert.True(login2Data.Success);
            Assert.True(login2Data.Data?.IsActive);
        }
    }
}
