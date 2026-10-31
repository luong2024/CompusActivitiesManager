# KẾ HOẠCH DỰ ÁN & PRODUCT BACKLOG CHUẨN HÓA (PLAN.MD)
## Dự án: Campus Activity Manager (.NET MAUI Mobile App & Firebase Serverless Cloud Services)
**Tác giả & Tech Lead**: **Nguyễn Đức Mạnh**  
**Cập nhật lần cuối**: 31/10/2026 (Đồng bộ chuẩn hóa 100% theo mã nguồn thực tế)

---

## 1. TỔNG QUAN ĐỐI SOÁT & ĐIỀU CHỈNH PLAN THEO MÃ NGUỒN HIỆN TẠI

Trước đây, tài liệu Backlog/Plan ban đầu tồn tại một số điểm không khớp với mã nguồn thực tế. Bảng dưới đây tóm tắt các điểm đã được chuẩn hóa lại toàn diện:

| Hạng mục đối soát | Mô tả trong Plan cũ (Lạc hậu / Lệch domain) | Mã nguồn thực tế hiện tại (Đã chuẩn hóa) |
|---|---|---|
| **Kiến trúc hệ thống** | Giả định có Web API ASP.NET Core trung gian với controller `/api/v1/...`, DB SQL Server/InMemory, JWT tự ký. | **Serverless Cloud-Native**: .NET MAUI kết nối trực tiếp **Google Cloud Firestore REST API** và **Firebase Authentication** thông qua `FirebaseSyncService` (Google OAuth2 RS256 Service Account exchange, mã hóa `PBKDF2-SHA256`, `SecureStorage`). Không có Web API trung gian thừa. |
| **Domain nghiệp vụ** | Bị lẫn các US về "đề tài tốt nghiệp/NCKH", "giảng viên hướng dẫn", "ghép nhóm đề tài" (`US02`, `US09`, `US14`). | Chuẩn hóa 100% về **Quản lý Hoạt động & Sự kiện Campus**: Quản lý sự kiện, phân loại danh mục, đăng ký tham gia, quét mã QR, điểm danh ảnh minh chứng, sổ cái Điểm Rèn Luyện (ĐRL). |
| **Tag sự kiện (`US15`)** | AC cũ ghi "trạng thái sinh viên: Đang học/Bảo lưu/Cảnh cáo/Tốt nghiệp". | Chuẩn hóa thành **Tag phân loại danh mục sự kiện**: `Hội thảo`, `Tình nguyện`, `Thể thao`, `Văn nghệ`, `Học thuật`. |
| **Điểm danh QR (`US17`)** | Ban đầu chỉ mô tả mock chuỗi QR hoặc nhập chuỗi giả lập. | Tích hợp **ZXing.Net.Maui.Controls** sử dụng Camera phần cứng trực tiếp của thiết bị Android/Windows, quét tự động và gửi ghi nhận điểm danh thời gian thực lên Firebase. |
| **Điểm danh minh chứng (`US21`)** | Chỉ là hàm cập nhật thủ công thô sơ. | Triển khai **Điểm danh bằng ảnh chụp (Photo Attendance)** tích hợp `MediaPicker`, xem trước ảnh, nhập ghi chú minh chứng và có nút Fallback điểm danh thủ công. |
| **Thông báo (`US18`/`US04`)** | Chỉ mô tả pop-up alert đơn giản. | Triển khai **Modal thông báo 2 tầng chuyên nghiệp** (`EventNotificationModal`) với tiêu đề ghim trên, huy hiệu trạng thái, vùng cuộn nội dung 250px và nút đóng cố định ở đáy. |
| **Lịch sử & ĐRL (`US19`)** | Chỉ xem lịch sử điểm danh đơn thuần. | Triển khai **Sổ cái Điểm Rèn Luyện (ĐRL Ledger)**: Tổng hợp điểm rèn luyện tích lũy, phân nhóm sự kiện đã tham gia, trạng thái xác thực. |
| **Xuất báo cáo (`US22`)** | API trả về tệp CSV tải về qua web. | **Xuất báo cáo CSV UTF-8 kèm BOM** ngay trên thiết bị di động, tự động kích hoạt trình chia sẻ tệp của HĐH (Zalo, Drive, Gmail) và hiển thị tiếng Việt chuẩn trên Excel. |

---

## 2. DANH MỤC CHI TIẾT 38 USER STORIES (PRODUCT BACKLOG)

Dưới đây là danh sách đầy đủ 38 User Stories (US01 – US38) của dự án, được chuẩn hóa chi tiết về mục tiêu, tiêu chí chấp nhận (AC), vai trò người dùng và liên kết trực tiếp tới các lớp/dịch vụ trong mã nguồn.

### US01: Xem Thông Tin Chi Tiết Sự Kiện
- **User Story**: Là một sinh viên, tôi muốn xem đầy đủ thông tin chi tiết của một sự kiện để tôi có thể nắm rõ thời gian, địa điểm, sức chứa và nội dung trước khi quyết định tham gia.
- **Tiêu chí chấp nhận (AC)**:
  - **AC1.1**: Nhấp vào một thẻ sự kiện từ trang chủ điều hướng mượt mà đến `EventDetailPage` qua Shell navigation.
  - **AC1.2**: Hiển thị đầy đủ: Tên sự kiện, banner/hình ảnh, thời gian bắt đầu/kết thúc, địa điểm, diễn giả/đơn vị tổ chức, số lượng đã đăng ký / sức chứa tối đa, điểm rèn luyện nhận được.
  - **AC1.3**: Hiển thị trạng thái đăng ký của cá nhân (Chưa đăng ký, Đã đăng ký, Đã điểm danh).
