# Nhiệm vụ: T17.1 - Xây dựng API xác thực mã QR điểm danh (POST /api/v1/attendance/qr-checkin)

- **Mã Task**: `T17.1`
- **User Story**: `US17`
- **Người phụ trách**: **Nguyễn Đức Mạnh** (API / Backend (.ASP.NET Core Web API, Firebase Admin SDK))
- **Thời gian ước tính**: `6 giờ`
- **Sprint**: `Sprint_03`

## 1. Tiêu chí nghiệm thu (Acceptance Criteria)
Tiếp nhận chuỗi QR vé định dạng chuẩn, xác thực vé hợp lệ, tránh quét trùng lặp, ghi nhận điểm danh và cộng điểm rèn luyện.

## 2. Danh mục mã nguồn trích xuất
- `AttendanceController.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Controllers/AttendanceController.cs`)
- `AttendanceModels.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Models/AttendanceModels.cs`)
- `FirebaseAttendanceService.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Services/FirebaseAttendanceService.cs`)
- `IFirebaseAttendanceService.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Services/IFirebaseAttendanceService.cs`)
