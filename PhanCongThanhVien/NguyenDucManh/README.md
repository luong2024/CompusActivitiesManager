# Hồ Sơ Phân Công & Mã Nguồn: Nguyễn Đức Mạnh

- **Họ và tên**: **Nguyễn Đức Mạnh**
- **Vai trò trong dự án**: `API / Backend (.ASP.NET Core Web API, Firebase Admin SDK)`
- **Tổng số task phụ trách**: `13 nhiệm vụ`
- **Tổng thời gian thực hiện**: `70 giờ`

---

## 1. Bảng Tổng Hợp Công Việc Từ Sprint 1 Đến Sprint 4

| Sprint | Mã Task | Mã US | Tên Nhiệm Vụ | Giờ | Thư Mục Trích Xuất Code |
| :---: | :---: | :---: | :--- | :---: | :--- |
| Sprint 01 | **T35.1** | US35 | Xây dựng Create Account + Update Account API với Firebase Admin SDK | 10h | [`Sprint_01/T35.1_API_Create_Update_Account`](./Sprint_01/T35.1_API_Create_Update_Account) |
| Sprint 01 | **T28.4** | US28 | API Đăng nhập và xác thực phiên làm việc | 1h | [`Sprint_01/T28.4_API_Login_Auth`](./Sprint_01/T28.4_API_Login_Auth) |
| Sprint 02 | **T13.1** | US13 | Xây dựng API CRUD cho sự kiện | 8h | [`Sprint_02/T13.1_API_CRUD_SuKien`](./Sprint_02/T13.1_API_CRUD_SuKien) |
| Sprint 02 | **T33.1** | US33 | Xây dựng API CRUD cho danh mục | 5h | [`Sprint_02/T33.1_API_CRUD_DanhMuc`](./Sprint_02/T33.1_API_CRUD_DanhMuc) |
| Sprint 02 | **T07.1** | US07 | Xây dựng API Update Profile | 4h | [`Sprint_02/T07.1_API_Update_Profile`](./Sprint_02/T07.1_API_Update_Profile) |
| Sprint 03 | **T17.1** | US17 | Xây dựng API xác thực mã QR điểm danh (POST /api/v1/attendance/qr-checkin) | 6h | [`Sprint_03/T17.1_API_DiemDanh_QR`](./Sprint_03/T17.1_API_DiemDanh_QR) |
| Sprint 03 | **T21.1** | US21 | Xây dựng API điểm danh bằng ảnh minh chứng (POST /photo-checkin) và điểm danh thủ công | 5h | [`Sprint_03/T21.1_API_DiemDanh_HinhAnh`](./Sprint_03/T21.1_API_DiemDanh_HinhAnh) |
| Sprint 03 | **T13.5** | US13 | Nâng cấp API sự kiện: bổ sung lịch trình (timeline), diễn giả, vị trí và trạng thái điểm danh | 5h | [`Sprint_03/T13.5_API_LichTrinh_DienGia_TrangThai`](./Sprint_03/T13.5_API_LichTrinh_DienGia_TrangThai) |
| Sprint 03 | **T23.1** | US23 | Xây dựng API cấu hình phương thức điểm danh (QR, Ảnh, Thủ công) cho từng sự kiện | 4h | [`Sprint_03/T23.1_API_CauHinh_PhuongThucDiemDanh`](./Sprint_03/T23.1_API_CauHinh_PhuongThucDiemDanh) |
| Sprint 04 | **T19.1** | US19 | Xây dựng API lịch sử điểm danh theo sinh viên (GET /api/v1/attendance/student/{code}) | 5h | [`Sprint_04/T19.1_API_LichSuDiemDanh_DiemRenLuyen`](./Sprint_04/T19.1_API_LichSuDiemDanh_DiemRenLuyen) |
| Sprint 04 | **T20.1** | US20 | Xây dựng API lấy danh sách điểm danh thời gian thực theo sự kiện (GET /event/{eventId}) | 4h | [`Sprint_04/T20.1_API_GiamSatDiemDanh_ThoiGianThuc`](./Sprint_04/T20.1_API_GiamSatDiemDanh_ThoiGianThuc) |
| Sprint 04 | **T22.1** | US22 | Xây dựng API và Service xuất file báo cáo điểm danh chuẩn CSV UTF-8 có BOM | 4h | [`Sprint_04/T22.1_API_XuatBaoCao_CSV`](./Sprint_04/T22.1_API_XuatBaoCao_CSV) |
| Sprint 04 | **T04.1** | US04 | Xây dựng API thông báo sự kiện, xác nhận điểm danh và kênh gửi câu hỏi hỗ trợ (Feedback) | 4h | [`Sprint_04/T04.1_API_ThongBao_Feedback`](./Sprint_04/T04.1_API_ThongBao_Feedback) |