- **Mã nguồn hiện thực**: `EventDetailPage.xaml`, `EventDetailPageModel.cs`, `EventService.cs`.

### US02: Lọc Sự Kiện Theo Đơn Vị Tổ Chức / Giảng Viên Phụ Trách
- **User Story**: Là một sinh viên, tôi muốn lọc danh sách sự kiện theo Đơn vị tổ chức / Câu lạc bộ / Giảng viên phụ trách để dễ dàng tìm kiếm các hoạt động phù hợp với chuyên ngành hoặc sở thích.
- **Tiêu chí chấp nhận (AC)**:
  - **AC2.1**: Thanh công cụ tìm kiếm và bộ lọc cho phép chọn hoặc nhập tên đơn vị tổ chức.
  - **AC2.2**: Danh sách tự động làm mới và chỉ hiển thị các sự kiện do đơn vị đó điều hành.
- **Mã nguồn hiện thực**: `EventListPageModel.cs`, `EventService.cs`.

### US03: Xem Lịch Trình Chi Tiết & Hướng Dẫn Tham Gia Sự Kiện
- **User Story**: Là một người tham gia, tôi muốn xem lịch trình chi tiết từng khung giờ và lưu ý chuẩn bị của sự kiện để tôi tham gia đúng giờ và đạt hiệu quả tốt nhất.
- **Tiêu chí chấp nhận (AC)**:
  - **AC3.1**: Hiển thị khối nội dung "Lịch trình & Lưu ý" trên `EventDetailPage`.
  - **AC3.2**: Có cảnh báo rõ ràng nếu sự kiện yêu cầu trang phục hoặc điều kiện tham dự đặc biệt.
- **Mã nguồn hiện thực**: `EventDetailPage.xaml`.

### US04: Nhận Thông Báo Nhắc Nhở Sự Kiện Sắp Diễn Ra
- **User Story**: Là một sinh viên đã đăng ký sự kiện, tôi muốn nhận được thông báo nhắc nhở trước khi sự kiện bắt đầu để không bị bỏ lỡ.
- **Tiêu chí chấp nhận (AC)**:
  - **AC4.1**: Hiển thị danh sách thông báo sự kiện trong `EventNotificationModal`.
  - **AC4.2**: Bố cục thông báo trực quan, hiển thị thời gian diễn ra và hướng dẫn check-in.
- **Mã nguồn hiện thực**: `EventNotificationModal` trong `MainPage.xaml`, `MainPageModel.cs`.

### US05: Quản Lý Danh Sách Tài Khoản Người Dùng (Admin)
- **User Story**: Là Quản trị viên, tôi muốn xem danh sách, tìm kiếm, lọc và quản lý trạng thái tài khoản của sinh viên và cán bộ trên hệ thống.
- **Tiêu chí chấp nhận (AC)**:
  - **AC5.1**: Trang `UserManagementPage` hiển thị danh sách người dùng lấy từ Cloud Firestore.
  - **AC5.2**: Cho phép tìm kiếm theo Tên, Email, Mã sinh viên và lọc theo Vai trò (`Admin`, `Manager`, `Student`).
  - **AC5.3**: Hỗ trợ phân trang / cuộn tải dữ liệu tối ưu, không giật lag.
- **Mã nguồn hiện thực**: `UserManagementPage.xaml`, `UserManagementPageModel.cs`, `UserService.cs`.

### US06: Lọc Sự Kiện Theo Danh Mục Hoạt Động
- **User Story**: Là một sinh viên, tôi muốn lọc danh sách sự kiện theo từng danh mục (Hội thảo, Thể thao, Tình nguyện...) để nhanh chóng tìm thấy hoạt động tôi yêu thích.
- **Tiêu chí chấp nhận (AC)**:
  - **AC6.1**: Thanh chọn tab danh mục ngang dạng pill chips ở đầu trang sự kiện.
  - **AC6.2**: Nhấp vào danh mục nào thì danh sách lập tức lọc theo danh mục đó, chọn "Tất cả" để xem toàn bộ.
- **Mã nguồn hiện thực**: `MainPage.xaml`, `EventListPage.xaml`, `CategoryRepository.cs`.

### US07: Xem & Cập Nhật Hồ Sơ Cá Nhân
- **User Story**: Là một người dùng, tôi muốn xem và cập nhật thông tin cá nhân (Họ tên, SĐT, Avatar) để giữ hồ sơ của tôi luôn chính xác.
- **Tiêu chí chấp nhận (AC)**:
  - **AC7.1**: `ProfilePage` hiển thị thông tin người dùng đăng nhập hiện tại từ `SecureStorage` và Firestore.
  - **AC7.2**: Cho phép chỉnh sửa Họ tên, Số điện thoại và cập nhật ảnh đại diện.
  - **AC7.3**: Nhấn "Lưu thay đổi", dữ liệu được đồng bộ lên Firebase và hiển thị thông báo thành công.
- **Mã nguồn hiện thực**: `ProfilePage.xaml`, `ProfilePageModel.cs`, `UserService.cs`.

### US08: Hỗ Trợ & Phản Hồi Trực Tiếp Với Ban Quản Trị
- **User Story**: Là một sinh viên, tôi muốn có kênh liên hệ hỗ trợ hoặc gửi thắc mắc đến Ban quản trị khi gặp sự cố về tài khoản hoặc điểm danh.
- **Tiêu chí chấp nhận (AC)**:
  - **AC8.1**: Trang thông tin cá nhân cung cấp mục "Trợ giúp & Hỗ trợ" kèm email hotline hoặc hộp thoại trợ giúp.
  - **AC8.2**: Khi xảy ra lỗi kết nối, `ModalErrorHandler` hiển thị hướng dẫn người dùng liên hệ hỗ trợ.
