#nullable disable
using System.Text.RegularExpressions;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics;

namespace CampusActivitiesManager.PageModels
{
    public partial class RegisterPageModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _studentCode = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _className = "D20CNTT1";

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private bool _agreeToTerms = false;

        [ObservableProperty]
        private bool _isPasswordHidden = true;

        [ObservableProperty]
        private bool _isConfirmPasswordHidden = true;

        [ObservableProperty]
        private string _passwordStrengthText = "Chưa nhập mật khẩu";

        [ObservableProperty]
        private Color _passwordStrengthColor = Color.FromArgb("#919191");

        [ObservableProperty]
        private double _passwordStrengthPercent = 0.0;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError = false;

        public RegisterPageModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        public void TogglePasswordVisibility()
        {
            IsPasswordHidden = !IsPasswordHidden;
        }

        [RelayCommand]
        public void ToggleConfirmPasswordVisibility()
        {
            IsConfirmPasswordHidden = !IsConfirmPasswordHidden;
        }

        // AC 26.3: Client-side password strength analyzer
        partial void OnPasswordChanged(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                PasswordStrengthText = "Chưa nhập mật khẩu";
                PasswordStrengthColor = Color.FromArgb("#919191");
                PasswordStrengthPercent = 0.0;
                return;
            }

            int score = 0;
            if (value.Length >= 6) score++;
            if (value.Length >= 8) score++;
            if (Regex.IsMatch(value, @"[0-9]") && Regex.IsMatch(value, @"[a-zA-Z]")) score++;
            if (Regex.IsMatch(value, @"[!@#$%^&*(),.?\:{}|<>]")) score++;

            switch (score)
            {
                case 1:
                    PasswordStrengthText = "Độ mạnh: Rất yếu (Cần ít nhất 6 ký tự)";
                    PasswordStrengthColor = Color.FromArgb("#A80000"); // Red
                    PasswordStrengthPercent = 0.25;
                    break;
                case 2:
                    PasswordStrengthText = "Độ mạnh: Yếu (Nên thêm số và chữ hoa)";
                    PasswordStrengthColor = Color.FromArgb("#D83B01"); // Orange
                    PasswordStrengthPercent = 0.50;
                    break;
                case 3:
                    PasswordStrengthText = "Độ mạnh: Khá tốt (Thêm ký tự đặc biệt để an toàn hơn)";
                    PasswordStrengthColor = Color.FromArgb("#008272"); // Teal
                    PasswordStrengthPercent = 0.75;
                    break;
                case 4:
                    PasswordStrengthText = "Độ mạnh: Rất mạnh (Mật khẩu an toàn tuyệt đối)";
                    PasswordStrengthColor = Color.FromArgb("#107C41"); // Green
                    PasswordStrengthPercent = 1.0;
                    break;
            }
        }

        [RelayCommand]
        public async Task Register()
        {
            ErrorMessage = string.Empty;
            HasError = false;

            // AC 26.3: Validate Full Name
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Vui lòng nhập Họ và tên đầy đủ.";
                HasError = true;
                return;
            }

            // Validate Student Code
            if (string.IsNullOrWhiteSpace(StudentCode))
            {
                ErrorMessage = "Vui lòng nhập Mã số sinh viên / Mã cán bộ.";
                HasError = true;
                return;
            }

            // AC 26.3: Validate Email format
            if (string.IsNullOrWhiteSpace(Email) || !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ErrorMessage = "Địa chỉ Email không đúng định dạng (Ví dụ: student@campus.edu.vn).";
                HasError = true;
                return;
            }

            // AC 26.3: Validate Password length & complexity
            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            {
                ErrorMessage = "Mật khẩu phải có tối thiểu 6 ký tự.";
                HasError = true;
                return;
            }

            // AC 26.3: Validate Confirm Password
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp. Vui lòng kiểm tra lại.";
                HasError = true;
                return;
            }

            // AC 1.4: Terms & Conditions Checkbox
            if (!AgreeToTerms)
            {
                ErrorMessage = "Bạn cần đồng ý với Điều khoản & Chính sách dịch vụ để tiếp tục.";
                HasError = true;
                return;
            }

            // AC 26.5.4: Loading state to prevent double submit
            IsLoading = true;

            try
            {
                await Task.Delay(400); // Simulate API network call
                var (success, message) = await _authService.RegisterAsync(FullName, Email, Password, ClassName);

                if (!success)
                {
                    ErrorMessage = message;
                    HasError = true;
                    return;
                }

                await Shell.Current.DisplayAlert("Thành công 🎉", "Tài khoản của bạn đã được tạo thành công! Bạn có thể đăng nhập ngay bây giờ.", "Đăng nhập ngay");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi đăng ký: {ex.Message}";
                HasError = true;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
