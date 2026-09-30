using CampusActivitiesManager.Models;
using CampusActivitiesManager.PageModels;
using CampusActivitiesManager.Pages;
using CampusActivitiesManager.Services;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Đăng ký Services
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
            builder.Services.AddSingleton<IUserService<User>, UserService>();
            builder.Services.AddSingleton<IErrorHandler, ModalErrorHandler>();
            
            // Sprint 2 Services
            builder.Services.AddSingleton<IEventService, EventService>();
            builder.Services.AddSingleton<CategoryService>();

            // Đăng ký Pages và PageModels (ViewModels)
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginPageModel>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainPageModel>();

            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<RegisterPageModel>();

            // Sprint 1
            builder.Services.AddTransient<UserManagementPage>();
            builder.Services.AddTransient<UserManagementPageModel>();

            builder.Services.AddTransient<CreateUserPage>();
            builder.Services.AddTransient<CreateUserViewModel>();

            builder.Services.AddTransient<EditUserRolePage>();
            builder.Services.AddTransient<EditUserRoleViewModel>();
            
            // Sprint 2
            builder.Services.AddTransient<EventListPage>();
            builder.Services.AddTransient<EventListPageModel>();
            
            builder.Services.AddTransient<EventDetailPage>();
            builder.Services.AddTransient<EventDetailPageModel>();
            
            builder.Services.AddTransient<ManageEventPage>();
            builder.Services.AddTransient<ManageEventPageModel>();
            
            builder.Services.AddTransient<ManageCategoryPage>();
            builder.Services.AddTransient<ManageCategoryPageModel>();
            
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<ProfilePageModel>();

            return builder.Build();
        }
    }
}
