using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    /// <summary>
    /// ViewModel xử lý đăng nhập, quản lý phiên và phân luồng điều hướng sau đăng nhập theo vai trò (Task.md mục 6).
    /// </summary>
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private readonly ModalErrorHandler _errorHandler;

        [ObservableProperty]
        private string _username = "admin";

        [ObservableProperty]
        private string _password = "123";

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private User? _currentUser;

        [ObservableProperty]
        private string _currentRoleDisplay = string.Empty;

        public LoginViewModel(IAuthenticationService authService, ModalErrorHandler errorHandler)
        {
            _authService = authService;
            _errorHandler = errorHandler;

            Title = "Đăng nhập";
            _authService.CurrentUserChanged += (s, e) => UpdateCurrentSessionInfo();
            UpdateCurrentSessionInfo();
        }

        private void UpdateCurrentSessionInfo()
        {
            CurrentUser = _authService.CurrentUser;
            CurrentRoleDisplay = _authService.CurrentRole.GetDisplayName();
        }

        [RelayCommand]
        public async Task Login()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                HasError = false;
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(Username))
                {
                    HasError = true;
                    ErrorMessage = "Vui lòng nhập tên đăng nhập!";
                    return;
                }

                var success = await _authService.LoginAsync(Username.Trim(), Password?.Trim() ?? string.Empty);
                if (success)
                {
                    UpdateCurrentSessionInfo();
                    await AppShell.DisplayToastAsync($"Xin chào, {_authService.CurrentUser?.FullName} ({_authService.CurrentRole.GetShortName()})");

                    // Phân luồng điều hướng theo từng Role
                    if (_authService.CurrentRole == Role.Admin)
                    {
                        // Admin có thể chuyển ngay đến trang chính hoặc trang quản lý
                        await Shell.Current.GoToAsync("//main");
                    }
                    else
                    {
                        // User / Guest điều hướng về trang chủ xem nội dung
                        await Shell.Current.GoToAsync("//main");
                    }
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác, hoặc tài khoản đã bị khóa!";
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
        public async Task QuickLogin(string targetUsername)
        {
            Username = targetUsername;
            Password = "123";
            await Login();
        }

        [RelayCommand]
        public async Task Logout()
        {
            _authService.Logout();
            UpdateCurrentSessionInfo();
            await AppShell.DisplayToastAsync("Đã đăng xuất");
        }
    }
}
