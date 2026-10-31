using OpenQA.Selenium.Appium;
using UITests.Android.Core;

namespace UITests.Android.Pages
{
    /// <summary>
    /// Page Object Model cho màn hình Đăng ký tài khoản sinh viên (RegisterPage).
    /// </summary>
    public class RegisterPageObject
    {
        private readonly BaseAndroidUITest _test;

        public RegisterPageObject(BaseAndroidUITest test)
        {
            _test = test;
        }

        public AppiumElement? TitleHeader => _test.FindByText("Đăng Ký Tài Khoản Sinh Viên");
        public AppiumElement? FullNameInput => _test.FindByAutomationId("txtRegisterFullName") ?? _test.FindByClassName("android.widget.EditText", 0);
        public AppiumElement? UsernameInput => _test.FindByAutomationId("txtRegisterUsername") ?? _test.FindByClassName("android.widget.EditText", 1);
        public AppiumElement? EmailInput => _test.FindByAutomationId("txtRegisterEmail") ?? _test.FindByClassName("android.widget.EditText", 2);
        public AppiumElement? PhoneInput => _test.FindByAutomationId("txtRegisterPhone") ?? _test.FindByClassName("android.widget.EditText", 3);
        public AppiumElement? ClassPicker => _test.FindByAutomationId("pickerRegisterClass");
        public AppiumElement? PasswordInput => _test.FindByAutomationId("txtRegisterPassword") ?? _test.FindByClassName("android.widget.EditText", 4);
        public AppiumElement? ConfirmPasswordInput => _test.FindByAutomationId("txtRegisterConfirmPassword") ?? _test.FindByClassName("android.widget.EditText", 5);

        public AppiumElement? SubmitButton
        {
            get
            {
                var btn = _test.FindByAutomationId("btnRegisterSubmit", System.TimeSpan.FromSeconds(1)) 
                    ?? _test.FindByText("Đăng Ký Tài Khoản", System.TimeSpan.FromSeconds(1));
                if (btn != null) return btn;

                _test.SwipeDown();
                return _test.FindByAutomationId("btnRegisterSubmit", System.TimeSpan.FromSeconds(2)) 
                    ?? _test.FindByText("Đăng Ký Tài Khoản", System.TimeSpan.FromSeconds(2)) 
                    ?? _test.FindByClassName("android.widget.Button", 0);
            }
        }

        public AppiumElement? BackToLoginButton
        {
            get
            {
                var btn = _test.FindByAutomationId("btnBackToLogin", System.TimeSpan.FromSeconds(1)) 
                    ?? _test.FindByText("Đã có tài khoản? Quay lại đăng nhập", System.TimeSpan.FromSeconds(1));
                if (btn != null) return btn;

                _test.SwipeDown();
                return _test.FindByAutomationId("btnBackToLogin", System.TimeSpan.FromSeconds(2)) 
                    ?? _test.FindByText("Đã có tài khoản? Quay lại đăng nhập", System.TimeSpan.FromSeconds(2)) 
                    ?? _test.FindByClassName("android.widget.Button", 1);
            }
        }

        public AppiumElement? ErrorAlert => _test.FindByText("Vui lòng nhập Họ và tên sinh viên.", System.TimeSpan.FromSeconds(2))
            ?? _test.FindByText("Vui lòng nhập Họ và tên", System.TimeSpan.FromSeconds(1))
            ?? _test.FindByText("Vui lòng nhập", System.TimeSpan.FromSeconds(1))
            ?? _test.FindByAutomationId("lblRegisterError", System.TimeSpan.FromSeconds(2))
            ?? (_test.Driver?.FindElements(OpenQA.Selenium.By.XPath("//android.widget.TextView[contains(@text, 'vui lòng') or contains(@text, 'Vui lòng') or contains(@text, 'bắt buộc') or contains(@text, 'Họ và tên') or contains(@text, 'Lỗi')]")).FirstOrDefault() as AppiumElement);

        /// <summary>
        /// Kiểm tra màn hình Đăng ký đã hiển thị đầy đủ các trường nhập hay chưa.
        /// </summary>
        public bool IsLoaded()
        {
            return FullNameInput != null && UsernameInput != null && EmailInput != null;
        }

        /// <summary>
        /// Nhập đầy đủ thông tin vào form đăng ký.
        /// </summary>
        public RegisterPageObject FillForm(string fullName, string username, string email, string phone, string password, string confirmPassword)
        {
            _test.SafeType(FullNameInput, fullName);
            _test.SafeType(UsernameInput, username);
            _test.SafeType(EmailInput, email);
            _test.SafeType(PhoneInput, phone);
            _test.SafeType(PasswordInput, password);
            _test.SafeType(ConfirmPasswordInput, confirmPassword);
            return this;
        }

        /// <summary>
        /// Nhấn nút gửi "Đăng Ký Tài Khoản".
        /// </summary>
        public void ClickSubmit()
        {
            _test.SwipeDown();
            System.Threading.Thread.Sleep(300);

            var btn = SubmitButton;
            if (btn != null)
            {
                _test.SafeClick(btn);
            }
            else
            {
                _test.TapCoordinate(538, 1961);
            }

            System.Threading.Thread.Sleep(500);
            _test.SwipeUp();
            System.Threading.Thread.Sleep(400);
        }

        /// <summary>
        /// Nhấn nút quay lại màn hình Đăng nhập.
        /// </summary>
        public void ClickBackToLogin()
        {
            var btn = BackToLoginButton;
            if (btn != null)
            {
                _test.SafeClick(btn);
            }
            else
            {
                _test.Driver?.Navigate().Back();
            }
            System.Threading.Thread.Sleep(1000);
        }

        /// <summary>
        /// Lấy nội dung lỗi validation (nếu có).
        /// </summary>
        public string? GetErrorMessage()
        {
            return ErrorAlert?.Text;
        }
    }
}
