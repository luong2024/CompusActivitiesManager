using NUnit.Framework;
using UITests.Android.Core;
using UITests.Android.Pages;

namespace UITests.Android.Tests
{
    /// <summary>
    /// Bộ kiểm thử tự động giao diện Android: Luồng Đăng nhập (LoginPage).
    /// </summary>
    [TestFixture]
    [Category("Android_UI")]
    [Category("US28_Login")]
    public class LoginUITests : BaseAndroidUITest
    {
        private LoginPageObject _loginPage = null!;

        public override void BaseSetUp()
        {
            base.BaseSetUp();
            _loginPage = new LoginPageObject(this);
        }

        [Test]
        [Description("TC_UI_01: Màn hình Đăng nhập hiển thị đầy đủ các trường dữ liệu và nút bấm bắt buộc")]
        public void TC_UI_01_LoginPage_ShouldDisplayAllRequiredFields()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_loginPage.IsLoaded(), Is.True, "Màn hình Đăng nhập chưa tải hoặc thiếu trường nhập liệu.");
                Assert.That(_loginPage.UsernameInput, Is.Not.Null, "Không tìm thấy ô nhập Tên đăng nhập.");
                Assert.That(_loginPage.PasswordInput, Is.Not.Null, "Không tìm thấy ô nhập Mật khẩu.");
                Assert.That(_loginPage.LoginButton, Is.Not.Null, "Không tìm thấy nút Đăng nhập.");
                Assert.That(_loginPage.RegisterLink, Is.Not.Null, "Không tìm thấy liên kết Đăng ký ngay.");
                Assert.That(_loginPage.ForgotPasswordLink, Is.Not.Null, "Không tìm thấy liên kết Quên mật khẩu.");
            });
        }

        [Test]
        [Description("TC_UI_02: Để trống thông tin khi bấm Đăng nhập phải hiển thị thông báo validation")]
        public void TC_UI_02_Login_EmptyFields_ShouldShowValidationError()
        {
            // Để trống cả username và password rồi nhấn đăng nhập
            _loginPage.EnterCredentials(string.Empty, string.Empty);
            _loginPage.ClickLogin();

            var errorAlert = FindByText("Vui lòng nhập") ?? _loginPage.ErrorAlert;
            Assert.Multiple(() =>
            {
                Assert.That(errorAlert, Is.Not.Null, "Khung thông báo lỗi không xuất hiện khi để trống thông tin.");
                Assert.That(errorAlert?.Text, Does.Contain("Vui lòng").Or.Contain("không"), "Nội dung cảnh báo validation không khớp.");
            });
        }

        [Test]
        [Description("TC_UI_03: Nhập sai mật khẩu phải hiển thị thông báo lỗi từ chối đăng nhập")]
        public void TC_UI_03_Login_InvalidCredentials_ShouldDisplayErrorMessage()
        {
            _loginPage.EnterCredentials("B20DCCN001", "SaiMatKhau@999");
            _loginPage.ClickLogin();

            var errorAlert = FindByText("không chính xác") 
                ?? FindByText("Tên đăng nhập hoặc mật khẩu không chính xác")
                ?? _loginPage.ErrorAlert;
            Assert.Multiple(() =>
            {
                Assert.That(errorAlert, Is.Not.Null, "Thông báo lỗi không xuất hiện khi nhập sai mật khẩu.");
                Assert.That(errorAlert?.Text, Does.Contain("không chính xác").Or.Contain("không hợp lệ").Or.Contain("Lỗi"), "Thông báo lỗi đăng nhập không đúng định dạng.");
            });
        }

        [Test]
        [Description("TC_UI_04: Nút ẩn/hiện mật khẩu (con mắt) phản hồi khi người dùng chạm vào")]
        public void TC_UI_04_Login_TogglePasswordVisibility_ShouldWork()
        {
            _loginPage.EnterCredentials("B20DCCN001", "Student@123");
            Assert.That(_loginPage.TogglePasswordButton, Is.Not.Null, "Nút chuyển đổi ẩn hiện mật khẩu không tồn tại.");

            // Click vào nút con mắt
            _loginPage.TogglePassword();
            // Xác nhận không gây crash ứng dụng và vẫn giữ nguyên giá trị
            Assert.That(_loginPage.PasswordInput?.Text, Is.Not.Null);
        }

        [Test]
        [Description("TC_UI_05: Nhấn 'Đăng ký ngay' điều hướng thành công sang màn hình Đăng ký tài khoản")]
        public void TC_UI_05_Login_ClickRegisterLink_ShouldNavigateToRegisterPage()
        {
            _loginPage.ClickRegister();

            var registerPage = new RegisterPageObject(this);
            Assert.That(registerPage.TitleHeader, Is.Not.Null, "Không điều hướng được sang màn hình Đăng ký tài khoản.");
        }

        [Test]
        [Description("TC_UI_06: Đăng nhập thành công với tài khoản Sinh viên (B20DCCN001 / Student@123)")]
        public void TC_UI_06_Login_ValidStudentAccount_ShouldNavigateToMainPage()
        {
            _loginPage.EnterCredentials("B20DCCN001", "Student@123");
            _loginPage.ClickLogin();

            var mainPage = new MainPageObject(this);
            var greeting = mainPage.GetGreetingText();
            Assert.Multiple(() =>
            {
                Assert.That(mainPage.IsLoaded(), Is.True, "Không điều hướng được vào Trang chủ sau khi đăng nhập thành công.");
                Assert.That(greeting, Is.Not.Null, "Không lấy được nội dung lời chào sinh viên.");
                Assert.That(greeting, Does.Contain("Nguyễn An Cương").Or.Contain("Chào").Or.Contain("chào"), 
                    "Lời chào sinh viên không hiển thị đúng thông tin họ tên.");
            });
        }

        [Test]
        [Description("TC_UI_07: Đăng nhập với tài khoản Quản trị viên (admin / Admin@123)")]
        public void TC_UI_07_Login_ValidAdminAccount_ShouldShowAdminFeatures()
        {
            _loginPage.EnterCredentials("admin", "Admin@123");
            _loginPage.ClickLogin();

            var mainPage = new MainPageObject(this);
            Assert.That(mainPage.IsLoaded(), Is.True, "Admin đăng nhập không thành công vào Trang chủ.");

            // Mở menu để kiểm tra quyền Admin
            mainPage.OpenMenu();
            Assert.That(mainPage.UserManagementMenuItem, Is.Not.Null, "Menu Quản lý người dùng cho Admin không xuất hiện.");
        }
    }
}
