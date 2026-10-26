using CampusActivitiesManager.BusinessLogic.Services;
using CampusActivitiesManager.Models;
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
        private List<Role> _availableRoles = [Role.Admin, Role.Manager, Role.Lecturer, Role.Secretary, Role.User, Role.Guest];

        [ObservableProperty]
        private string _roleDescription = string.Empty;

        [ObservableProperty]
        private List<ClassRoom> _availableClasses = [];

        [ObservableProperty]
        private ClassRoom? _selectedClass;

        [ObservableProperty]
        private string _className = string.Empty;

        [ObservableProperty]
        private string _managedClasses = string.Empty;

        public EditUserRoleViewModel(
            IUserService<User> userService,
            IAuthenticationService authService,
            ModalErrorHandler errorHandler)
        {
            _userService = userService;
            _authService = authService;
            _errorHandler = errorHandler;

            Title = "Chỉnh sửa Tài khoản";
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
                AvailableClasses = await _userService.GetClassesAsync();
                if (User != null)
                {
                    SelectedRole = User.Role;
                    RoleDescription = SelectedRole.GetDescription();
                    ClassName = User.ClassName;
                    ManagedClasses = string.IsNullOrWhiteSpace(User.ManagedClasses) ? User.AssignedClass : User.ManagedClasses;
                    SelectedClass = AvailableClasses.FirstOrDefault(c => string.Equals(c.ClassName, User.ClassName, StringComparison.OrdinalIgnoreCase))
                                    ?? AvailableClasses.FirstOrDefault();
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
                    await Shell.Current.DisplayAlert("Lỗi", "Vui lòng nhập Họ tên.", "Đóng");
                }
                return;
            }

            if (!_authService.IsAdmin)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Từ chối truy cập", "Chỉ tài khoản Admin mới có quyền sửa thông tin!", "Đóng");
                }
                return;
            }

            try
            {
                IsBusy = true;
                User.Role = SelectedRole;
                User.ClassName = SelectedClass?.ClassName ?? ClassName;
                if (SelectedRole == Role.Lecturer || SelectedRole == Role.Manager)
                {
                    User.AssignedClass = SelectedClass?.ClassName ?? ClassName;
                    User.ManagedClasses = !string.IsNullOrWhiteSpace(ManagedClasses) ? ManagedClasses : User.AssignedClass;
                }
                User.IsSecretary = (SelectedRole == Role.Secretary);
                var success = await _userService.UpdateUserAsync(User);
                if (success)
                {
                    if (_authService.CurrentUser?.Id == UserId)
                    {
                        await _authService.RefreshCurrentUserAsync();
                    }

                    await AppShell.DisplayToastAsync($"Đã cập nhật thông tin tài khoản: {User.FullName}");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    if (Shell.Current != null)
                    {
                        await Shell.Current.DisplayAlert("Lỗi", "Không thể lưu cập nhật vào CSDL.", "Đóng");
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
