using CampusActivitiesManager.BusinessLogic.Interfaces;
using CampusActivitiesManager.BusinessLogic.Services;
using CampusActivitiesManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class ProfilePageModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authService;
        private readonly ModalErrorHandler _errorHandler;
        private readonly IFirebaseSyncService? _firebaseSync;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        [ObservableProperty]
        private string _avatarUrl = string.Empty;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _roleDisplay = "Sinh viên";

        [ObservableProperty]
        private string _className = "D20CNTT1";

        [ObservableProperty]
        private int _trainingPoints = 0;

        // Trạng thái Cloud Sync tự động
        [ObservableProperty]
        private string _cloudSyncStatusText = "🟢 Tự động đồng bộ liên tục (Realtime Cloud Sync)";

        [ObservableProperty]
        private string _cloudSyncLastTime = "Vừa xong";

        // Modal Đổi Mật Khẩu
        [ObservableProperty]
        private bool _isChangePasswordModalVisible;

        [ObservableProperty]
        private string _currentPasswordInput = string.Empty;

        [ObservableProperty]
        private string _newPasswordInput = string.Empty;

        [ObservableProperty]
        private string _confirmPasswordInput = string.Empty;

        [ObservableProperty]
        private string _changePasswordErrorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasChangePasswordError = false;

        public ProfilePageModel(
            IUserService userService,
            IAuthenticationService authService,
            ModalErrorHandler errorHandler,
            IFirebaseSyncService? firebaseSync = null)
        {
            _userService = userService;
            _authService = authService;
            _errorHandler = errorHandler;
            _firebaseSync = firebaseSync;
            Title = "Thông tin cá nhân";
        }

        [RelayCommand]
        public void OpenChangePasswordModal()
        {
            CurrentPasswordInput = string.Empty;
            NewPasswordInput = string.Empty;
            ConfirmPasswordInput = string.Empty;
            ChangePasswordErrorMessage = string.Empty;
            HasChangePasswordError = false;
            IsChangePasswordModalVisible = true;
        }

        [RelayCommand]
        public void CloseChangePasswordModal()
        {
            IsChangePasswordModalVisible = false;
        }

        [RelayCommand]
        public async Task ExecuteChangePasswordAsync()
        {
            HasChangePasswordError = false;
            ChangePasswordErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(CurrentPasswordInput))
            {
                HasChangePasswordError = true;
                ChangePasswordErrorMessage = "Vui lòng nhập mật khẩu hiện tại.";
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPasswordInput) || NewPasswordInput.Trim().Length < 6)
            {
                HasChangePasswordError = true;
                ChangePasswordErrorMessage = "Mật khẩu mới phải có tối thiểu 6 ký tự.";
                return;
            }

            if (NewPasswordInput.Trim() != ConfirmPasswordInput?.Trim())
            {
                HasChangePasswordError = true;
                ChangePasswordErrorMessage = "Xác nhận mật khẩu mới không khớp.";
                return;
            }

            var user = _authService.CurrentUser;
            if (user == null) return;

            try
            {
                IsBusy = true;
                var (success, message) = await _userService.ChangePasswordAsync(user.Username, CurrentPasswordInput.Trim(), NewPasswordInput.Trim());
                if (success)
                {
                    IsChangePasswordModalVisible = false;
                    await Shell.Current.DisplayAlert("Thành công 🎉", "Mật khẩu tài khoản đã được thay đổi thành công!", "OK");
                }
                else
                {
                    HasChangePasswordError = true;
                    ChangePasswordErrorMessage = message;
                }
            }
            catch (Exception ex)
            {
                HasChangePasswordError = true;
                ChangePasswordErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SyncFirebaseDataAsync()
        {
            if (_firebaseSync == null) return;

            try
            {
                IsBusy = true;
                string currentUrl = _firebaseSync.DatabaseUrl;

                string action = await Shell.Current.DisplayActionSheet(
                    "Đồng bộ Firebase Cloud ☁️",
                    "Đóng",
                    null,
                    "⚡ Kích hoạt Đồng bộ Ngay lập tức",
                    "⚙️ Cấu hình URL Firebase Realtime DB");

                if (action == "⚡ Kích hoạt Đồng bộ Ngay lập tức")
                {
                    var users = await _userService.GetUsersListAsync();
                    var classes = await _userService.GetClassesAsync();
                    await _firebaseSync.PushUsersAsync(users);
                    await _firebaseSync.PushClassesAsync(classes);
                    CloudSyncLastTime = DateTime.Now.ToString("HH:mm:ss dd/MM");
                    await Shell.Current.DisplayAlert("Thành công 🎉", "Dữ liệu người dùng và lớp học đã được đồng bộ với Firebase Cloud Database!", "Đóng");
                }
                else if (action == "⚙️ Cấu hình URL Firebase Realtime DB")
                {
                    string newUrl = await Shell.Current.DisplayPromptAsync(
                        "Cấu hình Firebase Realtime DB ☁️",
                        "Nhập URL cơ sở dữ liệu Firebase (ví dụ: https://your-project-default-rtdb.firebaseio.com/):",
                        "Lưu",
                        "Hủy",
                        currentUrl);

                    if (!string.IsNullOrWhiteSpace(newUrl))
                    {
                        _firebaseSync.DatabaseUrl = newUrl.Trim();
                        CloudSyncLastTime = "Vừa cập nhật URL";
                        await Shell.Current.DisplayAlert("Đã lưu 💾", $"Đã cập nhật URL Firebase: {_firebaseSync.DatabaseUrl}", "OK");
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
        public async Task LoadProfileAsync()
        {
            var user = _authService.CurrentUser;
            if (user != null)
            {
                FullName = user.FullName;
                PhoneNumber = user.PhoneNumber;
                Username = user.Username;
                Email = user.Email;
                ClassName = user.ClassName ?? "D20CNTT1";
                TrainingPoints = user.TrainingPoints;
                RoleDisplay = user.Role switch
                {
                    Role.Admin => "Quản trị viên (Admin)",
                    Role.Manager => "Ban tổ chức / Quản lý sự kiện",
                    Role.Lecturer => "Giảng viên / Cố vấn",
                    Role.Secretary => "Bí thư chi đoàn lớp",
                    _ => "Sinh viên (User)"
                };
            }
            await Task.CompletedTask;
        }

        [RelayCommand]
        public void ToggleMenu()
        {
            if (Shell.Current != null)
            {
                Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
            }
        }

        [RelayCommand]
        public async Task LogoutAsync()
        {
            if (Shell.Current == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Đăng xuất",
                "Bạn có chắc chắn muốn đăng xuất khỏi ứng dụng Campus Activities Manager?",
                "Đăng xuất",
                "Hủy");

            if (!confirm) return;

            _authService.Logout();
            await Shell.Current.GoToAsync("//login");
        }

        [RelayCommand]
        private async Task UpdateProfileAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var request = new UpdateProfileRequest
                {
                    Email = _authService.CurrentUser?.Email,
                    FullName = FullName,
                    PhoneNumber = PhoneNumber,
                    AvatarUrl = AvatarUrl
                };

                var result = await _userService.UpdateProfileAsync(request);

                if (_authService.CurrentUser != null)
                {
                    _authService.CurrentUser.FullName = FullName;
                    _authService.CurrentUser.PhoneNumber = PhoneNumber;
                    var accountUpdated = await _userService.UpdateUserAsync(_authService.CurrentUser);
                    result = result || accountUpdated;
                }

                if (result)
                {
                    await Shell.Current.DisplayAlert("Thành công", "Cập nhật thông tin thành công", "OK");
                    await _authService.RefreshCurrentUserAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Lỗi", "Không thể cập nhật thông tin", "OK");
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
    }
}
