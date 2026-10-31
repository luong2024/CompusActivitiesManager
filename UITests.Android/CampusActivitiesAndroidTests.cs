using NUnit.Framework;
using CampusActivitiesManager.Models;
using UITests.Models;

namespace UITests
{
    [TestFixture]
    public class CampusActivitiesAndroidTests
    {
        private AccountTestService _service = null!;

        [SetUp]
        public void Setup()
        {
            _service = new AccountTestService();
        }

        // =====================================================================
        // 1. UNIT TEST US05 / T05.1 — QUẢN LÝ TÀI KHOẢN, BỘ LỌC & RESPONSIVE UI
        // =====================================================================

        [Test]
        [Category("US05_T05.1")]
        public void UserModel_PropertiesAndInitials_FormatCorrectly()
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
                Assert.That(user.AvatarInitials, Is.EqualTo("NC"));
                Assert.That(user.IsAdminRole, Is.True);
                Assert.That(user.StatusText, Is.EqualTo("Đang hoạt động"));
                Assert.That(user.StatusColorHex, Is.EqualTo("#16A34A"));
                Assert.That(user.RoleBadgeColor, Is.EqualTo("#DC2626"));
            });
        }

        [Test]
        [Category("US05_T05.1")]
        public void AccountFilterCriteria_ActiveFilterCountAndReset_WorkCorrectly()
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
                Assert.That(filter.ActiveFilterCount, Is.EqualTo(3));
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

        [TestCase(400, 1)]
        [TestCase(768, 2)]
        [TestCase(1280, 3)]
        [Category("US05_T05.1")]
        public void ResponsiveLayout_ColumnSpan_AdaptsToScreenWidth(double width, int expectedColumns)
        {
            int span = AccountTestService.CalculateResponsiveColumnSpan(width);
            Assert.That(span, Is.EqualTo(expectedColumns));
        }

        [Test]
        [Category("US05_T05.4")]
        public async Task SetAccountLockState_WhenAdminLocksSelf_Returns400BadRequest()
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
        [Category("US05_T05.4")]
        public async Task AssignUserRole_WhenCalledByNormalStudent_Returns403Forbidden()
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
        // 2. UNIT TEST ĐĂNG NHẬP (US28 - T28.5)
        // =====================================================================

        [Test]
        [Category("US28_T28.5")]
        public async Task Login_WithValidStudentCredentials_Returns200()
        {
            var result = await _service.LoginAsync("UST0504", "Dat@123456");

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(200));
                Assert.That(result.Data, Is.Not.Null);
            });
        }

        [Test]
        [Category("US28_T28.5")]
        public async Task Login_WithWrongPassword_Returns401GenericMessage()
        {
            var result = await _service.LoginAsync("UST0504", "SaiMatKhau@123");

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(401));
                Assert.That(result.Message, Is.EqualTo("Tài khoản hoặc mật khẩu không chính xác"));
            });
        }

        [Test]
        [Category("US28_T28.5")]
        public async Task Login_WithEmptyInput_Returns400ValidationMessage()
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
        [Category("US28_T28.5")]
        public async Task Login_After5ConsecutiveFailedAttempts_LocksAccountWith403()
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
        // 3. UNIT TEST ĐĂNG KÝ TÀI KHOẢN (US26 - T26.5) & TAG SỰ KIỆN (US15)
        // =====================================================================

        [Test]
        [Category("US26_T26.5")]
        public async Task Register_WithValidData_Returns201AndHashesPassword()
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
        [Category("US26_T26.5")]
        public async Task Register_WithDuplicateEmail_Returns409Conflict()
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
        [Category("US15_T15.3")]
        public void FormatCategoryTagText_WhenTitleExceeds15Chars_TruncatesWithEllipsis()
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
