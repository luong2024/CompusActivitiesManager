 Campus Activity Manager 

File này là luật bắt buộc cho AI khi làm việc trong repo này. Đọc hết trước khi viết bất kỳ dòng code nào. Nếu yêu cầu của người dùng mâu thuẫn với file này, hỏi lại thay vì tự quyết. Các mục đánh dấu [TODO] là chỗ nhóm cần điền cho đúng thực tế dự án. 

 

1. Tổng quan dự án 

Tên: Campus Activity Manager 

Loại: Mobile app quản lý hoạt động/sự kiện trong trường, viết bằng .NET MAUI (Android + iOS). 

Mục tiêu: xem sự kiện sắp diễn ra, đăng ký sự kiện, theo dõi lịch sử tham gia, xem chi tiết sự kiện. 

Vai trò người dùng: Student, Event Manager, Admin (trong code dùng role: ADMIN, USER, ... — xem mục 6). 

Backend: API dùng Firebase (theo AC của US35). Xác thực dùng Firebase Authentication (ID Token dạng JWT do Firebase cấp), dữ liệu lưu trên Firebase. [TODO: xác nhận Firestore hay Realtime Database; app gọi thẳng Firebase SDK hay qua REST API trung gian (Cloud Functions/ASP.NET)] 

Xem mục 7B cho quy tắc riêng của Firebase. 

Phương pháp: Scrum, làm theo Sprint. Chỉ làm đúng User Story (US) thuộc sprint hiện tại. 

Danh sách Feature 

Id 

Feature 

Id 

Feature 

F01 

Login 

F06 

My Registered Events 

F02 

Profile UI 

F07 

Event Categories 

F03 

Event List Screen 

F08 

Search & Filter UI 

F04 

Event Detail Screen 

F09 

Dashboard (statistics cards) 

F05 

Event Registration UI 

F10 

Settings & Preferences 

 

2. Phạm vi — AI chỉ được làm trong phạm vi này 

Sprint 01 (đang/đã làm) 

US05 (quản lý tài khoản – UI), US26 (đăng ký – UI), US28 (đăng nhập – UI), US15 (tag danh mục trên event card), và các API: US35 (quản lý tài khoản), US36 (đăng ký), US37 (đăng nhập + JWT), US38 (event list kèm category). 

Sprint 02 

US31 (danh sách sự kiện), US01 (chi tiết sự kiện), US13 (quản lý sự kiện – CRUD), US33 (quản lý danh mục sự kiện), US07 (profile). 

Quy tắc phạm vi 

KHÔNG tự làm US ngoài sprint hiện tại (ví dụ QR attendance US17, RFID/face recognition US23, analytics US25, group chat US16...). Nếu người dùng yêu cầu, nhắc rằng nó chưa nằm trong sprint và hỏi xác nhận. 

KHÔNG thêm tính năng "cho đẹp" ngoài Acceptance Criteria (AC) của task. 

Mỗi thay đổi phải truy được về một Task ID (vd T28.1). Ghi Task ID trong commit message và PR. 

Trước khi code, nêu ngắn gọn: làm task nào → AC nào → sẽ sửa file nào. Rồi mới code. 

 

3. Kiến trúc & cấu trúc thư mục 

[TODO: sửa theo cấu trúc thật của solution] 

CampusActivityManager/ 
├── Models/            # Entity, DTO 
├── Views/             # XAML pages (.xaml + .xaml.cs mỏng) 
├── ViewModels/        # Logic UI, binding 
├── Services/          # ApiService, AuthService, SecureStorage wrapper... 
├── Helpers/           # Validators, Converters, Constants 
├── Resources/ 
│   ├── Styles/        # Colors.xaml, Styles.xaml  ← bảng màu nằm ở đây 
│   ├── Images/ Fonts/ 
├── Platforms/         # Android, iOS 
└── MauiProgram.cs     # Đăng ký DI 
  

Bắt buộc: 

Dùng MVVM. Code-behind (.xaml.cs) chỉ chứa việc thuần UI; không gọi API, không xử lý nghiệp vụ trong code-behind. 

Dùng Dependency Injection (đăng ký trong MauiProgram.cs); không new Service() thủ công trong ViewModel. 

Dùng Shell navigation (hoặc cơ chế điều hướng đang dùng) — không trộn nhiều kiểu. 

