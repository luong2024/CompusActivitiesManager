# Campus Activity Manager 🎓✨

> Ứng dụng Quản lý Hoạt động & Điểm danh Ngoại khóa Sinh viên xây dựng bằng **.NET 9 MAUI** (Windows & Android), tích hợp sẵn toàn bộ dữ liệu mẫu nội bộ và hỗ trợ mở rộng kết nối **Backend REST API / Firebase**.

---

## ⚡ Hướng Dẫn Chạy Nhanh Cho Người Mới (Quick Start)

Dự án được thiết kế **hoàn toàn tự chủ (Self-contained / All-in-One)**:  
👉 **Chỉ cần mở và chạy duy nhất 1 dự án `.NET MAUI` (`CampusActivitiesManager`) là chạy được ngay 100% tính năng** mà không bắt buộc phải bật server API hay cấu hình Firebase nào!

### Yêu Cầu Môi Trường
- **.NET 9.0 SDK** ([Tải tại đây](https://dotnet.microsoft.com/download/dotnet/9.0))
- Workload MAUI: Chạy lệnh `dotnet workload install maui`
- Visual Studio 2022 (v17.10 trở lên) có cài gói *.NET Multi-platform App UI development* hoặc VS Code.

---

### 🚀 Cách Khởi Động Ứng Dụng (Đơn Giản Nhất)

#### 👉 Cách 1: Chạy 1-Click bằng file Batch (Khuyên Dùng)
Ở thư mục gốc của dự án, nhấp đúp vào:
- **`run-desktop.bat`**: Chạy trực tiếp ứng dụng Desktop Windows với đầy đủ dữ liệu mẫu (Admin, Manager, Sinh viên, Điểm danh QR & Ảnh).
- **`run-android.bat`**: Build và deploy chạy trên Android Emulator.

#### 👉 Cách 2: Mở bằng Visual Studio 2022
1. Mở file giải pháp **`CampusActivitiesManager.sln`**.
2. Chọn project khởi động:
   - **`CampusActivitiesManager`** (hoặc profile *MAUI Windows (Stand-alone)*): Chạy trực tiếp app MAUI độc lập.
   - Hoặc chọn profile **`Run All (API + MAUI Windows)`** nếu muốn chạy cả Backend REST API song song.
3. Nhấn **F5** (hoặc nút **Start** trên thanh công cụ) để chạy.

#### 👉 Cách 3: Chạy bằng Dòng lệnh CLI
Mở Terminal tại thư mục dự án và gõ:
```bash
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

*(Bạn cũng có thể tự do nhấn **Đăng ký (Register)** trên màn hình Login để tạo tài khoản sinh viên mới).*

---

## 🏗️ Cấu Trúc Dự Án

```
CompusActivitiesManager/
├── CampusActivitiesManager/              # Ứng dụng chính .NET MAUI (Chạy độc lập All-in-One)
│   └── CampusActivitiesManager/
│       ├── Pages/                        # Giao diện XAML (Home, Login, Events, Scan QR, Photo Check-in, Users)
│       ├── PageModels/                   # ViewModels xử lý logic theo mô hình MVVM
│       ├── Services/                     # Tầng Service tích hợp (Auth, Events, Attendance, Users)
│       └── Resources/                    # Bảng màu Xanh Dương chuẩn Brand, Style, Fonts, Icons
├── CampusActivitiesManager.Api/          # Backend ASP.NET Core Web API (Phục vụ mở rộng REST API / Firebase)
├── CampusActivitiesManager.Api.Tests/    # Bộ kiểm thử tự động Integration Tests (xUnit - 25 Tests Passed)
├── run-desktop.bat                       # Script 1-click chạy Desktop Windows
├── run-android.bat                       # Script 1-click chạy Android Emulator
└── run-api.bat                           # Script chạy độc lập Backend API Server
```

---

## 🧪 Kiểm Thử Tự Động (Integration Tests)

Chạy toàn bộ 25 bài kiểm thử tích hợp:
```bash
dotnet test CampusActivitiesManager.Api.Tests
```
Kết quả: **25/25 Tests Passed (100% Passed)**.
