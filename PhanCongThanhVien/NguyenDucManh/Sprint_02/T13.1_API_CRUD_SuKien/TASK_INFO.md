# Nhiệm vụ: T13.1 - Xây dựng API CRUD cho sự kiện

- **Mã Task**: `T13.1`
- **User Story**: `US13`
- **Người phụ trách**: **Nguyễn Đức Mạnh** (API / Backend (.ASP.NET Core Web API, Firebase Admin SDK))
- **Thời gian ước tính**: `8 giờ`
- **Sprint**: `Sprint_02`

## 1. Tiêu chí nghiệm thu (Acceptance Criteria)
API GET/POST/PUT/DELETE /events cho phép lấy danh sách, chi tiết, tạo mới, chỉnh sửa và xóa sự kiện trên Firebase Firestore.

## 2. Danh mục mã nguồn trích xuất
- `EventsController.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Controllers/EventsController.cs`)
- `EventModels.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Models/EventModels.cs`)
- `FirebaseEventService.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Services/FirebaseEventService.cs`)
- `IFirebaseEventService.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Services/IFirebaseEventService.cs`)
