# Nhiệm vụ: T28.3 - Lưu trữ token an toàn trên thiết bị (SecureStorage)

- **Mã Task**: `T28.3`
- **User Story**: `US28`
- **Người phụ trách**: **Trần Gia Khánh** (Integration / Client Services & Core (.NET MAUI, HTTP, SecureStorage, AppShell))
- **Thời gian ước tính**: `2 giờ`
- **Sprint**: `Sprint_01`

## 1. Tiêu chí nghiệm thu (Acceptance Criteria)
Sử dụng SecureStorage.Default lưu AccessToken và User Info, tự động đính kèm Bearer token vào HttpHelper.

## 2. Danh mục mã nguồn trích xuất
- `AuthService.cs` (Nguồn gốc: `CampusActivitiesManager/CampusActivitiesManager/Services/AuthService.cs`)
- `HttpHelper.cs` (Nguồn gốc: `CampusActivitiesManager/CampusActivitiesManager/Services/HttpHelper.cs`)