---

## 2. Chi Tiết Các Hạng Mục Theo Từng Sprint

### Sprint 01 (Tổng: 11 giờ)

#### 🔹 Task T35.1 (US35): Xây dựng Create Account + Update Account API với Firebase Admin SDK
- **Thời gian**: `10 giờ`
- **Thư mục trích xuất**: [`Sprint_01/T35.1_API_Create_Update_Account`](./Sprint_01/T35.1_API_Create_Update_Account)
- **Tiêu chí nghiệm thu (AC)**:
  > API POST/PUT /accounts tạo mới và cập nhật thông tin tài khoản, validate dữ liệu, đồng bộ giữa Firebase Auth và Firestore CSDL.
- **Các tệp mã nguồn phụ trách**:
  - `AccountsController.cs` (`CampusActivitiesManager.Api/Controllers/AccountsController.cs`)
  - `CreateAccountRequest.cs` (`CampusActivitiesManager.Api/Models/CreateAccountRequest.cs`)
  - `UpdateAccountRequest.cs` (`CampusActivitiesManager.Api/Models/UpdateAccountRequest.cs`)
  - `UserAccountDto.cs` (`CampusActivitiesManager.Api/Models/UserAccountDto.cs`)
  - `FirebaseAccountService.cs` (`CampusActivitiesManager.Api/Services/FirebaseAccountService.cs`)
  - `IFirebaseAccountService.cs` (`CampusActivitiesManager.Api/Services/IFirebaseAccountService.cs`)

#### 🔹 Task T28.4 (US28): API Đăng nhập và xác thực phiên làm việc
- **Thời gian**: `1 giờ`
- **Thư mục trích xuất**: [`Sprint_01/T28.4_API_Login_Auth`](./Sprint_01/T28.4_API_Login_Auth)
- **Tiêu chí nghiệm thu (AC)**:
  > API POST /auth/login kiểm tra thông tin đăng nhập, trả về JWT Token và thông tin phân quyền người dùng.
- **Các tệp mã nguồn phụ trách**:
  - `AuthController.cs` (`CampusActivitiesManager.Api/Controllers/AuthController.cs`)
  - `LoginRequest.cs` (`CampusActivitiesManager.Api/Models/LoginRequest.cs`)
  - `LoginResponse.cs` (`CampusActivitiesManager.Api/Models/LoginResponse.cs`)

### Sprint 02 (Tổng: 17 giờ)

#### 🔹 Task T13.1 (US13): Xây dựng API CRUD cho sự kiện
- **Thời gian**: `8 giờ`
- **Thư mục trích xuất**: [`Sprint_02/T13.1_API_CRUD_SuKien`](./Sprint_02/T13.1_API_CRUD_SuKien)
- **Tiêu chí nghiệm thu (AC)**:
  > API GET/POST/PUT/DELETE /events cho phép lấy danh sách, chi tiết, tạo mới, chỉnh sửa và xóa sự kiện trên Firebase Firestore.
- **Các tệp mã nguồn phụ trách**:
  - `EventsController.cs` (`CampusActivitiesManager.Api/Controllers/EventsController.cs`)
  - `EventModels.cs` (`CampusActivitiesManager.Api/Models/EventModels.cs`)
  - `FirebaseEventService.cs` (`CampusActivitiesManager.Api/Services/FirebaseEventService.cs`)
  - `IFirebaseEventService.cs` (`CampusActivitiesManager.Api/Services/IFirebaseEventService.cs`)

#### 🔹 Task T33.1 (US33): Xây dựng API CRUD cho danh mục
- **Thời gian**: `5 giờ`
- **Thư mục trích xuất**: [`Sprint_02/T33.1_API_CRUD_DanhMuc`](./Sprint_02/T33.1_API_CRUD_DanhMuc)
- **Tiêu chí nghiệm thu (AC)**:
  > API quản lý danh mục sự kiện (học thuật, tình nguyện, thể thao, hội thảo).
- **Các tệp mã nguồn phụ trách**:
  - `CategoriesController.cs` (`CampusActivitiesManager.Api/Controllers/CategoriesController.cs`)

#### 🔹 Task T07.1 (US07): Xây dựng API Update Profile
- **Thời gian**: `4 giờ`
- **Thư mục trích xuất**: [`Sprint_02/T07.1_API_Update_Profile`](./Sprint_02/T07.1_API_Update_Profile)
- **Tiêu chí nghiệm thu (AC)**:
  > API cập nhật thông tin cá nhân người dùng: Tên, SĐT, Avatar URL, Khoa, Lớp.
