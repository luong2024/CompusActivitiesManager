# Nhiệm vụ: Xây dựng API Quản lý Tài khoản (Get, Lock & Unlock)

**Mã Task**: T35.2
**User Story**: US35 – Account Management API (Phát triển API quản lý tài khoản: tạo, cập nhật, khóa/mở khóa tài khoản)
**Người được giao (Assignee)**: (Chưa phân công)
**Trạng thái**: Cần thực hiện (To-Do)
**Thời gian dự kiến (Estimate)**: 8 giờ

---

## 1. Mục tiêu nhiệm vụ
Xây dựng các RESTful API endpoints cho phép quản trị viên hệ thống có thể:
1. Lấy danh sách tài khoản và chi tiết một tài khoản (Get/GetById).
2. Khóa (Lock) hoặc Mở khóa (Unlock) tài khoản người dùng trong hệ thống. Việc khóa tài khoản cần được thực hiện bằng cách vô hiệu hóa (`disable`) trên Firebase Authentication và cập nhật trạng thái trên cơ sở dữ liệu Cloud Firestore.

## 2. Chi tiết các hạng mục cần thực hiện (Checklist)

### 2.1. Xây dựng DTO Models
- [ ] **Tạo class `AccountDto`**: Định nghĩa dữ liệu trả về khi truy vấn thông tin tài khoản (Id, Email, FullName, Role, Status, v.v.).
- [ ] **Tạo class `AccountFilterRequest`**: (Tùy chọn) Chứa các tham số để phân trang và tìm kiếm cho API lấy danh sách tài khoản.

### 2.2. Triển khai API Endpoints (AccountsController)
- [ ] **Tạo endpoint `GET /api/v1/accounts` (Lấy danh sách)**:
  - Lấy danh sách tài khoản từ Cloud Firestore (có hỗ trợ phân trang và tìm kiếm cơ bản).
  - Trả về danh sách `AccountDto` được bọc trong `ApiResponse<T>`.
- [ ] **Tạo endpoint `GET /api/v1/accounts/{id}` (Lấy chi tiết)**:
  - Lấy thông tin chi tiết một tài khoản dựa trên `id` (UID).
  - Trả về HTTP `404 Not Found` nếu tài khoản không tồn tại.
- [ ] **Tạo endpoint `PATCH /api/v1/accounts/{id}/lock` (Khóa tài khoản)**:
  - Gọi `FirebaseAuth.DefaultInstance.UpdateUserAsync()` với cờ `Disabled = true` để vô hiệu hóa đăng nhập trên Firebase.
  - Cập nhật trường trạng thái (ví dụ: `Status = "Locked"`) trong collection `users` trên Firestore.
  - Trả về HTTP `200 OK` nếu thao tác thành công.
- [ ] **Tạo endpoint `PATCH /api/v1/accounts/{id}/unlock` (Mở khóa tài khoản)**:
  - Gọi `FirebaseAuth.DefaultInstance.UpdateUserAsync()` với cờ `Disabled = false` để cho phép đăng nhập lại trên Firebase.
  - Cập nhật trường trạng thái (`Status = "Active"`) trên Firestore.
  - Trả về HTTP `200 OK` nếu thao tác thành công.

### 2.3. Xử lý nghiệp vụ & Bảo mật
- [ ] **Kiểm tra phân quyền (Authorization)**: Đảm bảo chỉ có role `Admin` mới có quyền gọi các API quản lý, lock/unlock và xem danh sách tài khoản này.
- [ ] **Tính đồng nhất dữ liệu**: Cố gắng đảm bảo việc gọi Firebase Auth và cập nhật Firestore đồng bộ nhất có thể. Xử lý lỗi `try-catch` chặt chẽ, log lỗi chi tiết nếu một trong hai thao tác thất bại.

## 3. Tiêu chí nghiệm thu (Acceptance Criteria)
- **AC1**: Gọi GET `/api/v1/accounts` trả về mã `200 OK` kèm danh sách tài khoản hợp lệ.
- **AC2**: Gọi PATCH để khóa tài khoản thành công trả về `200 OK`. Sau khi khóa, người dùng đó không thể sử dụng token để đăng nhập hoặc truy cập tài nguyên hệ thống.
- **AC3**: Gọi PATCH để mở khóa tài khoản thành công trả về `200 OK`. Người dùng có thể đăng nhập bình thường.
- **AC4**: Trả về `404 Not Found` nếu thực hiện Lock/Unlock cho một ID không tồn tại.
- **AC5**: Format Response trả về (thành công/thất bại) phải tuân thủ chuẩn JSON `ApiResponse<T>` hoặc RFC 7807 (như đã quy định trong T35.1).
