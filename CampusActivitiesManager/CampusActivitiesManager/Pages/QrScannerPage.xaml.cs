using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages
{
    public partial class QrScannerPage : ContentPage
    {
        private readonly QrScannerViewModel _viewModel;

        public QrScannerPage(QrScannerViewModel viewModel)
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
