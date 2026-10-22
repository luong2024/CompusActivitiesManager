using CampusActivitiesManager.Models;
using CampusActivitiesManager.Pages;
using CampusActivitiesManager.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;

namespace CampusActivitiesManager
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();

            var currentTheme = Application.Current?.RequestedTheme ?? AppTheme.Light;
            if (ThemeSegmentedControl != null)
            {
                ThemeSegmentedControl.SelectedIndex = currentTheme == AppTheme.Dark ? 1 : 0;
            }
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
                UpdateMenuVisibility();

                var authService = Handler.MauiContext?.Services.GetService<IAuthenticationService>();
                if (authService != null)
                {
                    authService.CurrentUserChanged += (s, e) =>
                    {
                        MainThread.BeginInvokeOnMainThread(() => UpdateMenuVisibility());
                    };

                    // Khi ứng dụng mở lên, nếu chưa đăng nhập thì bắt buộc chuyển về trang đăng nhập
                    if (authService.CurrentUser == null)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await GoToAsync("//login");
                        });
                    }
                }
            }
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            var authService = Handler?.MauiContext?.Services.GetService<IAuthenticationService>();
            if (authService != null && authService.CurrentUser == null)
            {
                var target = args.Target.Location.OriginalString;
                // Cho phép di chuyển đến trang login và register khi chưa xác thực
                if (!target.Contains("login", StringComparison.OrdinalIgnoreCase) && 
                    !target.Contains("register", StringComparison.OrdinalIgnoreCase))
                {
                    args.Cancel();
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await GoToAsync("//login");
                    });
                }
            }
        }

        private void UpdateMenuVisibility()
        {
            var authService = Handler?.MauiContext?.Services.GetService<IAuthenticationService>();
            if (authService == null) return;

            bool isLoggedIn = authService.CurrentUser != null;
            bool isAdmin = authService.IsAdmin;
            bool isManagerOrAdmin = isAdmin || (authService.CurrentUser != null && (authService.CurrentUser.Role == Role.Manager || authService.CurrentUser.Role == Role.Admin));

            if (FlyoutUsers != null)
                FlyoutUsers.IsVisible = isAdmin;

            if (FlyoutReports != null)
                FlyoutReports.IsVisible = isManagerOrAdmin;

            var user = authService.CurrentUser;
            if (user != null)
            {
                if (FlyoutUserName != null) FlyoutUserName.Text = user.FullName;
                if (FlyoutUserRole != null) FlyoutUserRole.Text = $"{user.RoleName} • {user.StudentCode}";
                if (FlyoutAvatarText != null) FlyoutAvatarText.Text = user.Initials;
            }
            else
            {
                if (FlyoutUserName != null) FlyoutUserName.Text = "Campus Hub";
                if (FlyoutUserRole != null) FlyoutUserRole.Text = "Quản lý Hoạt động Sinh viên";
                if (FlyoutAvatarText != null) FlyoutAvatarText.Text = "🎓";
            }
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(UserManagementPage), typeof(UserManagementPage));
            Routing.RegisterRoute(nameof(EventManagementPage), typeof(EventManagementPage));
            Routing.RegisterRoute(nameof(EditUserRolePage), typeof(EditUserRolePage));
            Routing.RegisterRoute(nameof(CreateUserPage), typeof(CreateUserPage));
            Routing.RegisterRoute(nameof(AccessDeniedPage), typeof(AccessDeniedPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(ProjectDetailPage), typeof(ProjectDetailPage));
            Routing.RegisterRoute(nameof(TaskDetailPage), typeof(TaskDetailPage));
            Routing.RegisterRoute(nameof(ManageMetaPage), typeof(ManageMetaPage));
            Routing.RegisterRoute(nameof(QrScannerPage), typeof(QrScannerPage));
            Routing.RegisterRoute(nameof(PhotoAttendancePage), typeof(PhotoAttendancePage));
            Routing.RegisterRoute(nameof(AttendanceHistoryPage), typeof(AttendanceHistoryPage));
            Routing.RegisterRoute(nameof(EventAttendanceReportPage), typeof(EventAttendanceReportPage));
            Routing.RegisterRoute(nameof(EventFormPage), typeof(EventFormPage));
            Routing.RegisterRoute(nameof(EventDetailPage), typeof(EventDetailPage));
            Routing.RegisterRoute(nameof(ClassHubPage), typeof(ClassHubPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

            // URL alias routes for GoToAsync calls
            Routing.RegisterRoute("createuser", typeof(CreateUserPage));
            Routing.RegisterRoute("editrole", typeof(EditUserRolePage));
            Routing.RegisterRoute("accessdenied", typeof(AccessDeniedPage));
            Routing.RegisterRoute("project", typeof(ProjectDetailPage));
            Routing.RegisterRoute("task", typeof(TaskDetailPage));
            Routing.RegisterRoute("meta", typeof(ManageMetaPage));
            Routing.RegisterRoute("profile", typeof(ProfilePage));
            Routing.RegisterRoute("qrscan", typeof(QrScannerPage));
            Routing.RegisterRoute("photoattendance", typeof(PhotoAttendancePage));
            Routing.RegisterRoute("attendancehistory", typeof(AttendanceHistoryPage));
            Routing.RegisterRoute("attendancereport", typeof(EventAttendanceReportPage));
            Routing.RegisterRoute("eventform", typeof(EventFormPage));
            Routing.RegisterRoute("eventdetail", typeof(EventDetailPage));
            Routing.RegisterRoute("classhub", typeof(ClassHubPage));
            Routing.RegisterRoute("register", typeof(RegisterPage));
        }

        private async void OnNavigateHomeClicked(object sender, EventArgs e)
        {
            FlyoutIsPresented = false;
            await GoToAsync("//homeTab/main");
        }

        private async void OnNavigateEventsClicked(object sender, EventArgs e)
        {
            FlyoutIsPresented = false;
            await GoToAsync("//eventsTab/events");
        }

        private async void OnNavigateQrClicked(object sender, EventArgs e)
        {
            FlyoutIsPresented = false;
            await GoToAsync("//qrTab/qrscan");
        }

        private async void OnNavigateClassClicked(object sender, EventArgs e)
        {
            FlyoutIsPresented = false;
            await GoToAsync("//classTab/classhub");
        }

        private async void OnNavigateProfileClicked(object sender, EventArgs e)
        {
            FlyoutIsPresented = false;
            await GoToAsync("//profileTab/profile");
        }

        private async void OnLogoutMenuItemClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Đăng xuất", "Bạn có chắc chắn muốn đăng xuất khỏi tài khoản?", "Đăng xuất", "Hủy");
            if (!confirm) return;

            var authService = Handler?.MauiContext?.Services.GetService<IAuthenticationService>();
            authService?.Logout();
            await GoToAsync("//login");
        }

        public static async Task NavigateWithGuardAsync(string route, Role requiredRole, IAuthenticationService authService)
        {
            if (Shell.Current == null) return;

            if (!authService.CheckPermission(requiredRole))
            {
                await Shell.Current.GoToAsync(nameof(AccessDeniedPage));
                return;
            }

            await Shell.Current.GoToAsync(route);
        }

        public static async Task DisplaySnackbarAsync(string message)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Color.FromArgb("#FF3300"),
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Yellow,
                CornerRadius = new CornerRadius(0),
                Font = Font.SystemFontOfSize(18),
                ActionButtonFont = Font.SystemFontOfSize(14)
            };

            var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);
            await snackbar.Show(cancellationTokenSource.Token);
        }

        public static async Task DisplayToastAsync(string message)
        {
            if (OperatingSystem.IsWindows())
            {
                await DisplaySnackbarAsync(message);
                return;
            }

            var toast = Toast.Make(message, textSize: 18);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await toast.Show(cts.Token);
        }

        private void SfSegmentedControl_SelectionChanged(object sender, Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
        {
            if (Application.Current != null && e.NewIndex >= 0)
            {
                Application.Current.UserAppTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;
            }
        }
    }
}
