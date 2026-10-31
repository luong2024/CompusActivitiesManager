using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace CampusActivitiesManager
{
    [Activity(Theme = "@style/Maui.SplashTheme", 
              MainLauncher = true, 
              LaunchMode = LaunchMode.SingleTask, 
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Ngăn chặn Android Launcher tạo mới instance Activity khi ứng dụng đang chạy nền
            if (!IsTaskRoot && Intent != null && Intent.HasCategory(Intent.CategoryLauncher) && Intent.ActionMain.Equals(Intent.Action))
            {
                Finish();
                return;
            }
        }
    }
}
