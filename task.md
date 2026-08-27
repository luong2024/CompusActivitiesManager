# Chi Tiết Nhiệm Vụ: Quản lý tài khoản (API)

**Mã Task**: T35.1 - Create Account + Update Account API
**User Story**: US35 – Account Management API
**Người thực hiện**: Nguyễn Đức Mạnh
**Trạng thái**: Đã hoàn thành (Done)

## 1. Mục tiêu nhiệm vụ
Xây dựng một hệ thống RESTful API an toàn, đáp ứng tiêu chuẩn để cho phép các đối tượng có thẩm quyền (như Quản trị viên) có thể tạo mới (Create) và cập nhật (Update) tài khoản người dùng, đồng thời tự động đồng bộ hóa xác thực với Firebase Authentication và lưu trữ siêu dữ liệu trên Cloud Firestore.

## 2. Chi tiết các hạng mục công việc đã thực hiện

### 2.1. Khởi tạo & Cấu hình Project
- [x] Khởi tạo dự án `CampusActivitiesManager.Api` dưới dạng ASP.NET Core Web API (sử dụng .NET 9).
- [x] Cài đặt các thư viện cần thiết: `FirebaseAdmin` và `Google.Cloud.Firestore`.
- [x] Thiết lập cấu hình Middleware tại `Program.cs` (Đăng ký Swagger, tắt tự động trả lỗi HTTP 400 mặc định của Framework để tùy chỉnh cấu trúc trả lỗi).
- [x] Tích hợp cơ chế xác thực an toàn thông qua biến môi trường `GOOGLE_APPLICATION_CREDENTIALS` (Firebase Service Account).

### 2.2. Chuẩn hóa Model & Kiểm duyệt dữ liệu (Validation)
- [x] Thiết kế **CreateAccountRequest**: Áp dụng Data Annotations để validate tính hợp lệ của Email, cấu trúc Password mạnh (Regex yêu cầu chữ hoa, thường, số, ký tự đặc biệt, tối thiểu 8 ký tự), tính bắt buộc của FullName, và chỉ định Role cụ thể (Admin, Lecturer, Student).
- [x] Thiết kế **UpdateAccountRequest**: Khai báo các trường dữ liệu tùy chọn (Nullable) nhưng yêu cầu tính hợp lệ nghiêm ngặt nếu Client có truyền dữ liệu lên.
- [x] Thiết kế **Cấu trúc Response Chuẩn**: Xây dựng class `ApiResponse<T>` (cho phản hồi thành công) và `ApiErrorResponse` (cho các phản hồi lỗi theo chuẩn RFC 7807) để API luôn trả về cấu trúc đồng nhất có thuộc tính `success`, `statusCode`, `message` và `data/errors`.

### 2.3. Triển khai API Endpoints (AccountsController)
- [x] **API Tạo tài khoản (`POST /api/v1/accounts`)**:
  - Phân tích và thực thi Validate Request Model.
  - Sử dụng lệnh `FirebaseAuth.DefaultInstance.CreateUserAsync()` để cấp phát tài khoản trên Firebase.
  - Sử dụng `FirestoreDb` để lưu trữ các thông tin mở rộng (vai trò, số điện thoại, mã sinh viên) vào collection `users` tương ứng với `UID` vừa tạo.
  - Xử lý chặn lỗi Trùng Email (trả về mã HTTP `409 Conflict`) hoặc Lỗi không lường trước (`500 Internal Server Error`).
- [x] **API Cập nhật tài khoản (`PUT/PATCH /api/v1/accounts/{id}`)**:
  - Tra cứu sự tồn tại của người dùng bằng `GetUserAsync()`. Bắt lỗi và trả về HTTP `404 Not Found` nếu ID truyền lên không hợp lệ.
  - Đồng bộ cập nhật thông tin trên Firebase Auth (`UpdateUserAsync`).
  - Hợp nhất (Merge) các trường dữ liệu mới (nếu có) trên document của Firestore thông qua `SetOptions.MergeAll`.
  - Phản hồi mã HTTP `200 OK` đi kèm thông tin cập nhật mới nhất cho Client.

## 3. Đối chiếu Acceptance Criteria (Kịch bản Nghiệm thu)
- **[AC1]** Đã xử lý API cho phép tạo account với dữ liệu hợp lệ -> Trả về mã `201 Created`.
- **[AC2]** Đã cài đặt cơ chế Validation chặn Request không đủ field, sai format -> Trả về mã `400 Bad Request` kèm theo chi tiết từng trường (field) bị lỗi.
- **[AC3]** Đã xử lý API cập nhật thông tin cho account hiện có -> Trả về mã `200 OK` và lưu thành công.
- **[AC4]** Đã chặn hoàn toàn trường hợp update account không tồn tại -> Trả về mã `404 Not Found`.
- **[AC5]** Đã đảm bảo định dạng JSON Response đầu ra bám sát đặc tả của quy trình nghiệp vụ (Business Analysis).
- **[AC6]** Đã làm chủ hoàn toàn các thao tác tích hợp SDK của Firebase.
