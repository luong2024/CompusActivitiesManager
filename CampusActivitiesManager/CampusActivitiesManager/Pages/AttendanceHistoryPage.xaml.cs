using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages
{
    public partial class AttendanceHistoryPage : ContentPage
    {
        private readonly AttendanceHistoryViewModel _viewModel;

        public AttendanceHistoryPage(AttendanceHistoryViewModel viewModel)
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
