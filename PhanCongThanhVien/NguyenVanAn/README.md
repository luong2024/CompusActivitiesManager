# Hồ Sơ Phân Công & Mã Nguồn: Nguyễn Văn An

- **Họ và tên**: **Nguyễn Văn An**
- **Vai trò trong dự án**: `Admin & Manager Views / Forms (.NET MAUI, Management Pages)`
- **Tổng số task phụ trách**: `7 nhiệm vụ`
- **Tổng thời gian thực hiện**: `33 giờ`

---

## 1. Bảng Tổng Hợp Công Việc Từ Sprint 1 Đến Sprint 4

| Sprint | Mã Task | Mã US | Tên Nhiệm Vụ | Giờ | Thư Mục Trích Xuất Code |
| :---: | :---: | :---: | :--- | :---: | :--- |
| Sprint 01 | **T05.2** | US05 | Phân quyền tài khoản (Admin cập nhật vai trò/quyền hạn cho tài khoản khác) | 4h | [`Sprint_01/T05.2_PhanQuyenTaiKhoan`](./Sprint_01/T05.2_PhanQuyenTaiKhoan) |
| Sprint 01 | **T35.1** | US35 | Giao diện và xử lý Khóa / Mở khóa tài khoản | 8h | [`Sprint_01/T35.1_GiaoDien_KhoaMoKhoaTaiKhoan`](./Sprint_01/T35.1_GiaoDien_KhoaMoKhoaTaiKhoan) |
| Sprint 02 | **T13.2** | US13 | Phát triển form tạo mới/sửa sự kiện trên MAUI | 6h | [`Sprint_02/T13.2_Form_TaoSuaSuKien`](./Sprint_02/T13.2_Form_TaoSuaSuKien) |
| Sprint 02 | **T33.2** | US33 | Phát triển UI và tích hợp quản lý danh mục | 5h | [`Sprint_02/T33.2_UI_QuanLyDanhMuc`](./Sprint_02/T33.2_UI_QuanLyDanhMuc) |
| Sprint 03 | **T13.6** | US13 | Phát triển giao diện quản lý sự kiện và lịch trình cho Ban tổ chức (EventManagementPage) | 4h | [`Sprint_03/T13.6_UI_QuanLySuKien_LichTrinh_BTC`](./Sprint_03/T13.6_UI_QuanLySuKien_LichTrinh_BTC) |
| Sprint 03 | **T23.2** | US23 | Phát triển UI cấu hình phương thức điểm danh trong form tạo/sửa sự kiện | 3h | [`Sprint_03/T23.2_UI_CauHinhPhuongThucDiemDanh`](./Sprint_03/T23.2_UI_CauHinhPhuongThucDiemDanh) |
| Sprint 04 | **T20.3** | US20 | Tích hợp tìm kiếm nhanh sinh viên và điểm danh bổ sung trực tiếp trên danh sách | 3h | [`Sprint_04/T20.3_TimKiem_DiemDanhBoSung_BTC`](./Sprint_04/T20.3_TimKiem_DiemDanhBoSung_BTC) |

---

## 2. Chi Tiết Các Hạng Mục Theo Từng Sprint

### Sprint 01 (Tổng: 12 giờ)

#### 🔹 Task T05.2 (US05): Phân quyền tài khoản (Admin cập nhật vai trò/quyền hạn cho tài khoản khác)
- **Thời gian**: `4 giờ`
- **Thư mục trích xuất**: [`Sprint_01/T05.2_PhanQuyenTaiKhoan`](./Sprint_01/T05.2_PhanQuyenTaiKhoan)
- **Tiêu chí nghiệm thu (AC)**:
  > AC 05.3.1: Cho phép Admin chọn và cập nhật quyền hạn (ADMIN, LECTURER, STUDENT) cho người dùng.
- **Các tệp mã nguồn phụ trách**:
  - `EditUserRolePage.xaml` (`CampusActivitiesManager/CampusActivitiesManager/Pages/EditUserRolePage.xaml`)
  - `EditUserRolePage.xaml.cs` (`CampusActivitiesManager/CampusActivitiesManager/Pages/EditUserRolePage.xaml.cs`)

#### 🔹 Task T35.1 (US35): Giao diện và xử lý Khóa / Mở khóa tài khoản
- **Thời gian**: `8 giờ`
- **Thư mục trích xuất**: [`Sprint_01/T35.1_GiaoDien_KhoaMoKhoaTaiKhoan`](./Sprint_01/T35.1_GiaoDien_KhoaMoKhoaTaiKhoan)
- **Tiêu chí nghiệm thu (AC)**:
  > AC 35.1.1: Khóa tài khoản thành công; AC 35.1.2: Mở khóa tài khoản; trạng thái cập nhật chính xác.
- **Các tệp mã nguồn phụ trách**:
  - `AccountListPage.xaml` (`CampusActivitiesManager/CampusActivitiesManager/Pages/AccountListPage.xaml`)
  - `AccountListPage.xaml.cs` (`CampusActivitiesManager/CampusActivitiesManager/Pages/AccountListPage.xaml.cs`)
  - `EditUserRolePage.xaml` (`CampusActivitiesManager/CampusActivitiesManager/Pages/EditUserRolePage.xaml`)
  - `EditUserRolePage.xaml.cs` (`CampusActivitiesManager/CampusActivitiesManager/Pages/EditUserRolePage.xaml.cs`)

### Sprint 02 (Tổng: 11 giờ)

