using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

namespace UITests.AndroidUI
{
    /// <summary>
    /// Cấu hình Appium Driver cho kiểm thử giao diện Android thực tế trên Emulator.
    /// Đặt trong namespace riêng (UITests.AndroidUI) để các Unit Test trong UITests
    /// có thể chạy độc lập 100% mà không bắt buộc phải bật máy ảo Android trước.
    /// </summary>
    [SetUpFixture]
    public class AppiumSetup
    {
        private static AppiumDriver? _driver;

        public static AppiumDriver App => _driver ?? throw new NullReferenceException("AppiumDriver has not been initialized.");

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var androidOptions = new AppiumOptions
            {
                AutomationName = "UIAutomator2",
                PlatformName = "Android",
                DeviceName = "Android Emulator"
            };

            androidOptions.AddAdditionalAppiumOption(AndroidMobileCapabilityType.AppPackage, "com.companyname.campusactivitiesmanager");
            androidOptions.AddAdditionalAppiumOption(AndroidMobileCapabilityType.AppActivity, "crc64e1fb321c08285b90.MainActivity");
            androidOptions.AddAdditionalAppiumOption(MobileCapabilityType.NoReset, true);

            _driver = new AndroidDriver(new Uri("http://127.0.0.1:4723/"), androidOptions);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }
    }
}
