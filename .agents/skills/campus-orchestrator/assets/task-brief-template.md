Task: <Txx.x> — <tiêu đề> (<USxx>, <Fxx>, ~<n>h, owner: <người phụ trách>)
Role: <api | ui | integration | qa> | Nhánh: agent/<role>/<task-id>
Mục tiêu cuối: <1–2 câu: kết quả người dùng nhìn thấy>
AC (nguyên văn từ plan):
<dán đầy đủ, không tóm tắt>
Contract: <link Artifact API Contract + endpoint liên quan, hoặc "không có">
Vùng được ghi: <đường dẫn>
Không được: ghi ngoài vùng; đổi contract; log mật khẩu thô; commit secret
Đã làm trước đó: <task phụ thuộc đã xong, nhánh nào>
Tiêu chí xong: tất cả AC đạt + DoD1–DoD6 + bằng chứng
Giới hạn thử lại: 2 lần; kẹt thì trả status "blocked"
Đầu ra: JSON theo campus-orchestrator/assets/output-schema.json
