using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages
{
    public partial class EventFormPage : ContentPage
    {
        public EventFormPage(EventFormPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
