using NUnit.Framework;
using CampusActivitiesManager.Models;
using UITests.Android.Models;

namespace UITests.Android.Tests
{
    /// <summary>
    /// Bộ kiểm thử logic giao diện (UI Logic & Component Tests) cho các nhiệm vụ chính của Nguyễn An Cương:
    /// - T05.1: Giao diện quản lý tài khoản, Thẻ người dùng, Bộ lọc Chip/Popup & Bố cục Responsive.
    /// - T28.1: Giao diện đăng nhập, xác thực dữ liệu đầu vào & cơ chế tự khóa sau 5 lần nhập sai.
    /// - T26.1 & T36.1: Giao diện đăng ký, mã hóa mật khẩu PBKDF2-SHA256 & kiểm tra trùng email.
    /// - T15.1: Tag danh mục sự kiện bo góc viên thuốc, rút gọn khi dài hơn 15 ký tự.
    /// Có thể thực thi độc lập 100% bằng 'dotnet test' mà không cần bật trước Appium Server.
    /// </summary>
    [TestFixture]
    [Category("NguyenAnCuong_UITest")]
    public class AccountManagementAndAuthUITests
    {
        private AccountTestService _service = null!;

        [SetUp]
        public void Setup()
        {
            _service = new AccountTestService();
        }

        // =====================================================================
        // 1. KIỂM THỬ GIAO DIỆN QUẢN LÝ TÀI KHOẢN (US05 - T05.1: NGUYỄN AN CƯƠNG)
        // =====================================================================

        [Test]
        [Category("T05.1_AccountManagement_UI")]
        [Description("AC 1.1: Thẻ tài khoản hiển thị đúng chữ cái đầu Avatar, màu sắc trạng thái và vai trò")]
        public void T05_1_UserModel_AvatarInitials_AndStatusColor_RenderCorrectly()
        {
            var user = new User
            {
                Id = "101",
                Username = "B20DCCN001",
                FullName = "Nguyễn An Cương",
                Email = "cuong.na@campus.edu.vn",
                Role = Role.Admin,
                IsActive = true
            };

            Assert.Multiple(() =>
            {
                Assert.That(user.IntId, Is.EqualTo(101));
                Assert.That(user.AvatarInitials, Is.EqualTo("NC"), "Avatar Initials cho 'Nguyễn An Cương' phải là 'NC'.");
                Assert.That(user.IsAdminRole, Is.True, "Cờ IsAdminRole phải là true đối với Role.Admin.");
                Assert.That(user.StatusText, Is.EqualTo("Đang hoạt động"));
                Assert.That(user.StatusColorHex, Is.EqualTo("#16A34A"), "Màu trạng thái hoạt động phải là xanh lá (#16A34A).");
                Assert.That(user.RoleBadgeColor, Is.EqualTo("#DC2626"), "Màu huy hiệu Admin phải là đỏ (#DC2626).");
            });
        }

        [Test]
        [Category("T05.1_AccountManagement_UI")]
        [Description("AC 1.3: Bộ lọc nâng cao tính đúng số lượng tiêu chí đang chọn và hỗ trợ Reset")]
        public void T05_1_AccountFilterCriteria_ActiveCount_AndReset_WorkCorrectly()
        {
            var filter = new AccountFilterCriteria
            {
                SearchText = "Cương",
                SelectedStatus = AccountStatus.DangHoc,
                SelectedClass = "D20CNTT1",
                SelectedAcademicYear = "K20",
                SelectedRole = AccountRole.LopTruong
            };

            Assert.Multiple(() =>
            {
                Assert.That(filter.ActiveFilterCount, Is.EqualTo(3), "Số lượng filter nâng cao (Lớp, Khóa, Quyền) phải bằng 3.");
                Assert.That(filter.HasAdvancedFilters, Is.True);
            });

            var cloned = filter.Clone();
            filter.Reset();

            Assert.Multiple(() =>
            {
                Assert.That(filter.ActiveFilterCount, Is.EqualTo(0));
                Assert.That(filter.HasAdvancedFilters, Is.False);
                Assert.That(filter.SearchText, Is.Empty);
                Assert.That(cloned.SelectedClass, Is.EqualTo("D20CNTT1"));
            });
        }

        [TestCase(400, 1, TestName = "Responsive: Màn hình Mobile (400px) hiển thị 1 cột")]
        [TestCase(768, 2, TestName = "Responsive: Màn hình Tablet (768px) hiển thị 2 cột")]
        [TestCase(1280, 3, TestName = "Responsive: Màn hình Desktop (1280px) hiển thị 3 cột")]
        [Category("T05.1_AccountManagement_UI")]
        [Description("AC 1.4: Bố cục lưới thích ứng (Responsive Grid) 1 cột trên Mobile, 2-3 cột trên Tablet/Desktop")]
        public void T05_1_ResponsiveLayout_ColumnSpan_AdaptsToScreenWidth(double width, int expectedColumns)
        {
            int span = AccountTestService.CalculateResponsiveColumnSpan(width);
            Assert.That(span, Is.EqualTo(expectedColumns));
        }

