using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    /// <summary>
    /// ViewModel chỉnh sửa phân quyền người dùng độc lập.
    /// Nhận tham số UserId qua Shell QueryProperty.
    /// </summary>
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

            Title = "Chỉnh sửa Phân quyền";
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

            // Kiểm tra quyền Admin
            if (!_authService.IsAdmin)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Từ chối truy cập", "Chỉ tài khoản Admin mới có quyền đổi vai trò!", "Đóng");
                }
                return;
            }

            try
            {
                IsBusy = true;
                var success = await _userService.UpdateUserRoleAsync(UserId, SelectedRole);
                if (success)
                {
                    User.Role = SelectedRole;

                    if (_authService.CurrentUser?.Id == UserId)
                    {
                        await _authService.RefreshCurrentUserAsync();
                    }

                    await AppShell.DisplayToastAsync($"Đã cập nhật vai trò cho {User.FullName}: {SelectedRole.GetShortName()}");

                    // Quay lại trang trước
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
