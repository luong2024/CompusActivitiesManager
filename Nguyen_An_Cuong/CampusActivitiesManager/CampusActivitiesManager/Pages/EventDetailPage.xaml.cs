using CampusActivitiesManager.PageModels;
using Microsoft.Maui.Controls;

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
