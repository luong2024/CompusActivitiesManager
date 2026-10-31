using System.Net.Sockets;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using UITests.Android.Configuration;

namespace UITests.Android.Core
{
    /// <summary>
    /// Factory tạo và quản lý AndroidDriver cho Appium.
    /// Có cơ chế tự động kiểm tra xem cổng Appium Server có đang mở không.
    /// </summary>
    public static class AppiumDriverFactory
    {
        /// <summary>
        /// Kiểm tra nhanh xem Appium Server có đang chạy và lắng nghe trên IP/Cổng cấu hình hay không.
        /// </summary>
        public static bool IsServerAvailable(int timeoutMilliseconds = 800)
        {
            try
            {
                using var tcpClient = new TcpClient();
                var connectTask = tcpClient.ConnectAsync(AppiumConfig.ServerHost, AppiumConfig.ServerPort);
                var completed = Task.WhenAny(connectTask, Task.Delay(timeoutMilliseconds)).GetAwaiter().GetResult();
                return completed == connectTask && tcpClient.Connected;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Khởi tạo AndroidDriver với đầy đủ cấu hình UIAutomator2 cho ứng dụng Campus Activities Manager.
        /// </summary>
        public static AndroidDriver CreateDriver()
        {
            var options = new AppiumOptions
            {
                PlatformName = AppiumConfig.PlatformName,
                AutomationName = AppiumConfig.AutomationName,
                DeviceName = AppiumConfig.DeviceName
            };

            // Thiết lập App Package & App Activity
            options.AddAdditionalAppiumOption(AndroidMobileCapabilityType.AppPackage, AppiumConfig.AppPackage);
            options.AddAdditionalAppiumOption(AndroidMobileCapabilityType.AppActivity, AppiumConfig.AppActivity);
            bool noReset = bool.TryParse(Environment.GetEnvironmentVariable("APPIUM_NO_RESET"), out var nr) && nr;
            options.AddAdditionalAppiumOption(MobileCapabilityType.NoReset, noReset);
            options.AddAdditionalAppiumOption(MobileCapabilityType.NewCommandTimeout, 120);
            options.AddAdditionalAppiumOption(AndroidMobileCapabilityType.AutoGrantPermissions, true);

            var driver = new AndroidDriver(AppiumConfig.ServerUri, options, AppiumConfig.CommandTimeout);
            driver.Manage().Timeouts().ImplicitWait = AppiumConfig.ImplicitWait;
            return driver;
        }
    }
}
