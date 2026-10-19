# Hồ Sơ Phân Công & Mã Nguồn: Trần Gia Khánh

- **Họ và tên**: **Trần Gia Khánh**
- **Vai trò trong dự án**: `Integration / Client Services & Core (.NET MAUI, HTTP, SecureStorage, AppShell)`
- **Tổng số task phụ trách**: `15 nhiệm vụ`
- **Tổng thời gian thực hiện**: `45 giờ`

---

## 1. Bảng Tổng Hợp Công Việc Từ Sprint 1 Đến Sprint 4

| Sprint | Mã Task | Mã US | Tên Nhiệm Vụ | Giờ | Thư Mục Trích Xuất Code |
| :---: | :---: | :---: | :--- | :---: | :--- |
| Sprint 01 | **T05.3** | US05 | Kết nối giao diện quản lý tài khoản với API & xử lý thông báo lỗi | 3h | [`Sprint_01/T05.3_KetNoiAPI_QuanLyTaiKhoan_XuLyLoi`](./Sprint_01/T05.3_KetNoiAPI_QuanLyTaiKhoan_XuLyLoi) |
| Sprint 01 | **T26.3** | US26 | Validate dữ liệu đầu vào phía client (định dạng email, độ mạnh mật khẩu) | 2h | [`Sprint_01/T26.3_ValidateDuLieuDauVao_Client`](./Sprint_01/T26.3_ValidateDuLieuDauVao_Client) |
| Sprint 01 | **T26.4** | US26 | Kết nối giao diện với API đăng ký & xử lý thông báo lỗi | 2h | [`Sprint_01/T26.4_KetNoiAPI_DangKy_XuLyLoi`](./Sprint_01/T26.4_KetNoiAPI_DangKy_XuLyLoi) |
| Sprint 01 | **T28.3** | US28 | Lưu trữ token an toàn trên thiết bị (SecureStorage) | 2h | [`Sprint_01/T28.3_LuuTruToken_SecureStorage`](./Sprint_01/T28.3_LuuTruToken_SecureStorage) |
| Sprint 01 | **T37.1** | US37 | Xây dựng AuthenticationService và tích hợp JWT Token | 9h | [`Sprint_01/T37.1_TichHopAuthentication_JWT`](./Sprint_01/T37.1_TichHopAuthentication_JWT) |
| Sprint 02 | **T31.2** | US31 | Gọi API lấy danh sách sự kiện và hiển thị | 2h | [`Sprint_02/T31.2_GoiAPI_DanhSachSuKien`](./Sprint_02/T31.2_GoiAPI_DanhSachSuKien) |
| Sprint 02 | **T01.2** | US01 | Gọi API lấy chi tiết sự kiện | 2h | [`Sprint_02/T01.2_GoiAPI_ChiTietSuKien`](./Sprint_02/T01.2_GoiAPI_ChiTietSuKien) |
| Sprint 02 | **T13.3** | US13 | Tích hợp gọi API CRUD sự kiện vào app | 4h | [`Sprint_02/T13.3_TichHopAPI_CRUD_SuKien`](./Sprint_02/T13.3_TichHopAPI_CRUD_SuKien) |
| Sprint 02 | **T07.3** | US07 | Tích hợp và Kiểm thử Profile | 3h | [`Sprint_02/T07.3_TichHop_UpdateProfile`](./Sprint_02/T07.3_TichHop_UpdateProfile) |
| Sprint 03 | **T17.2** | US17 | Tích hợp thư viện QRCoder sinh mã QR thật chuẩn PNG/Stream trên mobile app | 4h | [`Sprint_03/T17.2_TichHop_QRCoder_SinhMaQR`](./Sprint_03/T17.2_TichHop_QRCoder_SinhMaQR) |
| Sprint 03 | **T21.3** | US21 | Tích hợp dịch vụ điểm danh ảnh, kết nối API và xử lý lưu trữ ngoại tuyến | 3h | [`Sprint_03/T21.3_TichHop_DiemDanhHinhAnh_OfflineFallback`](./Sprint_03/T21.3_TichHop_DiemDanhHinhAnh_OfflineFallback) |
| Sprint 03 | **T01.5** | US01 | Tích hợp logic đếm ngược thời gian thực và điều hướng Shell đa nền tảng | 3h | [`Sprint_03/T01.5_LogicDemNguoc_DieuHuongShell`](./Sprint_03/T01.5_LogicDemNguoc_DieuHuongShell) |
| Sprint 04 | **T19.3** | US19 | Tích hợp tải lịch sử điểm danh, hỗ trợ lọc theo học kỳ và kéo làm mới (Pull-to-refresh) | 3h | [`Sprint_04/T19.3_TichHop_LichSuDiemDanh_PullToRefresh`](./Sprint_04/T19.3_TichHop_LichSuDiemDanh_PullToRefresh) |
| Sprint 04 | **T22.2** | US22 | Tích hợp tính năng lưu và chia sẻ file CSV báo cáo qua Share.RequestAsync() | 3h | [`Sprint_04/T22.2_LuuChiaSe_FileBaoCaoCSV`](./Sprint_04/T22.2_LuuChiaSe_FileBaoCaoCSV) |
| Sprint 04 | **T04.3** | US04 | Tích hợp thông báo Toast/Alert xác nhận điểm danh tức thì và đánh dấu đã đọc | 3h | [`Sprint_04/T04.3_ThongBaoToast_XacNhanDiemDanh`](./Sprint_04/T04.3_ThongBaoToast_XacNhanDiemDanh) |

