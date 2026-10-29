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
        public AppiumElement? FullNameInput => _test.FindByAutomationId("txtRegisterFullName");
        public AppiumElement? UsernameInput => _test.FindByAutomationId("txtRegisterUsername");
        public AppiumElement? EmailInput => _test.FindByAutomationId("txtRegisterEmail");
        public AppiumElement? PhoneInput => _test.FindByAutomationId("txtRegisterPhone");
        public AppiumElement? ClassPicker => _test.FindByAutomationId("pickerRegisterClass");
        public AppiumElement? PasswordInput => _test.FindByAutomationId("txtRegisterPassword");
        public AppiumElement? ConfirmPasswordInput => _test.FindByAutomationId("txtRegisterConfirmPassword");
        public AppiumElement? SubmitButton => _test.FindByAutomationId("btnRegisterSubmit") ?? _test.FindByText("Đăng Ký Tài Khoản");
        public AppiumElement? BackToLoginButton => _test.FindByAutomationId("btnBackToLogin") ?? _test.FindByText("Đã có tài khoản? Quay lại đăng nhập");
        public AppiumElement? ErrorAlert => _test.FindByAutomationId("lblRegisterError");

        /// <summary>
        /// Kiểm tra màn hình Đăng ký đã hiển thị đầy đủ các trường nhập hay chưa.
        /// </summary>
        public bool IsLoaded()
        {
            return FullNameInput != null && UsernameInput != null && EmailInput != null && SubmitButton != null;
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
            _test.SafeClick(SubmitButton);
        }

        /// <summary>
        /// Nhấn nút quay lại màn hình Đăng nhập.
        /// </summary>
        public void ClickBackToLogin()
        {
            _test.SafeClick(BackToLoginButton);
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