- **Các tệp mã nguồn phụ trách**:
  - `ProfileController.cs` (`CampusActivitiesManager.Api/Controllers/ProfileController.cs`)

### Sprint 03 (Tổng: 20 giờ)

#### 🔹 Task T17.1 (US17): Xây dựng API xác thực mã QR điểm danh (POST /api/v1/attendance/qr-checkin)
- **Thời gian**: `6 giờ`
- **Thư mục trích xuất**: [`Sprint_03/T17.1_API_DiemDanh_QR`](./Sprint_03/T17.1_API_DiemDanh_QR)
- **Tiêu chí nghiệm thu (AC)**:
  > Tiếp nhận chuỗi QR vé định dạng chuẩn, xác thực vé hợp lệ, tránh quét trùng lặp, ghi nhận điểm danh và cộng điểm rèn luyện.
- **Các tệp mã nguồn phụ trách**:
  - `AttendanceController.cs` (`CampusActivitiesManager.Api/Controllers/AttendanceController.cs`)
  - `AttendanceModels.cs` (`CampusActivitiesManager.Api/Models/AttendanceModels.cs`)
  - `FirebaseAttendanceService.cs` (`CampusActivitiesManager.Api/Services/FirebaseAttendanceService.cs`)
  - `IFirebaseAttendanceService.cs` (`CampusActivitiesManager.Api/Services/IFirebaseAttendanceService.cs`)

#### 🔹 Task T21.1 (US21): Xây dựng API điểm danh bằng ảnh minh chứng (POST /photo-checkin) và điểm danh thủ công
- **Thời gian**: `5 giờ`
- **Thư mục trích xuất**: [`Sprint_03/T21.1_API_DiemDanh_HinhAnh`](./Sprint_03/T21.1_API_DiemDanh_HinhAnh)
- **Tiêu chí nghiệm thu (AC)**:
  > Tiếp nhận URL/base64 ảnh minh chứng, lưu trữ mốc thời gian; hỗ trợ endpoint điểm danh thủ công cho BTC.
- **Các tệp mã nguồn phụ trách**:
  - `AttendanceController.cs` (`CampusActivitiesManager.Api/Controllers/AttendanceController.cs`)
  - `AttendanceModels.cs` (`CampusActivitiesManager.Api/Models/AttendanceModels.cs`)
  - `FirebaseAttendanceService.cs` (`CampusActivitiesManager.Api/Services/FirebaseAttendanceService.cs`)

#### 🔹 Task T13.5 (US13): Nâng cấp API sự kiện: bổ sung lịch trình (timeline), diễn giả, vị trí và trạng thái điểm danh
- **Thời gian**: `5 giờ`
- **Thư mục trích xuất**: [`Sprint_03/T13.5_API_LichTrinh_DienGia_TrangThai`](./Sprint_03/T13.5_API_LichTrinh_DienGia_TrangThai)
- **Tiêu chí nghiệm thu (AC)**:
  > Mở rộng EventDto chứa lịch trình chi tiết, thông tin diễn giả và lọc sự kiện theo trạng thái (Sắp diễn ra, Đang diễn ra, Đã kết thúc).
- **Các tệp mã nguồn phụ trách**:
  - `EventsController.cs` (`CampusActivitiesManager.Api/Controllers/EventsController.cs`)
  - `EventModels.cs` (`CampusActivitiesManager.Api/Models/EventModels.cs`)
  - `FirebaseEventService.cs` (`CampusActivitiesManager.Api/Services/FirebaseEventService.cs`)

#### 🔹 Task T23.1 (US23): Xây dựng API cấu hình phương thức điểm danh (QR, Ảnh, Thủ công) cho từng sự kiện
- **Thời gian**: `4 giờ`
- **Thư mục trích xuất**: [`Sprint_03/T23.1_API_CauHinh_PhuongThucDiemDanh`](./Sprint_03/T23.1_API_CauHinh_PhuongThucDiemDanh)
- **Tiêu chí nghiệm thu (AC)**:
  > Quản lý các cờ cấu hình AllowQrCheckIn, AllowPhotoCheckIn, AllowManualCheckIn trong Event Model.
- **Các tệp mã nguồn phụ trách**:
  - `EventsController.cs` (`CampusActivitiesManager.Api/Controllers/EventsController.cs`)
  - `EventModels.cs` (`CampusActivitiesManager.Api/Models/EventModels.cs`)

