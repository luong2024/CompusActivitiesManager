# Nhiệm vụ Sprint 04: Báo cáo Điểm danh Thời gian thực, Lịch sử & Điểm Rèn luyện, Kênh Hỗ trợ & Thông báo, Tối ưu Hoàn thiện Dự án

**Mã Task**: T18.1 / T19.1 / T20.1 / T22.1 / T04.1  
**User Story**: 
- **US18**: Real-time Attendance Confirmation & Toast / Push Notifications (Thông báo xác nhận điểm danh và cập nhật điểm rèn luyện tức thì)
- **US19**: Attendance History & Training Points Ledger (Xem lịch sử tham gia sự kiện và thống kê điểm rèn luyện tích lũy)
- **US20 / US22**: Real-time Attendees Monitoring & Attendance Report Export (Danh sách người tham dự thời gian thực cho Ban tổ chức & Xuất báo cáo CSV/Excel/PDF)
- **US04 / US08 / US10**: Notification Center & Support/Feedback Channel (Hộp thư thông báo sự kiện và kênh gửi phản hồi/hỗ trợ sinh viên)
- **Polish & Final DoD**: Tối ưu UI/UX bảng màu Xanh Dương chuẩn Microsoft Fluent, Dark/Light mode, hiệu ứng chuyển động và kiểm thử toàn diện.

**Người được giao (Assignees)**:
- `campus-worker-api`: API xuất báo cáo điểm danh (`GET /export`), API thông báo (`/notifications`), API gửi hỗ trợ (`/feedback`).
- `campus-worker-ui`: Thiết kế màn hình Lịch sử Điểm danh & Điểm rèn luyện, Màn hình Báo cáo Điểm danh & Xuất file cho Ban tổ chức, Trung tâm Thông báo.
- `campus-worker-integration`: Tích hợp Shell Navigation, kết nối các luồng thông báo, tính toán tổng hợp điểm rèn luyện, xuất file CSV.
- `campus-worker-qa`: Bổ sung kiểm thử tự động, xác minh bảo mật và đảm bảo toàn bộ hệ thống đạt chuẩn nghiệm thu DoD1–DoD6.

**Trạng thái**: In Progress  
**Thời gian dự kiến (Estimate)**: 16 giờ  

---

## 1. Mục tiêu Sprint 04
1. **Lịch sử Điểm danh & Điểm Rèn luyện (US19)**: Cung cấp màn hình trực quan cho sinh viên theo dõi toàn bộ các sự kiện đã tham gia, hình thức điểm danh (Mã QR, Ảnh minh chứng, Điểm danh thủ công), thời gian chính xác và tổng số điểm rèn luyện tích lũy đạt được.
2. **Dashboard Giám sát Điểm danh Thời gian thực & Xuất Báo cáo (US20 / US22)**:
   - Dành cho Ban tổ chức/Admin: Theo dõi số lượng sinh viên đã check-in theo thời gian thực (tỷ lệ tham dự so với đăng ký).
   - Xuất báo cáo danh sách điểm danh ra định dạng CSV/Text/Excel để phục vụ lưu trữ học vụ và đánh giá mức độ tương tác.
3. **Trung tâm Thông báo & Phản hồi Hỗ trợ (US04 / US08)**: Hiển thị các thông báo nhắc nhở sự kiện sắp diễn ra, thông báo cộng điểm rèn luyện sau điểm danh; form gửi câu hỏi hỗ trợ trực tiếp tới Admin.
4. **Hoàn thiện & Đánh bóng Toàn diện (Project Polish & DoD)**:
   - Chuẩn hóa toàn bộ giao diện theo Bảng màu 1 — Xanh Dương (`#2563EB`) hiện đại, chuyên nghiệp.
   - Hỗ trợ hoàn hảo Dark Theme / Light Theme.
   - Đảm bảo 100% không còn warning hoặc error khi biên dịch trên cả Android, Windows, iOS.

---

## 2. Chi tiết các hạng mục cần thực hiện (Checklist)

### 2.1. Backend API (`CampusActivitiesManager.Api`)
- [ ] Bổ sung endpoint xuất báo cáo: `GET /api/v1/attendance/event/{eventId}/export` (xuất file CSV danh sách sinh viên tham dự).
- [ ] Xây dựng `NotificationsController`: `GET /api/v1/notifications` và `POST /api/v1/notifications/mark-read`.
- [ ] Xây dựng `FeedbackController`: `POST /api/v1/feedback` (tiếp nhận câu hỏi/góp ý của sinh viên).
- [ ] Bổ sung Unit Tests kiểm tra các endpoint báo cáo và thông báo trong `CampusActivitiesManager.Api.Tests`.

### 2.2. Dịch vụ & Xử lý Core (`CampusActivitiesManager`)
- [ ] Xây dựng `INotificationService` & `NotificationService`: quản lý thông báo sự kiện cục bộ và server.
- [ ] Cập nhật `IAttendanceService` bổ sung hàm `ExportAttendanceReportAsync(int eventId)`.
- [ ] Cập nhật `AppShell.xaml` và `AppShell.xaml.cs` bổ sung route và menu item: Lịch sử điểm danh, Báo cáo sự kiện, Thông báo.

### 2.3. Giao diện Người dùng (.NET MAUI MVVM)
- [ ] **Màn hình Lịch sử Điểm danh (`AttendanceHistoryPage`)**: Thẻ thống kê điểm rèn luyện (Progress bar, xếp loại Xuất sắc/Tốt), danh sách sự kiện đã tham gia có icon phương thức điểm danh.
- [ ] **Màn hình Báo cáo Điểm danh Sự kiện (`EventAttendanceReportPage`)**: Bộ lọc trạng thái điểm danh, danh sách sinh viên check-in kèm ảnh (nếu có), nút "Xuất Báo Cáo CSV".
- [ ] **Màn hình Hỗ trợ & Thông báo (`NotificationCenterPage` / Modal Hỗ trợ)**: Danh sách thông báo mới, badge chưa đọc, form gửi câu hỏi tới ban quản trị.
- [ ] Tích hợp trọn vẹn vào `MainPage` (Cổng Sinh viên) và `AppShell` (Menu Flyout).

---

## 3. Tiêu chí Nghiệm thu (Acceptance Criteria)
- **AC 18.1 (Thông báo tức thì)**: Sau khi quét QR hoặc chụp ảnh điểm danh, thông báo Toast/Alert xuất hiện xác nhận số điểm rèn luyện được cộng.
- **AC 19.1 (Xem lịch sử điểm danh)**: Hiển thị danh sách sự kiện đã tham gia với đầy đủ ngày giờ, phương thức và số điểm rèn luyện. Điểm rèn luyện tổng được cộng dồn chính xác.
- **AC 20.1 (Giám sát thời gian thực)**: Ban tổ chức xem được danh sách sinh viên đã check-in, số lượng đã tham gia / tổng số đăng ký.
- **AC 22.1 (Xuất báo cáo)**: Bấm "Xuất Báo Cáo" tạo ra file CSV hoặc hiển thị bảng dữ liệu tổng kết với đầy đủ thông tin sinh viên, thời gian check-in.
- **AC 04.1 (Hộp thư thông báo)**: Hiển thị các thông báo hệ thống và sự kiện sắp diễn ra rõ ràng, hỗ trợ đánh dấu đã đọc.
- **DoD1–DoD6**: Tuân thủ chuẩn MVVM, 0 warning / 0 error trên mọi target, 100% test pass.
