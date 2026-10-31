# Nhiệm vụ Sprint 04: Báo cáo Điểm danh Thời gian thực, Lịch sử & Điểm Rèn luyện, Tối ưu Modal Thông báo, Hoàn thiện Toàn diện Dự án

**Mã Task**: T18.1 / T19.1 / T20.1 / T22.1 / T04.1  
**User Story**: 
- **US18**: Real-time Attendance Confirmation & Toast / Push Notifications (Thông báo xác nhận điểm danh và cập nhật điểm rèn luyện tức thì)
- **US19**: Attendance History & Training Points Ledger (Xem lịch sử tham gia sự kiện và thống kê điểm rèn luyện tích lũy)
- **US20 / US22**: Real-time Attendees Monitoring & Attendance Report Export (Danh sách người tham dự thời gian thực cho Ban tổ chức & Xuất báo cáo CSV)
- **US04 / US08 / US10**: Notification Center & Support Modal (Hộp thư thông báo sự kiện và kênh gửi phản hồi/hỗ trợ sinh viên)
- **Polish & Final DoD**: Tối ưu UI/UX bảng màu Xanh Dương chuẩn Microsoft Fluent, Dark/Light mode, sửa lỗi layout Modal thông báo và kiểm thử toàn diện.

**Người được giao (Assignee)**: **Nguyễn Đức Mạnh**  
**Trạng thái**: Hoàn thành (Done)  
**Thời gian dự kiến (Estimate)**: 16 giờ  

---

## 1. Mục tiêu Sprint 04
1. **Lịch sử Điểm danh & Điểm Rèn luyện (US19)**: Cung cấp màn hình trực quan cho sinh viên theo dõi toàn bộ các sự kiện đã tham gia, hình thức điểm danh (Mã QR, Ảnh minh chứng, Điểm danh thủ công), thời gian chính xác và tổng số điểm rèn luyện tích lũy đạt được.
2. **Dashboard Giám sát Điểm danh Thời gian thực & Xuất Báo cáo (US20 / US22)**:
   - Dành cho Ban tổ chức/Admin: Theo dõi số lượng sinh viên đã check-in theo thời gian thực trên Cloud Firestore.
   - Xuất báo cáo danh sách điểm danh ra định dạng CSV UTF-8 để phục vụ lưu trữ học vụ và đánh giá mức độ tương tác.
3. **Trung tâm Thông báo & Tối ưu Modal (US18 / US04 / T04.1)**:
   - Tái cấu trúc layout Modal thông báo: bố cục Grid 2 tầng chống đè huy hiệu chưa đọc, giới hạn chiều cao `ScrollView HeightRequest="250"`, ghim cố định nút Đóng ở hàng dưới cùng (`Grid.Row="4"`).
   - Tự động hiển thị các thông báo nhắc nhở sự kiện sắp diễn ra, thông báo cộng điểm rèn luyện sau điểm danh.
4. **Hoàn thiện & Đánh bóng Toàn diện (Project Polish & DoD)**:
   - Chuẩn hóa toàn bộ giao diện theo Bảng màu 1 — Xanh Dương (`#2563EB`) hiện đại, chuyên nghiệp.
   - Hỗ trợ hoàn hảo Dark Theme / Light Theme.
   - Đảm bảo 100% không còn warning hoặc error khi biên dịch trên cả Android và Windows.

---

## 2. Chi tiết các hạng mục thực hiện (Checklist)

### 2.1. Tầng Dịch vụ & Đồng bộ Firebase (`BusinessLogic/Services`)
- [x] Xây dựng `INotificationService` & `NotificationService`: quản lý thông báo sự kiện, đếm số chưa đọc, đánh dấu đã đọc trên Firestore.
- [x] Cập nhật `IAttendanceService` bổ sung hàm `ExportAttendanceReportAsync(int eventId)` (xuất file CSV danh sách sinh viên tham dự có BOM UTF-8).
- [x] Tích hợp Shell Navigation, kết nối các luồng thông báo, tính toán tổng hợp điểm rèn luyện.

### 2.2. Giao diện Người dùng (.NET MAUI MVVM)
- [x] **Màn hình Lịch sử Điểm danh (`AttendanceHistoryPage.xaml`)**: Thẻ thống kê điểm rèn luyện (Progress bar, xếp loại Xuất sắc/Tốt), danh sách sự kiện đã tham gia có icon phương thức điểm danh.
- [x] **Màn hình Báo cáo Điểm danh Sự kiện (`EventAttendanceReportPage.xaml`)**: Bộ lọc trạng thái điểm danh, danh sách sinh viên check-in kèm ảnh (nếu có), nút "Xuất Báo Cáo CSV".
- [x] **Modal Hỗ trợ & Thông báo trên `MainPage.xaml`**: Layout 2 tầng độc lập, badge số lượng chưa đọc không bị che khuất, thanh cuộn giới hạn độ cao và nút Đóng độc lập cố định ở chân modal.
- [x] Tích hợp trọn vẹn vào `MainPage` (Cổng Sinh viên) và `AppShell` (Menu Flyout).

---

## 3. Tiêu chí Nghiệm thu (Acceptance Criteria)
- [x] **AC 18.1 (Thông báo tức thì)**: Sau khi quét QR hoặc chụp ảnh điểm danh, thông báo Toast/Alert xuất hiện xác nhận số điểm rèn luyện được cộng.
- [x] **AC 19.1 (Xem lịch sử điểm danh)**: Hiển thị danh sách sự kiện đã tham gia với đầy đủ ngày giờ, phương thức và số điểm rèn luyện. Điểm rèn luyện tổng được cộng dồn chính xác.
- [x] **AC 20.1 (Giám sát thời gian thực)**: Ban tổ chức xem được danh sách sinh viên đã check-in, số lượng đã tham gia / tổng số đăng ký.
- [x] **AC 22.1 (Xuất báo cáo)**: Bấm "Xuất Báo Cáo" tạo ra file CSV với đầy đủ thông tin sinh viên, thời gian check-in.
- [x] **AC 04.1 (Hộp thư thông báo)**: Hiển thị các thông báo hệ thống và sự kiện sắp diễn ra rõ ràng, hỗ trợ đánh dấu đã đọc.
- [x] **DoD1–DoD6**: Tuân thủ chuẩn MVVM, 0 warning / 0 error trên mọi target, 100% test pass.
