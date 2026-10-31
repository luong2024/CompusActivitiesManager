# Vấn đề đã biết trong backlog & Phương án xử lý chuẩn hóa (Đã giải quyết)

Tất cả các vấn đề lịch sử dưới đây đã được **xử lý và chuẩn hóa triệt để 100%** trong tài liệu [`Plan.md`](file:///c:/Users/Admin/source/repos/CompusActivitiesManager/Plan.md) và [`BA.md`](file:///c:/Users/Admin/source/repos/CompusActivitiesManager/BA.md) để khớp hoàn toàn với mã nguồn thực tế của dự án:

1. **AC sai domain (Đã giải quyết):** T15.1 (`US15`) đã được chuẩn hóa về đúng **Tag phân loại danh mục sự kiện** (`Hội thảo`, `Tình nguyện`, `Thể thao`, `Văn nghệ`, `Học thuật`), loại bỏ hoàn toàn các mô tả lạc đề về trạng thái sinh viên Đang học/Bảo lưu.
2. **T37.1 - Dịch vụ Đăng nhập & Auth (Đã giải quyết):** Đã tách bạch với đăng ký, hiện thực trong `AuthenticationService.cs` với kiểm tra mật khẩu mã hóa PBKDF2, từ chối `403 Forbidden` khi tài khoản bị khóa (`IsActive == false`), lưu phiên làm việc an toàn trong `SecureStorage`.
3. **Trùng/thiếu Task ID (Đã giải quyết):** Chuẩn hóa `T35.1` (Tạo & Sửa hồ sơ) và `T35.2` (Khóa & Mở khóa tài khoản). Bổ sung đầy đủ chi tiết cho các task trong `Plan.md`.
4. **Số hiệu AC lệch (Đã giải quyết):** Chuẩn hóa toàn bộ nhãn AC từ AC1 đến AC6 đồng nhất theo mô hình BDD Gherkin trong `BA.md` và `Plan.md`.
5. **Quyền sở hữu Task (Đã giải quyết):** Xác định rõ ràng Tech Lead & Assignee chính là **Nguyễn Đức Mạnh** phụ trách các dịch vụ và màn hình nghiệp vụ trọng yếu của hệ thống.
6. **Công nghệ mâu thuẫn (Đã giải quyết):** Thống nhất mô hình **Serverless Cloud-Native**: Ứng dụng .NET MAUI kết nối trực tiếp **Google Cloud Firestore REST API** và **Firebase Authentication** thông qua `FirebaseSyncService` (OAuth2 RS256 Service Account exchange, mã hóa `PBKDF2-SHA256`). Loại bỏ hoàn toàn server Web API trung gian cũ.
7. **US ngoài phạm vi (Đã giải quyết):**
   - `US02`: Chuẩn hóa thành Lọc sự kiện theo Đơn vị tổ chức / Giảng viên phụ trách.
   - `US09`: Chuẩn hóa thành Quản lý danh sách đăng ký theo Lớp/Khoa (`ClassHubPage.xaml`).
   - `US14`: Chuẩn hóa thành Đồng hồ đếm ngược và thanh đo sức chứa sự kiện.
   - `US01`/`US03`/`US11`/`US12`: Gom nhóm phân cấp trải nghiệm chi tiết sự kiện cho Sinh viên và Quản trị viên (RBAC).
8. **Rủi ro & Kế hoạch (Đã giải quyết):** Đã bổ sung các phân tích rủi ro và biện pháp giảm thiểu trong sổ tay đồ án.
9. **Điều hướng Shell Route (Đã giải quyết):** Chuẩn hóa toàn bộ điều hướng theo chuẩn .NET MAUI Shell (`//AppShell/...`, `GoToAsync`), không sử dụng URL dạng web browser.
