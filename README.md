# Campus Activity Manager 🎓✨
> Ứng dụng Quản lý Hoạt động Ngoại khóa, Sự kiện & Điểm danh Sinh viên xây dựng bằng **.NET 9 MAUI** (Android APK & Windows Desktop) trên **Kiến trúc Serverless Cloud-Native** kết nối trực tiếp **Firebase Cloud Services** (Cloud Firestore & Firebase Authentication).
> 
> **Tác giả & Tech Lead**: **Nguyễn Đức Mạnh**

---

## 📋 Tài Liệu Dự Án Chuẩn Hóa
- 📖 [**Kế Hoạch Dự Án & 38 User Stories (Plan.md)**](file:///c:/Users/Admin/source/repos/CompusActivitiesManager/Plan.md): Toàn bộ Product Backlog chuẩn hóa 100% theo mã nguồn thực tế, 4 Sprints và ma trận phân công trách nhiệm.
- 📐 [**Đặc Tả Kỹ Thuật Nghiệp Vụ (BA.md)**](file:///c:/Users/Admin/source/repos/CompusActivitiesManager/BA.md): Kịch bản kiểm thử hành vi BDD Gherkin, cấu trúc `ApiResponse<T>`, quy chuẩn PBKDF2-SHA256 và OAuth2 Service Account.
- 🎓 [**Sổ Tay Phản Biện & Bảo Vệ Đồ Án**](file:///C:/Users/Admin/.gemini/antigravity/brain/c9b1bcd2-6c19-4dd9-9e98-3645ef844cca/so_tay_bao_ve_du_an_va_phan_bien_ai.md): Bộ câu hỏi và trả lời phỏng vấn kỹ thuật đồ án dành cho sinh viên **Nguyễn Đức Mạnh**.
- 📜 [**Quy Tắc Phát Triển & DoD (rule.md)**](file:///c:/Users/Admin/source/repos/CompusActivitiesManager/rule.md): Tiêu chuẩn Definition of Done (DoD1 - DoD6), kiến trúc MVVM, bảng màu Xanh Dương thương hiệu `#2563EB`.

---

## ⚡ Hướng Dẫn Chạy Nhanh Cho Người Mới (Quick Start)

Dự án được thiết kế **100% Native .NET MAUI thuần (All-in-One)** kết nối trực tiếp Firebase Cloud:  
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
- **`run-uitests.bat`**: Chạy toàn bộ 15 kịch bản kiểm thử tự động Android UI Test với Appium.

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
| **Quản trị viên (Admin)** | `admin` *(hoặc `admin@campus.edu.vn`)* | `Admin@123456` | Toàn quyền: Quản trị sự kiện, Quản lý tài khoản người dùng, Khóa/Mở khóa tài khoản, Thống kê điểm danh |
| **Quản lý sự kiện (Manager)** | `manager` *(hoặc `manager@campus.edu.vn`)* | `Manager@123456` | Tạo sự kiện, Quản trị hoạt động, Giám sát check-in thời gian thực, Xuất báo cáo CSV UTF-8 |
| **Sinh viên (Student)** | `B20DCCN001` *(hoặc `manh.nd@campus.edu.vn`)* | `Student@123456` | Xem sự kiện trang chủ, Quét mã QR Camera thực tế (ZXing), Điểm danh ảnh minh chứng, Sổ cái Điểm Rèn Luyện |

*(Bạn cũng có thể tự do nhấn **Đăng ký (Register)** trên màn hình Login để tạo tài khoản sinh viên mới).*

---

## 🏗️ Cấu Trúc Dự Án Thuần MAUI

```
CompusActivitiesManager/
├── CampusActivitiesManager/              # Thư mục mã nguồn DUY NHẤT của dự án (.NET 9 MAUI)
│   └── CampusActivitiesManager/
│       ├── Pages/                        # Giao diện XAML (Trang chủ, Đăng nhập, Sự kiện, Quét QR, Điểm danh ảnh...)
│       ├── PageModels/                   # ViewModels xử lý logic theo mô hình MVVM
│       ├── BusinessLogic/
│       │   ├── Interfaces/               # Interfaces nghiệp vụ (IUserService, IEventService, IAttendanceService, IQrCodeService, IFirebaseSyncService)
│       │   └── Services/                 # Hiện thực dịch vụ: kết nối Firebase Cloud Firestore, Auth, ZXing, PBKDF2
│       ├── Models/                       # Đối tượng dữ liệu nghiệp vụ (User, CampusEvent, AttendanceRecord, Category...)
│       ├── Data/                         # Cấu hình FirebaseConfig, CategoryRepository, ProjectRepository
│       └── Resources/                    # Bảng màu Xanh Dương chuẩn Brand, Style, Fonts, Icons
├── CampusActivitiesManagerTest/          # Bộ kiểm thử Unit Test (80 test methods)
├── UITests.Android/                      # Bộ kiểm thử tự động Android UI Test với Appium (15 kịch bản E2E)
├── CampusActivitiesManager.sln           # File giải pháp Solution .NET 9
├── Plan.md                               # Kế hoạch dự án & 38 User Stories chuẩn hóa
├── BA.md                                 # Đặc tả kỹ thuật & kịch bản BDD Gherkin
├── rule.md                               # Quy tắc phát triển & tiêu chuẩn DoD
├── run-desktop.bat                       # Script 1-click chạy Desktop Windows
├── run-android.bat                       # Script 1-click chạy Android APK
└── run-uitests.bat                       # Script chạy Appium UI Tests
```
