using CampusActivitiesManager.BusinessLogic.Services;

namespace CampusActivitiesManager
{
    public partial class App : Application
    {
        private readonly IServiceProvider? _services;
        private static IDispatcherTimer? _autoSyncTimer;

        public App(IServiceProvider? services = null)
        {
            InitializeComponent();
            _services = services;
            StartPeriodicAutoSync();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        private void StartPeriodicAutoSync()
        {
            // Tự động đồng bộ toàn diện ngay khi khởi động
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                await TriggerGlobalSyncAsync();
            });

            // Tự động đồng bộ định kỳ mỗi 30 giây chạy ngầm
            try
            {
                _autoSyncTimer = Dispatcher.CreateTimer();
                _autoSyncTimer.Interval = TimeSpan.FromSeconds(30);
                _autoSyncTimer.Tick += async (s, e) =>
                {
                    await TriggerGlobalSyncAsync();
                };
                _autoSyncTimer.Start();
            }
            catch { }
        }

        public async Task TriggerGlobalSyncAsync()
        {
            try
            {
                var services = _services ?? Handler?.MauiContext?.Services;
                if (services == null) return;

                var eventService = services.GetService<IEventService>();
                var userService = services.GetService<IUserService>();
                var attendanceService = services.GetService<IAttendanceService>();

                if (eventService != null) await eventService.GetEventsAsync();
                if (userService != null)
                {
                    await userService.GetUsersListAsync();
                    await userService.GetClassesAsync();
                }
                if (attendanceService != null)
                {
                    await attendanceService.GetAllAttendanceRecordsAsync();
                    await attendanceService.GetNotificationsAsync();
                }
            }
            catch { }
        }
    }
}
