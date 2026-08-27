# Nhiệm vụ: Xây dựng API Quản lý Tài khoản (Create & Update)

**Mã Task**: T35.1
**User Story**: US35 – Account Management API (Phát triển API quản lý tài khoản: tạo, cập nhật, khóa/mở khóa tài khoản)
**Người được giao (Assignee)**: Nguyễn Đức Mạnh
**Trạng thái**: Cần thực hiện (To-Do)
**Thời gian dự kiến (Estimate)**: 10 giờ

---

## 1. Mục tiêu nhiệm vụ
Xây dựng các RESTful API endpoints an toàn cho phép quản trị viên hệ thống có thể tạo mới (Create) và cập nhật (Update) tài khoản người dùng.
Yêu cầu bắt buộc: API phải được tích hợp và đồng bộ hóa với Firebase Authentication để quản lý đăng nhập, đồng thời lưu trữ siêu dữ liệu (metadata) trên Cloud Firestore/Realtime Database.

## 2. Chi tiết các hạng mục cần thực hiện (Checklist)

### 2.1. Khởi tạo & Cấu hình Project
- [ ] Khởi tạo hoặc cập nhật project `CampusActivitiesManager.Api` (ASP.NET Core Web API).
- [ ] Cài đặt các thư viện/SDK cần thiết: `FirebaseAdmin`, `Google.Cloud.Firestore`.
- [ ] Thiết lập cấu hình Middleware và Dependency Injection trong `Program.cs`.
- [ ] Cấu hình xác thực Firebase thông qua Environment Variable `GOOGLE_APPLICATION_CREDENTIALS` để bảo mật.

### 2.2. Xây dựng DTO Models & Xử lý Validation
- [ ] **Tạo class `CreateAccountRequest`**: Thiết lập Data Annotations để validate (bắt buộc nhập, đúng định dạng Email, Password tối thiểu 8 ký tự, có đủ chữ hoa, thường, số, ký tự đặc biệt, Role chỉ nhận giá trị Admin/Lecturer/Student).
- [ ] **Tạo class `UpdateAccountRequest`**: Thiết lập các trường cho phép tuỳ chọn (nullable) như FullName, PhoneNumber, AvatarUrl, Role nhưng nếu có gửi lên thì phải đúng định dạng.
- [ ] **Tạo class Response chuẩn**: Xây dựng `ApiResponse<T>` và `ApiErrorResponse` theo chuẩn RFC 7807 đảm bảo JSON luôn trả về cấu trúc gồm `success`, `statusCode`, `message`, `data`/`errors`.
- [ ] Cấu hình tùy chỉnh (SuppressModelStateInvalidFilter) để ASP.NET Core không tự trả về lỗi 400 mặc định mà trả về định dạng `ApiErrorResponse` do lập trình viên định nghĩa.

### 2.3. Triển khai API Endpoints (AccountsController)
- [ ] **Tạo endpoint `POST /api/v1/accounts` (Tạo tài khoản)**:
  - Lấy dữ liệu từ Request, kiểm tra tính hợp lệ (Validation).
  - Gọi `FirebaseAuth.DefaultInstance.CreateUserAsync()` để tạo tài khoản trên Firebase.
  - Sử dụng `FirestoreDb` để lưu thêm thông tin (role, số điện thoại, avatar...) vào collection `users` tương ứng với `UID` vừa tạo.
  - Xử lý các ngoại lệ (Exception) như: Trùng Email -> Trả về `409 Conflict`.
  - Trả về HTTP `201 Created` kèm dữ liệu tài khoản nếu thành công.

- [ ] **Tạo endpoint `PUT / PATCH /api/v1/accounts/{id}` (Cập nhật tài khoản)**:
  - Kiểm tra xem user có tồn tại hay không bằng hàm `GetUserAsync()`. Nếu không, trả về `404 Not Found`.
  - Nếu tồn tại, đồng bộ cập nhật trên Firebase Auth (`UpdateUserAsync`).
  - Hợp nhất dữ liệu (Merge) các thay đổi vào document trên Firestore.
  - Xử lý ngoại lệ bảo mật và hệ thống (500 Internal Server Error).
  - Trả về HTTP `200 OK` kèm theo dữ liệu đã cập nhật.

## 3. Tiêu chí nghiệm thu (Acceptance Criteria)
- **AC1**: Trả về `201 Created` và lưu DB thành công khi Request payload (Tạo mới) hợp lệ.
- **AC2**: Trả về `400 Bad Request` và thông báo lỗi rõ ràng của từng field nếu Validation thất bại.
- **AC3**: Trả về `200 OK` và lưu thông tin thành công khi cập nhật account đang tồn tại.
- **AC4**: Trả về `404 Not Found` nếu gọi API cập nhật cho một ID "ma" (không tồn tại trong Firebase).
- **AC5**: Format Response trả về (khi thành công và khi lỗi) phải chính xác với chuẩn JSON được định nghĩa trong tài liệu `BA.md`.
- **AC6**: Mã nguồn phải gọi đúng và đủ các phương thức tích hợp SDK của `FirebaseAdmin` và `Google.Cloud.Firestore`.