- **Mã nguồn hiện thực**: `ModalErrorHandler.cs`, `ProfilePage.xaml`.

### US09: Quản Lý Danh Sách Đăng Ký Theo Lớp & Khoa
- **User Story**: Là Ban tổ chức hoặc Quản trị viên, tôi muốn xem danh sách sinh viên đăng ký tham gia sự kiện được nhóm theo Lớp/Khoa để thuận tiện sắp xếp chỗ ngồi và điểm danh.
- **Tiêu chí chấp nhận (AC)**:
  - **AC9.1**: Danh sách người tham gia hiển thị rõ thông tin Mã SV, Lớp, Khoa.
  - **AC9.2**: Thống kê số lượng sinh viên tham dự theo từng lớp/khoa.
- **Mã nguồn hiện thực**: `ClassHubPage.xaml`, `ClassHubPageModel.cs`.

### US10: Dashboard Tổng Quan Thống Kê Hoạt Động (Admin)
- **User Story**: Là Quản trị viên, tôi muốn có màn hình tổng quan thống kê số lượng sự kiện, tổng số lượt tham gia và trạng thái hệ thống để điều hành hiệu quả.
- **Tiêu chí chấp nhận (AC)**:
  - **AC10.1**: Bảng điều khiển hiển thị số liệu: Sự kiện đang mở, Sự kiện đã kết thúc, Tổng số tài khoản, Tổng lượt điểm danh.
  - **AC10.2**: Cập nhật dữ liệu từ Cloud Firestore theo thời gian thực.
- **Mã nguồn hiện thực**: `MainPage.xaml`, `MainPageModel.cs`.

### US11: Trải Nghiệm Chi Tiết Sự Kiện Toàn Diện Trên Một Màn Hình
- **User Story**: Là người dùng di động, tôi muốn tất cả thông tin quan trọng của sự kiện (mô tả, địa điểm bản đồ, nút đăng ký/check-in) được sắp xếp gọn gàng trên một màn hình cuộn mượt mà.
- **Tiêu chí chấp nhận (AC)**:
  - **AC11.1**: Bố cục dạng thẻ với visual hierarchy rõ ràng (Banner -> Tiêu đề -> Thống kê nhanh -> Chi tiết -> Action bar).
  - **AC11.2**: Action bar ghim cố định ở cạnh dưới để người dùng bấm Đăng ký hoặc Điểm danh một chạm.
- **Mã nguồn hiện thực**: `EventDetailPage.xaml`.

### US12: Xem Chi Tiết Sự Kiện Dưới Quyền Quản Trị Viên
- **User Story**: Là Quản trị viên, khi mở chi tiết sự kiện tôi muốn thấy thêm các công cụ điều hành (Chỉnh sửa, Xem báo cáo điểm danh, Tạo mã QR) mà sinh viên không thấy.
- **Tiêu chí chấp nhận (AC)**:
  - **AC12.1**: Áp dụng cơ chế RBAC (Role-Based Access Control) tự động kiểm tra `CurrentUser.Role`.
  - **AC12.2**: Nút "Quản lý sự kiện", "Xem danh sách điểm danh", "Tạo QR" chỉ hiển thị khi vai trò là `Admin` hoặc `Manager`.
- **Mã nguồn hiện thực**: `EventDetailPage.xaml`, `EventDetailPageModel.cs`.

### US13: Quản Trị Vòng Đời Sự Kiện (CRUD Sự Kiện)
- **User Story**: Là Người quản lý sự kiện, tôi muốn tạo mới, chỉnh sửa, xóa và cập nhật lịch trình sự kiện để tổ chức các hoạt động trường học linh hoạt.
- **Tiêu chí chấp nhận (AC)**:
  - **AC13.1**: Form `EventFormPage` hỗ trợ nhập Tên sự kiện, Danh mục, Thời gian bắt đầu/kết thúc, Địa điểm, Sức chứa tối đa, Điểm rèn luyện.
  - **AC13.2**: Lưu trực tiếp vào collection `events` trên Cloud Firestore qua `EventService`.
  - **AC13.3**: Cho phép sửa đổi thông tin hoặc xóa sự kiện khi chưa diễn ra.
- **Mã nguồn hiện thực**: `EventFormPage.xaml`, `EventManagementPage.xaml`, `EventService.cs`.

### US14: Đồng Hồ Đếm Ngược & Cảnh Báo Giới Hạn Sức Chứa
- **User Story**: Là sinh viên, tôi muốn nhìn thấy đồng hồ đếm ngược thời gian mở đăng ký và thanh đo sức chứa (đã đăng ký / tối đa) để đăng ký kịp thời trước khi hết chỗ.
- **Tiêu chí chấp nhận (AC)**:
  - **AC14.1**: Hiển thị nhãn tiến độ sức chứa (Ví dụ: "95/100 chỗ").
  - **AC14.2**: Khi sự kiện đạt 100% sức chứa, nút "Đăng ký" tự động chuyển sang trạng thái "Đã đủ số lượng" và vô hiệu hóa.
- **Mã nguồn hiện thực**: `EventDetailPage.xaml`, `EventDetailPageModel.cs`.

