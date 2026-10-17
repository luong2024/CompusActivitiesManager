# Nhiệm vụ: T19.1 - Xây dựng API lịch sử điểm danh theo sinh viên (GET /api/v1/attendance/student/{code})

- **Mã Task**: `T19.1`
- **User Story**: `US19`
- **Người phụ trách**: **Nguyễn Đức Mạnh** (API / Backend (.ASP.NET Core Web API, Firebase Admin SDK))
- **Thời gian ước tính**: `5 giờ`
- **Sprint**: `Sprint_04`

## 1. Tiêu chí nghiệm thu (Acceptance Criteria)
Truy vấn danh sách toàn bộ các sự kiện sinh viên đã điểm danh, phương thức thực hiện và tính tổng điểm rèn luyện tích lũy.

## 2. Danh mục mã nguồn trích xuất
- `AttendanceController.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Controllers/AttendanceController.cs`)
- `AttendanceModels.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Models/AttendanceModels.cs`)
- `FirebaseAttendanceService.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Services/FirebaseAttendanceService.cs`)
