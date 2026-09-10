using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages
{
    public partial class AccessDeniedPage : ContentPage
    {
        public AccessDeniedPage(AccessDeniedViewModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
