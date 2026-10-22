using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using CampusActivitiesManager.Data;
using CampusActivitiesManager.PageModels;
using CampusActivitiesManager.Pages;
using CampusActivitiesManager.Services;

namespace CampusActivitiesManager
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
#if IOS || MACCATALYST
    				handlers.AddHandler<Microsoft.Maui.Controls.CollectionView, Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2>();
#endif
#if WINDOWS
                    Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
                    {
                        handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                        handler.PlatformView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                        handler.PlatformView.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
                        handler.PlatformView.Resources["TextControlBorderThemeThickness"] = new Microsoft.UI.Xaml.Thickness(0);
                    });
                    Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
                    {
                        handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                        handler.PlatformView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                        handler.PlatformView.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
                        handler.PlatformView.Resources["TextControlBorderThemeThickness"] = new Microsoft.UI.Xaml.Thickness(0);
                    });

                    Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
                    {
                        handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                        handler.PlatformView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                    });
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
    		builder.Logging.AddDebug();
    		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            builder.Services.AddSingleton<ProjectRepository>();
            builder.Services.AddSingleton<TaskRepository>();
            builder.Services.AddSingleton<CategoryRepository>();
            builder.Services.AddSingleton<TagRepository>();
            builder.Services.AddSingleton<UserRepository>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<IUserService<User>, UserService>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<IEventService, EventService>();
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
            builder.Services.AddSingleton<IQrCodeService, QrCodeService>();
            builder.Services.AddSingleton<IAttendanceService, AttendanceService>();
            builder.Services.AddSingleton<SeedDataService>();
            builder.Services.AddSingleton<ModalErrorHandler>();

            // PageModels
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<ProjectListPageModel>();
            builder.Services.AddSingleton<ManageMetaPageModel>();
            builder.Services.AddSingleton<UserManagementPageModel>();
            builder.Services.AddTransient<EventManagementPageModel>();
            builder.Services.AddTransient<LoginPageModel>();
            builder.Services.AddTransient<ProfilePageModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<CreateUserViewModel>();
            builder.Services.AddTransient<EditUserRoleViewModel>();
            builder.Services.AddTransient<AccessDeniedViewModel>();
            builder.Services.AddTransient<ProjectDetailPageModel>();
            builder.Services.AddTransient<TaskDetailPageModel>();
            builder.Services.AddTransient<QrScannerViewModel>();
            builder.Services.AddTransient<PhotoAttendanceViewModel>();
            builder.Services.AddTransient<AttendanceHistoryViewModel>();
            builder.Services.AddTransient<EventAttendanceReportViewModel>();
            builder.Services.AddTransient<EventFormPageModel>();
            builder.Services.AddTransient<EventListPageModel>();
            builder.Services.AddTransient<EventDetailPageModel>();

            // Pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<UserManagementPage>();
            builder.Services.AddTransient<EventManagementPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ManageMetaPage>();
            builder.Services.AddTransient<QrScannerPage>();
            builder.Services.AddTransient<PhotoAttendancePage>();
            builder.Services.AddTransient<AttendanceHistoryPage>();
            builder.Services.AddTransient<EventAttendanceReportPage>();
            builder.Services.AddTransient<EventFormPage>();

            builder.Services.AddTransientWithShellRoute<RegisterPage, RegisterViewModel>("register");
            builder.Services.AddTransientWithShellRoute<CreateUserPage, CreateUserViewModel>("createuser");
            builder.Services.AddTransientWithShellRoute<EditUserRolePage, EditUserRoleViewModel>("editrole");
            builder.Services.AddTransientWithShellRoute<AccessDeniedPage, AccessDeniedViewModel>("accessdenied");
            builder.Services.AddTransientWithShellRoute<ProjectDetailPage, ProjectDetailPageModel>("project");
            builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");
            builder.Services.AddTransientWithShellRoute<ManageMetaPage, ManageMetaPageModel>("meta");
            builder.Services.AddTransientWithShellRoute<QrScannerPage, QrScannerViewModel>("qrscan");
            builder.Services.AddTransientWithShellRoute<PhotoAttendancePage, PhotoAttendanceViewModel>("photoattendance");
            builder.Services.AddTransientWithShellRoute<AttendanceHistoryPage, AttendanceHistoryViewModel>("attendancehistory");
            builder.Services.AddTransientWithShellRoute<EventAttendanceReportPage, EventAttendanceReportViewModel>("attendancereport");
            builder.Services.AddTransientWithShellRoute<EventListPage, EventListPageModel>("eventlist");
            builder.Services.AddTransientWithShellRoute<EventDetailPage, EventDetailPageModel>("eventdetails");
            return builder.Build();
        }
    }
}
