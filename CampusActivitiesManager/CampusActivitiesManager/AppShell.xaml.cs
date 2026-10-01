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

            var currentTheme = Application.Current!.RequestedTheme;
            ThemeSegmentedControl.SelectedIndex = currentTheme == AppTheme.Light ? 0 : 1;
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
                }
            }
        }

        private void UpdateMenuVisibility()
        {
            var authService = Handler?.MauiContext?.Services.GetService<IAuthenticationService>();
            if (authService == null) return;

            bool isLoggedIn = authService.CurrentUser != null;
            bool isAdmin = authService.IsAdmin;

            // Show Login if not logged in
            NavLogin.IsVisible = !isLoggedIn;

            // Show profile and events if logged in
            
            

            // Show User management only for admin
            NavUsers.IsVisible = isAdmin;

            // Show Student dashboard only for non-admin logged in users
            NavStudentDashboard.IsVisible = isLoggedIn && !isAdmin;
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(UserManagementPage), typeof(UserManagementPage));
            Routing.RegisterRoute(nameof(EditUserRolePage), typeof(EditUserRolePage));
            Routing.RegisterRoute(nameof(CreateUserPage), typeof(CreateUserPage));
            Routing.RegisterRoute(nameof(AccessDeniedPage), typeof(AccessDeniedPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        }

        /// <summary>
        /// Navigation Guard: Kiểm tra thẩm quyền trước khi chuyển hướng (Task.md mục 5).
        /// Nếu không đủ quyền, tự động chuyển hướng về //AccessDeniedPage.
        /// </summary>
        public static async Task NavigateWithGuardAsync(string route, Role requiredRole, IAuthenticationService authService)
        {
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
            // Toast is currently not working in MCT on Windows, fallback to Snackbar
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
            Application.Current!.UserAppTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;
        }
    }
}
