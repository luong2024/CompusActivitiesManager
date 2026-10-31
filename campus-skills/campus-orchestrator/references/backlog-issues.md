# Vấn đề đã biết trong backlog

Gặp mục nào trong scope: ghi vào Open Questions và hỏi người dùng. Không giao worker làm theo AC lỗi.

1. **AC sai domain:** T15.1 (tag danh mục sự kiện) và T05.1 có AC nói về "tên sinh viên", trạng thái Đang học/Bảo lưu/Cảnh cáo/Tốt nghiệp, hệ đào tạo Chính quy/CLC. Cần xác nhận tag là danh mục sự kiện hay trạng thái tài khoản.
2. **T37.1 (API đăng nhập)** mang AC của đăng ký (email trùng, hash password). Thiếu AC đăng nhập + JWT (401, 403 khi khóa, token hết hạn).
3. **Trùng/thiếu Task ID:** hai dòng `T35.1`; thiếu `T26.2`, `T28.2`, `T15.2`; T05.2 không có mô tả.
4. **Số hiệu AC lệch:** T36.1 dùng nhãn `AC 26.2.x`. T05.4 gộp test tạo/sửa/khóa/phân quyền nhưng ước 3h.
5. **Task "LÀM HỘ"** (T05.4, T26.5, T28.5 AC4): xác nhận ai sở hữu.
6. **Công nghệ mâu thuẫn:** T35.1 AC6 "dùng Firebase"; AC khác nhắc CSDL, Bcrypt/Argon2, JWT. Hỏi backend/DB thực tế.
7. **US ngoài phạm vi:** US02, US09, US14 nói về đề tài/nhóm/giảng viên; US01/US03/US11/US12 trùng ý (xem chi tiết sự kiện). Không đưa vào sprint nếu chưa xác nhận.
8. **Sheet "Rủi ro" trống:** đề xuất nội dung để người dùng duyệt; không tự điền vào file.
9. **AC 28.4.3** dùng route kiểu web (`/admin/dashboard`); MAUI dùng Shell route, xác nhận tên route.