Tầng gọi API tách riêng trong Services/. ViewModel chỉ gọi interface (IEventService, IAuthService...). 

Ưu tiên CommunityToolkit.Mvvm ([ObservableProperty], [RelayCommand]) nếu dự án đã cài. Không tự thêm NuGet package mới khi chưa hỏi. 

 

4. Quy ước code (DoD1) 

Ngôn ngữ code: tên class/method/biến/comment kỹ thuật bằng tiếng Anh. Chuỗi hiển thị cho người dùng bằng tiếng Việt và đặt tập trung (Resources/Strings hoặc hằng số), không rải rác. 

Đặt tên: PascalCase cho class/method/property; camelCase cho biến cục bộ/tham số; field private _camelCase; interface bắt đầu bằng I; method bất đồng bộ kết thúc bằng Async. 

Async: luôn async/await; cấm .Result, .Wait(), async void (trừ event handler UI). 

Nullable: bật nullable reference types, xử lý null rõ ràng. 

Không hard-code: URL API, key, màu, kích thước → đưa vào Constants/appsettings/Colors.xaml. 

Hàm ngắn, một việc. Không comment code chết; xoá luôn. 

Màu & style: dùng bảng màu Xanh Dương (Modern Tech & Corporate) đã chốt. Mọi màu lấy từ Resources/Styles/Colors.xaml qua StaticResource/DynamicResource. Cấm viết mã màu hex trực tiếp trong XAML trang. 

Dùng Style/ControlTemplate tái sử dụng thay vì lặp thuộc tính. 

 

5. Quy tắc UI/UX (rút từ AC của dự án) 

Hỗ trợ cả Android và iOS — không viết code chỉ chạy một nền tảng..

Responsive: 1 cột trên điện thoại, 2–3 cột trên tablet/desktop (dùng CollectionView với GridItemsLayout + Span theo kích thước). 

Form dài phải nằm trong ScrollView để bàn phím ảo không che nội dung. 

Nút gửi dữ liệu (Đăng nhập/Đăng ký/Lưu):  

Khi đang gọi API: Disabled + hiện ActivityIndicator/loading text → chống double submit. 

Disable khi dữ liệu chưa hợp lệ (client-side validation). 

Danh sách: hỗ trợ Pull-to-Refresh (RefreshView) và tải thêm khi cuộn (RemainingItemsThreshold). Có trạng thái loading, rỗng (empty state) và lỗi (kèm nút thử lại). 

Ô mật khẩu: có icon con mắt bật/tắt hiển thị. 

Tìm kiếm: có cơ chế debounce (hoãn gõ) và nút xoá nhanh. 

Thông báo lỗi: rõ ràng, đặt dưới ô nhập hoặc popup; không lộ chi tiết kỹ thuật/stack trace ra người dùng. 

Event card: hiển thị tag danh mục theo dạng viên thuốc (bo góc), tối đa 2 tag/thẻ, tên > 15 ký tự thì rút gọn; chạm vào tag để lọc. 

Mất mạng: bắt lỗi và hiển thị thông báo thân thiện, không crash. 

 

6. Xác thực, phân quyền & bảo mật (QUAN TRỌNG) 

Tuyệt đối không vi phạm: 

Token (Firebase ID Token + Refresh Token) lưu bằng SecureStorage, không dùng Preferences, file thường hay biến static. ID Token hết hạn sau ~1 giờ → phải có cơ chế tự refresh bằng refresh token, không bắt người dùng đăng nhập lại. 

Không bao giờ log mật khẩu, token, hay dữ liệu cá nhân ra Console/Debug/file log. 

Không hard-code key/secret/connection string/Firebase config vào code hoặc commit lên Git. Dùng file cấu hình đã .gitignore. [TODO: ghi nơi chứa config] 

Mật khẩu không được lưu/trả về dạng plaintext; phía server phải hash bằng Bcrypt/Argon2. 

Thông báo đăng nhập sai dùng thông báo chung: "Tên đăng nhập hoặc mật khẩu không chính xác" (không tiết lộ tài khoản có tồn tại hay không). 

Token tự gắn vào mọi request qua một DelegatingHandler cho HttpClient (trong MAUI không dùng Axios; AC nhắc Axios Interceptor nghĩa là cơ chế tương đương). 

Điều hướng sau đăng nhập:  

ADMIN → màn admin dashboard; USER → dashboard thường. 

