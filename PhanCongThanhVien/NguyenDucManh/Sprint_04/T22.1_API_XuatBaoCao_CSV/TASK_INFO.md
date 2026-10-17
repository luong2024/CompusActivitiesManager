# Nhiệm vụ: T22.1 - Xây dựng API và Service xuất file báo cáo điểm danh chuẩn CSV UTF-8 có BOM

- **Mã Task**: `T22.1`
- **User Story**: `US22`
- **Người phụ trách**: **Nguyễn Đức Mạnh** (API / Backend (.ASP.NET Core Web API, Firebase Admin SDK))
- **Thời gian ước tính**: `4 giờ`
- **Sprint**: `Sprint_04`

## 1. Tiêu chí nghiệm thu (Acceptance Criteria)
Xuất file CSV chứa BOM UTF-8 không lỗi font khi mở bằng Microsoft Excel, đầy đủ thông tin sinh viên, thời gian và điểm.

## 2. Danh mục mã nguồn trích xuất
- `AttendanceController.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Controllers/AttendanceController.cs`)
- `FirebaseAttendanceService.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Services/FirebaseAttendanceService.cs`)
