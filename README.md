# Campus Activity Manager 🎓✨

> Ứng dụng Quản lý Hoạt động & Điểm danh Ngoại khóa Sinh viên xây dựng bằng **.NET 9 MAUI** (Windows & Android APK), hoạt động hoàn toàn tự chủ thông qua hệ thống **Services nội bộ** độc lập, không phụ thuộc vào bất kỳ máy chủ bên ngoài hay cấu hình mạng nào.

---

## ⚡ Hướng Dẫn Chạy Nhanh Cho Người Mới (Quick Start)

Dự án được thiết kế **100% Native .NET MAUI thuần (All-in-One)**:  
👉 **Chỉ dùng đúng duy nhất thư mục dự án `.NET MAUI` (`CampusActivitiesManager`) là chạy mượt mà tất cả các chức năng** trên cả máy tính Windows lẫn thiết bị/máy ảo Android (APK).

### Yêu Cầu Môi Trường
- **.NET 9.0 SDK** ([Tải tại đây](https://dotnet.microsoft.com/download/dotnet/9.0))
- Workload MAUI: Chạy lệnh `dotnet workload install maui`
- Visual Studio 2022 (v17.10 trở lên) có cài gói *.NET Multi-platform App UI development* hoặc VS Code.

---

## 🚀 Cách Khởi Động Ứng Dụng

### 👉 Cách 1: Chạy 1-Click bằng file Batch (Khuyên Dùng)
Ở thư mục gốc của dự án:
- **`run-desktop.bat`**: Nhấp đúp chuột để build và chạy ứng dụng Desktop Windows.
- **`run-android.bat`**: Nhấp đúp chuột để build và deploy chạy trên thiết bị / Android Emulator.

### 👉 Cách 2: Mở bằng Visual Studio 2022
1. Mở file giải pháp **`CampusActivitiesManager.sln`**.
2. Chọn target thiết bị:
   - **Windows Machine**: Chạy phiên bản Desktop.
   - **Android Emulator / Thiết bị Android thật**: Chạy phiên bản APK Android.
3. Nhấn **F5** (hoặc nút **Start** trên thanh công cụ) để chạy.

### 👉 Cách 3: Chạy bằng Dòng lệnh CLI

- **Chạy trên Windows Desktop**:
```bash
dotnet run --project CampusActivitiesManager\CampusActivitiesManager\CampusActivitiesManager.csproj -f net9.0-windows10.0.19041.0
```

- **Build gói APK cho Android**:
```bash
dotnet build CampusActivitiesManager\CampusActivitiesManager\CampusActivitiesManager.csproj -f net9.0-android
```

---

## 🔑 Tài Khoản Trải Nghiệm Mẫu (Pre-seeded Accounts)

Hệ thống tích hợp sẵn các tài khoản thử nghiệm với đầy đủ phân quyền theo chuẩn RBAC:

| Vai trò | Tên đăng nhập | Mật khẩu | Chức năng chính trong App |
|---|---|---|---|
| **Quản trị viên (Admin)** | `admin` *(hoặc `admin@campus.edu.vn`)* | `Admin@123456` | Toàn quyền: Duyệt / Từ chối hoạt động từ Manager, Quản lý tài khoản người dùng, Thống kê điểm danh |
| **Quản lý sự kiện (Manager)** | `manager` *(hoặc `manager@campus.edu.vn`)* | `Manager@123456` | Tạo sự kiện kèm ảnh/video (chờ Admin duyệt), Quản trị hoạt động, Xem báo cáo điểm danh |
| **Sinh viên (Student)** | `B20DCCN001` *(hoặc `cuong.na@campus.edu.vn`)* | `Student@123456` | Xem các hoạt động đã được Admin duyệt trên Trang chủ, Quét mã QR, Điểm danh ảnh, Xem lịch sử |

*(Bạn cũng có thể tự do nhấn **Đăng ký (Register)** trên màn hình Login để tạo tài khoản sinh viên mới).*

---

## 🏗️ Cấu Trúc Dự Án Thuần MAUI

```
CompusActivitiesManager/
├── CampusActivitiesManager/              # Thư mục mã nguồn DUY NHẤT của dự án (.NET 9 MAUI)
│   └── CampusActivitiesManager/
│       ├── Pages/                        # Giao diện XAML (Trang chủ, Đăng nhập, Sự kiện, Quét QR, Điểm danh ảnh...)
│       ├── PageModels/                   # ViewModels xử lý logic theo mô hình MVVM
│       ├── Services/                     # Hệ thống Service nội bộ thuần MAUI (Auth, Events, Attendance, Users, QR)
│       ├── Models/                       # Đối tượng dữ liệu nghiệp vụ (User, Event, Attendance, Category...)
│       ├── Data/                         # SQLite DataStore & Repositories lưu trữ dữ liệu bền vững
│       └── Resources/                    # Bảng màu Xanh Dương chuẩn Brand, Style, Fonts, Icons
├── CampusActivitiesManager.sln           # File giải pháp Solution (Chỉ chứa duy nhất dự án MAUI)
├── run-desktop.bat                       # Script 1-click chạy Desktop Windows
└── run-android.bat                       # Script 1-click chạy Android APK
```