Nếu bị chuyển về Login từ một trang được bảo vệ, đăng nhập xong phải quay lại đúng trang đó (deep link / protected route). 

Ghi nhớ đăng nhập (Remember me) và sinh trắc học (vân tay/Face ID): chỉ bật cho tài khoản đã kích hoạt trước. 

Khoá tài khoản: tài khoản bị khoá → hiển thị "Tài khoản của bạn đã bị khóa" (403/ACCOUNT_LOCKED). Sai mật khẩu liên tiếp 5 lần → tạm khoá/cảnh báo. 

RBAC: UI ẩn chức năng không đủ quyền và server phải trả 403 Forbidden. Không tin riêng việc ẩn UI. 

Admin không thể tự khoá tài khoản của chính mình. 

Không cho sửa Mật khẩu/Trạng thái qua endpoint/màn hình "cập nhật thông tin chung". 

 

7. Quy ước API 

Trả đúng HTTP status code và response format thống nhất [TODO: dán mẫu JSON response chuẩn của nhóm]. 

Bảng mã chuẩn đang dùng: 

Tình huống 

Status 

Tạo thành công 

201 Created 

Dữ liệu sai định dạng / thiếu / mật khẩu yếu 

400 Bad Request 

Chưa đăng nhập / sai thông tin 

401 Unauthorized 

Không đủ quyền / tài khoản bị khoá 

403 Forbidden 

Không tìm thấy (vd cập nhật account không tồn tại) 

404 Not Found 

Trùng dữ liệu (email đã tồn tại) 

409 Conflict 

Danh sách rỗng → trả 200 với mảng rỗng (không phải lỗi). 

Event không có Category vẫn phải xử lý đúng (không crash, không null reference). 

Phía app: ViewModel xử lý lỗi theo chiến lược thống nhất — phân biệt lỗi toàn màn hình (mất mạng, 500) và lỗi cục bộ từng chỗ (validate field). 

Có timeout và CancellationToken cho request dài. 

7B. Quy tắc riêng cho Firebase 

Không đưa Admin SDK / service account JSON vào app MAUI hay commit lên Git. Chúng chỉ được nằm ở server/Cloud Functions. App chỉ dùng cấu hình client (google-services.json cho Android, GoogleService-Info.plist cho iOS). 

Bảo mật dựa vào Firebase Security Rules, không dựa vào việc ẩn nút trên UI. Mọi collection/node mới phải đi kèm rule; cấm để rule allow read, write: if true. 

Phân quyền (RBAC): lưu role (ADMIN, USER, ...) bằng custom claims hoặc field role mà client không tự ghi được. Client không được tự nâng quyền cho mình. 

Khoá/Mở khoá tài khoản (US35): thực hiện phía server bằng Admin SDK (disabled = true và thu hồi refresh token) để khoá có hiệu lực ngay; không chỉ đổi cờ ở client. 

Tạo/cập nhật tài khoản, đặt role, khoá/mở khoá là thao tác đặc quyền → chạy ở server (Cloud Functions/API), không cho client gọi trực tiếp. 

Mật khẩu: do Firebase Auth hash và quản lý. App không tự lưu hay tự mã hoá mật khẩu vào database. Không tạo bảng/field chứa mật khẩu. 

Email xác nhận / quên mật khẩu (US27, US29): dùng chức năng sẵn có của Firebase Auth (verification email, password reset email), không tự viết lại. 

Chuẩn hoá lỗi: map mã lỗi Firebase (auth/user-not-found, auth/wrong-password, auth/email-already-in-use, auth/weak-password, auth/user-disabled, auth/too-many-requests...) sang thông báo tiếng Việt thống nhất. Với sai tài khoản/mật khẩu, gộp user-not-found và wrong-password thành một thông báo chung. 

Dữ liệu: thiết kế truy vấn có phân trang (limit + cursor) cho danh sách sự kiện; tránh tải toàn bộ collection. Chú ý index cho truy vấn kết hợp (vd lọc theo category + sắp theo ngày). 

Chi phí/hiệu năng: không đặt listener realtime nếu không cần; huỷ đăng ký listener khi rời trang. 

Môi trường: tách project Firebase dev và prod; không dùng dữ liệu thật khi phát triển. Mock data (DoD6) chỉ nạp vào dev. 

