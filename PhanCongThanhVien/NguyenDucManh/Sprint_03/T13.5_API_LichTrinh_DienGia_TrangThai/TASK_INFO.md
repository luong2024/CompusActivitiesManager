# Nhiệm vụ: T13.5 - Nâng cấp API sự kiện: bổ sung lịch trình (timeline), diễn giả, vị trí và trạng thái điểm danh

- **Mã Task**: `T13.5`
- **User Story**: `US13`
- **Người phụ trách**: **Nguyễn Đức Mạnh** (API / Backend (.ASP.NET Core Web API, Firebase Admin SDK))
- **Thời gian ước tính**: `5 giờ`
- **Sprint**: `Sprint_03`

## 1. Tiêu chí nghiệm thu (Acceptance Criteria)
Mở rộng EventDto chứa lịch trình chi tiết, thông tin diễn giả và lọc sự kiện theo trạng thái (Sắp diễn ra, Đang diễn ra, Đã kết thúc).

## 2. Danh mục mã nguồn trích xuất
- `EventsController.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Controllers/EventsController.cs`)
- `EventModels.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Models/EventModels.cs`)
- `FirebaseEventService.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Services/FirebaseEventService.cs`)
