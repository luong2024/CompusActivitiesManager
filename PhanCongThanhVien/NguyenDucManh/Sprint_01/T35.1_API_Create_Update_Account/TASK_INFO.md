# Nhiệm vụ: T35.1 - Xây dựng Create Account + Update Account API với Firebase Admin SDK

- **Mã Task**: `T35.1`
- **User Story**: `US35`
- **Người phụ trách**: **Nguyễn Đức Mạnh** (API / Backend (.ASP.NET Core Web API, Firebase Admin SDK))
- **Thời gian ước tính**: `10 giờ`
- **Sprint**: `Sprint_01`

## 1. Tiêu chí nghiệm thu (Acceptance Criteria)
API POST/PUT /accounts tạo mới và cập nhật thông tin tài khoản, validate dữ liệu, đồng bộ giữa Firebase Auth và Firestore CSDL.

## 2. Danh mục mã nguồn trích xuất
- `AccountsController.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Controllers/AccountsController.cs`)
- `CreateAccountRequest.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Models/CreateAccountRequest.cs`)
- `UpdateAccountRequest.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Models/UpdateAccountRequest.cs`)
- `UserAccountDto.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Models/UserAccountDto.cs`)
- `FirebaseAccountService.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Services/FirebaseAccountService.cs`)
- `IFirebaseAccountService.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Services/IFirebaseAccountService.cs`)
