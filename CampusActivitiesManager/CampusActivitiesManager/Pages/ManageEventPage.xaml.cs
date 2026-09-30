using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages;

public partial class ManageEventPage : ContentPage
{
    public ManageEventPage(ManageEventPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
