using System;

namespace CampusActivitiesManager.Services
{
    /// <summary>
    /// Ngoại lệ xảy ra khi không thể kết nối mạng hoặc máy chủ không phản hồi
    /// (HttpRequestException, SocketException, Timeout, v.v.)
    /// </summary>
    public class NetworkException : Exception
    {
        public NetworkException(string message) : base(message) { }
        public NetworkException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Ngoại lệ xảy ra khi dữ liệu đầu vào không hợp lệ hoặc máy chủ trả về mã HTTP 400 Bad Request
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Ngoại lệ xảy ra khi máy chủ API trả về mã lỗi HTTP 4xx, 5xx (ngoại trừ 400 Validation)
    /// </summary>
    public class ApiException : Exception
    {
        public int StatusCode { get; }

        public ApiException(string message, int statusCode = 500) : base(message)
        {
            StatusCode = statusCode;
        }

        public ApiException(string message, int statusCode, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
