using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages
{
    public partial class EventManagementPage : ContentPage
    {
        public EventManagementPage(EventManagementPageModel model)
        {
            BindingContext = model;
            InitializeComponent();
        }
    }
}
