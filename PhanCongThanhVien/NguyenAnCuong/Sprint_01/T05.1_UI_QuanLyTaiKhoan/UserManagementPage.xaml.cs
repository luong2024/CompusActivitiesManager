using CampusActivitiesManager.Models;
using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages
{
    public partial class UserManagementPage : ContentPage
    {
        public UserManagementPage(UserManagementPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }

        private void OnRoleRadioChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value && sender is RadioButton radioButton && BindingContext is UserManagementPageModel vm)
            {
                if (radioButton.Value is string roleStr && Enum.TryParse<Role>(roleStr, true, out var role))
                {
                    vm.SelectedNewRole = role;
                }
            }
        }
    }
}
