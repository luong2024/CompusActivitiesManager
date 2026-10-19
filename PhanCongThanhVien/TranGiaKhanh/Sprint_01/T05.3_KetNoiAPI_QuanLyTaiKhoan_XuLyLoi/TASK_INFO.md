# Nhiệm vụ: T05.3 - Kết nối giao diện quản lý tài khoản với API & xử lý thông báo lỗi

- **Mã Task**: `T05.3`
- **User Story**: `US05`
- **Người phụ trách**: **Trần Gia Khánh** (Integration / Client Services & Core (.NET MAUI, HTTP, SecureStorage, AppShell))
- **Thời gian ước tính**: `3 giờ`
- **Sprint**: `Sprint_01`

## 1. Tiêu chí nghiệm thu (Acceptance Criteria)
Xây dựng UserService và HttpHelper gọi API /accounts, xử lý lỗi mạng và hiển thị dialog thông báo lỗi.

## 2. Danh mục mã nguồn trích xuất
- `UserService.cs` (Nguồn gốc: `CampusActivitiesManager/CampusActivitiesManager/Services/UserService.cs`)
- `IUserService.cs` (Nguồn gốc: `CampusActivitiesManager/CampusActivitiesManager/Services/IUserService.cs`)
- `HttpHelper.cs` (Nguồn gốc: `CampusActivitiesManager/CampusActivitiesManager/Services/HttpHelper.cs`)
- `IErrorHandler.cs` (Nguồn gốc: `CampusActivitiesManager/CampusActivitiesManager/Services/IErrorHandler.cs`)
- `ModalErrorHandler.cs` (Nguồn gốc: `CampusActivitiesManager/CampusActivitiesManager/Services/ModalErrorHandler.cs`)
