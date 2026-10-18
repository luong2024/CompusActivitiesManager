# Nhiệm vụ Sprint 03: Quản lý Sự kiện Nâng cao, Tạo & Quét mã QR Thực tế, Điểm danh bằng Hình ảnh

**Mã Task**: T17.1 / T21.1 / T13.1  
**User Story**: 
- **US13**: Event Management (Tạo, cập nhật, lịch trình, số lượng tham gia, trạng thái sự kiện)
- **US17**: QR Attendance (Tạo vé QR thực tế quét được bằng điện thoại & Máy quét QR điểm danh)
- **US21 / US23**: Photo Attendance (Điểm danh bằng hình ảnh minh chứng/chụp ảnh từ camera thực tế)
- **US01 / US03 / US11**: Event Details (Xem chi tiết toàn diện sự kiện, diễn giả, đếm ngược, vị trí)

**Người được giao (Assignees)**:
- `campus-worker-api`: Xây dựng `AttendanceController`, `AttendanceService` (API QR Check-in, Photo Check-in, Real-time List).
- `campus-worker-ui`: Thiết kế giao diện Vé QR điện tử, Màn hình Quét QR, Màn hình Điểm danh bằng hình ảnh (MediaPicker Camera).
- `campus-worker-integration`: Tích hợp thư viện `QRCoder` sinh mã QR thật, kết nối API điểm danh, lưu trữ cục bộ & Firebase.
- `campus-worker-qa`: Viết & chạy unit test cho các endpoint điểm danh (100% pass), xác minh DoD1–DoD6.

**Trạng thái**: In Progress  
**Thời gian dự kiến (Estimate)**: 16 giờ  

---

## 1. Mục tiêu Sprint 03
1. **Tạo mã QR Vé điện tử thực tế (Real Scannable QR Code)**: Tích hợp thư viện `QRCoder` tạo mã QR độ phân giải cao dạng ảnh PNG/Stream, chứa dữ liệu vé điện tử chuẩn hóa (`CAMPUS-ACT:EVT-{id}:STU-{code}:{timestamp}`). Bất kỳ camera điện thoại hoặc app nào cũng có thể quét và nhận diện được nội dung.
2. **Quét mã QR & Điểm danh Thực tế (QR Attendance Scanner)**: Cung cấp giao diện quét mã QR chuyên nghiệp, hỗ trợ quét qua Camera / Thư viện ảnh hoặc nhập mã nhanh, lập tức gửi request xác thực lên API và cập nhật trạng thái tham gia.
3. **Điểm danh bằng Hình ảnh minh chứng (Photo-based Attendance Check-in)**: Tích hợp `MediaPicker.Default.CapturePhotoAsync()` cho phép sinh viên chụp ảnh selfie / ảnh tham dự tại sự kiện, xem trước ảnh, nhập ghi chú và gửi lên hệ thống để ban tổ chức đối soát.
4. **Trang Chi tiết Sự kiện (Event Detail View - US01/US03)**: Trình bày chi tiết sự kiện với ảnh bìa, thời gian đếm ngược, thông tin diễn giả, địa điểm, sức chứa và nút thao tác nhanh (Đăng ký, Xem vé QR, Điểm danh ảnh).

---

## 2. Chi tiết các hạng mục cần thực hiện (Checklist)

### 2.1. Backend API (`CampusActivitiesManager.Api`)
- [ ] Xây dựng DTO Models: `QrCheckInRequest`, `PhotoCheckInRequest`, `AttendanceRecordDto`, `ManualAttendanceRequest`.
- [ ] Xây dựng `AttendanceController`:
  - `POST /api/v1/attendance/qr-checkin`: Điểm danh qua mã QR, kiểm tra trùng lặp, cộng điểm rèn luyện và lưu vào Firebase / CSDL.
  - `POST /api/v1/attendance/photo-checkin`: Điểm danh bằng ảnh chụp minh chứng kèm ghi chú và mốc thời gian.
  - `GET /api/v1/attendance/event/{eventId}`: Lấy danh sách điểm danh thời gian thực theo sự kiện cho ban tổ chức.
  - `GET /api/v1/attendance/student/{studentCode}`: Lấy lịch sử điểm danh của sinh viên.
  - `POST /api/v1/attendance/manual`: Điểm danh thủ công cho sinh viên không quét được (dành cho Admin/Manager).
- [ ] Viết unit tests kiểm thử toàn bộ luồng điểm danh trong `CampusActivitiesManager.Api.Tests`.

### 2.2. Dịch vụ & Xử lý Core (`CampusActivitiesManager`)
- [ ] Cài đặt và cấu hình thư viện `QRCoder` sinh ảnh QR PNG thực tế.
- [ ] Xây dựng `IQrCodeService` & `QrCodeService`: hàm sinh ảnh QR thật trả về `ImageSource` từ chuỗi vé.
- [ ] Xây dựng `IAttendanceService` & `AttendanceService`: kết nối API điểm danh, hỗ trợ fallback offline SQLite.
- [ ] Đăng ký dịch vụ trong `MauiProgram.cs` và cấu hình Dependency Injection.

### 2.3. Giao diện Người dùng (.NET MAUI MVVM)
- [ ] **Vé QR Điện tử**: Hiển thị ảnh QR thực tế sinh động, thông tin mã vé, hướng dẫn đưa mã cho ban tổ chức quét.
- [ ] **Màn hình Quét QR (`QrScannerPage`)**: Giao diện khung ngắm hiện đại, hiệu ứng tia quét laser, hỗ trợ quét ảnh và nhập nhanh.
- [ ] **Màn hình Điểm danh bằng Hình ảnh (`PhotoAttendancePage`)**: Cho phép kích hoạt Camera chụp ảnh thật hoặc chọn ảnh từ thư viện, xem trước ảnh thumbnail, hiển thị mốc thời gian và nút "Gửi xác nhận điểm danh".
- [ ] **Màn hình Chi tiết Sự kiện (`EventDetailPage`)**: Thẻ thông tin đầy đủ, đồng hồ đếm ngược, quyền lợi điểm rèn luyện, liên kết mở vé QR và điểm danh ảnh.

---

## 3. Tiêu chí Nghiệm thu (Acceptance Criteria)
- **AC 17.1 (Tạo mã QR thật)**: Mã QR hiển thị rõ ràng trên màn hình, quét bằng Camera iPhone/Android đều đọc ra đúng định dạng vé sự kiện.
- **AC 17.2 (Quét điểm danh thành công)**: Khi quét đúng mã QR sự kiện, hệ thống phản hồi `200 OK`, hiển thị popup chúc mừng điểm danh thành công và cộng điểm rèn luyện tương ứng.
- **AC 21.1 (Điểm danh bằng ảnh)**: Chụp được ảnh trực tiếp từ camera thiết bị hoặc chọn ảnh từ bộ sưu tập; hiển thị preview ảnh đầy đủ; gửi thành công lên API kèm trạng thái "Đã ghi nhận điểm danh có ảnh minh chứng".
- **AC 13.1 (Xem chi tiết sự kiện)**: Hiển thị đầy đủ thông tin: Tên, Mô tả, Thời gian, Địa điểm, Sức chứa, Diễn giả và nút đăng ký/xem vé.
- **DoD1–DoD6**: Tuân thủ bảng màu Xanh Dương (`#2563EB`), vượt qua 100% unit tests, không có warning biên dịch, mock data phong phú.
