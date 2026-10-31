# Business Analysis & Technical Specification (BA.md)

## 1. General Information
- **Project**: Campus Activity Manager (.NET MAUI Mobile App & Firebase Cloud Services)
- **User Story**: US35 – Account Management & Cloud Sync (Quản lý tài khoản người dùng: tạo mới, cập nhật hồ sơ, khóa/mở khóa tài khoản đồng bộ trên Firebase)
- **Task ID**: T35.1 – Create Account & Update Profile Service / T35.2 – Get, Lock & Unlock Account Service
- **Estimated Time**: 10 hours
- **Assignee**: **Nguyen Duc Manh** (Tech Lead & Integration Developer)
- **Architecture & Tech Stack**:
  - **Mobile Client**: .NET MAUI 9.0 (C# / XAML) tuân thủ mô hình MVVM (Model - View - ViewModel) đa nền tảng (Android & Windows Desktop).
  - **Cloud Backend & Database**: **Firebase Cloud Services** (Google Cloud Firestore & Firebase Authentication) hoạt động theo mô hình **Serverless Cloud Architecture** (không phụ thuộc máy chủ trung gian truyền thống).
  - **Security & Cloud Sync**: Dịch vụ `FirebaseSyncService` tích hợp cơ chế Google OAuth2 Service Account RS256 JWT Token Exchange, mã hóa mật khẩu chuẩn RFC 2898 **PBKDF2-SHA256**, lưu trữ phiên làm việc an toàn qua `SecureStorage`.
  - **Service Pattern**: Phân lớp rõ ràng `PageModel -> Service (UserService, AuthenticationService, FirebaseSyncService) -> Cloud Firestore`.

---

## 2. Business Objectives
Cung cấp các hàm dịch vụ và giao diện chuẩn hóa cho phép Quản trị viên (Admin) và Ban tổ chức hệ thống:
1. Tạo mới tài khoản sinh viên/cán bộ và tự động tạo danh tính trên Firebase Authentication kết hợp lưu hồ sơ chi tiết trên Cloud Firestore.
2. Cập nhật thông tin hồ sơ (Họ tên, Số điện thoại, Avatar URL, Vai trò) và đồng bộ thời gian thực.
3. Khóa (Lock) hoặc Mở khóa (Unlock) tài khoản: vô hiệu hóa quyền đăng nhập trên Firebase Auth (`disabled = true/false`) và cập nhật trạng thái `IsActive` trên Cloud Firestore.
4. Bảo đảm toàn vẹn dữ liệu, phân cấp mã lỗi chuẩn RFC 7807 và bọc kết quả trong cấu trúc chuẩn `ApiResponse<T>`.

---

## 3. Detailed Acceptance Criteria & Scenarios

### AC1: The Service successfully creates an account when input data is valid
- **Description**: Khi dữ liệu đầu vào hợp lệ, hệ thống tạo bản ghi danh tính trên Firebase Authentication, tạo tài liệu người dùng tương ứng trong collection `users` trên Cloud Firestore và trả về thông tin tài khoản hoàn chỉnh kèm mã trạng thái `201 Created`.
- **Gherkin Scenario**:
```gherkin
Scenario: Successfully create a new user account
  Given An authenticated administrator with valid administrative permissions
  When An account creation request is triggered with:
    | email               | password      | fullName       | role    | studentCode |
    | student1@campus.edu | SecureP@ss123 | Nguyen Van A   | Student | B20DCCN001  |
  Then A new user identity is provisioned in Firebase Auth
  And A user profile document is stored in Cloud Firestore
  And The service returns an ApiResponse with StatusCode 201 Created
  And The response body contains the new account ID, email, fullName, role, and createdAt timestamp
```

### AC2: Validation of required fields when creating an account
- **Description**: Hệ thống kiểm tra tính hợp lệ của dữ liệu trước khi xử lý. Nếu dữ liệu thiếu hoặc sai quy cách, hệ thống từ chối và trả về HTTP Status Code `400 Bad Request` hoặc `409 Conflict`.
- **Validation Rules**:
  - `email`: Bắt buộc, đúng định dạng RFC 5322 regex, không được trùng lặp trong hệ thống (trả về `409 Conflict` nếu đã tồn tại).
  - `password`: Bắt buộc, tối thiểu 6-8 ký tự, có độ phức tạp an toàn.
  - `fullName`: Bắt buộc, không được để trống hoặc chỉ chứa khoảng trắng.
  - `role`: Bắt buộc, phải thuộc một trong các vai trò hợp lệ: `Admin`, `Manager`, `Student`.
- **Gherkin Scenario**:
```gherkin
Scenario Outline: Fail account creation due to invalid input
  Given An account creation payload with invalid or missing <Field> value <Value>
  When Account creation is requested
  Then No new account is created in Firebase Auth or Cloud Firestore
  And The service returns StatusCode <HTTPCode>
  And The response body contains the relevant error message for <Field>

  Examples:
    | Field    | Value            | HTTPCode | Message                             |
    | email    | ""               | 400      | "Email is required"                 |
    | email    | "invalid-format" | 400      | "Invalid email format"              |
    | email    | "exist@knu.ac.kr"| 409      | "Email is already registered"       |
    | password | "123"            | 400      | "Password must be at least 6 chars" |
    | fullName | ""               | 400      | "Full name cannot be blank"         |
    | role     | "UnknownRole"    | 400      | "Invalid user role specified"       |
```

### AC3: The Service successfully updates existing account information
- **Description**: Cho phép Admin hoặc chính chủ tài khoản cập nhật thông tin cá nhân (Họ tên, SĐT, Avatar URL, Vai trò) dựa trên `accountId` (hoặc Firebase UID). Dữ liệu được đồng bộ ngay lên Cloud Firestore và trả về mã `200 OK`.
- **Gherkin Scenario**:
```gherkin
Scenario: Successfully update existing account details
  Given An account with ID "acc_12345" exists in Cloud Firestore
  When An update request is submitted with:
    | fullName     | phoneNumber | avatarUrl                      |
    | Nguyen Duc M | 0987654321  | https://cdn.campus.edu/avt.png |
  Then The account record is updated in Cloud Firestore
  And The service returns StatusCode 200 OK
  And The response data contains the updated profile details
```

### AC4: Updating non-existent accounts is rejected
- **Description**: Nếu cố gắng cập nhật một tài khoản không tồn tại trong hệ thống, hệ thống từ chối và trả về `404 Not Found`.
- **Gherkin Scenario**:
```gherkin
Scenario: Reject account update when account does not exist
  Given No account exists with ID "acc_99999"
  When An update request is sent for "acc_99999"
  Then The update operation is aborted
  And The service returns StatusCode 404 Not Found
  And The response message specifies "Account with ID acc_99999 not found"
```

### AC5: Lock & Unlock Account Workflow (T35.2)
- **Description**: Khóa hoặc Mở khóa tài khoản người dùng:
  - Khi Khóa: Đặt `disabled = true` trên Firebase Auth và `IsActive = false` trên Firestore, trả về `200 OK` kèm `Status = "Locked"`.
  - Khi Mở khóa: Đặt `disabled = false` trên Firebase Auth và `IsActive = true` trên Firestore, trả về `200 OK` kèm `Status = "Active"`.
  - Không cho phép Admin tự khóa tài khoản của chính mình.
- **Gherkin Scenario**:
```gherkin
Scenario: Successfully lock an active account
  Given An active user account "acc_001" exists in the system
  And The current actor is an Administrator (not "acc_001")
  When The administrator requests to lock "acc_001"
  Then The account is disabled in Firebase Authentication
  And The document in Cloud Firestore has IsActive set to false
  And The service returns StatusCode 200 OK with Message "Account locked successfully"
```

### AC6: Standard Response Envelope (RFC 7807 Compliant)
Tất cả các hàm dịch vụ trong dự án đều trả về chuẩn hóa qua đối tượng `ApiResponse<T>`:
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Account updated successfully",
  "data": {
    "id": "acc_12345",
    "email": "student@campus.edu",
    "fullName": "Nguyen Duc Manh",
    "role": "Student",
    "updatedAt": "2026-10-31T09:00:00Z"
  }
}
```

---

## 4. Technical Architecture: Serverless Cloud Sync
Thay vì dựng một API Gateway/Web API trung gian cồng kềnh tốn kém tài nguyên server, dự án sử dụng **Serverless Cloud Sync Service (`FirebaseSyncService`)**:
1. **Google OAuth2 Service Account Authentication**: Sử dụng Private Key và Client Email của Firebase Service Account để sinh JWT Assertion, ký bằng thuật toán `RS256` và gửi tới Google Token Endpoint (`https://oauth2.googleapis.com/token`) để lấy Access Token có hiệu lực 60 phút.
2. **Cloud Firestore REST Protocol**: Thực hiện các thao tác tạo/sửa/đọc tài liệu trực tiếp qua endpoint Google Cloud Firestore (`https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/...`).
3. **Local Cache & Offline Resilience**: Bộ đệm danh sách người dùng và sự kiện trong bộ nhớ hoặc SQLite cục bộ giúp ứng dụng phản hồi mượt mà ngay cả khi mạng chập chờn.
