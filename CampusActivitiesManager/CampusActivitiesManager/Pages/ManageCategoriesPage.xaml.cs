using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages;

public partial class ManageCategoriesPage : ContentPage
{
    public ManageCategoriesPage(ManageCategoriesPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
