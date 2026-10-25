# Nhiệm vụ: Xây dựng Chức năng Đăng ký Tài khoản Người dùng (Student & User Registration)

**Mã Task**: T37.2  
**User Story**: US37 – Student & User Self-Registration (Đăng ký tài khoản người dùng và sinh viên mới trên ứng dụng)  
**Người được giao (Assignee)**: `campus-worker-ui` & `campus-worker-integration`  
**Trạng thái**: Hoàn thành (Done)  
**Thời gian dự kiến (Estimate)**: 8 giờ  

---

## 1. Mục tiêu nhiệm vụ
Cung cấp màn hình và luồng nghiệp vụ cho phép sinh viên / người dùng mới tự đăng ký tài khoản tham gia ứng dụng **Campus Activities Manager**:
1. Cho phép nhập đầy đủ thông tin: Họ và tên, Mã sinh viên (Tên đăng nhập), Email, Số điện thoại, Lớp sinh hoạt và Mật khẩu.
2. Kiểm tra tính hợp lệ dữ liệu (Client-side validation): các trường bắt buộc, định dạng email, độ dài mật khẩu tối thiểu 6 ký tự, mật khẩu xác nhận trùng khớp.
3. Kiểm tra tính duy nhất (Unique check): ngăn chặn đăng ký trùng Mã sinh viên hoặc Email đã tồn tại trong hệ thống.
4. Thiết lập đúng các quy tắc nghiệp vụ dự án:
   - Vai trò mặc định: `Role.Student` (`Role.User`).
   - **Điểm rèn luyện khởi điểm ban đầu: 0 ĐRL** (Chỉ tiêu tối thiểu: 50 ĐRL/học kỳ, 250 ĐRL/năm học).
   - Trạng thái tài khoản: `IsActive = true`.
5. Trải nghiệm người dùng (UX): Bảng màu Xanh Dương (`#2563EB`) chuẩn mực, có nút chuyển qua lại mượt mà giữa màn hình Đăng nhập (`LoginPage`) và Đăng ký (`RegisterPage`).

---

## 2. Chi tiết các hạng mục thực hiện (Checklist)

### 2.1. Quản lý Dữ liệu & Dịch vụ Xác thực (`Services`)
- [x] Cập nhật `IAuthenticationService` và `AuthenticationService` hỗ trợ xác thực người dùng mới đăng ký.
- [x] Tích hợp với `IUserService<User>`: Kiểm tra `GetUserByUsernameAsync`, kiểm tra trùng lặp email và lưu qua `SaveUserAsync`.
- [x] Khởi tạo tài khoản với `TrainingPoints = 0`, phân lớp `ClassName` (`D20CNTT1`, `D21CNTT1`, `D22CNTT1`, `D20ATTT1`...).

### 2.2. Giao diện Người dùng (.NET MAUI MVVM)
- [x] **`RegisterPage.xaml`**:
  - Giao diện form thẻ bo tròn chuẩn Fluent UI, bảng màu Xanh Dương `PrimaryGradientBrush`.
  - Ô nhập Họ tên, Mã sinh viên, Email, Số điện thoại.
  - Bộ chọn Lớp học (`Picker` danh sách lớp có sẵn).
  - Ô nhập Mật khẩu và Xác nhận Mật khẩu kèm nút bật/tắt ẩn hiện mắt xem mật khẩu và thanh đánh giá độ mạnh.
  - Hộp thông báo quy tắc ĐRL: *0 ĐRL ban đầu, 50 ĐRL/kỳ, 250 ĐRL/năm*.
  - Nút "Đăng ký tài khoản" với hiệu ứng loading và nút "Quay lại đăng nhập".
- [x] **`RegisterViewModel.cs`**:
  - Quản lý trạng thái form, validation, kiểm tra trùng lặp và lưu trữ.
  - Hiển thị Alert chúc mừng đăng ký thành công và điều hướng trở lại `LoginPage`.
- [x] **Cập nhật `LoginPage.xaml` & `LoginPageModel.cs`**:
  - Thay thế thông báo cũ bằng liên kết *"Chưa có tài khoản? Đăng ký ngay"*.
  - Gắn lệnh `NavigateToRegisterCommand` gọi điều hướng Shell route `register`.

### 2.3. Cấu hình Điều hướng (Shell Routing & DI)
- [x] Đăng ký Route `register` và `nameof(RegisterPage)` trong `AppShell.xaml.cs`.
- [x] Đăng ký `RegisterPage` và `RegisterViewModel` trong `MauiProgram.cs`.

---

## 3. Tiêu chí Nghiệm thu (Acceptance Criteria)
- [x] **AC 37.2.1 (Form đăng ký đầy đủ)**: Người dùng có thể nhập đầy đủ Họ tên, Mã sinh viên, Email, Số điện thoại, Lớp sinh hoạt và Mật khẩu.
- [x] **AC 37.2.2 (Validation chặt chẽ)**: Báo lỗi khi thiếu trường, email sai định dạng, mật khẩu < 6 ký tự hoặc mật khẩu xác nhận không khớp.
- [x] **AC 37.2.3 (Kiểm tra trùng lặp)**: Báo lỗi rõ ràng nếu Mã sinh viên hoặc Email đã được đăng ký trước đó.
- [x] **AC 37.2.4 (Quy tắc ĐRL 0 điểm)**: Tài khoản mới tạo có chính xác 0 Điểm rèn luyện ban đầu và trạng thái kích hoạt `IsActive = true`.
- [x] **AC 37.2.5 (Đăng nhập ngay sau đăng ký)**: Sau khi đăng ký thành công, người dùng có thể dùng tài khoản vừa tạo để đăng nhập vào ứng dụng và vào thẳng Trang chủ (`MainPage`).
- [x] **DoD1–DoD6**: Giao diện Xanh Dương `#2563EB`, biên dịch 0 lỗi, 0 cảnh báo.