---

## 2. Chi Tiết Các Hạng Mục Theo Từng Sprint

### Sprint 01 (Tổng: 18 giờ)

#### 🔹 Task T05.3 (US05): Kết nối giao diện quản lý tài khoản với API & xử lý thông báo lỗi
- **Thời gian**: `3 giờ`
- **Thư mục trích xuất**: [`Sprint_01/T05.3_KetNoiAPI_QuanLyTaiKhoan_XuLyLoi`](./Sprint_01/T05.3_KetNoiAPI_QuanLyTaiKhoan_XuLyLoi)
- **Tiêu chí nghiệm thu (AC)**:
  > Xây dựng UserService và HttpHelper gọi API /accounts, xử lý lỗi mạng và hiển thị dialog thông báo lỗi.
- **Các tệp mã nguồn phụ trách**:
  - `UserService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/UserService.cs`)
  - `IUserService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/IUserService.cs`)
  - `HttpHelper.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/HttpHelper.cs`)
  - `IErrorHandler.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/IErrorHandler.cs`)
  - `ModalErrorHandler.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/ModalErrorHandler.cs`)

#### 🔹 Task T26.3 (US26): Validate dữ liệu đầu vào phía client (định dạng email, độ mạnh mật khẩu)
- **Thời gian**: `2 giờ`
- **Thư mục trích xuất**: [`Sprint_01/T26.3_ValidateDuLieuDauVao_Client`](./Sprint_01/T26.3_ValidateDuLieuDauVao_Client)
- **Tiêu chí nghiệm thu (AC)**:
  > Kiểm tra định dạng email và độ phức tạp mật khẩu trước khi gửi request đăng ký.
- **Các tệp mã nguồn phụ trách**:
  - `AuthService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/AuthService.cs`)
  - `AuthModels.cs` (`CampusActivitiesManager/CampusActivitiesManager/Models/AuthModels.cs`)

#### 🔹 Task T26.4 (US26): Kết nối giao diện với API đăng ký & xử lý thông báo lỗi
- **Thời gian**: `2 giờ`
- **Thư mục trích xuất**: [`Sprint_01/T26.4_KetNoiAPI_DangKy_XuLyLoi`](./Sprint_01/T26.4_KetNoiAPI_DangKy_XuLyLoi)
- **Tiêu chí nghiệm thu (AC)**:
  > Gửi request đăng ký tài khoản tới API và xử lý kết quả trả về, hiển thị popup thông báo.
- **Các tệp mã nguồn phụ trách**:
  - `AuthService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/AuthService.cs`)
  - `IAuthenticationService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/IAuthenticationService.cs`)

#### 🔹 Task T28.3 (US28): Lưu trữ token an toàn trên thiết bị (SecureStorage)
- **Thời gian**: `2 giờ`
- **Thư mục trích xuất**: [`Sprint_01/T28.3_LuuTruToken_SecureStorage`](./Sprint_01/T28.3_LuuTruToken_SecureStorage)
- **Tiêu chí nghiệm thu (AC)**:
  > Sử dụng SecureStorage.Default lưu AccessToken và User Info, tự động đính kèm Bearer token vào HttpHelper.
- **Các tệp mã nguồn phụ trách**:
  - `AuthService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/AuthService.cs`)
  - `HttpHelper.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/HttpHelper.cs`)

#### 🔹 Task T37.1 (US37): Xây dựng AuthenticationService và tích hợp JWT Token
- **Thời gian**: `9 giờ`
- **Thư mục trích xuất**: [`Sprint_01/T37.1_TichHopAuthentication_JWT`](./Sprint_01/T37.1_TichHopAuthentication_JWT)
- **Tiêu chí nghiệm thu (AC)**:
  > Triển khai trọn vẹn luồng đăng nhập, lưu token, quản lý phiên làm việc và đăng ký Dependency Injection.
