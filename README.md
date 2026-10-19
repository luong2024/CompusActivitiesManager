# Campus Activity Manager 🎓✨

> Ứng dụng Quản lý Hoạt động & Điểm danh Ngoại khóa Sinh viên xây dựng bằng **.NET 9 MAUI** (Windows & Android) kết hợp **ASP.NET Core Web API** và **Firebase**.

---

## ⚡ Hướng Dẫn Chạy Nhanh Cho Người Mới (Quick Start)

Dự án được tích hợp **Chế độ Tự Động Fallback (Offline / In-Memory Mode)**.  
👉 **Bất kỳ ai clone repo về máy đều có thể chạy và trải nghiệm ngay 100% tính năng** mà không bắt buộc phải có file khóa Google Cloud Firebase!

### Yêu Cầu Môi Trường
- **.NET 9.0 SDK** ([Tải tại đây](https://dotnet.microsoft.com/download/dotnet/9.0))
- Workload MAUI (nếu chạy Client MAUI): chạy lệnh `dotnet workload install maui`
- Visual Studio 2022 (v17.10 trở lên) có cài gói *.NET Multi-platform App UI development* hoặc VS Code.

---

### 🚀 3 Cách Khởi Động Ứng Dụng

#### Cách 1: Chạy bằng file Batch (Khuyên Dùng - 1 Click)
Ở thư mục gốc của dự án, nhấp đúp vào:
- **`run-desktop.bat`**: Tự động bật Backend API và khởi chạy giao diện Desktop Windows.
- **`run-android.bat`**: Tự động bật Backend API và build chạy trên Android Emulator.
- **`run-api.bat`**: Khởi chạy độc lập Backend API Server.

#### Cách 2: Mở bằng Visual Studio 2022
1. Mở file giải pháp `CampusActivitiesManager.sln`.
2. Dự án đã cấu hình sẵn profile chạy đa dự án **`Run All (API + MAUI Windows)`** trong file `CampusActivitiesManager.slnLaunch`.
3. Nhấn **F5** (hoặc nút **Start** trên thanh công cụ). Cả API Server và App MAUI sẽ tự động khởi chạy song song.

#### Cách 3: Chạy bằng Dòng lệnh (CLI)
Mở 2 cửa sổ Terminal tại thư mục dự án:
```bash
# Terminal 1: Khởi động Backend API
dotnet run --project CampusActivitiesManager.Api

# Terminal 2: Khởi động MAUI Windows App
dotnet run --project CampusActivitiesManager\CampusActivitiesManager\CampusActivitiesManager.csproj -f net9.0-windows10.0.19041.0
```

---

## 🔑 Tài Khoản Trải Nghiệm Mẫu (Pre-seeded Accounts)

Hệ thống đã nạp sẵn các tài khoản thử nghiệm với đầy đủ phân quyền theo chuẩn RBAC:

| Vai trò | Tên người dùng / Email | Tên đăng nhập rút gọn | Mật khẩu | Chức năng chính |
|---|---|---|---|---|
| **Quản trị viên (Admin)** | `admin@campus.edu.vn` | `admin` | `Admin@123456` | Quản lý người dùng, duyệt/từ chối sự kiện, xem báo cáo, toàn quyền hệ thống |
| **Quản lý sự kiện (Manager)** | `manager@campus.edu.vn` | `manager` | `Manager@123456` | Tạo sự kiện mới, quản trị sự kiện, xem danh sách điểm danh, xuất báo cáo |
| **Sinh viên (Student)** | `cuong.na@campus.edu.vn` | `B20DCCN001` | `Student@123456` | Xem danh sách sự kiện, check-in quét mã QR, điểm danh bằng hình ảnh, xem lịch sử điểm danh |
| **Giảng viên (Lecturer)** | `lecturer@campus.edu.vn` | `lecturer` | `Lecturer@123456` | Xem sự kiện khoa/viện |

*(Bạn cũng có thể tự do nhấn **Đăng ký (Register)** trên màn hình Login để tạo tài khoản sinh viên mới).*

---

## 🏗️ Cấu Trúc Dự Án

```
CompusActivitiesManager/
├── CampusActivitiesManager/              # Ứng dụng Client đa nền tảng .NET MAUI
│   └── CampusActivitiesManager/
│       ├── Pages/                        # Giao diện XAML (Home, Login, Events, Scan QR, Photo Check-in, Users)
│       ├── PageModels/                   # ViewModels xử lý logic theo mô hình MVVM
│       ├── Services/                     # Tầng kết nối REST API, SecureStorage, Audio, Camera
│       └── Resources/                    # Bảng màu Xanh Dương chuẩn Brand, Style, Fonts, Icons
├── CampusActivitiesManager.Api/          # Backend ASP.NET Core Web API v1
│   ├── Controllers/                      # REST Endpoints: Auth, Accounts, Events, Attendance, Notifications
│   ├── Services/                         # Tích hợp Firebase Cloud & In-Memory Fallback Service
│   └── Models/                           # Data Transfer Objects (DTO) & Request/Response Contracts
├── CampusActivitiesManager.Api.Tests/    # Bộ kiểm thử tự động Integration Tests (xUnit)
├── run-desktop.bat                       # Script 1-click chạy Desktop Windows
├── run-android.bat                       # Script 1-click chạy Android Emulator
└── run-api.bat                           # Script 1-click chạy Backend API
```

---

## 🛡️ Tích Hợp Firebase & Chế Độ Fallback

- **Khi có file Google Service Account Key**: API sẽ tự động kết nối trực tiếp đến **Firebase Authentication** và **Cloud Firestore**.
- **Khi chạy trên máy ngoài chưa cấu hình Google Cloud**: API sẽ kích hoạt **Chế độ In-Memory Fallback**:
  - Dữ liệu tài khoản, danh mục, sự kiện phong phú và điểm danh được nạp sẵn vào bộ nhớ.
  - Mã token JWT được ký và xác thực cục bộ bằng chuẩn HMAC-SHA256.
  - Điểm danh mã QR, điểm danh hình ảnh, duyệt sự kiện và quản lý tài khoản hoạt động mượt mà 100%.

---

## 🧪 Kiểm Thử Tự Động (Integration Tests)

Chạy toàn bộ 25 bài kiểm thử tích hợp:
```bash
dotnet test CampusActivitiesManager.Api.Tests
```
Kết quả: **25/25 Tests Passed (100% Passed)**.