### Sprint 04 (Tổng: 17 giờ)

#### 🔹 Task T19.1 (US19): Xây dựng API lịch sử điểm danh theo sinh viên (GET /api/v1/attendance/student/{code})
- **Thời gian**: `5 giờ`
- **Thư mục trích xuất**: [`Sprint_04/T19.1_API_LichSuDiemDanh_DiemRenLuyen`](./Sprint_04/T19.1_API_LichSuDiemDanh_DiemRenLuyen)
- **Tiêu chí nghiệm thu (AC)**:
  > Truy vấn danh sách toàn bộ các sự kiện sinh viên đã điểm danh, phương thức thực hiện và tính tổng điểm rèn luyện tích lũy.
- **Các tệp mã nguồn phụ trách**:
  - `AttendanceController.cs` (`CampusActivitiesManager.Api/Controllers/AttendanceController.cs`)
  - `AttendanceModels.cs` (`CampusActivitiesManager.Api/Models/AttendanceModels.cs`)
  - `FirebaseAttendanceService.cs` (`CampusActivitiesManager.Api/Services/FirebaseAttendanceService.cs`)

#### 🔹 Task T20.1 (US20): Xây dựng API lấy danh sách điểm danh thời gian thực theo sự kiện (GET /event/{eventId})
- **Thời gian**: `4 giờ`
- **Thư mục trích xuất**: [`Sprint_04/T20.1_API_GiamSatDiemDanh_ThoiGianThuc`](./Sprint_04/T20.1_API_GiamSatDiemDanh_ThoiGianThuc)
- **Tiêu chí nghiệm thu (AC)**:
  > Thống kê tổng quan số lượng check-in / đăng ký, danh sách chi tiết sinh viên và thời điểm điểm danh.
- **Các tệp mã nguồn phụ trách**:
  - `AttendanceController.cs` (`CampusActivitiesManager.Api/Controllers/AttendanceController.cs`)
  - `AttendanceModels.cs` (`CampusActivitiesManager.Api/Models/AttendanceModels.cs`)

#### 🔹 Task T22.1 (US22): Xây dựng API và Service xuất file báo cáo điểm danh chuẩn CSV UTF-8 có BOM
- **Thời gian**: `4 giờ`
- **Thư mục trích xuất**: [`Sprint_04/T22.1_API_XuatBaoCao_CSV`](./Sprint_04/T22.1_API_XuatBaoCao_CSV)
- **Tiêu chí nghiệm thu (AC)**:
  > Xuất file CSV chứa BOM UTF-8 không lỗi font khi mở bằng Microsoft Excel, đầy đủ thông tin sinh viên, thời gian và điểm.
- **Các tệp mã nguồn phụ trách**:
  - `AttendanceController.cs` (`CampusActivitiesManager.Api/Controllers/AttendanceController.cs`)
  - `FirebaseAttendanceService.cs` (`CampusActivitiesManager.Api/Services/FirebaseAttendanceService.cs`)

#### 🔹 Task T04.1 (US04): Xây dựng API thông báo sự kiện, xác nhận điểm danh và kênh gửi câu hỏi hỗ trợ (Feedback)
- **Thời gian**: `4 giờ`
- **Thư mục trích xuất**: [`Sprint_04/T04.1_API_ThongBao_Feedback`](./Sprint_04/T04.1_API_ThongBao_Feedback)
- **Tiêu chí nghiệm thu (AC)**:
  > API cung cấp danh sách thông báo hệ thống và tiếp nhận câu hỏi hỗ trợ của sinh viên gửi tới ban quản trị.
- **Các tệp mã nguồn phụ trách**:
  - `NotificationsController.cs` (`CampusActivitiesManager.Api/Controllers/NotificationsController.cs`)
  - `Program.cs` (`CampusActivitiesManager.Api/Program.cs`)

---

## 3. Tiêu Chuẩn Nghiệm Thu & Đánh Giá Chất Lượng (DoD)
- **DoD1**: Mã nguồn tuân thủ kiến trúc phân tầng chuẩn và cấu trúc thư mục quy định.
- **DoD2**: Giao diện chuẩn bảng màu Xanh Dương (`#2563EB`), hỗ trợ Dark/Light Theme hoặc API bảo mật Firebase Admin SDK.
- **DoD3**: Vượt qua 100% kiểm thử tự động, không có warning hoặc lỗi khi biên dịch.
- **DoD4**: Tài liệu hóa chi tiết, commit rõ ràng theo chuẩn `<TaskID>: <mô tả>`.
