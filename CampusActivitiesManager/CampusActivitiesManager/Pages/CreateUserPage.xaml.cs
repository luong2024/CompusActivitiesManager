using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages;

public partial class CreateUserPage : ContentPage
{
    public CreateUserPage(CreateUserViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
