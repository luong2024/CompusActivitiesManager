# Nhiệm vụ: T21.3 - Tích hợp dịch vụ điểm danh ảnh, kết nối API và xử lý lưu trữ ngoại tuyến

- **Mã Task**: `T21.3`
- **User Story**: `US21`
- **Người phụ trách**: **Trần Gia Khánh** (Integration / Client Services & Core (.NET MAUI, HTTP, SecureStorage, AppShell))
- **Thời gian ước tính**: `3 giờ`
- **Sprint**: `Sprint_03`

## 1. Tiêu chí nghiệm thu (Acceptance Criteria)
AttendanceService gửi ảnh và dữ liệu check-in lên API, xử lý offline fallback khi mất kết nối.

## 2. Danh mục mã nguồn trích xuất
- `IAttendanceService.cs` (Nguồn gốc: `CampusActivitiesManager/CampusActivitiesManager/Services/IAttendanceService.cs`)
- `AttendanceService.cs` (Nguồn gốc: `CampusActivitiesManager/CampusActivitiesManager/Services/AttendanceService.cs`)