Không tự thêm dịch vụ Firebase mới (Storage, Messaging, Analytics...) khi chưa hỏi nhóm. Riêng avatar (US07) cần Storage và thông báo (US04) cần FCM → hỏi trước. 

 

8. Validation phía client 

Email: kiểm tra định dạng (chấp nhận email cá nhân lẫn email tổ chức/tập thể). user@com, thiếu @ là sai. 

Mật khẩu: kiểm tra độ mạnh (không dùng chuỗi như 123456; yêu cầu độ phức tạp gồm ký tự đặc biệt) và báo lại cho người dùng mức mạnh/yếu. 

Đăng ký: có Họ tên, Email, Mật khẩu, Xác nhận mật khẩu, checkbox đồng ý Điều khoản; có link "Đã có tài khoản? Đăng nhập". 

Đăng nhập: bỏ trống → chặn gửi và báo "Vui lòng nhập đầy đủ thông tin". 

Đặt logic validate trong Helpers/Validators (dễ unit test), không nhét vào code-behind. 

 

9. Kiểm thử 

Logic nghiệp vụ (validation, hash mật khẩu, kiểm tra trùng email...) cần unit test, coverage tối thiểu 85%. 

Mỗi task chức năng có task kiểm thử đi kèm (xem AC của task .4/.5). Khi AI viết code mới, viết kèm unit test cho phần logic. 

Test phải bao gồm cả luồng thành công và luồng lỗi (sai định dạng, trùng, thiếu quyền, mất mạng). 

Không sửa test cho "pass" bằng cách bỏ assert hoặc xoá test. Nếu test sai, giải thích vì sao rồi mới sửa. 

 

10. Definition of Done — AI phải tự kiểm tra trước khi báo "xong" 

[ ] DoD1 Tuân thủ code convention & bảng màu (mục 4). 

[ ] DoD2 Toàn bộ unit test pass. 

[ ] DoD3 Đã qua code review (người trong nhóm duyệt; AI không tự merge). 

[ ] DoD4 Merge nhánh cá nhân vào main (chỉ sau khi review). 

[ ] DoD5 Chạy được trên simulator/emulator (Android và/hoặc iOS). 

[ ] DoD6 Có ít nhất 5 mock data để demo. 

[ ] Đã đối chiếu đủ AC của task, không thiếu AC nào. 

Khi hoàn thành, AI báo cáo theo mẫu ở mục 12, không chỉ nói "đã xong". 

 

11. Quy trình Git 

Mỗi người làm trên nhánh cá nhân; tên nhánh gợi ý: feature/<TaskID>-<mo-ta-ngan> (vd feature/T28.1-login-ui). [TODO: xác nhận quy ước nhánh] 

Commit message: <TaskID>: <mô tả ngắn> (vd T28.1: add login form validation). 

Cấm git push --force lên main, cấm commit thẳng vào main. 

Cấm commit: file build (bin/, obj/), key/secret, file cá nhân IDE. 

Mỗi commit một mục đích; không gộp nhiều task vào một commit. 

 

12. Cách AI làm việc & báo cáo 

Trước khi code 

Xác định Task ID + AC liên quan. Nếu người dùng không nói, hỏi. 

Nếu yêu cầu mơ hồ hoặc thiếu thông tin (API schema, thiết kế UI, tên bảng...), hỏi lại. Không tự bịa endpoint, field, package hay tên màu. 

Đọc code hiện có trước khi viết mới; tái sử dụng service/style/component đã có, không tạo trùng. 

Khi code 

Thay đổi nhỏ nhất đủ để đạt AC. Không refactor lan man, không đổi tên hàng loạt, không sửa file ngoài phạm vi nếu chưa được phép. 

Không xoá/ghi đè code của thành viên khác khi chưa hỏi. 

Code phải build được. Nếu không chạy thử được, nói rõ là chưa kiểm chứng. 

Gặp lỗi build/test: sửa nguyên nhân gốc, không tắt cảnh báo, không try/catch rỗng để che lỗi. 

Báo cáo sau khi xong (bắt buộc) 

Task: <ID> – <tên> 
Đã làm: <các thay đổi chính, file đã sửa/tạo> 
AC đã đạt: <liệt kê>  |  AC chưa đạt/chưa kiểm chứng: <liệt kê> 
Test: <đã viết/chạy gì, kết quả> 
Cần người review chú ý: <rủi ro, giả định> 
  

