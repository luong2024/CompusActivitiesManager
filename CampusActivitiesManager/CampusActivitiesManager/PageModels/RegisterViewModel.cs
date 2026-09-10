using System.Text.RegularExpressions;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IUserService<User> _userService;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private string _passwordStrength = string.Empty;

        [ObservableProperty]
        private Microsoft.Maui.Graphics.Color _passwordStrengthColor = Microsoft.Maui.Graphics.Colors.Transparent;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError = false;

        public RegisterViewModel(IUserService<User> userService)
        {
            _userService = userService;
            Title = "Đăng ký tài khoản";
        }

        partial void OnPasswordChanged(string value)
        {
            CheckPasswordStrength(value);
        }

        private void CheckPasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                PasswordStrength = "";
                PasswordStrengthColor = Microsoft.Maui.Graphics.Colors.Transparent;
                return;
            }

            if (password.Length < 6)
            {
                PasswordStrength = "Yếu (Ngắn)";
                PasswordStrengthColor = Microsoft.Maui.Graphics.Colors.Red;
            }
            else if (Regex.IsMatch(password, @"[A-Z]") && Regex.IsMatch(password, @"[0-9]") && password.Length >= 8)
            {
                PasswordStrength = "Mạnh";
                PasswordStrengthColor = Microsoft.Maui.Graphics.Colors.Green;
            }
            else
            {
                PasswordStrength = "Trung bình";
                PasswordStrengthColor = Microsoft.Maui.Graphics.Colors.Orange;
            }
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            HasError = false;

            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || 
                string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin!";
                HasError = true;
                return;
            }

            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ErrorMessage = "Email không đúng định dạng!";
                HasError = true;
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp!";
                HasError = true;
                return;
            }

            IsBusy = true;

            try
            {
                var newUser = new User
                {
                    FullName = FullName.Trim(),
                    Email = Email.Trim(),
                    Username = Username.Trim(),
                    PasswordHash = Password,
                    Role = Role.User,
                    IsActive = true
                };

                await _userService.SaveUserAsync(newUser);
                
                await Shell.Current.DisplayAlert("Thành công", "Đăng ký tài khoản thành công!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi đăng ký: {ex.Message}";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

