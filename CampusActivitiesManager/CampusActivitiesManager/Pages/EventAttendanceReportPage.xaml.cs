using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages
{
    public partial class EventAttendanceReportPage : ContentPage
    {
        private readonly EventAttendanceReportViewModel _viewModel;

        public EventAttendanceReportPage(EventAttendanceReportViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.AppearingAsync();
        }
    }
}
