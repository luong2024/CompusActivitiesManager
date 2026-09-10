# Nhiệm vụ: Xây dựng Event List API + Category Information

**Mã Task**: T38.1 / T05.4  
**User Story**: US38 – Cập nhật danh sách sự kiện theo danh mục (API) / US05 – Quản lý hoạt động Campus  
**Người phụ trách**: Vũ Tiến Đạt  
**Trạng thái**: Hoàn thành (Done)  
**Thời gian dự kiến (Estimate)**: 12 giờ  

---

## 1. Mục tiêu nhiệm vụ
Xây dựng RESTful API endpoint cung cấp danh sách sự kiện (Events) của trường, bao gồm thông tin chi tiết về danh mục (Category) tương ứng, xử lý đúng đắn các sự kiện không thuộc danh mục nào, hỗ trợ lọc/tìm kiếm, trả về chuẩn `ApiResponse<T>`, và xây dựng bộ kiểm thử tự động toàn diện.

---

## 2. Tiêu chí nghiệm thu (Acceptance Criteria)

| Tiêu chí | Trạng thái | Chi tiết triển khai & Kiểm chứng |
| :--- | :---: | :--- |
| **AC1** | **PASS** | API trả về danh sách Event thành công với HTTP status code `200 OK`. Endpoint: `GET /api/v1/events` (alias `GET /api/events`). |
| **AC2** | **PASS** | Mỗi Event trong response có thông tin Category tương ứng (`id`, `name`/`title`, `description`, `color`, `icon`). |
| **AC3** | **PASS** | Event không có Category vẫn được xử lý đúng theo business rule (`categoryId: null`, `category: null`), không gây ngoại lệ hoặc lỗi hệ thống. Hỗ trợ lọc qua tham số `hasCategory=false`. |
| **AC4** | **PASS** | API trả về danh sách rỗng `[]` (không phải 404) nếu không có Event phù hợp với bộ lọc (ví dụ `categoryId` không tồn tại). |
| **AC5** | **PASS** | API trả về HTTP status và response format đúng quy định theo chuẩn `ApiResponse<T>` (`success: true`, `statusCode: 200`, `message`, `data`) và `ApiErrorResponse` đối với lỗi 400, 404, 500. |
| **AC6** | **PASS** | API được kiểm thử với Event có và không có Category: Đã triển khai bộ unit test xUnit với 11 test cases trong `CampusActivitiesManager.Api.Tests` đạt 100% Pass. |

---

## 3. Cấu trúc Triển khai Mã nguồn

1. **Models / DTOs**:
   - `CategoryDto.cs`: Đại diện thông tin danh mục (`Id`, `Name`, `Title`, `Description`, `Color`, `Icon`).
   - `EventDto.cs`: Đại diện thông tin sự kiện (`Id`, `Title`, `Name`, `Description`, `Location`, `StartDate`, `EndDate`, `Status`, `CategoryId`, `Category`, `MaxParticipants`, `CurrentParticipants`, `BannerUrl`).
2. **Service Layer**:
   - `IEventService.cs`: Interface định nghĩa các phương thức lấy danh sách Event (có lọc theo `categoryId`, `hasCategory`, `search`, `status`), lấy chi tiết Event, lấy danh mục.
   - `EventService.cs`: Hiện thực hóa nghiệp vụ với dataset sự kiện thực tế gồm cả sự kiện có danh mục và không có danh mục.
3. **Controller Layer**:
   - `EventsController.cs`: Cung cấp các endpoints RESTful với chuẩn phản hồi `ApiResponse<T>` và `ApiErrorResponse`.
4. **Test Suite**:
   - `CampusActivitiesManager.Api.Tests`: Project xUnit kiểm thử độc lập cho `EventsController` và `EventService`, bao phủ 100% các tiêu chí AC1 đến AC6.
