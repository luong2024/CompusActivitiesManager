namespace CampusActivitiesManager.Pages
{
    public partial class UserManagementPage : ContentPage
    {
        public UserManagementPage(UserManagementPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
