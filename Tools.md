Nhiệm vụ: Phân quyền truy cập
Các công cụ cần sử dụng:
1. Môi trường phát triển
- Visual Studio Community (với workload .NET Multi-platform App UI development): Dùng làm môi trường IDE chính để lập trình C#, thiết kế giao diện XAML, cấu hình project và debug ứng dụng đa nền tảng (Android, iOS, Windows, macOS).
2. Công cụ thiết kế Giao diện
- XAML (ContentPage, Layouts như Grid, StackLayout): Dùng để khai báo và định hình giao diện trang quản lý người dùng / phân quyền.
- ListView / CollectionView: Dùng để hiển thị danh sách các tài khoản người dùng kèm vai trò/quyền hiện tại.
- Context Actions (MenuItem trong ViewCell.ContextActions): Dùng để tạo menu tác vụ khi Admin nhấn giữ hoặc tương tác trên một dòng người dùng (ví dụ: bấm "Edit/Chỉnh sửa quyền", "Delete").
- Entry, Editor và Picker / Button: Cung cấp ô nhập thông tin, danh sách chọn vai trò/quyền hạn mới và nút bấm "Save / Cập nhật".
- DisplayActionSheet & DisplayAlert:
  + DisplayActionSheet: Hiển thị bảng chọn nhanh các loại vai trò/mẫu quyền hạn cho tài khoản.
  + DisplayAlert: Hiển thị hộp thoại pop-up thông báo kết quả cập nhật thành công vào CSDL hoặc cảnh báo khi có lỗi.
3. Mô hình kiến trúc MVVM & Cơ chế Data Binding
- Mô hình MVVM (Model - View - ViewModel): Tách UI khỏi logic nghiệp vụ phân quyền.
- INotifyPropertyChanged / BaseViewModel (SetProperty): Đồng bộ dữ liệu vai trò giữa ViewModel và View.
- ObservableCollection<T>: Lưu danh sách tài khoản trong ViewModel, tự cập nhật UI khi có thay đổi. 
- Command / ICommand (kết hợp Validate qua canExecute): Gắn lệnh UpdateCommand/SaveCommand khi Admin bấm nút cập nhật quyền 
- Binding Modes (TwoWay, OneWay, OneWayToSource): Đồng bộ giá trị vai trò được chọn giữa View và ViewModel.
4. Dependency Injection & Service Layer
- Microsoft.Extensions.DependencyInjection (MS.DI) trong .NET Generic Host (MauiProgram.cs): Đăng ký dịch vụ người dùng/cơ sở dữ liệu ở dạng Singleton/Scoped (builder.Services.AddSingleton<IUserService<User>, UserService>() hoặc IDataStore).
- Constructor Injection / ServiceHelper: Lấy các service vào ViewModel để xử lý nghiệp vụ.
- Giao diện Dịch vụ IUserService<T> & IDataStore<T> (CRUDL operations):
- GetUsersList() / GetItemsAsync(): Đọc danh sách tài khoản từ cơ sở dữ liệu.
- UpdateUserAsync / UpdateItemAsync: Thực thi cập nhật vai trò, quyền hạn mới của người dùng và lưu dữ liệu xuống CSDL.
5. Khung Điều hướng (.NET MAUI Shell & Navigation)
- .NET MAUI Shell & Routing.RegisterRoute: Quản lý cấu trúc điều hướng toàn ứng dụng, đăng ký route đến trang chỉnh sửa phân quyền.
- Shell.Current.GoToAsync() & QueryProperty: Điều hướng từ danh sách tài khoản sang trang Edit Role, truyền UserId qua tham số truy vấn 
