using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages
{
    public partial class PhotoAttendancePage : ContentPage
    {
        private readonly PhotoAttendanceViewModel _viewModel;

        public PhotoAttendancePage(PhotoAttendanceViewModel viewModel)
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
