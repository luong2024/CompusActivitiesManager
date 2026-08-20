### 1\. Ở Backend & Cơ sở dữ liệu (Database) \- *Quan trọng nhất*

#### A. Đối tượng Người dùng (User Object / Entity / Record)

Mỗi tài khoản được lưu trữ như một cấu trúc Bản ghi (Record) hoặc Dictionary / JSON Object chứa các trường dữ liệu:

#### B. Mảng / Danh sách (Array / List)

* Danh sách tài khoản (User List): Dùng để chứa danh sách trả về khi hiển thị Bảng quản lý người dùng ở trang Admin.  
* Danh sách Quyền (Roles / Permissions List): Dùng lưu các quyền cụ thể của một Admin (ví dụ: `["READ_USER", "DELETE_USER", "EDIT_ROLE"]`).

#### C. Bảng băm / Mã băm (Hash Table / Hash Map / Key-Value)

* Tra cứu nhanh theo ID hoặc Email: Trong Database (như SQL, MongoDB) hoặc Bộ nhớ đệm Cache (Redis), dữ liệu được lưu dưới dạng `Key-Value` (với Key là `User ID` hoặc `Session Token`, Value là `User Info`) để kiểm tra quyền truy cập và thông tin đăng nhập trong thời gian O(1).  
* Lưu phiên đăng nhập (Session / Token Storage): Redis dùng Hash Map để lưu chuỗi JWT/SessionID kèm theo trạng thái đăng nhập của người dùng.

#### D. Cây chỉ mục (B-Tree / B+ Tree)

* Cấu trúc cơ sở dữ liệu ngầm định (Index) trên các trường như `email` hoặc `username`. Giúp hệ thống kiểm tra email trùng lặp khi Đăng ký cực kỳ nhanh chóng mà không cần duyệt toàn bộ cơ sở dữ liệu.

### 2\. Ở Frontend (Giao diện người dùng)

#### A. Đối tượng & Trạng thái (Object & State)

* Form State (Object): Dùng lưu trữ dữ liệu người dùng đang nhập trên giao diện.  
* JavaScript

const formData \= {  
  email: "user@example.com",  
  password: "Password123\!",  
  confirmPassword: "Password123\!"  
};

*   
*   
* Auth State / Global State (Object): Lưu thông tin người dùng đang đăng nhập hiện tại trên toàn ứng dụng (dùng trong React Context, Redux, Vuex...):  
* JavaScript

const authState \= {  
  isAuthenticated: true,  
  user: { id: "USR1002", role: "ADMIN", name: "Admin Manager" },  
  token: "eyJhbGciOi..."  
};

*   
* 

#### B. Mảng (Array)

* Render Bảng Admin: Dùng `Array` chứa danh sách người dùng thu thập từ API để duyệt qua (`.map()`) và hiển thị thành các hàng (Rows) trong Bảng quản lý.

#### C. Tập hợp (Set)

* Quản lý lựa chọn hàng loạt (Bulk Actions): Khi Admin chọn tích vào nhiều ô người dùng để "Xóa nhiều" hoặc "Duyệt nhiều", cấu trúc Set giúp lưu danh sách các `User ID` đã chọn để đảm bảo các ID không bị trùng lặp và thao tác thêm/xóa ID diễn ra nhanh chóng.

### 3\. Cấu trúc dữ liệu cho các tính năng nâng cao (Tùy chọn)

1. Thống kê & Phân trang trang Admin:  
   * Phân trang (Pagination): Dùng tham số `Limit` và `Offset` (hoặc `Cursor-based`) khi truy vấn danh sách người dùng.  
   * Hàng đợi (Queue): Dùng để gửi email xác thực đăng ký/quên mật khẩu (đẩy tác vụ gửi mail vào Queue để tránh làm treo ứng dụng).  
2. Lịch sử hoạt động (Activity Logs / Audit Trail):  
   * Ngăn xếp (Stack) hoặc Danh sách liên kết (Linked List): Lưu nhật ký thao tác của Admin (như: khoá tài khoản, sửa quyền) theo thứ tự thời gian gần nhất lên đầu.