### US15: Hiển Thị Tag Danh Mục Trên Thẻ Sự Kiện
- **User Story**: Là sinh viên, tôi muốn thấy huy hiệu danh mục (Tag Badge) nổi bật trên từng thẻ sự kiện để nhận diện nhanh loại hoạt động.
- **Tiêu chí chấp nhận (AC)**:
  - **AC15.1**: Mỗi thẻ sự kiện có Badge danh mục ở góc với màu sắc đặc trưng (Xanh dương cho Hội thảo, Xanh lá cho Tình nguyện, Đỏ cho Thể thao...).
  - **AC15.2**: Tag hiển thị đồng bộ giữa trang chủ `MainPage` và danh sách `EventListPage`.
- **Mã nguồn hiện thực**: `MainPage.xaml`, `EventListPage.xaml`.

### US16: Kênh Trao Đổi & Bình Luận Cho Từng Sự Kiện
- **User Story**: Là người tham gia, tôi muốn có khu vực ghi chú trao đổi trong sự kiện để cập nhật tin tức hoặc giải đáp thắc mắc với ban tổ chức.
- **Tiêu chí chấp nhận (AC)**:
  - **AC16.1**: Khối ghi chú thông báo từ ban tổ chức hiển thị trong chi tiết sự kiện.
  - **AC16.2**: Dữ liệu thông báo đồng bộ từ Firestore.
- **Mã nguồn hiện thực**: `EventDetailPage.xaml`, `EventDetailPageModel.cs`.

### US17: Điểm Danh Bằng Quét Mã QR Camera Thực Tế (Core High-Tech)
- **User Story**: Là một sinh viên tham gia sự kiện, tôi muốn mở camera trên điện thoại để quét mã QR check-in tại bàn lễ tân để điểm danh nhanh chóng mà không cần xếp hàng.
- **Tiêu chí chấp nhận (AC)**:
  - **AC17.1**: Tích hợp thư viện `ZXing.Net.Maui.Controls` trực tiếp điều khiển Camera phần cứng của thiết bị Android/Windows.
  - **AC17.2**: Tự động nhận diện barcode/QR định dạng payload JSON chứa `eventId` và mã bí mật.
  - **AC17.3**: Gửi request điểm danh lên Firebase Firestore qua `AttendanceService`, ghi nhận thời gian check-in chính xác từng giây.
  - **AC17.4**: Tự động chặn điểm danh trùng lặp (nếu đã điểm danh rồi thì thông báo rõ ràng).
- **Mã nguồn hiện thực**: `QrScannerPage.xaml`, `QrScannerViewModel.cs`, `AttendanceService.cs`, `ZXing.Net.Maui.Controls`.

### US18: Nhận Phản Hồi Xác Nhận Điểm Danh Thành Công
- **User Story**: Là người tham gia, ngay sau khi quét mã QR thành công, tôi muốn màn hình hiển thị ngay thông báo xác nhận kèm điểm rèn luyện nhận được để tôi yên tâm.
- **Tiêu chí chấp nhận (AC)**:
  - **AC18.1**: Khi check-in thành công, hệ thống hiển thị pop-up/modal thông báo với huy hiệu màu xanh lá "Điểm danh thành công!".
  - **AC18.2**: Hiển thị rõ số điểm rèn luyện cộng thêm và thời gian ghi nhận.
- **Mã nguồn hiện thực**: `QrScannerViewModel.cs`, `EventNotificationModal`.

### US19: Tra Cứu Lịch Sử Điểm Danh & Sổ Cái Điểm Rèn Luyện (ĐRL)
- **User Story**: Là một sinh viên, tôi muốn xem lại toàn bộ lịch sử các sự kiện tôi đã tham gia và tổng số điểm rèn luyện tích lũy để nộp báo cáo rèn luyện cuối kỳ.
- **Tiêu chí chấp nhận (AC)**:
  - **AC19.1**: Trang `AttendanceHistoryPage` hiển thị danh sách các sự kiện đã check-in thành công.
  - **AC19.2**: Card thống kê trên đầu trang tính tổng điểm rèn luyện tích lũy (ví dụ: `Tổng điểm: 85 ĐRL`).
  - **AC19.3**: Hỗ trợ kéo để làm mới (Pull-to-refresh) đồng bộ dữ liệu mới nhất từ Firestore.
- **Mã nguồn hiện thực**: `AttendanceHistoryPage.xaml`, `AttendanceHistoryViewModel.cs`, `AttendanceService.cs`.

### US20: Giám Sát Danh Sách Người Tham Dự Thời Gian Thực
- **User Story**: Là Ban tổ chức sự kiện, tôi muốn theo dõi danh sách sinh viên đã check-in theo thời gian thực để nắm bắt tỷ lệ tham gia trong lúc sự kiện đang diễn ra.
- **Tiêu chí chấp nhận (AC)**:
  - **AC20.1**: Trang `EventAttendanceReportPage` hiển thị danh sách sinh viên đã điểm danh (Họ tên, Mã SV, Thời gian check-in, Phương thức QR/Ảnh/Thủ công).
  - **AC20.2**: Hiển thị tổng số lượng người tham dự thực tế so với sức chứa.
- **Mã nguồn hiện thực**: `EventAttendanceReportPage.xaml`, `EventAttendanceReportViewModel.cs`.

### US21: Điểm Danh Bằng Ảnh Chụp Minh Chứng & Fallback Thủ Công
- **User Story**: Là sinh viên hoặc Ban tổ chức, khi thiết bị không thể quét mã QR (camera mờ, lỗi mạng tại chỗ), tôi muốn chụp ảnh minh chứng có mặt tại sự kiện hoặc quản lý điểm danh thủ công để quyền lợi của tôi được bảo đảm.
- **Tiêu chí chấp nhận (AC)**:
  - **AC21.1**: `PhotoAttendancePage` cho phép chụp ảnh trực tiếp từ Camera hoặc chọn ảnh từ thư viện (`MediaPicker`).
  - **AC21.2**: Hiển thị khung xem trước ảnh chụp và ô nhập ghi chú minh chứng.
  - **AC21.3**: Hỗ trợ nút "Điểm danh thủ công (Manual Fallback)" cho phép ghi nhận trực tiếp trạng thái State A khi có sự xác nhận của giám sát viên.
