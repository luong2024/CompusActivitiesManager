# Nhiệm vụ Sprint 03: Quản lý Sự kiện Nâng cao, Tạo & Quét mã QR Thực tế, Điểm danh bằng Hình ảnh Minh chứng

**Mã Task**: T17.1 / T21.1 / T13.1  
**User Story**: 
- **US13**: Event Management (Tạo, cập nhật, lịch trình, số lượng tham gia, trạng thái sự kiện trên Firebase)
- **US17**: QR Attendance (Tạo vé QR thực tế quét được bằng Camera phần cứng điện thoại di động)
- **US21 / US23**: Photo Attendance (Điểm danh bằng hình ảnh minh chứng chụp từ camera thực tế & quy trình phê duyệt)
- **US01 / US03 / US11**: Event Details (Xem chi tiết toàn diện sự kiện, diễn giả, đếm ngược, vị trí)

**Người được giao (Assignee)**: **Nguyễn Đức Mạnh**  
**Trạng thái**: Hoàn thành (Done)  
**Thời gian dự kiến (Estimate)**: 16 giờ  

---

## 1. Mục tiêu Sprint 03
1. **Tạo mã QR Vé điện tử thực tế (Real Scannable QR Code)**: Tích hợp thư viện `QRCoder` tạo mã QR độ phân giải cao dạng ảnh PNG/Stream, chứa dữ liệu vé điện tử chuẩn hóa (`CAMPUS-ACT:EVT-{id}:STU-{code}:{timestamp}`). Bất kỳ camera điện thoại hoặc app nào cũng có thể quét và nhận diện được nội dung.
2. **Quét mã QR Bằng Camera Thật (`ZXing.Net.Maui.Controls 0.6.0`)**: Tích hợp phần cứng camera di động với bộ chuyển đổi camera trước/sau, bật/tắt đèn Flash, bộ lọc Debounce 2.5s chống quét lặp mã vé và tự động ghi nhận điểm danh lên Firebase Firestore.
3. **Điểm danh bằng Hình ảnh minh chứng Thực tế (Photo Attendance)**: Kích hoạt `MediaPicker.Default.CapturePhotoAsync()` cho phép sinh viên chụp ảnh minh chứng tại hội trường, chuyển đổi trạng thái tức thì sang Thẻ minh chứng (Trạng thái A: Đã điểm danh kèm tem thời gian và huy hiệu phê duyệt).
4. **Trang Chi tiết Sự kiện (Event Detail View - US01/US03)**: Trình bày chi tiết sự kiện với ảnh bìa, thời gian đếm ngược, thông tin diễn giả, địa điểm, sức chứa và nút thao tác nhanh (Đăng ký, Xem vé QR, Điểm danh ảnh).

---

## 2. Chi tiết các hạng mục thực hiện (Checklist)

### 2.1. Tầng Dịch vụ & Đồng bộ Firebase (`BusinessLogic/Services`)
- [x] Xây dựng DTO Models: `QrCheckInRequest`, `PhotoCheckInRequest`, `AttendanceRecordDto`, `ManualAttendanceRequest`.
- [x] Xây dựng `IAttendanceService` & `AttendanceService`:
  - `CheckInWithQrCodeAsync()`: Điểm danh qua mã QR, kiểm tra chống trùng lặp, cộng điểm rèn luyện và lưu vào Cloud Firestore collection `attendances`.
  - `SubmitPhotoAttendanceAsync()`: Điểm danh bằng ảnh chụp minh chứng kèm ghi chú và mốc thời gian ISO 8601.
  - `GetEventAttendanceListAsync()`: Lấy danh sách điểm danh thời gian thực theo sự kiện cho ban tổ chức.
  - `GetStudentAttendanceHistoryAsync()`: Lấy lịch sử điểm danh và tính tổng điểm rèn luyện tích lũy.
  - `ManualCheckInAsync()`: Điểm danh thủ công cho sinh viên không quét được (dành cho Admin/Manager).
- [x] Xây dựng `IQrCodeService` & `QrCodeService`: hàm sinh ảnh QR thật trả về `ImageSource` từ chuỗi vé bảo mật.

### 2.2. Giao diện Người dùng (.NET MAUI MVVM)
- [x] **Vé QR Điện tử**: Hiển thị ảnh QR thực tế sinh động, thông tin mã vé, hướng dẫn đưa mã cho ban tổ chức quét.
- [x] **Màn hình Quét QR (`QrScannerPage.xaml`)**: Khung ngắm camera trực tiếp qua `zxing:CameraBarcodeReaderView`, nút bật/tắt Flash, nút đổi camera trước/sau.
- [x] **Màn hình Điểm danh bằng Hình ảnh (`PhotoAttendancePage.xaml`)**: Chụp ảnh trực tiếp từ camera, xem trước ảnh thumbnail, hiển thị mốc thời gian và nút gửi xác nhận điểm danh (loại bỏ hoàn toàn nút test ảo).
- [x] **Màn hình Chi tiết Sự kiện (`EventDetailPage.xaml`)**: Thẻ thông tin đầy đủ, đồng hồ đếm ngược, quyền lợi điểm rèn luyện, liên kết mở vé QR và điểm danh ảnh.

---

## 3. Tiêu chí Nghiệm thu (Acceptance Criteria)
- [x] **AC 17.1 (Tạo mã QR thật)**: Mã QR hiển thị rõ ràng trên màn hình, quét bằng Camera iPhone/Android đều đọc ra đúng định dạng vé sự kiện.
- [x] **AC 17.2 (Quét điểm danh thành công)**: Khi quét đúng mã QR sự kiện, hệ thống phản hồi `200 OK`, hiển thị popup chúc mừng điểm danh thành công và cộng điểm rèn luyện tương ứng.
- [x] **AC 21.1 (Điểm danh bằng ảnh)**: Chụp được ảnh trực tiếp từ camera thiết bị; hiển thị preview ảnh đầy đủ; chuyển sang trạng thái đã điểm danh có ảnh minh chứng.
- [x] **AC 13.1 (Xem chi tiết sự kiện)**: Hiển thị đầy đủ thông tin: Tên, Mô tả, Thời gian, Địa điểm, Sức chứa, Diễn giả và nút đăng ký/xem vé.
- [x] **DoD1–DoD6**: Tuân thủ bảng màu Xanh Dương (`#2563EB`), vượt qua 100% unit tests, không có warning biên dịch, đồng bộ dữ liệu chuẩn mực.
