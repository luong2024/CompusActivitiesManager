using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class CreateUserViewModel : BaseViewModel
    {
        private readonly IUserService<User> _userService;
        private readonly ModalErrorHandler _errorHandler;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        [ObservableProperty]
        private Role _selectedRole = Role.User;

        [ObservableProperty]
        private List<Role> _availableRoles = [Role.Admin, Role.Manager, Role.User, Role.Guest];

        public CreateUserViewModel(IUserService<User> userService, ModalErrorHandler errorHandler)
        {
            _userService = userService;
            _errorHandler = errorHandler;
            Title = "Thêm tài kho?n";
        }

        [RelayCommand]
        public async Task Save()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(FullName))
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("L?i", "Vui lòng nh?p d?y d? Email và H? tên.", "Ðóng");
                }
                return;
            }

            try
            {
                IsBusy = true;
                var user = new User
                {
                    Email = Email,
                    FullName = FullName,
                    Username = string.IsNullOrWhiteSpace(Username) ? Email.Split('@')[0] : Username,
                    PhoneNumber = PhoneNumber,
                    Role = SelectedRole
                };

                var result = await _userService.SaveUserAsync(user);
                if (result > 0)
                {
                    await AppShell.DisplayToastAsync("Tài kho?n dã du?c t?o thành công.");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    if (Shell.Current != null)
                    {
                        await Shell.Current.DisplayAlert("L?i", "Không th? t?o tài kho?n. Có th? Email dã t?n t?i ho?c Role không h?p l?.", "Ðóng");
                    }
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
