# Nhiệm vụ: Xây dựng Chức năng Quản lý Tài khoản (Get, Lock & Unlock qua Firebase & Firestore)

**Mã Task**: T35.2  
**User Story**: US35 – Account Management & Cloud Sync (Phát triển chức năng quản lý tài khoản: lấy danh sách, chi tiết, khóa/mở khóa tài khoản đồng bộ trên Firebase)  
**Người được giao (Assignee)**: **Nguyễn Đức Mạnh**  
**Trạng thái**: Hoàn thành (Done)  
**Thời gian dự kiến (Estimate)**: 8 giờ  

---

## 1. Mục tiêu nhiệm vụ
Cung cấp các hàm dịch vụ trong tầng Core Service (`UserService`, `FirebaseSyncService`, `AuthenticationService`) cho phép Quản trị viên (Admin):
1. Lấy danh sách toàn bộ tài khoản và xem chi tiết một tài khoản (Get/GetById) kèm tìm kiếm và lọc đa tiêu chí (Vai trò, Trạng thái hoạt động, Lớp học).
2. **Khóa (Lock) hoặc Mở khóa (Unlock)** tài khoản người dùng:
   - **Đồng bộ Firebase Authentication**: Vô hiệu hóa (`disabled: true/false`) tài khoản trên Firebase Auth, khiến người dùng không thể lấy được JWT ID Token mới để thao tác.
   - **Đồng bộ Cloud Firestore**: Cập nhật trường trạng thái (`IsActive = false/true`, `Status = "Locked"/"Active"`) trên collection `users`.
   - Chặn Admin tự khóa tài khoản của chính mình để tránh lỗi khóa nhầm tài khoản quản trị tối cao.

---

## 2. Chi tiết các hạng mục thực hiện (Checklist)

### 2.1. Xây dựng DTO Models & Request/Response
- [x] **Tạo class `AccountDto` (`UserAccountDto`)**: Định nghĩa dữ liệu trả về khi truy vấn thông tin tài khoản (Id, Email, FullName, Role, Status, IsActive, IsDisabled, PhoneNumber, StudentCode, ClassName, v.v.).
- [x] **Tạo bộ lọc `AccountFilterCriteria`**: Chứa các tham số `SearchText`, `SelectedStatus`, `SelectedClass`, `SelectedAcademicYear`, `SelectedRole` và thuộc tính đếm `ActiveFilterCount`.

### 2.2. Triển khai Tầng Dịch vụ & Đồng bộ Firebase (`UserService` & `FirebaseSyncService`)
- [x] **Hàm `GetUsersAsync()` (Lấy danh sách)**:
  - Lấy danh sách tài khoản từ Cloud Firestore (hỗ trợ lọc theo `role`, `status`, tìm kiếm từ khóa họ tên/email/MSSV).
  - Trả về danh sách `User` được bọc trong đối tượng chuẩn hóa `ApiResponse<List<User>>`.
- [x] **Hàm `GetUserByIdAsync(string id)` (Lấy chi tiết)**:
  - Lấy thông tin chi tiết một tài khoản dựa trên UID.
  - Trả về HTTP `404 Not Found` nếu tài khoản không tồn tại.
- [x] **Hàm `LockAccountAsync(string id)` (Khóa tài khoản)**:
  - Vô hiệu hóa (`Disabled = true`) trên Firebase Authentication.
  - Cập nhật trường trạng thái (`Status = "Locked"`, `IsActive = false`) trên Cloud Firestore.
  - Trả về HTTP `200 OK` với `ApiResponse<User>`.
- [x] **Hàm `UnlockAccountAsync(string id)` (Mở khóa tài khoản)**:
  - Mở khóa (`Disabled = false`) trên Firebase Authentication.
  - Cập nhật trường trạng thái (`Status = "Active"`, `IsActive = true`) trên Firestore.
  - Trả về HTTP `200 OK` với `ApiResponse<User>`.

### 2.3. Xử lý nghiệp vụ & Bảo mật
- [x] **Kiểm tra phân quyền (Authorization)**: Chặn Admin tự khóa tài khoản của chính mình (`Admin cannot lock their own account`).
- [x] **Bảo vệ mật khẩu**: Áp dụng mã hóa RFC 2898 **PBKDF2-SHA256** (10.000 vòng + Salt 16 bytes ngẫu nhiên) và so sánh chuỗi bằng `CryptographicOperations.FixedTimeEquals` để chống Timing Attack.
- [x] **Tự động khóa sau 5 lần nhập sai**: Khóa tài khoản và trả về `ACCOUNT_LOCKED` (HTTP 403).

---

## 3. Tiêu chí Nghiệm thu (Acceptance Criteria)
- [x] **AC1**: Lấy danh sách tài khoản thành công trả về mã `200 OK` kèm danh sách tài khoản hợp lệ từ Cloud Firestore.
- [x] **AC2**: Khóa tài khoản thành công trả về `200 OK`. Sau khi khóa, tài khoản bị vô hiệu hóa trên Firebase Auth và `IsActive = false` trên Firestore; người dùng đăng nhập bị báo lỗi `ACCOUNT_LOCKED`.
- [x] **AC3**: Mở khóa tài khoản thành công trả về `200 OK`. Tài khoản phục hồi trạng thái hoạt động và có thể đăng nhập bình thường.
- [x] **AC4**: Trả về `404 Not Found` nếu thực hiện Lock/Unlock cho một ID không tồn tại.
- [x] **AC5**: Format Response trả về (thành công/thất bại) tuân thủ chuẩn JSON `ApiResponse<T>`.
- [x] **DoD1–DoD6**: Toàn bộ unit tests và UI component tests liên quan đến quản lý tài khoản đạt 100% Passed.