AI luôn phải 

Trả lời bằng tiếng Việt, ngắn gọn, đi thẳng vấn đề (code và tên kỹ thuật giữ tiếng Anh). 

Nói rõ giả định nào mình đã đặt ra. 

Nói thẳng khi một AC không khả thi / mâu thuẫn, thay vì làm qua loa. 

AI tuyệt đối không 

Tự thêm tính năng, package, hoặc đổi kiến trúc khi chưa hỏi. 

Hard-code secret, log mật khẩu/token, lưu token ngoài SecureStorage. 

Tự merge, tự force-push, hoặc bỏ qua code review. 

Bỏ qua DoD hoặc tuyên bố "xong" khi chưa đối chiếu AC. 

Làm theo AC rõ ràng bị sao chép nhầm (xem mục 13) mà không báo. 

 

13. Lưu ý: các chỗ kế hoạch có vẻ sai/nhầm — AI phải HỎI, không tự làm theo 

Kế hoạch dự án có một số AC/US bị sao chép nhầm từ dự án khác. Khi gặp, dừng lại và hỏi nhóm: 

Vị trí 

Vấn đề 

T15.1 (tag danh mục trên event card) 

AC nói về "tên sinh viên", trạng thái Đang học/Bảo lưu/Cảnh cáo/Tốt nghiệp, Chính quy/CLC, filter "danh sách sinh viên" → đây là AC của màn quản lý sinh viên, không phải event. Tag phải là danh mục sự kiện (Category). Cũng có AC "Hover/Tooltip" — không áp dụng trên mobile cảm ứng. 

T05.1 (quản lý tài khoản) 

AC dùng tag Đang học/Bảo lưu/Bị khóa, MSV — hợp lý cho tài khoản sinh viên nhưng cần xác nhận có đúng với mô hình tài khoản của app này không. 

US37 / T37.1 (tự sinh JWT) 

Nếu dùng Firebase Auth thì Firebase đã cấp ID Token (JWT), không cần tự viết API sinh JWT. Cần nhóm xác nhận: dùng Firebase Auth thuần, hay vẫn tự sinh JWT riêng ở server (khi đó phải xác minh Firebase token trước). AI không tự chọn. 

T37.1 (API đăng nhập + JWT) 

AC1–AC8 mô tả đăng ký, không phải đăng nhập/sinh JWT. Cần AC đúng cho login (xác thực, sinh/hạn JWT, refresh, lỗi 401/403). 

US36 / T36.1 

Mã AC ghi 26.2.x thay vì 36.x. Nội dung đúng (đăng ký API) nhưng đánh số lệch. 

T35.1 

Trùng Task ID hai lần (Create/Update và Lock/Unlock). Nên là T35.1 và T35.2. 

T05.2 

Thiếu tên task (chỉ có AC phân quyền RBAC). 

US02, US09, US14 

Nhắc "đề tài / topic / nhóm / giảng viên hướng dẫn" → thuộc dự án quản lý đề tài, không thuộc app sự kiện. Không làm. 

US17–US25 

Điểm danh QR/RFID/face, analytics: chưa thuộc sprint nào. Không làm khi chưa có yêu cầu. 

AC "Axios Interceptor" 

Không dùng trong MAUI → dùng DelegatingHandler. 

AC "/dashboard", "/settings", "/admin/dashboard" 

Dạng route web → ánh xạ sang route Shell tương ứng (vd //dashboard). 

 

14. Thông tin cần nhóm bổ sung (AI không được tự đoán) 

[TODO] Phiên bản .NET / MAUI và danh sách NuGet đang dùng (thư viện Firebase cho MAUI: vd Plugin.Firebase hay gọi REST API của Firebase) 

[TODO] Firestore hay Realtime Database; cấu trúc collection (users, events, categories, registrations...) 

[TODO] Có dùng Cloud Functions / server trung gian không; nơi chạy Admin SDK 

[TODO] Tên project Firebase dev/prod và nơi giữ file cấu hình (không commit key) 

[TODO] Base URL API (dev/staging) và cách đưa vào app 

[TODO] Mẫu JSON response/error chuẩn 

[TODO] Mã màu Xanh Dương chính thức (Primary/Secondary/Accent...) 

[TODO] Link thiết kế Figma (nếu có) 

[TODO] Quy ước nhánh Git & ai là người review 

 