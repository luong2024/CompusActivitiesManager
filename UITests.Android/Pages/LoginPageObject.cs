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
        public AppiumElement? UsernameInput => _test.FindByAutomationId("txtUsername") ?? _test.FindByClassName("android.widget.EditText", 0);
        public AppiumElement? PasswordInput => _test.FindByAutomationId("txtPassword") ?? _test.FindByClassName("android.widget.EditText", 1);
        public AppiumElement? TogglePasswordButton => _test.FindByAutomationId("btnTogglePassword") ?? _test.FindByClassName("android.widget.ImageView", 0);
        public AppiumElement? RememberMeCheckbox => _test.FindByAutomationId("chkRememberMe") ?? _test.FindByClassName("android.widget.CheckBox", 0);
        public AppiumElement? LoginButton => _test.FindByAutomationId("btnLogin") ?? _test.FindByClassName("android.widget.Button", 0);
        public AppiumElement? ErrorAlert => _test.FindByAutomationId("lblError") ?? (_test.Driver?.FindElements(OpenQA.Selenium.By.XPath("//android.widget.TextView[contains(@text, 'chính xác') or contains(@text, 'không đúng') or contains(@text, 'bị khóa') or contains(@text, 'Vui lòng nhập Tên') or contains(@text, 'Vui lòng nhập Mật') or contains(@text, 'Lỗi')]")).FirstOrDefault() as AppiumElement);
        public AppiumElement? RegisterLink => _test.FindByAutomationId("btnRegister") ?? _test.FindByClassName("android.widget.Button", 1);
        public AppiumElement? ForgotPasswordLink => _test.FindByAutomationId("btnForgotPassword") ?? _test.FindByClassName("android.widget.Button", 2);

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
