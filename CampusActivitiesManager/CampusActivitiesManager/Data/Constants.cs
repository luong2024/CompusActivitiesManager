namespace CampusActivitiesManager.Data
{
    public static class Constants
    {
        public const string DatabaseFilename = "AppSQLite.db3";
        public const string AuthTokenKey = "api_auth_token";

        public static string DatabasePath =>
            $"Data Source={Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename)}";

        public static string ApiBaseUrl =>
            DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5073/api/v1"
                : "https://localhost:7258/api/v1";

        public static string AuthLoginUrl => $"{ApiBaseUrl}/auth/login";
        public static string AccountsApiUrl => $"{ApiBaseUrl}/accounts";
        public static string ProfileApiUrl => $"{ApiBaseUrl}/profile";
        public static string EventsApiUrl => $"{ApiBaseUrl}/events";
        public static string CategoriesApiUrl => $"{ApiBaseUrl}/categories";
        public static string AttendanceApiUrl => $"{ApiBaseUrl}/attendance";
        public static string NotificationsApiUrl => $"{ApiBaseUrl}/notifications";
    }
}