        [Test]
        [Category("T05.1_AccountManagement_UI")]
        [Description("Ràng buộc bảo mật: Admin không thể tự khóa tài khoản của chính mình")]
        public async Task T05_1_AdminCannotLockSelf_Returns400BadRequest()
        {
            var adminLogin = await _service.LoginAsync("admin", "Admin@123456");
            var adminUser = adminLogin.Data!;

            var response = await _service.SetLockStateAsync(adminUser, adminUser.Id, lockAccount: true);

            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.StatusCode, Is.EqualTo(400));
                Assert.That(response.Message, Does.Contain("Admin không thể tự khóa tài khoản của chính mình"));
            });
        }

        [Test]
        [Category("T05.1_AccountManagement_UI")]
        [Description("Ràng buộc phân quyền: Sinh viên thông thường không được phép đổi quyền tài khoản (403 Forbidden)")]
        public async Task T05_1_StudentCannotAssignRole_Returns403Forbidden()
        {
            var studentLogin = await _service.LoginAsync("UST0504", "Dat@123456");
            var studentUser = studentLogin.Data!;

            var response = await _service.AssignRoleAsync(studentUser, studentUser.Id, Role.Admin);

            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.StatusCode, Is.EqualTo(403));
            });
        }

        // =====================================================================
        // 2. KIỂM THỬ GIAO DIỆN ĐĂNG NHẬP (US28 - T28.1: NGUYỄN AN CƯƠNG)
        // =====================================================================

        [Test]
        [Category("T28.1_Login_UI")]
        [Description("AC 2.2: Đăng nhập thành công với tài khoản sinh viên hợp lệ")]
        public async Task T28_1_Login_WithValidStudentCredentials_Returns200()
        {
            var result = await _service.LoginAsync("B20DCCN001", "Student@123456");

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(200));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data!.FullName, Is.EqualTo("Nguyễn An Cương"));
            });
        }

        [Test]
        [Category("T28.1_Login_UI")]
        [Description("AC 2.3: Nhập sai mật khẩu hiển thị thông báo lỗi bảo mật chung")]
        public async Task T28_1_Login_WithWrongPassword_Returns401()
        {
            var result = await _service.LoginAsync("B20DCCN001", "SaiMatKhau@123");

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(401));
                Assert.That(result.Message, Is.EqualTo("Tài khoản hoặc mật khẩu không chính xác"));
            });
        }

        [Test]
        [Category("T28.1_Login_UI")]
        [Description("AC 2.1: Bỏ trống thông tin đăng nhập bị chặn và báo lỗi validation")]
        public async Task T28_1_Login_WithEmptyInput_Returns400()
        {
            var result = await _service.LoginAsync("", "");

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(400));
                Assert.That(result.Message, Is.EqualTo("Vui lòng nhập đầy đủ thông tin"));
            });
        }

        [Test]
        [Category("T28.1_Login_UI")]
        [Description("AC 4: Tự động khóa tài khoản (ACCOUNT_LOCKED) sau 5 lần nhập sai mật khẩu liên tiếp")]
        public async Task T28_1_Login_LocksAccountAfter5FailedAttempts()
        {
            ApiResponse<User>? lastAttempt = null;
            for (int i = 1; i <= 5; i++)
            {
                lastAttempt = await _service.LoginAsync("B20DCCN001", $"WrongPass_{i}");
            }

            Assert.That(lastAttempt, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(lastAttempt!.Success, Is.False);
                Assert.That(lastAttempt.StatusCode, Is.EqualTo(403));
                Assert.That(lastAttempt.Message, Is.EqualTo("ACCOUNT_LOCKED"));
            });
        }

        // =====================================================================
        // 3. KIỂM THỬ ĐĂNG KÝ (US26 - T26.1 / T36.1) & TAG SỰ KIỆN (US15 - T15.1)
        // =====================================================================

        [Test]
        [Category("T26.1_Register_UI")]
        [Description("Đăng ký thành công: Mật khẩu được mã hóa an toàn bằng chuẩn PBKDF2-SHA256")]
        public async Task T26_1_Register_WithValidData_Returns201AndHashesPassword()
        {
            var response = await _service.RegisterAsync(
                fullName: "Nguyễn Văn Test",
                email: "test.nv@campus.edu.vn",
                password: "Test@123456",
                confirmPassword: "Test@123456",
                acceptedTerms: true);

            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.True);
                Assert.That(response.StatusCode, Is.EqualTo(201));
                Assert.That(response.Data, Is.Not.Null);
                Assert.That(response.Data!.PasswordHash, Does.StartWith("$PBKDF2-SHA256$"));
            });
        }

        [Test]
        [Category("T26.1_Register_UI")]
        [Description("Đăng ký trùng email báo lỗi 409 Conflict")]
        public async Task T26_1_Register_WithDuplicateEmail_Returns409Conflict()
        {
            var response = await _service.RegisterAsync(
                fullName: "Nguyễn An Cương",
                email: "cuong.na@campus.edu.vn",
                password: "Student@123456",
                confirmPassword: "Student@123456",
                acceptedTerms: true);

            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.StatusCode, Is.EqualTo(409));
            });
        }

        [Test]
        [Category("T15.1_TagDanhMuc_UI")]
        [Description("AC 1.3: Tag danh mục sự kiện dài quá 15 ký tự tự động rút gọn bằng dấu '...'")]
        public void T15_1_EventCategoryTag_TruncatesWithEllipsisWhenExceeds15Chars()
        {
            string longCategory = "Văn hóa - Nghệ thuật";
            string formatted = AccountTestService.FormatCategoryTagText(longCategory);

            Assert.Multiple(() =>
            {
                Assert.That(formatted.Length, Is.LessThanOrEqualTo(15));
                Assert.That(formatted, Is.EqualTo("Văn hóa - Ng..."));
            });
        }
    }
}
