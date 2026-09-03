using CampusActivitiesManager.PageModels;

namespace CampusActivitiesManager.Pages
{
    public partial class EditUserRolePage : ContentPage
    {
        public EditUserRolePage(EditUserRoleViewModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
