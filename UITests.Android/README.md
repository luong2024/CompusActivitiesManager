# Bộ Kiểm Thử Tự Động Giao Diện Android (UITests.Android)

Dự án kiểm thử tự động giao diện người dùng (UI Test / E2E) trên nền tảng **Android** cho ứng dụng **Campus Activities Manager** (.NET MAUI) sử dụng **Appium WebDriver**, **UIAutomator2** và **NUnit 4**.

---

## 1. Cấu trúc Thư mục

```
UITests.Android/
├── Configuration/
│   └── AppiumConfig.cs               # Cấu hình IP, Cổng Appium, Package Name, Activity, Timeouts
├── Core/
│   ├── AppiumDriverFactory.cs        # Khởi tạo AndroidDriver, kiểm tra cổng Server
│   └── BaseAndroidUITest.cs          # Base fixture: vòng đời Driver, explicit waits, screenshot
├── Pages/                            # Page Object Model (POM)
│   ├── LoginPageObject.cs            # Màn hình Đăng nhập
│   ├── RegisterPageObject.cs         # Màn hình Đăng ký
│   ├── MainPageObject.cs             # Trang chủ, Bottom TabBar và Flyout Menu
│   └── EventManagementPageObject.cs  # Màn hình Quản lý & Tổ chức sự kiện
├── Tests/                            # Kịch bản kiểm thử (Test Cases)
│   ├── LoginUITests.cs               # 7 test case luồng đăng nhập, validation, chuyển trang
│   ├── RegisterUITests.cs            # 3 test case luồng đăng ký tài khoản sinh viên
│   ├── NavigationUITests.cs          # 3 test case chuyển 5 bottom tabs & tìm kiếm sự kiện
│   └── ProfileAndLogoutUITests.cs    # 2 test case hồ sơ cá nhân & quy trình đăng xuất
├── run-uitests.bat                   # Script chạy nhanh kiểm thử
└── UITests.Android.csproj            # File cấu hình .NET 9
```

---

## 2. Danh mục 15 Kịch bản Kiểm thử (Test Cases)

| Mã Test | Tên Test Case | Mô tả kiểm thử |
|---|---|---|
| `TC_UI_01` | `LoginPage_ShouldDisplayAllRequiredFields` | Kiểm tra đầy đủ logo, các ô nhập Username, Password, nút Đăng nhập, nút Đăng ký |
| `TC_UI_02` | `Login_EmptyFields_ShouldShowValidationError` | Bấm Đăng nhập khi để trống -> Hiển thị thông báo validation |
| `TC_UI_03` | `Login_InvalidCredentials_ShouldDisplayErrorMessage` | Nhập sai mật khẩu -> Báo lỗi từ chối đăng nhập màu đỏ |
| `TC_UI_04` | `Login_TogglePasswordVisibility_ShouldWork` | Bấm nút con mắt xem mật khẩu -> Chuyển trạng thái ẩn/hiện an toàn |
| `TC_UI_05` | `Login_ClickRegisterLink_ShouldNavigateToRegisterPage` | Nhấn "Đăng ký ngay" -> Chuyển sang màn hình Đăng ký tài khoản |
| `TC_UI_06` | `Login_ValidStudentAccount_ShouldNavigateToMainPage` | Đăng nhập tài khoản sinh viên `B20DCCN001` -> Vào trang chủ, chào đúng tên |
| `TC_UI_07` | `Login_ValidAdminAccount_ShouldShowAdminFeatures` | Đăng nhập tài khoản `admin` -> Xuất hiện tính năng Quản lý người dùng |
| `TC_UI_08` | `RegisterPage_ShouldDisplayAllRegistrationFields` | Kiểm tra màn hình đăng ký có đủ Họ tên, MSSV, Email, SĐT, Lớp, Mật khẩu |
| `TC_UI_09` | `Register_EmptyFields_ShouldPreventSubmissionOrShowError` | Gửi form đăng ký rỗng -> Báo lỗi validation bắt buộc |
| `TC_UI_10` | `Register_ClickBackToLogin_ShouldReturnToLoginPage` | Bấm nút quay lại -> Điều hướng trở lại màn hình Đăng nhập |
| `TC_UI_11` | `TabNavigation_ShouldSwitchBetweenTabs` | Chuyển đổi qua lại giữa 5 Bottom Tabs (Trang chủ, Sự kiện, Quét QR, Lớp học, Cá nhân) |
| `TC_UI_12` | `EventManagement_SearchEvents_ShouldFilterList` | Nhập từ khóa tìm kiếm sự kiện -> Lọc danh sách sự kiện theo thời gian thực |
| `TC_UI_13` | `EventManagement_FilterChips_ShouldSelectCategories` | Chạm vào các chip lọc danh mục sự kiện (Tất cả, Đang tham gia) |
| `TC_UI_14` | `ProfilePage_ShouldDisplayUserInfo` | Vào trang cá nhân -> Hiển thị đúng Họ tên, SĐT, Nút lưu thay đổi |
| `TC_UI_15` | `Logout_ShouldReturnUserToLoginPage` | Mở Menu -> Bấm Đăng xuất -> Xác nhận hộp thoại -> Quay lại màn hình Đăng nhập |

---

## 3. Hướng dẫn Chạy Kiểm Thử UI trên Máy tính

### Bước 1: Mở máy ảo Android Emulator
Đảm bảo máy ảo Android đã được bật và kết nối:
```cmd
adb devices
```
*(Kết quả hiển thị `emulator-5554 device` là sẵn sàng)*

### Bước 2: Build & Cài ứng dụng lên Emulator
```cmd
dotnet build CampusActivitiesManager\CampusActivitiesManager\CampusActivitiesManager.csproj -t:Run -f net9.0-android
```

### Bước 3: Khởi động Appium Server
Cài đặt Appium (nếu chưa có):
```cmd
npm install -g appium
appium driver install uiautomator2
```
Khởi động Appium Server trên cổng mặc định 4723:
```cmd
appium
```

### Bước 4: Thực thi kiểm thử UI
Chạy toàn bộ bộ test:
```cmd
dotnet test UITests.Android\UITests.Android.csproj
```
Hoặc chạy riêng nhóm test UI mong muốn:
```cmd
dotnet test UITests.Android\UITests.Android.csproj --filter "Category=US28_Login"
```
Hoặc click đúp file `run-uitests.bat` trong thư mục này.

*Lưu ý: Nếu Appium Server chưa được bật, các test case sẽ tự động chuyển sang trạng thái Ignore an toàn, không làm hỏng quy trình build của solution.*
