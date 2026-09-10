using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    /// <summary>
    /// Service thực thi gọi HTTP RESTful API quản lý danh sách tài khoản
    /// </summary>
    public class AccountService : IUserService<Account>
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AccountService>? _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public AccountService(HttpClient httpClient, ILogger<AccountService>? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        /// <summary>
        /// GET /api/accounts - Lấy toàn bộ danh sách tài khoản từ API
        /// </summary>
        public async Task<List<Account>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/accounts");

                if (!response.IsSuccessStatusCode)
                {
                    await HandleErrorResponseAsync(response);
                }

                var accounts = await response.Content.ReadFromJsonAsync<List<Account>>(_jsonOptions);
                return accounts ?? new List<Account>();
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "Lỗi mạng khi gọi GET /api/accounts");
                throw new NetworkException("Lỗi kết nối mạng (Network Error): Không thể kết nối tới máy chủ API. Vui lòng kiểm tra kết nối mạng hoặc máy chủ.", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout khi gọi GET /api/accounts");
                throw new NetworkException("Lỗi kết nối mạng: Yêu cầu tới máy chủ bị quá thời gian (Timeout).", ex);
            }
        }

        /// <summary>
        /// GET /api/accounts/{id} - Lấy chi tiết tài khoản theo ID
        /// </summary>
        public async Task<Account?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException("Lỗi kiểm tra dữ liệu (Validation Error): ID tài khoản không hợp lệ.");
            }

            try
            {
                var response = await _httpClient.GetAsync($"/api/accounts/{id}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    await HandleErrorResponseAsync(response);
                }

                return await response.Content.ReadFromJsonAsync<Account>(_jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "Lỗi mạng khi gọi GET /api/accounts/{Id}", id);
                throw new NetworkException($"Lỗi kết nối mạng: Không thể lấy thông tin tài khoản #{id} từ máy chủ.", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout khi gọi GET /api/accounts/{Id}", id);
                throw new NetworkException("Lỗi kết nối mạng: Quá thời gian chờ phản hồi từ máy chủ.", ex);
            }
        }

        /// <summary>
        /// POST /api/accounts - Thêm mới tài khoản
        /// </summary>
        public async Task<Account> CreateAsync(Account item)
        {
            if (item == null)
            {
                throw new ValidationException("Lỗi kiểm tra dữ liệu (Validation Error): Dữ liệu tài khoản không được để trống.");
            }

            // Client-side validation
            if (string.IsNullOrWhiteSpace(item.FullName))
            {
                throw new ValidationException("Lỗi kiểm tra dữ liệu (Validation Error): Họ và tên không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(item.StudentCode))
            {
                throw new ValidationException("Lỗi kiểm tra dữ liệu (Validation Error): Mã sinh viên/cán bộ không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(item.Email) || !item.Email.Contains("@"))
            {
                throw new ValidationException("Lỗi kiểm tra dữ liệu (Validation Error): Địa chỉ email không đúng định dạng.");
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/accounts", item, _jsonOptions);

                if (!response.IsSuccessStatusCode)
                {
                    await HandleErrorResponseAsync(response);
                }

                var created = await response.Content.ReadFromJsonAsync<Account>(_jsonOptions);
                return created ?? item;
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "Lỗi mạng khi tạo tài khoản");
                throw new NetworkException("Lỗi kết nối mạng: Không thể gửi yêu cầu tạo tài khoản tới máy chủ API.", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout khi tạo tài khoản");
                throw new NetworkException("Lỗi kết nối mạng: Quá thời gian kết nối tới máy chủ.", ex);
            }
        }

        /// <summary>
        /// PUT /api/accounts/{id} - Cập nhật thông tin tài khoản
        /// </summary>
        public async Task<bool> UpdateAsync(int id, Account item)
        {
            if (id <= 0 || item == null)
            {
                throw new ValidationException("Lỗi kiểm tra dữ liệu (Validation Error): Thông tin cập nhật không hợp lệ.");
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/api/accounts/{id}", item, _jsonOptions);

                if (!response.IsSuccessStatusCode)
                {
                    await HandleErrorResponseAsync(response);
                }

                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "Lỗi mạng khi cập nhật tài khoản #{Id}", id);
                throw new NetworkException($"Lỗi kết nối mạng: Không thể cập nhật tài khoản #{id} trên máy chủ.", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout khi cập nhật tài khoản #{Id}", id);
                throw new NetworkException("Lỗi kết nối mạng: Quá thời gian kết nối tới máy chủ.", ex);
            }
        }

        /// <summary>
        /// DELETE /api/accounts/{id} - Xóa tài khoản theo ID
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException("Lỗi kiểm tra dữ liệu (Validation Error): ID tài khoản cần xóa không hợp lệ.");
            }

            try
            {
                var response = await _httpClient.DeleteAsync($"/api/accounts/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    await HandleErrorResponseAsync(response);
                }

                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "Lỗi mạng khi xóa tài khoản #{Id}", id);
                throw new NetworkException($"Lỗi kết nối mạng: Không thể xóa tài khoản #{id} trên máy chủ.", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout khi xóa tài khoản #{Id}", id);
                throw new NetworkException("Lỗi kết nối mạng: Quá thời gian kết nối tới máy chủ.", ex);
            }
        }

        /// <summary>
        /// Phân tích mã phản hồi HTTP và ném ra ngoại lệ tương ứng (Validation error / API error)
        /// </summary>
        private static async Task HandleErrorResponseAsync(HttpResponseMessage response)
        {
            string errorBody = string.Empty;
            try
            {
                errorBody = await response.Content.ReadAsStringAsync();
            }
            catch
            {
                // Bỏ qua lỗi đọc body
            }

            int statusCode = (int)response.StatusCode;

            // 400 Bad Request -> Validation error
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                string message = !string.IsNullOrWhiteSpace(errorBody)
                    ? $"Lỗi xác thực (Validation Error): {errorBody}"
                    : "Lỗi xác thực (Validation Error): Dữ liệu gửi lên không đúng định dạng hoặc thiếu thông tin bắt buộc.";
                throw new ValidationException(message);
            }

            // 404 Not Found -> API error
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new ApiException("Lỗi API (404 Not Found): Không tìm thấy tài nguyên trên máy chủ.", 404);
            }

            // 401 Unauthorized / 403 Forbidden
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new ApiException("Lỗi API: Bạn không có quyền thực hiện thao tác này trên hệ thống.", statusCode);
            }

            // 5xx hoặc các lỗi khác -> API error
            string serverMessage = !string.IsNullOrWhiteSpace(errorBody)
                ? $"Lỗi máy chủ API ({statusCode}): {errorBody}"
                : $"Lỗi máy chủ API ({statusCode}): Đã xảy ra sự cố từ phía server.";
            throw new ApiException(serverMessage, statusCode);
        }
    }
}