- **Mã nguồn hiện thực**: `PhotoAttendancePage.xaml`, `PhotoAttendanceViewModel.cs`, `AttendanceService.cs`.

### US22: Xuất Báo Cáo Điểm Danh Ra Tệp CSV Chuẩn UTF-8
- **User Story**: Là Quản lý sự kiện, tôi muốn xuất danh sách sinh viên tham dự sự kiện ra file CSV để mở trên Excel hoặc chuyển giao cho Phòng Công tác Sinh viên tính điểm rèn luyện.
- **Tiêu chí chấp nhận (AC)**:
  - **AC22.1**: Nút "Xuất báo cáo CSV" tạo file định dạng chuẩn CSV UTF-8 có chèn tiền tố BOM (`\uFEFF`) giúp Microsoft Excel hiển thị tiếng Việt có dấu hoàn hảo không bị lỗi font.
  - **AC22.2**: Tự động mở hộp thoại chia sẻ tệp của thiết bị (`Share.RequestAsync`) cho phép gửi qua Email, Drive, Zalo hoặc lưu vào máy.
- **Mã nguồn hiện thực**: `EventAttendanceReportViewModel.cs`, `AttendanceService.cs`.

### US23: Cấu Hình Phương Thức Điểm Danh Cho Sự Kiện
- **User Story**: Là Ban tổ chức, tôi muốn bật hoặc tắt các phương thức điểm danh (cho phép QR, cho phép Ảnh minh chứng, cho phép Thủ công) tùy thuộc vào tính chất của từng sự kiện.
- **Tiêu chí chấp nhận (AC)**:
  - **AC23.1**: Model sự kiện lưu trữ cờ cấu hình `AllowedAttendanceMethods`.
  - **AC23.2**: Nếu sự kiện chỉ cho phép QR, giao diện sẽ ẩn tùy chọn nộp ảnh minh chứng.
- **Mã nguồn hiện thực**: `CampusEvent.cs`, `EventService.cs`.

### US24: Kiểm Soát Thời Gian Điểm Danh (Đúng Giờ / Đi Muộn)
- **User Story**: Là Ban tổ chức, tôi muốn hệ thống tự động ghi nhận trạng thái tham dự ("Đúng giờ" hoặc "Đi muộn") dựa trên thời gian bắt đầu của sự kiện.
- **Tiêu chí chấp nhận (AC)**:
  - **AC24.1**: So sánh timestamp điểm danh với `StartTime` của sự kiện.
  - **AC24.2**: Ghi nhận cờ `IsLate` và hiển thị nhãn cảnh báo trên báo cáo điểm danh nếu check-in sau giờ quy định.
- **Mã nguồn hiện thực**: `AttendanceService.cs`, `AttendanceModels.cs`.

### US25: Phân Tích & Thống Kê Dữ Liệu Tham Gia Hoạt Động
- **User Story**: Là Quản trị viên, tôi muốn xem các chỉ số thống kê về mức độ chuyên cần và tỷ lệ sinh viên tham gia hoạt động campus theo từng học kỳ.
- **Tiêu chí chấp nhận (AC)**:
  - **AC25.1**: Thống kê số lượng sinh viên đạt điểm rèn luyện xuất sắc, khá, trung bình.
  - **AC25.2**: Danh mục sự kiện nào thu hút đông đảo sinh viên tham gia nhất.
- **Mã nguồn hiện thực**: `MainPageModel.cs`, `AttendanceService.cs`.

### US26: Đăng Ký Tài Khoản Sinh Viên (UI & Client Validation)
- **User Story**: Là sinh viên mới, tôi muốn tự đăng ký tài khoản với Mã SV, Email trường và Mật khẩu để bắt đầu sử dụng ứng dụng.
- **Tiêu chí chấp nhận (AC)**:
  - **AC26.1**: Trang `RegisterPage` thiết kế chuẩn màu Xanh Dương `#2563EB`.
  - **AC26.2**: Thanh đo độ mạnh mật khẩu (Password Strength Meter) phản hồi theo thời gian thực (Yếu, Trung bình, Mạnh).
  - **AC26.3**: Kiểm tra định dạng Email `@ictu.edu.vn` / `@campus.edu`, mật khẩu xác nhận trùng khớp.
- **Mã nguồn hiện thực**: `RegisterPage.xaml`, `RegisterViewModel.cs`, `RegisterPageModel.cs`.

### US27: Xác Nhận Danh Tính Sau Khi Đăng Ký
- **User Story**: Là người dùng vừa đăng ký, tôi muốn được tạo hồ sơ định danh đầy đủ với trạng thái kích hoạt mặc định và khởi tạo 0 Điểm Rèn Luyện.
- **Tiêu chí chấp nhận (AC)**:
  - **AC27.1**: Tạo bản ghi trong collection `users` trên Firestore.
  - **AC27.2**: Khởi tạo `TrainingPoints = 0`, `IsActive = true`, vai trò `Student`.
- **Mã nguồn hiện thực**: `UserService.cs`, `RegisterViewModel.cs`.

