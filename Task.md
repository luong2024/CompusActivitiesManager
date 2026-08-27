Danh sách nhiệm vụ cần thực hiện

1. Thiết kế Model & Enum
Tạo class User: Id, Username, PasswordHash, Role, IsActive
Tạo enum Role { Admin, Manager, User, Guest }

2. Xây dựng tầng dữ liệu (Repository/Service CSDL)
Khởi tạo kết nối CSDL, tạo bảng User bằng thư viện Data Model đã chọn
Viết CRUD: Thêm/Sửa/Xóa/Lấy danh sách tài khoản
Viết hàm lấy quyền (Role) theo User

3. AuthenticationService
Lưu trạng thái đăng nhập hiện tại (Current User, Current Role)
Viết CheckPermission(Role requiredRole)

4. Đăng ký Dependency Injection
Đăng ký AuthenticationService, UserRepository dạng Singleton/Scoped trong MauiProgram.cs
Đăng ký các ViewModel (LoginViewModel, UserManagementViewModel) vào DI container

5. Cấu hình Shell Navigation
Routing.RegisterRoute cho UserManagementPage, AccessDeniedPage
Dùng QueryPropertyAttribute để truyền dữ liệu User qua các trang
Kiểm tra CheckPermission trước khi Shell.Current.GoToAsync(), điều hướng sang AccessDeniedPage nếu thiếu quyền

6. LoginViewModel
Xác thực Username/Password
Lưu phiên đăng nhập vào AuthenticationService
Điều hướng theo Role sau khi đăng nhập thành công

7. UserManagementViewModel
ObservableCollection<User> để bind danh sách
Command: AddCommand, EditCommand, DeleteCommand, ChangeRoleCommand
Binding Modes: TwoWay (form nhập), OneWay (danh sách hiển thị)

8. Giao diện XAML
CollectionView/ListView hiển thị danh sách người dùng
Picker chọn Role, Entry/Editor chỉnh sửa thông tin
Context Actions cho thao tác nhanh (Đổi quyền, Khóa, Xóa) trên từng dòng

9. Tương tác & cảnh báo
DisplayAlert khi từ chối truy cập / báo lỗi
DisplayActionSheet khi xác nhận đổi quyền người dùng

10. Kiểm thử phân quyền
Test từng Role (Admin/Manager/User/Guest) với các trang/chức năng tương ứng
Test trường hợp điều hướng bị chặn → AccessDeniedPage