- **Các tệp mã nguồn phụ trách**:
  - `AuthenticationService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/AuthenticationService.cs`)
  - `AuthService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/AuthService.cs`)
  - `MauiProgram.cs` (`CampusActivitiesManager/CampusActivitiesManager/MauiProgram.cs`)

### Sprint 02 (Tổng: 11 giờ)

#### 🔹 Task T31.2 (US31): Gọi API lấy danh sách sự kiện và hiển thị
- **Thời gian**: `2 giờ`
- **Thư mục trích xuất**: [`Sprint_02/T31.2_GoiAPI_DanhSachSuKien`](./Sprint_02/T31.2_GoiAPI_DanhSachSuKien)
- **Tiêu chí nghiệm thu (AC)**:
  > EventService gọi API lấy danh sách sự kiện, deserialize dữ liệu và cấp phát cho View.
- **Các tệp mã nguồn phụ trách**:
  - `EventService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/EventService.cs`)
  - `IEventService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/IEventService.cs`)

#### 🔹 Task T01.2 (US01): Gọi API lấy chi tiết sự kiện
- **Thời gian**: `2 giờ`
- **Thư mục trích xuất**: [`Sprint_02/T01.2_GoiAPI_ChiTietSuKien`](./Sprint_02/T01.2_GoiAPI_ChiTietSuKien)
- **Tiêu chí nghiệm thu (AC)**:
  > EventService lấy chi tiết sự kiện theo ID từ API và xử lý trường hợp không tìm thấy sự kiện.
- **Các tệp mã nguồn phụ trách**:
  - `EventService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/EventService.cs`)

#### 🔹 Task T13.3 (US13): Tích hợp gọi API CRUD sự kiện vào app
- **Thời gian**: `4 giờ`
- **Thư mục trích xuất**: [`Sprint_02/T13.3_TichHopAPI_CRUD_SuKien`](./Sprint_02/T13.3_TichHopAPI_CRUD_SuKien)
- **Tiêu chí nghiệm thu (AC)**:
  > Tích hợp các hàm Create, Update, Delete sự kiện từ app lên server backend.
- **Các tệp mã nguồn phụ trách**:
  - `EventService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/EventService.cs`)
  - `CampusEvent.cs` (`CampusActivitiesManager/CampusActivitiesManager/Models/CampusEvent.cs`)
  - `Project.cs` (`CampusActivitiesManager/CampusActivitiesManager/Models/Project.cs`)

#### 🔹 Task T07.3 (US07): Tích hợp và Kiểm thử Profile
- **Thời gian**: `3 giờ`
- **Thư mục trích xuất**: [`Sprint_02/T07.3_TichHop_UpdateProfile`](./Sprint_02/T07.3_TichHop_UpdateProfile)
- **Tiêu chí nghiệm thu (AC)**:
  > Tích hợp hàm cập nhật thông tin cá nhân trong UserService và cập nhật ngay vào giao diện.
- **Các tệp mã nguồn phụ trách**:
  - `UserService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/UserService.cs`)
  - `UpdateProfileRequest.cs` (`CampusActivitiesManager/CampusActivitiesManager/Models/UpdateProfileRequest.cs`)

### Sprint 03 (Tổng: 10 giờ)

#### 🔹 Task T17.2 (US17): Tích hợp thư viện QRCoder sinh mã QR thật chuẩn PNG/Stream trên mobile app
- **Thời gian**: `4 giờ`
- **Thư mục trích xuất**: [`Sprint_03/T17.2_TichHop_QRCoder_SinhMaQR`](./Sprint_03/T17.2_TichHop_QRCoder_SinhMaQR)
- **Tiêu chí nghiệm thu (AC)**:
  > Cài đặt QRCoder, xây dựng QrCodeService sinh ảnh mã QR độ nét cao từ chuỗi vé điện tử, bất kỳ điện thoại nào cũng quét được.
- **Các tệp mã nguồn phụ trách**:
  - `IQrCodeService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/IQrCodeService.cs`)
  - `QrCodeService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/QrCodeService.cs`)
  - `CampusActivitiesManager.csproj` (`CampusActivitiesManager/CampusActivitiesManager/CampusActivitiesManager.csproj`)

#### 🔹 Task T21.3 (US21): Tích hợp dịch vụ điểm danh ảnh, kết nối API và xử lý lưu trữ ngoại tuyến
- **Thời gian**: `3 giờ`
- **Thư mục trích xuất**: [`Sprint_03/T21.3_TichHop_DiemDanhHinhAnh_OfflineFallback`](./Sprint_03/T21.3_TichHop_DiemDanhHinhAnh_OfflineFallback)
- **Tiêu chí nghiệm thu (AC)**:
  > AttendanceService gửi ảnh và dữ liệu check-in lên API, xử lý offline fallback khi mất kết nối.
