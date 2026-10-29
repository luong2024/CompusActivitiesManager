namespace UITests.Android.Configuration
{
    /// <summary>
    /// Cấu hình trung tâm cho Appium Driver trên nền tảng Android.
    /// Hỗ trợ đọc các biến môi trường APPIUM_HOST, APPIUM_PORT hoặc fallback mặc định.
    /// </summary>
    public static class AppiumConfig
    {
        public static string ServerHost => Environment.GetEnvironmentVariable("APPIUM_HOST") ?? "127.0.0.1";
        public static int ServerPort => int.TryParse(Environment.GetEnvironmentVariable("APPIUM_PORT"), out var port) ? port : 4723;
        public static Uri ServerUri => new($"http://{ServerHost}:{ServerPort}/");

        public const string PlatformName = "Android";
        public const string AutomationName = "UIAutomator2";
        public const string DeviceName = "Android Emulator";
        public const string AppPackage = "com.companyname.campusactivitiesmanager";
        public const string AppActivity = "crc64e1fb321c08285b90.MainActivity";

        public static readonly TimeSpan CommandTimeout = TimeSpan.FromSeconds(120);
        public static readonly TimeSpan ImplicitWait = TimeSpan.FromSeconds(10);
        public static readonly TimeSpan ExplicitWait = TimeSpan.FromSeconds(15);
    }
}
