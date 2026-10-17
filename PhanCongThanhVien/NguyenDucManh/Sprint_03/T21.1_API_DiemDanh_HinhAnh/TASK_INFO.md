# Nhiệm vụ: T21.1 - Xây dựng API điểm danh bằng ảnh minh chứng (POST /photo-checkin) và điểm danh thủ công

- **Mã Task**: `T21.1`
- **User Story**: `US21`
- **Người phụ trách**: **Nguyễn Đức Mạnh** (API / Backend (.ASP.NET Core Web API, Firebase Admin SDK))
- **Thời gian ước tính**: `5 giờ`
- **Sprint**: `Sprint_03`

## 1. Tiêu chí nghiệm thu (Acceptance Criteria)
Tiếp nhận URL/base64 ảnh minh chứng, lưu trữ mốc thời gian; hỗ trợ endpoint điểm danh thủ công cho BTC.

## 2. Danh mục mã nguồn trích xuất
- `AttendanceController.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Controllers/AttendanceController.cs`)
- `AttendanceModels.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Models/AttendanceModels.cs`)
- `FirebaseAttendanceService.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Services/FirebaseAttendanceService.cs`)