### US28: Đăng Nhập Hệ Thống & Bảo Mật Phiên Làm Việc (Login & SecureStorage)
- **User Story**: Là một người dùng, tôi muốn đăng nhập bằng Email và Mật khẩu của mình và lưu trữ phiên an toàn để sử dụng các tính năng cá nhân.
- **Tiêu chí chấp nhận (AC)**:
  - **AC28.1**: Giao diện đăng nhập `LoginPage` tối ưu trải nghiệm, hỗ trợ hiển thị/ẩn mật khẩu.
  - **AC28.2**: Lưu token phiên làm việc vào `SecureStorage.Default`.
  - **AC28.3**: Phân quyền điều hướng tự động: Quản trị viên điều hướng đến trang quản trị, Sinh viên điều hướng đến trang chủ sự kiện.
  - **AC28.4**: Hỗ trợ cơ chế Quick Login cho tài khoản Admin mẫu để phục vụ trình diễn và kiểm thử nhanh.
- **Mã nguồn hiện thực**: `LoginPage.xaml`, `LoginPageModel.cs`, `AuthenticationService.cs`, `AuthService.cs`.

### US29: Đặt Lại Mật Khẩu Cho Người Dùng
- **User Story**: Là người dùng quên mật khẩu, tôi muốn có cơ chế đặt lại mật khẩu an toàn để khôi phục quyền truy cập vào tài khoản của mình.
- **Tiêu chí chấp nhận (AC)**:
  - **AC29.1**: Cho phép Quản trị viên đặt lại mật khẩu trong `UserManagementPage` hoặc gửi yêu cầu reset mật khẩu.
  - **AC29.2**: Mật khẩu mới được hash an toàn qua chuẩn PBKDF2-SHA256 trước khi lưu vào cơ sở dữ liệu.
- **Mã nguồn hiện thực**: `UserService.cs`, `UserManagementPageModel.cs`.

### US30: Ghi Nhớ Đăng Nhập & Tự Động Khôi Phục Phiên (Auto-Login)
- **User Story**: Là người dùng thường xuyên, tôi muốn ứng dụng ghi nhớ thông tin đăng nhập để khi mở lại app tôi không phải gõ lại tài khoản/mật khẩu.
- **Tiêu chí chấp nhận (AC)**:
  - **AC30.1**: Tùy chọn "Ghi nhớ đăng nhập" lưu thông tin an toàn trên thiết bị.
  - **AC30.2**: Khi khởi động app, `App.xaml.cs` kiểm tra token hợp lệ trong `SecureStorage` và tự động vào màn hình chính nếu phiên còn hiệu lực.
- **Mã nguồn hiện thực**: `App.xaml.cs`, `AuthenticationService.cs`.

### US31: Xem Danh Sách Sự Kiện Sắp Diễn Ra (Event Feed)
- **User Story**: Là sinh viên, tôi muốn xem dòng sự kiện sắp diễn ra trên trang chủ với ảnh đại diện, tiêu đề, ngày giờ và địa điểm để nhanh chóng nắm bắt các hoạt động hot.
- **Tiêu chí chấp nhận (AC)**:
  - **AC31.1**: Trang `MainPage` và `EventListPage` hiển thị danh sách các sự kiện đang mở.
  - **AC31.2**: Các sự kiện được sắp xếp theo thời gian diễn ra gần nhất lên trước.
- **Mã nguồn hiện thực**: `MainPage.xaml`, `EventListPage.xaml`, `EventService.cs`.

### US32: Cuộn Mượt Mà & Tải Dữ Liệu Tối Ưu (Pull-To-Refresh)
- **User Story**: Là người dùng, tôi muốn danh sách sự kiện có tính năng kéo để tải lại (Pull-to-refresh) và cuộn mượt mà không bị nghẽn giao diện.
- **Tiêu chí chấp nhận (AC)**:
  - **AC32.1**: Tích hợp `RefreshView` bao ngoài danh sách sự kiện.
  - **AC32.2**: Khi kéo xuống, ứng dụng kích hoạt tải lại dữ liệu mới nhất từ Cloud Firestore và tắt spinner tải khi hoàn tất.
- **Mã nguồn hiện thực**: `MainPage.xaml`, `EventListPage.xaml`.

### US33: Quản Lý Danh Mục Sự Kiện (Category CRUD)
- **User Story**: Là Quản trị viên, tôi muốn tạo mới, sửa và quản lý các danh mục sự kiện (Hội thảo, Tình nguyện, Thể thao...) để phân loại các hoạt động trường học quy củ.
- **Tiêu chí chấp nhận (AC)**:
  - **AC33.1**: Trang `ManageCategoriesPage` cho phép xem danh sách danh mục hiện có.
  - **AC33.2**: Thêm mới danh mục với Tên, Mô tả, Màu sắc và Icon hiển thị.
  - **AC33.3**: Chỉnh sửa hoặc xóa danh mục khỏi Firestore.
- **Mã nguồn hiện thực**: `ManageCategoriesPage.xaml`, `ManageCategoriesPageModel.cs`, `CategoryRepository.cs`.

### US34: Khóa & Lưu Trữ Danh Mục Sự Kiện Cũ
- **User Story**: Là Quản trị viên, tôi muốn có thể ẩn hoặc lưu trữ các danh mục sự kiện của năm học cũ mà không làm mất lịch sử các sự kiện đã tổ chức trước đó.
- **Tiêu chí chấp nhận (AC)**:
  - **AC34.1**: Hỗ trợ cờ trạng thái `IsActive` cho danh mục.
  - **AC34.2**: Danh mục bị ẩn sẽ không hiển thị trên bộ lọc sự kiện mới nhưng vẫn giữ nguyên nhãn trên các sự kiện cũ.
- **Mã nguồn hiện thực**: `CategoryRepository.cs`, `CampusEvent.cs`.

