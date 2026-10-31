using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages
{
    public partial class ClassHubPage : ContentPage
    {
        private readonly ClassHubPageModel _vm;

        public ClassHubPage(ClassHubPageModel viewModel)
        {
            InitializeComponent();
            BindingContext = _vm = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_vm != null)
            {
                await _vm.AppearingAsync();
            }
        }
    }
}