#### 🔹 Task T13.2 (US13): Phát triển form tạo mới/sửa sự kiện trên MAUI
- **Thời gian**: `6 giờ`
- **Thư mục trích xuất**: [`Sprint_02/T13.2_Form_TaoSuaSuKien`](./Sprint_02/T13.2_Form_TaoSuaSuKien)
- **Tiêu chí nghiệm thu (AC)**:
  > Form nhập thông tin sự kiện có đầy đủ validate: Tên sự kiện, ngày bắt đầu, địa điểm, mô tả.
- **Các tệp mã nguồn phụ trách**:
  - `TaskDetailPage.xaml` (`CampusActivitiesManager/CampusActivitiesManager/Pages/TaskDetailPage.xaml`)
  - `TaskDetailPage.xaml.cs` (`CampusActivitiesManager/CampusActivitiesManager/Pages/TaskDetailPage.xaml.cs`)

#### 🔹 Task T33.2 (US33): Phát triển UI và tích hợp quản lý danh mục
- **Thời gian**: `5 giờ`
- **Thư mục trích xuất**: [`Sprint_02/T33.2_UI_QuanLyDanhMuc`](./Sprint_02/T33.2_UI_QuanLyDanhMuc)
- **Tiêu chí nghiệm thu (AC)**:
  > Màn hình quản lý danh mục, biểu đồ phân bổ danh mục sự kiện trực quan.
- **Các tệp mã nguồn phụ trách**:
  - `ManageMetaPage.xaml` (`CampusActivitiesManager/CampusActivitiesManager/Pages/ManageMetaPage.xaml`)
  - `ManageMetaPage.xaml.cs` (`CampusActivitiesManager/CampusActivitiesManager/Pages/ManageMetaPage.xaml.cs`)
  - `CategoryChart.xaml` (`CampusActivitiesManager/CampusActivitiesManager/Pages/Controls/CategoryChart.xaml`)
  - `CategoryChart.xaml.cs` (`CampusActivitiesManager/CampusActivitiesManager/Pages/Controls/CategoryChart.xaml.cs`)

### Sprint 03 (Tổng: 7 giờ)

#### 🔹 Task T13.6 (US13): Phát triển giao diện quản lý sự kiện và lịch trình cho Ban tổ chức (EventManagementPage)
- **Thời gian**: `4 giờ`
- **Thư mục trích xuất**: [`Sprint_03/T13.6_UI_QuanLySuKien_LichTrinh_BTC`](./Sprint_03/T13.6_UI_QuanLySuKien_LichTrinh_BTC)
- **Tiêu chí nghiệm thu (AC)**:
  > Giao diện quản lý dành riêng cho Event Manager/Admin, cập nhật thời gian check-in, số lượng tham gia và danh sách nhanh.
- **Các tệp mã nguồn phụ trách**:
  - `EventManagementPage.xaml` (`CampusActivitiesManager/CampusActivitiesManager/Pages/EventManagementPage.xaml`)
  - `EventManagementPage.xaml.cs` (`CampusActivitiesManager/CampusActivitiesManager/Pages/EventManagementPage.xaml.cs`)

#### 🔹 Task T23.2 (US23): Phát triển UI cấu hình phương thức điểm danh trong form tạo/sửa sự kiện
- **Thời gian**: `3 giờ`
- **Thư mục trích xuất**: [`Sprint_03/T23.2_UI_CauHinhPhuongThucDiemDanh`](./Sprint_03/T23.2_UI_CauHinhPhuongThucDiemDanh)
- **Tiêu chí nghiệm thu (AC)**:
  > Bộ công tắc Switch trực quan cho phép Ban tổ chức bật/tắt từng hình thức điểm danh (QR, Ảnh, Thủ công).
- **Các tệp mã nguồn phụ trách**:
  - `EventManagementPage.xaml` (`CampusActivitiesManager/CampusActivitiesManager/Pages/EventManagementPage.xaml`)
  - `EventManagementPage.xaml.cs` (`CampusActivitiesManager/CampusActivitiesManager/Pages/EventManagementPage.xaml.cs`)

### Sprint 04 (Tổng: 3 giờ)

#### 🔹 Task T20.3 (US20): Tích hợp tìm kiếm nhanh sinh viên và điểm danh bổ sung trực tiếp trên danh sách
- **Thời gian**: `3 giờ`
- **Thư mục trích xuất**: [`Sprint_04/T20.3_TimKiem_DiemDanhBoSung_BTC`](./Sprint_04/T20.3_TimKiem_DiemDanhBoSung_BTC)
- **Tiêu chí nghiệm thu (AC)**:
  > Ô tìm kiếm sinh viên theo MSV/Tên phản hồi tức thì, hỗ trợ nút điểm danh bù tại chỗ cho sinh viên chưa quét được vé.
- **Các tệp mã nguồn phụ trách**:
  - `EventAttendanceReportPage.xaml` (`CampusActivitiesManager/CampusActivitiesManager/Pages/EventAttendanceReportPage.xaml`)
  - `EventAttendanceReportPage.xaml.cs` (`CampusActivitiesManager/CampusActivitiesManager/Pages/EventAttendanceReportPage.xaml.cs`)

---

## 3. Tiêu Chuẩn Nghiệm Thu & Đánh Giá Chất Lượng (DoD)
- **DoD1**: Mã nguồn tuân thủ kiến trúc phân tầng chuẩn và cấu trúc thư mục quy định.
- **DoD2**: Giao diện chuẩn bảng màu Xanh Dương (`#2563EB`), hỗ trợ Dark/Light Theme hoặc API bảo mật Firebase Admin SDK.
- **DoD3**: Vượt qua 100% kiểm thử tự động, không có warning hoặc lỗi khi biên dịch.
- **DoD4**: Tài liệu hóa chi tiết, commit rõ ràng theo chuẩn `<TaskID>: <mô tả>`.
