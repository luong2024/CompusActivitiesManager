using System.Text.RegularExpressions;
using CampusActivitiesManager.BusinessLogic.Services;
using CampusActivitiesManager.Models;
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
        private string _username = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        [ObservableProperty]
        private string _selectedClass = "D20CNTT1";

        [ObservableProperty]
        private List<string> _availableClasses = new()
        {
            "D20CNTT1", "D21CNTT1", "D22CNTT1", "D20ATTT1", "D21ATTT1", "D22ATTT1", "D20DTVT1", "D21QTKD1"
        };

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private bool _isPasswordHidden = true;

        [ObservableProperty]
        private bool _isConfirmPasswordHidden = true;

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
            Title = "Đăng ký tài khoản sinh viên";
            _ = Task.Run(async () => await LoadClassesAsync());
        }

        private async Task LoadClassesAsync()
        {
            try
            {
                var classes = await _userService.GetClassesAsync();
                if (classes != null && classes.Any())
                {
                    var names = classes.Select(c => c.ClassName).Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToList();
                    if (names.Any())
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            AvailableClasses = names;
                            if (string.IsNullOrEmpty(SelectedClass) || !AvailableClasses.Contains(SelectedClass))
                            {
                                SelectedClass = AvailableClasses.FirstOrDefault() ?? "D20CNTT1";
                            }
                        });
                    }
                }
            }
            catch { }
        }

        partial void OnPasswordChanged(string value)
        {
            CheckPasswordStrength(value);
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
                PasswordStrength = "Yếu (ít hơn 6 ký tự)";
                PasswordStrengthColor = Microsoft.Maui.Graphics.Colors.Red;
            }
            else if (Regex.IsMatch(password, @"[A-Z]") && Regex.IsMatch(password, @"[0-9]") && password.Length >= 8)
            {
                PasswordStrength = "Mạnh (Tốt) ✅";
                PasswordStrengthColor = Microsoft.Maui.Graphics.Color.FromArgb("#10B981");
            }
            else
            {
                PasswordStrength = "Trung bình";
                PasswordStrengthColor = Microsoft.Maui.Graphics.Color.FromArgb("#F59E0B");
            }
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            ErrorMessage = string.Empty;
            HasError = false;

            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Vui lòng nhập Họ và tên sinh viên.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Vui lòng nhập Mã sinh viên hoặc Tên đăng nhập.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Vui lòng nhập Email.";
                HasError = true;
                return;
            }

            if (!Regex.IsMatch(Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ErrorMessage = "Địa chỉ Email không đúng định dạng.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập Mật khẩu.";
                HasError = true;
                return;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "Mật khẩu phải có độ dài tối thiểu 6 ký tự.";
                HasError = true;
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp.";
                HasError = true;
                return;
            }

            IsBusy = true;

            try
            {
                var cleanUser = Username.Trim().ToLower();
                var cleanEmail = Email.Trim().ToLower();

                // Kiểm tra trùng lặp mã sinh viên / username
                var existingUser = await _userService.GetUserByUsernameAsync(cleanUser);
                if (existingUser != null)
                {
                    ErrorMessage = $"Mã sinh viên hoặc Tên đăng nhập '{Username}' đã tồn tại trong hệ thống.";
                    HasError = true;
                    return;
                }

                // Kiểm tra trùng lặp email
                var allUsers = await _userService.GetUsersListAsync();
                if (allUsers.Any(u => string.Equals(u.Email, cleanEmail, StringComparison.OrdinalIgnoreCase)))
                {
                    ErrorMessage = $"Email '{Email}' đã được sử dụng bởi một tài khoản khác.";
                    HasError = true;
                    return;
                }

                var newUser = new User
                {
                    Id = "stu_" + Guid.NewGuid().ToString("N")[..8],
                    FullName = FullName.Trim(),
                    Username = cleanUser,
                    Email = cleanEmail,
                    PhoneNumber = PhoneNumber?.Trim() ?? string.Empty,
                    ClassName = string.IsNullOrWhiteSpace(SelectedClass) ? "D20CNTT1" : SelectedClass,
                    Role = Role.Student,
                    IsSecretary = false,
                    TrainingPoints = 0, // Quy tắc: điểm rèn luyện khởi điểm ban đầu là 0 ĐRL
                    IsActive = true,
                    Password = Password,
                    PasswordHash = Password
                };

                int saved = await _userService.SaveUserAsync(newUser);
                if (saved > 0)
                {
                    if (Shell.Current != null)
                    {
                        await Shell.Current.DisplayAlert(
                            "Đăng ký thành công 🎉",
                            $"Chào mừng {newUser.FullName}!\n\n" +
                            $"• Mã sinh viên: {newUser.Username}\n" +
                            $"• Lớp sinh hoạt: {newUser.ClassName}\n" +
                            $"• Điểm rèn luyện khởi điểm: 0 ĐRL\n" +
                            $"• Mục tiêu tối thiểu: 50 ĐRL / kỳ • 250 ĐRL / năm\n\n" +
                            "Tài khoản của bạn đã sẵn sàng. Hãy đăng nhập để bắt đầu!",
                            "Đăng nhập ngay");

                        await Shell.Current.GoToAsync("//login");
                    }
                }
                else
                {
                    ErrorMessage = "Không thể lưu thông tin tài khoản. Vui lòng thử lại.";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi khi đăng ký: {ex.Message}";
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
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//login");
            }
        }
    }
}
