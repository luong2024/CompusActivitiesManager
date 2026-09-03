using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    [QueryProperty(nameof(UserId), "UserId")]
    public partial class EditUserRoleViewModel : BaseViewModel
    {
        private readonly IUserService<User> _userService;
        private readonly IAuthenticationService _authService;
        private readonly ModalErrorHandler _errorHandler;

        [ObservableProperty]
        private string _userId = string.Empty;

        [ObservableProperty]
        private User? _user;

        [ObservableProperty]
        private Role _selectedRole = Role.User;

        [ObservableProperty]
        private List<Role> _availableRoles = [Role.Admin, Role.Manager, Role.User, Role.Guest];

        [ObservableProperty]
        private string _roleDescription = string.Empty;

        public EditUserRoleViewModel(
            IUserService<User> userService,
            IAuthenticationService authService,
            ModalErrorHandler errorHandler)
        {
            _userService = userService;
            _authService = authService;
            _errorHandler = errorHandler;

            Title = "Ch?nh s?a Tài kho?n";
        }

        async partial void OnUserIdChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                await LoadUserAsync(value);
            }
        }

        partial void OnSelectedRoleChanged(Role value)
        {
            RoleDescription = value.GetDescription();
        }

        private async Task LoadUserAsync(string id)
        {
            try
            {
                IsBusy = true;
                User = await _userService.GetUserByIdAsync(id);
                if (User != null)
                {
                    SelectedRole = User.Role;
                    RoleDescription = SelectedRole.GetDescription();
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
        public async Task SaveRole()
        {
            if (User == null || string.IsNullOrEmpty(UserId))
                return;

            if (string.IsNullOrWhiteSpace(User.FullName))
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("L?i", "Vui lòng nh?p H? tên.", "Ðóng");
                }
                return;
            }

            if (!_authService.IsAdmin)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("T? ch?i truy c?p", "Ch? tài kho?n Admin m?i có quy?n s?a thông tin!", "Ðóng");
                }
                return;
            }

            try
            {
                IsBusy = true;
                User.Role = SelectedRole;
                var success = await _userService.UpdateUserAsync(User);
                if (success)
                {
                    if (_authService.CurrentUser?.Id == UserId)
                    {
                        await _authService.RefreshCurrentUserAsync();
                    }

                    await AppShell.DisplayToastAsync($"Ðã c?p nh?t thông tin tài kho?n: {User.FullName}");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    if (Shell.Current != null)
                    {
                        await Shell.Current.DisplayAlert("L?i", "Không th? luu c?p nh?t vào CSDL.", "Ðóng");
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
