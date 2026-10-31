#nullable disable

using CampusActivitiesManager.BusinessLogic.Services;
using CampusActivitiesManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class LoginPageModel : ObservableObject
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserService _userService;

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

        [RelayCommand]
        public void QuickSelectRole(string role)
        {
            switch (role?.ToLower())
            {
                case "admin":
                    UsernameOrEmail = "admin@campus.edu.vn";
                    Password = "Admin@123";
                    break;
                case "manager":
                    UsernameOrEmail = "manager@campus.edu.vn";
                    Password = "Manager@123";
                    break;
                case "giangvien":
                case "lecturer":
                    UsernameOrEmail = "giangvien@campus.edu.vn";
                    Password = "Gv@123";
                    break;
                case "bithu":
                case "secretary":
                    UsernameOrEmail = "bithu@campus.edu.vn";
                    Password = "Bithu@123";
                    break;
                case "student":
                    UsernameOrEmail = "B20DCCN001";
                    Password = "Student@123";
                    break;
            }
        }

        public LoginPageModel(IAuthenticationService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
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

            IsLoading = true;
            try
            {
                await Task.Delay(250);
                bool success = await _authService.LoginAsync(UsernameOrEmail, Password);

                if (!success)
                {
                    ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác, hoặc tài khoản đã bị khóa.";
                    HasError = true;
                    return;
                }

                await Shell.Current.GoToAsync("//homeTab/main");
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

        [RelayCommand]
        public async Task QuickBiometricLogin()
        {
            IsLoading = true;
            try
            {
                await Task.Delay(300);
                bool success = await _authService.LoginAsync("admin@campus.edu.vn", "Admin@123");
                if (success)
                {
                    await Shell.Current.GoToAsync("//homeTab/main");
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
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("register");
            }
        }

        [RelayCommand]
        public async Task ForgotPassword()
        {
            string input = await Shell.Current.DisplayPromptAsync(
                "Quên mật khẩu 🔑",
                "Nhập Tên đăng nhập, Email hoặc Mã sinh viên để gửi yêu cầu đặt lại mật khẩu tới Ban Quản Trị (Admin):",
                "Gửi yêu cầu",
                "Hủy",
                placeholder: "Ví dụ: B20DCCN001 hoặc admin@campus.edu.vn"
            );

            if (string.IsNullOrWhiteSpace(input)) return;

            IsLoading = true;
            try
            {
                var (success, message, defaultPw) = await _userService.RequestPasswordResetAsync(input.Trim(), "Quên mật khẩu từ màn hình đăng nhập");
                if (success)
                {
                    await Shell.Current.DisplayAlert(
                        "Đã gửi yêu cầu tới Admin 🛡️",
                        $"Yêu cầu đặt lại mật khẩu cho '{input.Trim()}' đã được chuyển đến Quản trị viên hệ thống.\n\nSau khi Admin xác nhận, mật khẩu của bạn sẽ được đặt về mật khẩu mặc định của vai trò ({defaultPw}).\n\nVui lòng liên hệ Admin hoặc thử đăng nhập lại với mật khẩu mặc định sau khi được duyệt.",
                        "OK"
                    );
                }
                else
                {
                    await Shell.Current.DisplayAlert(
                        "Thông báo ⚠️",
                        message,
                        "Đóng"
                    );
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Lỗi", $"Không thể gửi yêu cầu: {ex.Message}", "Đóng");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
