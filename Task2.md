# Nhiệm vụ: Xây dựng API Quản lý Tài khoản (Get, Lock & Unlock)

**Mã Task**: T35.2
**User Story**: US35 – Account Management API (Phát triển API quản lý tài khoản: tạo, cập nhật, khóa/mở khóa tài khoản)
**Người được giao (Assignee)**: campus-worker-api & campus-worker-integration
**Trạng thái**: Hoàn thành (Done)
**Thời gian dự kiến (Estimate)**: 8 giờ

---

## 1. Mục tiêu nhiệm vụ
Xây dựng các RESTful API endpoints cho phép quản trị viên hệ thống có thể:
1. Lấy danh sách tài khoản và chi tiết một tài khoản (Get/GetById).
2. Khóa (Lock) hoặc Mở khóa (Unlock) tài khoản người dùng trong hệ thống. Việc khóa tài khoản được thực hiện bằng cách vô hiệu hóa (`disable`) trên Firebase Authentication và cập nhật trạng thái trên cơ sở dữ liệu Cloud Firestore.

## 2. Chi tiết các hạng mục cần thực hiện (Checklist)

### 2.1. Xây dựng DTO Models
- [x] **Tạo class `AccountDto` (`UserAccountDto`)**: Định nghĩa dữ liệu trả về khi truy vấn thông tin tài khoản (Id, Email, FullName, Role, Status, IsActive, IsDisabled, PhoneNumber, StudentCode, v.v.).
- [x] **Tạo hỗ trợ lọc `AccountFilterRequest` / Query Params**: Chứa các tham số `role`, `status`, `search` để tìm kiếm và lọc cho API lấy danh sách tài khoản.

### 2.2. Triển khai API Endpoints (AccountsController)
- [x] **Tạo endpoint `GET /api/v1/accounts` (Lấy danh sách)**:
  - Lấy danh sách tài khoản từ Cloud Firestore (hỗ trợ lọc theo `role`, `status`, tìm kiếm từ khóa).
  - Trả về danh sách `UserAccountDto` được bọc trong `ApiResponse<T>`.
- [x] **Tạo endpoint `GET /api/v1/accounts/{id}` (Lấy chi tiết)**:
  - Lấy thông tin chi tiết một tài khoản dựa trên `id` (UID).
  - Trả về HTTP `404 Not Found` nếu tài khoản không tồn tại.
- [x] **Tạo endpoint `PATCH /api/v1/accounts/{id}/lock` (Khóa tài khoản)**:
  - Hỗ trợ cả `PATCH` và `POST`.
  - Vô hiệu hóa (`Disabled = true`) trên Firebase Authentication.
  - Cập nhật trường trạng thái (`Status = "Locked"`, `IsActive = false`) trên Cloud Firestore.
  - Trả về HTTP `200 OK` với `AccountStatusResponse`.
- [x] **Tạo endpoint `PATCH /api/v1/accounts/{id}/unlock` (Mở khóa tài khoản)**:
  - Hỗ trợ cả `PATCH` và `POST`.
  - Mở khóa (`Disabled = false`) trên Firebase Authentication.
  - Cập nhật trường trạng thái (`Status = "Active"`, `IsActive = true`) trên Firestore.
  - Trả về HTTP `200 OK` với `AccountStatusResponse`.

### 2.3. Xử lý nghiệp vụ & Bảo mật
- [x] **Kiểm tra phân quyền (Authorization)**: Chặn Admin tự khóa tài khoản của chính mình (`Admin cannot lock their own account`).
- [x] **Tính đồng nhất dữ liệu**: Đồng bộ giữa Firebase Auth và Firestore; log lỗi chi tiết; trả về chuẩn RFC 7807 / `ApiResponse<T>`.

## 3. Tiêu chí nghiệm thu (Acceptance Criteria)
- [x] **AC1**: Gọi GET `/api/v1/accounts` trả về mã `200 OK` kèm danh sách tài khoản hợp lệ.
- [x] **AC2**: Gọi PATCH/POST để khóa tài khoản thành công trả về `200 OK`. Sau khi khóa, tài khoản bị vô hiệu hóa trên Firebase Auth và Firestore.
- [x] **AC3**: Gọi PATCH/POST để mở khóa tài khoản thành công trả về `200 OK`. Người dùng có thể đăng nhập bình thường.
- [x] **AC4**: Trả về `404 Not Found` nếu thực hiện Lock/Unlock cho một ID không tồn tại.
- [x] **AC5**: Format Response trả về (thành công/thất bại) tuân thủ chuẩn JSON `ApiResponse<T>`.
- [x] **DoD1–DoD6**: Toàn bộ 17 unit test trong `CampusActivitiesManager.Api.Tests` đạt 100%.
