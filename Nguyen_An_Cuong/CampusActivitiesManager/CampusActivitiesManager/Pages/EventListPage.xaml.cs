using CampusActivitiesManager.PageModels;
using Microsoft.Maui.Controls;

namespace CampusActivitiesManager.Pages
{
    public partial class EventListPage : ContentPage
    {
        public EventListPage(EventListPageModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
