using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages
{
    public partial class EventDetailPage : ContentPage
    {
        public EventDetailPage(EventDetailPageModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
