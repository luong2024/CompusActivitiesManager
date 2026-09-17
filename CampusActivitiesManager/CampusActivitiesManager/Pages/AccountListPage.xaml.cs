using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages
{
    public partial class AccountListPage : ContentPage
    {
        private readonly AccountListPageModel _viewModel;

        public AccountListPage(AccountListPageModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // AC 1.4: Dynamic Responsive Layout (1 column Mobile, 2-3 columns Tablet/Desktop)
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > 0 && _viewModel != null)
            {
                _viewModel.UpdateResponsiveLayout(width);
            }
        }
    }
}
