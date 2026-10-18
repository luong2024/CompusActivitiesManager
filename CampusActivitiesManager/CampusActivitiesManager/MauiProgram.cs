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

#if WINDOWS && DEBUG
            EnsureLocalBackendRunning();
#endif

            return builder.Build();
        }

#if WINDOWS && DEBUG
        private static void EnsureLocalBackendRunning()
        {
            try
            {
                using var tcpClient = new System.Net.Sockets.TcpClient();
                var connectTask = tcpClient.ConnectAsync("127.0.0.1", 7258);
                if (connectTask.Wait(TimeSpan.FromMilliseconds(150)) && tcpClient.Connected)
                {
                    return;
                }
            }
            catch
            {
                // Port 7258 is not listening yet; launch background API server
            }

            try
            {
                var dir = new DirectoryInfo(AppContext.BaseDirectory);
                while (dir != null)
                {
                    var candidateApiDir = Path.Combine(dir.FullName, "CampusActivitiesManager.Api");
                    var candidateCsproj = Path.Combine(candidateApiDir, "CampusActivitiesManager.Api.csproj");
                    if (File.Exists(candidateCsproj))
                    {
                        var apiExe = Path.Combine(candidateApiDir, "bin", "Debug", "net9.0", "CampusActivitiesManager.Api.exe");
                        var startInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            WorkingDirectory = candidateApiDir,
                            CreateNoWindow = true,
                            UseShellExecute = false
                        };

                        if (File.Exists(apiExe))
                        {
                            startInfo.FileName = apiExe;
                            startInfo.Arguments = "--urls \"https://localhost:7258;http://localhost:5073\"";
                        }
                        else
                        {
                            startInfo.FileName = "dotnet";
                            startInfo.Arguments = $"run --project \"{candidateCsproj}\" --no-launch-profile --urls \"https://localhost:7258;http://localhost:5073\"";
                        }

                        var launchSettingsPath = Path.Combine(candidateApiDir, "Properties", "launchSettings.json");
                        if (File.Exists(launchSettingsPath))
                        {
                            using var doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(launchSettingsPath));
                            if (doc.RootElement.TryGetProperty("profiles", out var profiles))
                            {
                                foreach (var profile in profiles.EnumerateObject())
                                {
                                    if (profile.Value.TryGetProperty("environmentVariables", out var envVars))
                                    {
                                        foreach (var envVar in envVars.EnumerateObject())
                                        {
                                            if (envVar.Value.ValueKind == System.Text.Json.JsonValueKind.String)
                                            {
                                                startInfo.EnvironmentVariables[envVar.Name] = envVar.Value.GetString();
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        }

                        System.Diagnostics.Process.Start(startInfo);
                        break;
                    }
                    dir = dir.Parent;
                }
            }
            catch
            {
                // Ignore if background launcher cannot start
            }
        }
#endif
    }
}
