                                                                       NHIỆM VỤ
                                            Kết nối giao diện quản lý tài khoản với API & xử lý thông báo lỗi


Đầu tiên xác định API mà giao diện cần kết nối:
| Chức năng       | Method | Endpoint           |
| --------------- | ------ | ------------------ |
| Lấy danh sách   | GET    | /api/accounts      |
| Lấy 1 tài khoản | GET    | /api/accounts/{id} |
| Thêm            | POST   | /api/accounts      |
| Sửa             | PUT    | /api/accounts/{id} |
| Xóa             | DELETE | /api/accounts/{id} |
- Tạo model account: sử dụng model để biểu diễn dữ liệu và sau đó service thực hiện các thao tác trên dữ liệu đó
- Tạo interface cho service: sử dụng IUserService<T> để định nghĩa các thao tác với user, áp dụng nó cho account
- Tạo AccountService để gọi API
- Làm đầy đủ CRUD ( lấy danh sách, lấy tài khoản, thêm, sửa, xóa )
- Đăng ký Service bằng Dependency Injection
- Kết nối ViewModel với Service
- Tạo chức năng load danh sách trong ViewModel
- Kết nối ViewModel với giao diện
- Dùng command cho thêm/sửa/xóa sau đó làm các chức năng đó
- Lỗi chia thành: Validation error,API error,Network error
- Tạo ErrorMessage trong ViewModel
- Hiển thị lỗi giao diện 
<Label
    Text="{Binding ErrorMessage}"
    IsVisible="{Binding HasError}" />
- Xử lý lỗi API: để service kiểm tra
- Sử dụng IsBusy trong ViewModel và binding nó với ActivityIndicator để biểu diễn trạng thái đang xử lý 
