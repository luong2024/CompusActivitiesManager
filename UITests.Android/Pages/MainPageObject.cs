using OpenQA.Selenium.Appium;
using UITests.Android.Core;

namespace UITests.Android.Pages
{
    /// <summary>
    /// Page Object Model cho Trang chủ (MainPage), Bottom TabBar và Flyout Menu.
    /// </summary>
    public class MainPageObject
    {
        private readonly BaseAndroidUITest _test;

        public MainPageObject(BaseAndroidUITest test)
        {
            _test = test;
        }

        public AppiumElement? MenuButton => _test.FindByAutomationId("btnMenuToggle") ?? _test.FindByText("☰") ?? (_test.Driver?.FindElements(OpenQA.Selenium.By.XPath("//*[@text='☰' or contains(@text, '☰')]")).FirstOrDefault() as AppiumElement);
        public AppiumElement? GreetingLabel => _test.FindByAutomationId("lblStudentGreeting") ?? (_test.Driver?.FindElements(OpenQA.Selenium.By.XPath("//android.widget.TextView[contains(@text, 'Xin chào') or contains(@text, 'chào') or contains(@text, 'Chào')]")).FirstOrDefault() as AppiumElement);
        public AppiumElement? NotificationButton => _test.FindByAutomationId("btnNotifications") ?? _test.FindByText("🔔");

        // Bottom TabBar items
        public AppiumElement? HomeTab => _test.FindByText("Trang chủ");
        public AppiumElement? EventsTab => _test.FindByText("Sự kiện");
        public AppiumElement? QrTab => _test.FindByText("Quét QR");
        public AppiumElement? ClassTab => _test.FindByText("Lớp học");
        public AppiumElement? ProfileTab => _test.FindByText("Cá nhân");

        // Flyout Menu items
        public AppiumElement? LogoutMenuItem => _test.FindByText("🚪 Đăng xuất khỏi hệ thống") ?? _test.FindByText("Đăng xuất khỏi hệ thống") ?? (_test.Driver?.FindElements(OpenQA.Selenium.By.XPath("//*[contains(@text, 'Đăng xuất')]")).FirstOrDefault() as AppiumElement);
        public AppiumElement? UserManagementMenuItem => _test.FindByText("👥 Quản lý Người dùng (Admin)") ?? _test.FindByText("Quản lý Người dùng (Admin)") ?? (_test.Driver?.FindElements(OpenQA.Selenium.By.XPath("//*[contains(@text, 'Quản lý Người dùng')]")).FirstOrDefault() as AppiumElement);

        /// <summary>
        /// Kiểm tra Trang chủ đã tải thành công.
        /// </summary>
        public bool IsLoaded()
        {
            return HomeTab != null || MenuButton != null || GreetingLabel != null;
        }

        /// <summary>
        /// Lấy nội dung câu chào sinh viên / người dùng trên trang chủ.
        /// </summary>
        public string? GetGreetingText()
        {
            return GreetingLabel?.Text;
        }

        /// <summary>
        /// Mở Flyout menu bằng nút Hamburger.
        /// </summary>
        public void OpenMenu()
        {
            _test.SafeClick(MenuButton);
        }

        /// <summary>
        /// Chuyển đổi tab trên thanh điều hướng dưới cùng (Bottom TabBar).
        /// </summary>
        public void SwitchTab(string tabName)
        {
            switch (tabName.ToLowerInvariant())
            {
                case "home":
                case "trang chu":
                case "trang chủ":
                    _test.SafeClick(HomeTab);
                    break;
                case "events":
                case "su kien":
                case "sự kiện":
                    _test.SafeClick(EventsTab);
                    break;
                case "qr":
                case "quet qr":
                case "quét qr":
                    _test.SafeClick(QrTab);
                    break;
                case "class":
                case "lop hoc":
                case "lớp học":
                    _test.SafeClick(ClassTab);
                    break;
                case "profile":
                case "ca nhan":
                case "cá nhân":
                    _test.SafeClick(ProfileTab);
                    break;
            }
        }
    }
}
