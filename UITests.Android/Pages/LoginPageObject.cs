using OpenQA.Selenium.Appium;
using UITests.Android.Core;

namespace UITests.Android.Pages
{
    /// <summary>
    /// Page Object Model cho màn hình Đăng nhập (LoginPage).
    /// </summary>
    public class LoginPageObject
    {
        private readonly BaseAndroidUITest _test;

        public LoginPageObject(BaseAndroidUITest test)
        {
            _test = test;
        }

        public AppiumElement? TitleHeader => _test.FindByText("Campus Activities Manager") ?? _test.FindByText("Đăng nhập Hệ thống");
        public AppiumElement? UsernameInput => _test.FindByAutomationId("txtUsername");
        public AppiumElement? PasswordInput => _test.FindByAutomationId("txtPassword");
        public AppiumElement? TogglePasswordButton => _test.FindByAutomationId("btnTogglePassword");
        public AppiumElement? RememberMeCheckbox => _test.FindByAutomationId("chkRememberMe");
        public AppiumElement? LoginButton => _test.FindByAutomationId("btnLogin") ?? _test.FindByText("Đăng nhập");
        public AppiumElement? ErrorAlert => _test.FindByAutomationId("lblError");
        public AppiumElement? RegisterLink => _test.FindByAutomationId("btnRegister") ?? _test.FindByText("Đăng ký ngay");
        public AppiumElement? ForgotPasswordLink => _test.FindByAutomationId("btnForgotPassword") ?? _test.FindByText("Quên mật khẩu?");

        /// <summary>
        /// Kiểm tra màn hình Đăng nhập đã hiển thị đầy đủ các phần tử thiết yếu hay chưa.
        /// </summary>
        public bool IsLoaded()
        {
            return UsernameInput != null && PasswordInput != null && LoginButton != null;
        }

        /// <summary>
        /// Nhập tên đăng nhập và mật khẩu vào form.
        /// </summary>
        public LoginPageObject EnterCredentials(string username, string password)
        {
            _test.SafeType(UsernameInput, username);
            _test.SafeType(PasswordInput, password);
            return this;
        }

        /// <summary>
        /// Nhấn nút "Đăng nhập".
        /// </summary>
        public void ClickLogin()
        {
            _test.SafeClick(LoginButton);
        }

        /// <summary>
        /// Nhấn nút ẩn/hiện mật khẩu (con mắt).
        /// </summary>
        public void TogglePassword()
        {
            _test.SafeClick(TogglePasswordButton);
        }

        /// <summary>
        /// Nhấn vào liên kết "Đăng ký ngay".
        /// </summary>
        public void ClickRegister()
        {
            _test.SafeClick(RegisterLink);
        }

        /// <summary>
        /// Lấy nội dung thông báo lỗi (nếu có).
        /// </summary>
        public string? GetErrorMessage()
        {
            return ErrorAlert?.Text;
        }
    }
}
