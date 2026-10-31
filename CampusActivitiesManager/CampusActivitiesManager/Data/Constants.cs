namespace CampusActivitiesManager.Data
{
    public static class Constants
    {
        public const string DatabaseFilename = "AppSQLite.db3";
        public const string AuthTokenKey = "campus_auth_token";

        public static string DatabasePath =>
            $"Data Source={Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename)}";

        // Firebase Cloud Firestore Configuration
        public const string DefaultFirebaseDbUrl = FirebaseConfig.FirestoreBaseUrl;
        public const string FirebaseDbUrlPrefKey = "firebase_database_url";
    }
}
