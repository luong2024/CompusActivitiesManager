using NUnit.Framework;
using UITests.Android.Core;
using UITests.Android.Pages;

namespace UITests.Android.Tests
{
    /// <summary>
    /// Bộ kiểm thử tự động giao diện Android: Luồng Đăng ký (RegisterPage).
    /// </summary>
    [TestFixture]
    [Category("Android_UI")]
    [Category("US26_US37_Register")]
    public class RegisterUITests : BaseAndroidUITest
    {
        private LoginPageObject _loginPage = null!;
        private RegisterPageObject _registerPage = null!;

        public override void BaseSetUp()
        {
            base.BaseSetUp();
            _loginPage = new LoginPageObject(this);
            _registerPage = new RegisterPageObject(this);

            // Từ màn hình Login, điều hướng sang màn hình Register
            _loginPage.ClickRegister();
            // Đảm bảo màn hình Đăng ký đã tải xong trước khi thực thi ca test
            FindByText("Đăng Ký Tài Khoản Sinh Viên");
        }

        [Test]
        [Description("TC_UI_08: Màn hình Đăng ký hiển thị đầy đủ các trường thông tin")]
        public void TC_UI_08_RegisterPage_ShouldDisplayAllRegistrationFields()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_registerPage.IsLoaded(), Is.True, "Màn hình Đăng ký chưa tải đầy đủ.");
                Assert.That(_registerPage.FullNameInput, Is.Not.Null, "Thiếu ô Họ và Tên.");
                Assert.That(_registerPage.UsernameInput, Is.Not.Null, "Thiếu ô MSSV / Username.");
                Assert.That(_registerPage.EmailInput, Is.Not.Null, "Thiếu ô Email.");
                Assert.That(_registerPage.PhoneInput, Is.Not.Null, "Thiếu ô Số điện thoại.");
                Assert.That(_registerPage.PasswordInput, Is.Not.Null, "Thiếu ô Mật khẩu.");
                Assert.That(_registerPage.ConfirmPasswordInput, Is.Not.Null, "Thiếu ô Xác nhận mật khẩu.");
                Assert.That(_registerPage.SubmitButton, Is.Not.Null, "Thiếu nút Đăng ký tài khoản.");
                Assert.That(_registerPage.BackToLoginButton, Is.Not.Null, "Thiếu nút Quay lại đăng nhập.");
            });
        }

        [Test]
        [Description("TC_UI_09: Bấm Đăng ký khi chưa nhập dữ liệu phải hiển thị thông báo lỗi")]
        public void TC_UI_09_Register_EmptyFields_ShouldPreventSubmissionOrShowError()
        {
            _registerPage.ClickSubmit();

            var errorAlert = _registerPage.ErrorAlert;
            Assert.Multiple(() =>
            {
                Assert.That(errorAlert, Is.Not.Null, "Thông báo validation đăng ký không xuất hiện khi form rỗng.");
                Assert.That(_registerPage.GetErrorMessage(), Does.Contain("vui lòng").IgnoreCase
                    .Or.Contain("bắt buộc")
                    .Or.Contain("đầy đủ"), "Nội dung cảnh báo validation không phù hợp.");
            });
        }

        [Test]
        [Description("TC_UI_10: Nhấn 'Quay lại đăng nhập' điều hướng trở lại màn hình LoginPage")]
        public void TC_UI_10_Register_ClickBackToLogin_ShouldReturnToLoginPage()
        {
            _registerPage.ClickBackToLogin();

            Assert.Multiple(() =>
            {
                Assert.That(_loginPage.IsLoaded(), Is.True, "Không quay trở lại được màn hình Đăng nhập.");
                Assert.That(_loginPage.UsernameInput, Is.Not.Null, "Màn hình đăng nhập thiếu ô nhập username.");
            });
        }
    }
}