- **Các tệp mã nguồn phụ trách**:
  - `IAttendanceService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/IAttendanceService.cs`)
  - `AttendanceService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/AttendanceService.cs`)

#### 🔹 Task T01.5 (US01): Tích hợp logic đếm ngược thời gian thực và điều hướng Shell đa nền tảng
- **Thời gian**: `3 giờ`
- **Thư mục trích xuất**: [`Sprint_03/T01.5_LogicDemNguoc_DieuHuongShell`](./Sprint_03/T01.5_LogicDemNguoc_DieuHuongShell)
- **Tiêu chí nghiệm thu (AC)**:
  > Đăng ký các route điều hướng Shell: QrScanner, PhotoAttendance, AttendanceHistory, EventReport; cấu hình DI trong MauiProgram.
- **Các tệp mã nguồn phụ trách**:
  - `AppShell.xaml` (`CampusActivitiesManager/CampusActivitiesManager/AppShell.xaml`)
  - `AppShell.xaml.cs` (`CampusActivitiesManager/CampusActivitiesManager/AppShell.xaml.cs`)
  - `MauiProgram.cs` (`CampusActivitiesManager/CampusActivitiesManager/MauiProgram.cs`)

### Sprint 04 (Tổng: 9 giờ)

#### 🔹 Task T19.3 (US19): Tích hợp tải lịch sử điểm danh, hỗ trợ lọc theo học kỳ và kéo làm mới (Pull-to-refresh)
- **Thời gian**: `3 giờ`
- **Thư mục trích xuất**: [`Sprint_04/T19.3_TichHop_LichSuDiemDanh_PullToRefresh`](./Sprint_04/T19.3_TichHop_LichSuDiemDanh_PullToRefresh)
- **Tiêu chí nghiệm thu (AC)**:
  > Tải lịch sử điểm danh sinh viên, hỗ trợ kéo làm mới (Pull-to-refresh), tính toán cộng dồn điểm rèn luyện thời gian thực.
- **Các tệp mã nguồn phụ trách**:
  - `AttendanceService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/AttendanceService.cs`)
  - `AttendanceModels.cs` (`CampusActivitiesManager/CampusActivitiesManager/Models/AttendanceModels.cs`)

#### 🔹 Task T22.2 (US22): Tích hợp tính năng lưu và chia sẻ file CSV báo cáo qua Share.RequestAsync()
- **Thời gian**: `3 giờ`
- **Thư mục trích xuất**: [`Sprint_04/T22.2_LuuChiaSe_FileBaoCaoCSV`](./Sprint_04/T22.2_LuuChiaSe_FileBaoCaoCSV)
- **Tiêu chí nghiệm thu (AC)**:
  > Lưu file CSV báo cáo vào thư mục ứng dụng và mở menu chia sẻ file hệ thống qua Share.RequestAsync().
- **Các tệp mã nguồn phụ trách**:
  - `AttendanceService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/AttendanceService.cs`)

#### 🔹 Task T04.3 (US04): Tích hợp thông báo Toast/Alert xác nhận điểm danh tức thì và đánh dấu đã đọc
- **Thời gian**: `3 giờ`
- **Thư mục trích xuất**: [`Sprint_04/T04.3_ThongBaoToast_XacNhanDiemDanh`](./Sprint_04/T04.3_ThongBaoToast_XacNhanDiemDanh)
- **Tiêu chí nghiệm thu (AC)**:
  > Hiển thị thông báo Toast/Alert chúc mừng ngay sau khi điểm danh QR hoặc chụp ảnh thành công.
- **Các tệp mã nguồn phụ trách**:
  - `AttendanceService.cs` (`CampusActivitiesManager/CampusActivitiesManager/Services/AttendanceService.cs`)
  - `AppShell.xaml.cs` (`CampusActivitiesManager/CampusActivitiesManager/AppShell.xaml.cs`)

---

## 3. Tiêu Chuẩn Nghiệm Thu & Đánh Giá Chất Lượng (DoD)
- **DoD1**: Mã nguồn tuân thủ kiến trúc phân tầng chuẩn và cấu trúc thư mục quy định.
- **DoD2**: Giao diện chuẩn bảng màu Xanh Dương (`#2563EB`), hỗ trợ Dark/Light Theme hoặc API bảo mật Firebase Admin SDK.
- **DoD3**: Vượt qua 100% kiểm thử tự động, không có warning hoặc lỗi khi biên dịch.
- **DoD4**: Tài liệu hóa chi tiết, commit rõ ràng theo chuẩn `<TaskID>: <mô tả>`.
