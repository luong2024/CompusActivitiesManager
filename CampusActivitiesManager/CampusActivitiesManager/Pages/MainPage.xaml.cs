using CampusActivitiesManager.Models;
using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}