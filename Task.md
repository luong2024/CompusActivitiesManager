Danh sách nhiệm vụ cần thực hiện

1. Thiết kế Model, Enum & Cấu trúc Dữ liệu
- Tạo Enum & Model:
  + Định nghĩa `enum Role { Admin, Manager, User, Guest }`.
  + Khởi tạo Model `User` với các trường: `Id`, `Username`, `PasswordHash`, `Role` (kiểu `Role`), `IsActive` (kiểu `bool`).
- Kế thừa Base Notifier: Cho phép `User` triển khai `INotifyPropertyChanged` hoặc kế thừa từ `BaseViewModel` để tự động kích hoạt cập nhật giao diện khi trường `Role` hoặc `IsActive` bị thay đổi.

2. Xây dựng Data Store & Dịch vụ CSDL
- Định nghĩa Interface: Tạo `IUserService<User>` hoặc `IDataStore<User>` bao gồm các phương thức: `GetUsersListAsync()`, `GetUserByIdAsync(string id)`, `UpdateUserRoleAsync(string id, Role newRole)`, `DeleteUserAsync(string id)`.
- Hiện thực hóa Repository: Triển khai truy vấn CSDL (SQLite/MockData) để thực thi các tác vụ CRUD, xử lý cập nhật trạng thái `Role` và `IsActive` xuống bộ nhớ cục bộ.

3. Xây dựng AuthenticationService & Logic kiểm tra quyền
- Quản lý phiên (Session): Tạo `IAuthenticationService` lưu trữ `CurrentUser` và `CurrentRole` trong suốt vòng đời phiên làm việc.
- Xác thực logic: Viết hàm `bool CheckPermission(Role requiredRole)` so sánh cấp bậc phân quyền hiện tại với quyền tối thiểu cần để thực thi tác vụ/truy cập trang.

4. Cấu hình Dependency Injection (MauiProgram.cs)
- Đăng ký Services:
  + `builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>()`
  + `builder.Services.AddSingleton<IUserService<User>, UserService>()`
- Đăng ký View & ViewModel:
  + Thêm `Transient` hoặc `Singleton` cho `LoginViewModel`, `UserManagementViewModel`, `EditUserRoleViewModel`.
  + Đăng ký tương ứng cho `LoginPage`, `UserManagementPage`, `AccessDeniedPage`.

5. Khung điều hướng .NET MAUI Shell & Routing
- Đăng ký Route: Dùng `Routing.RegisterRoute(nameof(UserManagementPage), typeof(UserManagementPage))` và `Routing.RegisterRoute(nameof(AccessDeniedPage), typeof(AccessDeniedPage))` trong `AppShell.xaml.cs`.
- Guard Navigation: Trước khi gọi `Shell.Current.GoToAsync()`, gọi `CheckPermission(Role.Admin)`. Nếu không đủ điều kiện, chuyển hướng ngay về `//AccessDeniedPage`.
- Truyền tham số: Cấu hình `[QueryProperty(nameof(UserId), "UserId")]` trên ViewModel nhận dữ liệu để load thông tin tài khoản cụ thể.

6. Xây dựng LoginViewModel & Luồng phân trang sau đăng nhập
- Khai báo thuộc tính: `Username`, `Password`, `IsBusy` kết hợp `SetProperty` từ `BaseViewModel`.
- LoginCommand:
- Kiểm tra tài khoản qua `IUserService`.
- Lưu thông tin người dùng vào `IAuthenticationService`.
- Dùng `Shell.Current.GoToAsync("//...")` điều hướng về trang chủ tương ứng theo từng `Role` (Admin vào trang quản trị, User/Guest vào trang xem nội dung).

7. Xây dựng UserManagementViewModel
- Danh sách phản ứng: Khởi tạo `ObservableCollection<User> UsersList` để tự động đồng bộ hiển thị.
- Tích hợp Constructor Injection: Inject `IUserService` và `IAuthenticationService`.
- Xây dựng Commands (ICommand):
  + `LoadUsersCommand`: Gọi `GetUsersListAsync()` và nạp vào `UsersList`.
  + `ChangeRoleCommand`: Gọi cập nhật quyền và làm mới danh sách.
  + `DeleteUserCommand`: Gắn hàm `canExecute` để ngăn chặn việc Admin tự xóa chính mình.

8. Thiết kế Giao diện XAML (UserManagementPage.xaml)
- Bố cục Layout: Dùng `Grid` và `StackLayout` chia khối danh sách người dùng và thanh công cụ tìm kiếm/lọc vai trò.
- Hiển thị danh sách (CollectionView/ListView):
  + `ItemTemplate`: `DataTemplate` hiển thị `Username`, nhãn vai trò (`Role`).
  + `Context Actions`: Khai báo `MenuItem` ("Đổi quyền", "Khóa/Mở tài khoản", "Xóa") bên trong `ViewCell.ContextActions` hoặc `SwipeView` để thao tác nhanh từng dòng.
- Form biên tập: Sử dụng `Picker` (Binding `TwoWay` với `SelectedRole`) và `Button` gắn `Command="{Binding SaveRoleCommand}"`.

9. Xử lý Tương tác & Thông báo (Dialogs & Prompts)
- DisplayActionSheet: Khi bấm "Đổi quyền" từ menu ngữ cảnh, mở hộp thoại chọn nhanh giữa các `Role` (`"Admin"`, `"Manager"`, `"User"`, `"Guest"`).
- DisplayAlert:
  + Pop-up xác nhận thao tác xóa tài khoản hoặc cảnh báo lỗi kết nối CSDL.
  + Thông báo từ chối truy cập: *"Bạn không có quyền thực hiện hành động này!"*.

10. Kiểm thử Toàn diện & Rà soát Phân quyền
- Role Verification Matrix:
  + Đăng nhập lần lượt bằng 4 tài khoản ứng với 4 vai trò (`Admin`, `Manager`, `User`, `Guest`).
  + Kiểm tra tính khả dụng của nút bấm, menu quản lý và khả năng mở `UserManagementPage`.
- Edge Cases & Security Test:
  + Test cố tình điều hướng URL Shell trực tiếp đến trang quản trị khi đang ở tài khoản `Guest`/`User` (đảm bảo chuyển hướng chính xác về `AccessDeniedPage`).
  + Kiểm tra khả năng lưu trữ trạng thái `Role` bền vững qua CSDL sau khi ứng dụng restart.