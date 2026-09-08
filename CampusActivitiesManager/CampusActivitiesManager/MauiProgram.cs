using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;

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

            // -------------------------
            // Layer 1: Repositories (Data Access)
            // -------------------------
            builder.Services.AddSingleton<ProjectRepository>();
            builder.Services.AddSingleton<TaskRepository>();
            builder.Services.AddSingleton<CategoryRepository>();
            builder.Services.AddSingleton<TagRepository>();
            builder.Services.AddSingleton<AccountRepository>();   // AC 1.1 – Quản lý tài khoản

            // -------------------------
            // Layer 2: Services (Business Logic)
            // -------------------------
            builder.Services.AddSingleton<SeedDataService>();
            builder.Services.AddSingleton<ModalErrorHandler>();
            builder.Services.AddSingleton<AuthService>();          // AC 1.1 – Xác thực & phân quyền

            // -------------------------
            // Layer 3: PageModels / ViewModels (MVVM)
            // -------------------------
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<ProjectListPageModel>();
            builder.Services.AddSingleton<ManageMetaPageModel>();
            builder.Services.AddSingleton<AccountListPageModel>(); // AC 1.1 – Danh sách tài khoản
            builder.Services.AddTransient<LoginPageModel>();       // Transient – mỗi lần mở trang login là instance mới
            builder.Services.AddTransient<RegisterPageModel>();    // Transient – mỗi lần mở trang đăng ký là instance mới

            // -------------------------
            // Layer 4: Pages / Views (Shell Navigation Routes)
            // -------------------------
            // Transient pages có route Shell (navigate bằng Shell.GoToAsync)
            builder.Services.AddTransientWithShellRoute<ProjectDetailPage, ProjectDetailPageModel>("project");
            builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");

            // Singleton pages (được DataTemplate lazy-load bởi Shell flyout)
            builder.Services.AddSingleton<AccountListPage>();      // AC 1.1 – Giao diện danh sách tài khoản
            builder.Services.AddSingleton<LoginPage>();            // Trang đăng nhập
            builder.Services.AddSingleton<RegisterPage>();         // Trang đăng ký

            return builder.Build();
        }
    }
}
