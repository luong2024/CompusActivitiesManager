using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using CampusActivitiesManager.Data;
using CampusActivitiesManager.PageModels;
using CampusActivitiesManager.Pages;
using CampusActivitiesManager.Services;
using CampusActivitiesManager.Models;

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

            builder.Services.AddSingleton<ProjectRepository>();
            builder.Services.AddSingleton<TaskRepository>();
            builder.Services.AddSingleton<CategoryRepository>();
            builder.Services.AddSingleton<TagRepository>();
            builder.Services.AddSingleton<UserRepository>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<IUserService<User>, UserService>();
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
            builder.Services.AddSingleton<SeedDataService>();
            builder.Services.AddSingleton<ModalErrorHandler>();
            
            // Sprint 2 Services
            builder.Services.AddSingleton<IEventService, EventService>();
            builder.Services.AddSingleton<CategoryService>();

            // PageModels
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<ProjectListPageModel>();
            builder.Services.AddSingleton<ManageMetaPageModel>();
            builder.Services.AddSingleton<UserManagementPageModel>();
            builder.Services.AddTransient<LoginPageModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<CreateUserViewModel>();
            builder.Services.AddTransient<EditUserRoleViewModel>();
            builder.Services.AddTransient<AccessDeniedViewModel>();
            builder.Services.AddTransient<ProjectDetailPageModel>();
            builder.Services.AddTransient<TaskDetailPageModel>();
            
            // Sprint 2 PageModels
            builder.Services.AddTransient<EventListPageModel>();
            builder.Services.AddTransient<EventDetailPageModel>();
            builder.Services.AddTransient<ManageEventPageModel>();
            builder.Services.AddTransient<ManageCategoryPageModel>();
            builder.Services.AddTransient<ProfilePageModel>();

            // Pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<UserManagementPage>();
            builder.Services.AddTransient<LoginPage>();
            
            // Sprint 2 Pages
            builder.Services.AddTransient<EventListPage>();
            builder.Services.AddTransient<EventDetailPage>();
            builder.Services.AddTransient<ManageEventPage>();
            builder.Services.AddTransient<ManageCategoryPage>();
            builder.Services.AddTransient<ProfilePage>();

            builder.Services.AddTransientWithShellRoute<RegisterPage, RegisterViewModel>("register");
            builder.Services.AddTransientWithShellRoute<CreateUserPage, CreateUserViewModel>("createuser");
            builder.Services.AddTransientWithShellRoute<EditUserRolePage, EditUserRoleViewModel>("editrole");
            builder.Services.AddTransientWithShellRoute<AccessDeniedPage, AccessDeniedViewModel>("accessdenied");
            builder.Services.AddTransientWithShellRoute<ProjectDetailPage, ProjectDetailPageModel>("project");
            builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");

            return builder.Build();
        }
    }
}
