# Nhiệm vụ: T28.4 - API Đăng nhập và xác thực phiên làm việc

- **Mã Task**: `T28.4`
- **User Story**: `US28`
- **Người phụ trách**: **Nguyễn Đức Mạnh** (API / Backend (.ASP.NET Core Web API, Firebase Admin SDK))
- **Thời gian ước tính**: `1 giờ`
- **Sprint**: `Sprint_01`

## 1. Tiêu chí nghiệm thu (Acceptance Criteria)
API POST /auth/login kiểm tra thông tin đăng nhập, trả về JWT Token và thông tin phân quyền người dùng.

## 2. Danh mục mã nguồn trích xuất
- `AuthController.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Controllers/AuthController.cs`)
- `LoginRequest.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Models/LoginRequest.cs`)
- `LoginResponse.cs` (Nguồn gốc: `CampusActivitiesManager.Api/Models/LoginResponse.cs`)
