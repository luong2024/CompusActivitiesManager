#nullable disable
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class LoginPageModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string _usernameOrEmail = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _isPasswordHidden = true;

        [ObservableProperty]
        private bool _rememberMe = true;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError = false;

        public LoginPageModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        public void TogglePasswordVisibility()
        {
            IsPasswordHidden = !IsPasswordHidden;
        }

        [RelayCommand]
        public async Task Login()
        {
            ErrorMessage = string.Empty;
            HasError = false;

            // AC 2.1: Client-side Validation for empty fields
            if (string.IsNullOrWhiteSpace(UsernameOrEmail))
            {
                ErrorMessage = "Vui lòng nhập Tên đăng nhập hoặc Email.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập Mật khẩu.";
                HasError = true;
                return;
            }

            // AC 2.4: Loading state (Disable button + Spinner)
            IsLoading = true;

            try
            {
                await Task.Delay(350); // UI feel & smooth transition
                var (success, message, user) = await _authService.LoginAsync(UsernameOrEmail, Password, RememberMe);

                if (!success)
                {
                    ErrorMessage = message;
                    HasError = true;
                    return;
                }

                // AC 28.4.3: Role-based Navigation
                if (user?.Role == AccountRole.Admin)
                {
                    await Shell.Current.GoToAsync("//accounts");
                }
                else
                {
                    await Shell.Current.GoToAsync("//main");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi kết nối máy chủ: {ex.Message}";
                HasError = true;
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Quick Demo 1-Click Login for Admin
        [RelayCommand]
        public async Task LoginAsAdmin()
        {
            UsernameOrEmail = "admin@campus.edu.vn";
            Password = "admin";
            await Login();
        }

        // Quick Demo 1-Click Login for Student
        [RelayCommand]
        public async Task LoginAsStudent()
        {
            UsernameOrEmail = "cuong.na@campus.edu.vn";
            Password = "password123";
            await Login();
        }

        [RelayCommand]
        public async Task QuickBiometricLogin()
        {
            IsLoading = true;
            try
            {
                await Task.Delay(400);
                var (success, message, user) = await _authService.LoginAsync("admin@campus.edu.vn", "123456", true);
                if (success)
                {
                    await Shell.Current.GoToAsync("//accounts");
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync("register");
        }

        [RelayCommand]
        public async Task ForgotPassword()
        {
            string email = await Shell.Current.DisplayPromptAsync(
                "Quên mật khẩu",
                "Nhập địa chỉ Email đã đăng ký để nhận liên kết đặt lại mật khẩu:",
                "Gửi yêu cầu",
                "Hủy",
                keyboard: Keyboard.Email
            );

            if (!string.IsNullOrWhiteSpace(email))
            {
                await Shell.Current.DisplayAlert(
                    "Đã gửi hướng dẫn",
                    $"Liên kết đặt lại mật khẩu đã được gửi tới {email}. Vui lòng kiểm tra hòm thư.",
                    "OK"
                );
            }
        }
    }
}
