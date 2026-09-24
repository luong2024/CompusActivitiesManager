using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfilePageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ProfilePageModel vm)
        {
            await vm.LoadProfileAsync();
        }
    }
}