### US35: Quản Lý Tài Khoản Cloud & Đồng Bộ Firebase (Core Serverless)
- **User Story**: Là Quản trị viên, tôi muốn hệ thống cung cấp dịch vụ quản lý tài khoản đồng bộ trực tiếp lên Firebase (Tạo mới, Cập nhật hồ sơ, Khóa/Mở khóa tài khoản) theo chuẩn Serverless an toàn.
- **Tiêu chí chấp nhận (AC)**:
  - **AC35.1**: Tạo tài khoản mới trên Firebase Authentication và tạo document trên Cloud Firestore (`T35.1`).
  - **AC35.2**: Cập nhật thông tin hồ sơ và đồng bộ thời gian thực (`T35.1`).
  - **AC35.3**: Khóa (`disabled = true`, `IsActive = false`) và Mở khóa tài khoản, ngăn chặn Admin tự khóa chính mình (`T35.2`).
  - **AC35.4**: Chuẩn hóa phản hồi qua đối tượng `ApiResponse<T>`.
- **Mã nguồn hiện thực**: `UserService.cs`, `FirebaseSyncService.cs`, `UserManagementPageModel.cs`.

### US36: Đăng Ký Tài Khoản Người Dùng Phía Dịch Vụ
- **User Story**: Là hệ thống, tôi muốn cung cấp dịch vụ xử lý đăng ký tài khoản tự phục vụ an toàn, mã hóa mật khẩu và tạo hồ sơ người dùng trên Firestore.
- **Tiêu chí chấp nhận (AC)**:
  - **AC36.1**: Kiểm tra email trùng lặp trên Firebase Firestore trước khi tạo.
  - **AC36.2**: Mã hóa mật khẩu an toàn theo thuật toán PBKDF2-SHA256 với Salt 128-bit.
  - **AC36.3**: Trả về mã trạng thái `201 Created` và gán vai trò mặc định `Student`.
- **Mã nguồn hiện thực**: `UserService.cs`, `AuthenticationService.cs`.

### US37: Dịch Vụ Xác Thực & Quản Lý Phiên Làm Việc (Auth Service)
- **User Story**: Là hệ thống, tôi muốn dịch vụ xác thực tài khoản kiểm tra chính xác thông tin đăng nhập, xác minh mật khẩu mã hóa và cấp quyền sử dụng cho ứng dụng.
- **Tiêu chí chấp nhận (AC)**:
  - **AC37.1**: Kiểm tra email và so khớp mật khẩu bằng PBKDF2 hash verification.
  - **AC37.2**: Từ chối đăng nhập với mã `403 Forbidden` kèm thông báo "Tài khoản của bạn đã bị khóa" nếu `IsActive == false`.
  - **AC37.3**: Cấp token phiên làm việc và lưu thông tin `CurrentUser` vào `AuthenticationService`.
- **Mã nguồn hiện thực**: `AuthenticationService.cs`, `AuthService.cs`, `UserService.cs`.

### US38: Dịch Vụ Cung Cấp Dữ Liệu Sự Kiện & Danh Mục Đám Mây
- **User Story**: Là hệ thống, tôi muốn dịch vụ dữ liệu sự kiện kết nối đồng bộ hai chiều với Cloud Firestore để phục vụ danh sách sự kiện và danh mục theo thời gian thực.
- **Tiêu chí chấp nhận (AC)**:
  - **AC38.1**: Tải danh sách sự kiện kèm thông tin danh mục tương ứng từ Cloud Firestore.
  - **AC38.2**: Tự động chuyển đổi định dạng Firestore REST API sang các Model C# strongly-typed (`CampusEvent`, `Category`).
  - **AC38.3**: Có cơ chế dự phòng dữ liệu cục bộ (Local fallback) khi thiết bị mất kết nối Internet tạm thời.
- **Mã nguồn hiện thực**: `EventService.cs`, `FirebaseSyncService.cs`, `CategoryRepository.cs`.

---

## 3. PHÂN BỔ SPRINT PLAN (4 SPRINTS) ĐÃ HOÀN THIỆN

### Sprint 01: Core Authentication & User Management
- **Mục tiêu**: Xây dựng nền tảng kiến trúc Serverless, xác thực người dùng, bảo mật phiên làm việc và quản lý tài khoản.
- **User Stories**: `US26`, `US27`, `US28`, `US30`, `US35`, `US36`, `US37`.
- **Nhiệm vụ chính**:
  - T37.1 / T28.3: Tích hợp `AuthenticationService`, mã hóa PBKDF2, lưu trữ an toàn trong `SecureStorage`.
  - T35.1: Xây dựng `UserService` đồng bộ tạo mới và cập nhật tài khoản trên Cloud Firestore.
  - T35.2: Triển khai tính năng Khóa / Mở khóa tài khoản người dùng (`UserManagementPage`).
  - T26.1 / T28.1: Thiết kế giao diện Đăng ký & Đăng nhập theo chuẩn màu Xanh Dương `#2563EB`.

### Sprint 02: Event Lifecycle & Category Management
- **Mục tiêu**: Quản trị vòng đời sự kiện, danh mục hoạt động, hiển thị danh sách và lọc tìm kiếm.
- **User Stories**: `US01`, `US06`, `US13`, `US15`, `US31`, `US32`, `US33`, `US34`, `US38`.
- **Nhiệm vụ chính**:
  - T13.1 / T13.3: Triển khai CRUD sự kiện trên `EventService` và kết nối Cloud Firestore.
  - T33.1 / T33.2: Triển khai CRUD danh mục sự kiện trong `ManageCategoriesPage`.
  - T31.1 / T01.1: Giao diện danh sách sự kiện trang chủ và trang chi tiết sự kiện `EventDetailPage`.
  - T15.1: Gắn nhãn Tag danh mục trên từng thẻ sự kiện.

