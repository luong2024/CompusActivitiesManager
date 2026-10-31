# Vai worker và vùng file

Đường dẫn thực tế trong dự án `CompusActivitiesManager` (`.NET MAUI` + `CampusActivitiesManager.Api` + `CampusActivitiesManager.Api.Tests`):

| role | Subagent | Làm gì | Vùng ghi thực tế | Task trong plan |
|---|---|---|---|---|
| `api` | `campus-worker-api` / `campus-worker` | Endpoint, validation, JWT, RBAC, DB/Firebase, HTTP status | `CampusActivitiesManager.Api/**` | T35.x, T36.1, T37.1, T38.1, T13.1, T33.1, T07.1 |
| `ui` | `campus-worker-ui` / `campus-worker` | XAML MAUI, style, responsive, dữ liệu mock | `CampusActivitiesManager/CampusActivitiesManager/Views/**`, `CampusActivitiesManager/CampusActivitiesManager/Resources/**`, `CampusActivitiesManager/CampusActivitiesManager/Converters/**` | T05.1, T26.1, T28.1, T15.1, T31.1, T01.1, T13.2, T33.2, T07.2 |
| `integration` | `campus-worker-integration` / `campus-worker` | Tầng gọi API, gắn token, xử lý lỗi, SecureStorage, điều hướng, validate client, file chung | `CampusActivitiesManager/CampusActivitiesManager/Services/**`, `CampusActivitiesManager/CampusActivitiesManager/ViewModels/**`, `CampusActivitiesManager/CampusActivitiesManager/Models/**`, `CampusActivitiesManager/CampusActivitiesManager/MauiProgram.cs`, `CampusActivitiesManager/CampusActivitiesManager/AppShell.xaml*` | T05.3, T26.3, T26.4, T28.3, T28.4, T31.2, T01.2, T13.3, T07.3 |
| `qa` | `campus-worker-qa` / `campus-worker` | Unit/integration/manual test theo AC | `CampusActivitiesManager.Api.Tests/**` | T05.4, T26.5, T28.5, T15.3, T31.3, T01.3, T13.4, T33.3 |

DTO/model dùng chung: `CampusActivitiesManager/CampusActivitiesManager/Models/**` và `CampusActivitiesManager.Api/Models/**`, chỉ đổi qua thay đổi contract.

Cột "Người phụ trách" trong plan là con người sở hữu task. Worker làm cho người đó; ghi `owner` vào brief và báo cáo, người đó duyệt kết quả cuối.

Nhánh: `agent/<role>/<task-id>` (hoặc nhánh task theo quy ước dự án trong `rule.md`). Mỗi worker một nhánh để DoD4 kiểm soát được từng đóng góp.

## Quy ước mã trạng thái HTTP (từ AC trong plan)
201 tạo thành công; 400 dữ liệu sai/thiếu; 401 sai thông tin đăng nhập; 403 tài khoản bị khóa (`ACCOUNT_LOCKED`) hoặc thiếu quyền; 404 không tồn tại; 409 trùng (email).
