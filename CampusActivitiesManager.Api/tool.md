Nhiệm vụ: Quản lý tài khoản (Create Account + Update Account API)
Các công cụ cần sử dụng:
1. Môi trường phát triển
- Visual Studio Community (với workload ASP.NET and web development): Dùng làm môi trường IDE chính để lập trình C#, cấu hình project Web API, và debug dịch vụ backend.

2. Framework & Thư viện (Libraries & SDKs)
- ASP.NET Core Web API (.NET 9): Framework nền tảng để xây dựng các RESTful endpoints (Controllers) xử lý các HTTP requests (POST, PUT, PATCH).
- FirebaseAdmin SDK: Tích hợp với Firebase Authentication để xử lý logic tạo mới (`CreateUserAsync`) và cập nhật (`UpdateUserAsync`) thông tin tài khoản người dùng một cách bảo mật.
- Google.Cloud.Firestore: Kết nối và tương tác với cơ sở dữ liệu Cloud Firestore (NoSQL) để đồng bộ và lưu trữ các thuộc tính mở rộng của tài khoản người dùng (role, phoneNumber, studentCode, avatarUrl).

3. Mô hình kiến trúc & Thiết kế API
- Kiến trúc Web API (Controller Pattern): Sử dụng `AccountsController` để tiếp nhận request, gọi đến Firebase SDK/Firestore và trả về response cho client.
- Data Annotations (`System.ComponentModel.DataAnnotations`): Cung cấp các attribute (`[Required]`, `[MinLength]`, `[RegularExpression]`) để tự động validate (xác thực) dữ liệu đầu vào trên các DTO Models (`CreateAccountRequest`, `UpdateAccountRequest`).
- Chuẩn hóa Response (API Response Wrapper): Định nghĩa cấu trúc `ApiResponse<T>` và `ApiErrorResponse` để đảm bảo định dạng trả về thống nhất, tường minh (hỗ trợ bóc tách chi tiết các field lỗi) đi kèm với các HTTP Status Codes chuẩn REST (200, 201, 400, 404, 409, 500).

4. Dependency Injection & Cấu hình hệ thống
- Microsoft.Extensions.DependencyInjection (MS.DI): Đăng ký các dịch vụ (Controllers, Swagger) trong `Program.cs`. Tùy biến `ApiBehaviorOptions` để chặn response mặc định và thay bằng custom validation response.
- Environment Variables (Biến môi trường): Quản lý và nạp file chứng chỉ Firebase (Service Account Key) thông qua biến `GOOGLE_APPLICATION_CREDENTIALS` để đảm bảo bảo mật cao nhất, không lộ key trong source code.
- Swagger / OpenAPI: Tích hợp sẵn để tự động sinh tài liệu mô tả API và giao diện web giúp nhà phát triển dễ dàng test các endpoints.

5. Quản lý mã nguồn & Hệ thống version
- Git & GitHub: Công cụ quản lý phiên bản mã nguồn, tạo luồng công việc rẽ nhánh (branching workflow như `us35_t35.1`), thực hiện commit các thay đổi và đẩy code lên kho lưu trữ chung.
