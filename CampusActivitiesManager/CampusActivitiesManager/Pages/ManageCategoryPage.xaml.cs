using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages;

public partial class ManageCategoryPage : ContentPage
{
    public ManageCategoryPage(ManageCategoryPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
