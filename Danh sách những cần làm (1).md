Tài li?u yêu c?u nghi?p v? Business Requirement Document

Thông tin chung
Mã User Story US 05
Tên tính n?ng Phát tri?n giao di?n qu?n lý tài kho?n Danh sách Tìm ki?m L?c
N?n t?ng NET MAUI Cross platform Mobile Tablet Desktop
Ki?n trúc MVVM Model View ViewModel Shell Navigation
??i t??ng s? d?ng Qu?n tr? viên Admin Qu?n lý h? th?ng

1 M?c tiêu và t?ng quan Overview
Cung c?p giao di?n qu?n lý danh sách tài kho?n ng??i dùng tr?c quan t?i ?u cho ?a n?n t?ng Responsive Adaptive UI h? th?ng h? tr? tìm ki?m nhanh l?c d? li?u ?a tiêu chí và x? lý t?i d? li?u l?n m??t mà Lazy loading Infinite Scroll

2 Danh sách criteria ch?p nh?n Acceptance Criteria

AC 1 1 Hi?n th? danh sách tài kho?n
Th? tài kho?n Account Card bao g?m
Avatar ng??i dùng n?u không có avatar thì hi?n th? ?nh m?c ??nh Ký t? ??u tên
H? và tên Text Bold
Mã sinh viên MSV Email
Tag tr?ng thái Badge Chip Status phân bi?t rõ màu s?c
?ang h?c Màu xanh lá Success
B?o l?u Màu cam vàng Warning
B? khóa Màu ?? Danger Error
Nút Menu hành ??ng Ellipsis Icon m? ra các tùy ch?n nhanh Xem chi ti?t S?a Khóa M? khóa Xóa

AC 1 2 Tìm ki?m Search Bar
Ph?m vi tìm ki?m H? tên MSV Email
C? ch? Debounce Hoãn gõ
T? ??ng trigger tìm ki?m sau 300ms 500ms tính t? lúc ng??i dùng ng?ng gõ nh?m gi?m thi?u request liên t?c t?i API
Nút xóa nhanh Clear Button Icon X cho phép xóa toàn b? t? khóa trong ô tìm ki?m và reset danh sách v? tr?ng thái ban ??u

AC 1 3 B? l?c Filter
L?c nhanh Quick Filter
Thánh Chip tr?ng thái ngang bên d??i thanh tìm ki?m T?t c? ?ang h?c B?o l?u B? khóa
L?c nâng cao Advanced Filter Popup
M? Popup Modal ch?a các b? l?c
L?p Class Dropdown Picker danh sách l?p
Khóa Batch Cohort Dropdown Picker khóa h?c VD K14 K15
Quy?n Role Dropdown Picker phân quy?n VD Sinh viên Gi?ng viên Admin
Nút b?m Áp d?ng và Xóa b? l?c

AC 1 4 Tr?i nghi?m ng??i dùng UX UI và Layout Adaptive
Thao tác c? ch? Gestures
Kéo ?? làm m?i Pull to Refresh c?p nh?t danh sách m?i nh?t
Cu?n t?i thêm Infinite Scroll Load More t? ??ng l?y trang d? li?u ti?p theo khi cu?n g?n t?i cu?i danh sách
Responsive Grid Layout
Mobile Phone hi?n th? 1 c?t ListView Vertical CollectionView
Tablet Desktop hi?n th? Grid 2 3 c?t thích ?ng theo chi?u r?ng màn hình
