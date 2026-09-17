
$path = "CampusActivitiesManager\CampusActivitiesManager\PageModels\LoginViewModel.cs"
$content = @"
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private readonly ModalErrorHandler _errorHandler;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private bool _isPasswordHidden = true;

        [ObservableProperty]
        private bool _rememberMe = false;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private User? _currentUser;

        [ObservableProperty]
        private string _currentRoleDisplay = string.Empty;

        public LoginViewModel(IAuthenticationService authService, ModalErrorHandler errorHandler)
        {
            _authService = authService;
            _errorHandler = errorHandler;
            Title = "Ðang nh?p";
            _authService.CurrentUserChanged += (s, e) => UpdateCurrentSessionInfo();
            UpdateCurrentSessionInfo();
        }

        private void UpdateCurrentSessionInfo()
        {
            CurrentUser = _authService.CurrentUser;
            CurrentRoleDisplay = _authService.CurrentRole.GetDisplayName();
        }

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordHidden = !IsPasswordHidden;
        }

        [RelayCommand]
        private async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync("register");
        }

        [RelayCommand]
        private async Task ForgotPassword()
        {
            await Shell.Current.DisplayAlert("Quên m?t kh?u", "Tính nang dang du?c phát tri?n.", "OK");
        }

        [RelayCommand]
        public async Task Login()
        {
            if (IsLoading) return;
            try
            {
                IsLoading = true;
                HasError = false;
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(Username))
                {
                    HasError = true;
                    ErrorMessage = "Vui lòng nh?p tên dang nh?p / email!";
                    return;
                }

                var success = await _authService.LoginAsync(Username.Trim(), Password?.Trim() ?? string.Empty);
                if (success)
                {
                    await SecureStorage.Default.SetAsync("auth_token", "dummy_secure_token_" + _authService.CurrentUser?.Id);
                    UpdateCurrentSessionInfo();
                    await AppShell.DisplayToastAsync($"Xin chào, {_authService.CurrentUser?.FullName} ({_authService.CurrentRole.GetShortName()})");
                    await Shell.Current.GoToAsync("//main");
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Tên dang nh?p ho?c m?t kh?u không chính xác, ho?c tài kho?n dã b? khóa!";
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task LoginAsAdmin()
        {
            Username = "admin@campus.edu.vn";
            Password = "123";
            await Login();
        }

        [RelayCommand]
        public async Task LoginAsStudent()
        {
            Username = "duc.tm@sinhvien.campus.edu.vn";
            Password = "123";
            await Login();
        }

        [RelayCommand]
        public async Task Logout()
        {
            _authService.Logout();
            SecureStorage.Default.Remove("auth_token");
            UpdateCurrentSessionInfo();
            await AppShell.DisplayToastAsync("Ðã dang xu?t");
        }
    }
}
"@
Set-Content $path -Value $content -Encoding UTF8