### Sprint 03: Attendance QR Scanner & Photo Check-in (Core High-Tech)
- **Mục tiêu**: Tích hợp quét mã QR bằng Camera phần cứng thực tế và điểm danh bằng hình ảnh minh chứng.
- **User Stories**: `US17`, `US18`, `US21`, `US23`, `US24`.
- **Nhiệm vụ chính**:
  - T17.1 / T17.2: Tích hợp thư viện `ZXing.Net.Maui.Controls`, xử lý camera quét mã QR thực tế trong `QrScannerPage`.
  - T21.1 / T21.3: Phát triển màn hình `PhotoAttendancePage` chụp ảnh minh chứng bằng `MediaPicker` kèm nút chuyển đổi điểm danh thủ công (Manual Fallback).
  - T18.1: Tích hợp `EventNotificationModal` thông báo 2 tầng chuyên nghiệp phản hồi kết quả check-in.

### Sprint 04: Real-time Attendance Monitoring, ĐRL Ledger, CSV Export & Verification
- **Mục tiêu**: Giám sát thời gian thực, tổng hợp sổ cái Điểm Rèn Luyện, xuất báo cáo CSV UTF-8 và kiểm thử tự động toàn diện.
- **User Stories**: `US04`, `US05`, `US07`, `US19`, `US20`, `US22`, `US25`.
- **Nhiệm vụ chính**:
  - T19.1: Xây dựng `AttendanceHistoryPage` hiển thị lịch sử tham dự và sổ cái tổng hợp Điểm Rèn Luyện (ĐRL).
  - T20.1 / T22.1 / T22.2: Màn hình `EventAttendanceReportPage` giám sát người tham gia và tính năng xuất file CSV chuẩn UTF-8 kèm BOM mở trực tiếp trên Excel tiếng Việt.
  - T07.1 / T07.2: Hoàn thiện trang hồ sơ cá nhân `ProfilePage`.
  - Kiểm thử toàn diện: 80 Unit Tests (`CampusActivitiesManagerTest`) và 15 Appium E2E Scenarios (`UITests.Android`) trên Android Emulator API 35.

---

## 4. MA TRẬN PHÂN CÔNG NHIỆM VỤ (RESPONSIBILITY ASSIGNMENT MATRIX)

| Mã US | Tên User Story | Assignee chính | Trạng thái hiện tại |
|---|---|---|---|
| **US01** | Xem thông tin chi tiết sự kiện | Nguyễn Đức Mạnh | **Đã hoàn thành (Done)** |
| **US05** | Quản lý danh sách tài khoản (Admin) | Nguyễn Đức Mạnh | **Đã hoàn thành (Done)** |
| **US06** | Lọc sự kiện theo danh mục | Nguyễn Đức Mạnh | **Đã hoàn thành (Done)** |
| **US07** | Xem & Cập nhật hồ sơ cá nhân | Nguyễn Đức Mạnh | **Đã hoàn thành (Done)** |
| **US13** | Quản trị sự kiện (CRUD) | Nguyễn Đức Mạnh | **Đã hoàn thành (Done)** |
| **US15** | Hiển thị tag danh mục sự kiện | Nguyễn Đức Mạnh | **Đã hoàn thành (Done)** |
| **US17** | Điểm danh bằng QR Scanner (Camera ZXing) | **Nguyễn Đức Mạnh** | **Đã hoàn thành (Done)** |
| **US18** | Thông báo xác nhận điểm danh (Modal 2 tầng) | **Nguyễn Đức Mạnh** | **Đã hoàn thành (Done)** |
| **US19** | Lịch sử điểm danh & Sổ cái Điểm Rèn Luyện | **Nguyễn Đức Mạnh** | **Đã hoàn thành (Done)** |
| **US20** | Giám sát điểm danh thời gian thực | **Nguyễn Đức Mạnh** | **Đã hoàn thành (Done)** |
| **US21** | Điểm danh ảnh minh chứng & Fallback | **Nguyễn Đức Mạnh** | **Đã hoàn thành (Done)** |
| **US22** | Xuất báo cáo điểm danh CSV UTF-8 | **Nguyễn Đức Mạnh** | **Đã hoàn thành (Done)** |
| **US26** | Đăng ký tài khoản sinh viên (UI) | Nguyễn Đức Mạnh | **Đã hoàn thành (Done)** |
| **US28** | Đăng nhập hệ thống & SecureStorage | Nguyễn Đức Mạnh | **Đã hoàn thành (Done)** |
| **US31** | Danh sách sự kiện sắp diễn ra | Nguyễn Đức Mạnh | **Đã hoàn thành (Done)** |
| **US33** | Quản lý danh mục sự kiện (CRUD) | Nguyễn Đức Mạnh | **Đã hoàn thành (Done)** |
| **US35** | Quản lý tài khoản Firebase Cloud (T35.1, T35.2)| **Nguyễn Đức Mạnh** | **Đã hoàn thành (Done)** |
| **US36** | Đăng ký tài khoản phía dịch vụ | Nguyễn Đức Mạnh | **Đã hoàn thành (Done)** |
| **US37** | Dịch vụ xác thực & Phiên làm việc (Auth) | **Nguyễn Đức Mạnh** | **Đã hoàn thành (Done)** |
| **US38** | Dịch vụ dữ liệu sự kiện & Danh mục Cloud | **Nguyễn Đức Mạnh** | **Đã hoàn thành (Done)** |
