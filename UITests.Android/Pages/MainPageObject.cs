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

        public AppiumElement? MenuButton => _test.FindByAutomationId("btnMenuToggle") ?? _test.FindByText("☰");
        public AppiumElement? GreetingLabel => _test.FindByAutomationId("lblStudentGreeting");
        public AppiumElement? NotificationButton => _test.FindByAutomationId("btnNotifications") ?? _test.FindByText("🔔");

        // Bottom TabBar items
        public AppiumElement? HomeTab => _test.FindByText("Trang chủ");
        public AppiumElement? EventsTab => _test.FindByText("Sự kiện");
        public AppiumElement? QrTab => _test.FindByText("Quét QR");
        public AppiumElement? ClassTab => _test.FindByText("Lớp học");
        public AppiumElement? ProfileTab => _test.FindByText("Cá nhân");

        // Flyout Menu items
        public AppiumElement? LogoutMenuItem => _test.FindByText("🚪 Đăng xuất khỏi hệ thống");
        public AppiumElement? UserManagementMenuItem => _test.FindByText("👥 Quản lý Người dùng (Admin)");

        /// <summary>
        /// Kiểm tra Trang chủ đã tải thành công.
        /// </summary>
        public bool IsLoaded()
        {
            return MenuButton != null && (GreetingLabel != null || HomeTab != null);
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
