using System.Collections.ObjectModel;
using CampusActivitiesManager.BusinessLogic.Services;
using CampusActivitiesManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    /// <summary>
    /// ViewModel quản lý người dùng, duyệt reset mật khẩu và thực thi nghiệp vụ phân quyền truy cập (RBAC).
    /// </summary>
    public partial class UserManagementPageModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authService;
        private readonly ModalErrorHandler _errorHandler;

        [ObservableProperty]
        private ObservableCollection<User> _usersList = [];

        [ObservableProperty]
        private ObservableCollection<User> _filteredUsers = [];

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _selectedFilterRole = "All";

        [ObservableProperty]
        private int _totalUsersCount;

        [ObservableProperty]
        private int _adminCount;

        [ObservableProperty]
        private int _managerCount;

        [ObservableProperty]
        private int _userCount;

        [ObservableProperty]
        private int _guestCount;

        [ObservableProperty]
        private bool _isAdmin;

        [ObservableProperty]
        private string _currentUserName = string.Empty;

        [ObservableProperty]
        private string _currentUserRoleName = string.Empty;

        [ObservableProperty]
        private User? _selectedUser;

        [ObservableProperty]
        private Role _selectedNewRole = Role.User;

        [ObservableProperty]
        private bool _isRoleModalVisible;

        // Quản lý yêu cầu Reset mật khẩu
        [ObservableProperty]
        private ObservableCollection<PasswordResetRequest> _pendingResetRequests = [];

        [ObservableProperty]
        private int _pendingResetCount;

        [ObservableProperty]
        private bool _hasPendingResets;

        public UserManagementPageModel(
            IUserService userService,
            IAuthenticationService authService,
            ModalErrorHandler errorHandler)
        {
            _userService = userService;
            _authService = authService;
            _errorHandler = errorHandler;

            Title = "Quản lý & Phân quyền";
            _authService.CurrentUserChanged += OnCurrentUserChanged;
            SyncCurrentUserState();
        }

        private void OnCurrentUserChanged(object? sender, EventArgs e)
        {
            SyncCurrentUserState();
            ApplyFilter();
        }

        private void SyncCurrentUserState()
        {
            IsAdmin = _authService.IsAdmin;
            CurrentUserName = _authService.CurrentUser?.FullName ?? "Chưa đăng nhập";
            CurrentUserRoleName = _authService.CurrentRole.GetDisplayName();
        }

        [RelayCommand]
        private async Task Appearing()
        {
            await LoadUsersAsync();
        }

        [RelayCommand]
        public async Task LoadUsersAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                SyncCurrentUserState();

                var users = await _userService.GetUsersListAsync();
                UsersList = new ObservableCollection<User>(users);

                // Tải yêu cầu reset mật khẩu
                var pendingResets = await _userService.GetPendingPasswordResetRequestsAsync();
                PendingResetRequests = new ObservableCollection<PasswordResetRequest>(pendingResets);
                PendingResetCount = pendingResets.Count;
                HasPendingResets = IsAdmin && PendingResetCount > 0;

                UpdateStatistics();
                ApplyFilter();
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

        private void UpdateStatistics()
        {
            TotalUsersCount = UsersList.Count;
            AdminCount = UsersList.Count(u => u.Role == Role.Admin);
            ManagerCount = UsersList.Count(u => u.Role == Role.Manager);
            UserCount = UsersList.Count(u => u.Role == Role.User);
            GuestCount = UsersList.Count(u => u.Role == Role.Guest);
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        [RelayCommand]
        private void SetFilterRole(string role)
        {
            SelectedFilterRole = role;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var query = UsersList.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SelectedFilterRole) && SelectedFilterRole != "All")
            {
                if (Enum.TryParse<Role>(SelectedFilterRole, true, out var role))
                {
                    query = query.Where(u => u.Role == role);
                }
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var keyword = SearchText.Trim().ToLowerInvariant();
                query = query.Where(u =>
                    (!string.IsNullOrEmpty(u.FullName) && u.FullName.ToLowerInvariant().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.Username) && u.Username.ToLowerInvariant().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.Email) && u.Email.ToLowerInvariant().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.Department) && u.Department.ToLowerInvariant().Contains(keyword)));
            }

            FilteredUsers = new ObservableCollection<User>(query);
        }

        // ==================== RESET MẬT KHẨU CHO NGƯỜI DÙNG ====================

        [RelayCommand]
        public async Task ResolveResetRequest(PasswordResetRequest request)
        {
            if (request == null) return;

            if (!_authService.IsAdmin)
            {
                await Shell.Current.DisplayAlert("Quyền bị từ chối", "Chỉ Quản trị viên (Admin) mới có quyền reset mật khẩu!", "Đóng");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert(
                "Xác nhận Reset Mật khẩu 🔄",
                $"Bạn có chắc chắn muốn đặt lại mật khẩu cho tài khoản '{request.FullName}' (@{request.Username}) về mật khẩu mặc định của vai trò {request.Role.GetDisplayName()} ({request.DefaultPasswordAssigned})?",
                "Reset ngay",
                "Hủy");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                var (success, message, defaultPw) = await _userService.ResolvePasswordResetAsync(request.Id, CurrentUserName);
                if (success)
                {
                    PendingResetRequests.Remove(request);
                    PendingResetCount = PendingResetRequests.Count;
                    HasPendingResets = IsAdmin && PendingResetCount > 0;

                    await AppShell.DisplayToastAsync($"Đã reset mật khẩu của {request.FullName} về: {defaultPw}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Lỗi", message, "Đóng");
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
        public async Task ResetPasswordForUser(User user)
        {
            if (user == null) return;

            if (!_authService.IsAdmin)
            {
                await Shell.Current.DisplayAlert("Quyền bị từ chối", "Chỉ Quản trị viên (Admin) mới có quyền reset mật khẩu!", "Đóng");
                return;
            }

            string defaultPw = user.Role.GetDefaultPassword();

            bool confirm = await Shell.Current.DisplayAlert(
                "Reset Mật khẩu về Mặc định 🔄",
                $"Đặt lại mật khẩu cho '{user.FullName}' (@{user.Username}) về mật khẩu mặc định của vai trò {user.Role.GetDisplayName()}?\n\nMật khẩu mặc định: {defaultPw}",
                "Xác nhận Reset",
                "Hủy");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                var (success, message, newPw) = await _userService.ResetUserPasswordToDefaultAsync(user.Username, CurrentUserName);
                if (success)
                {
                    var req = PendingResetRequests.FirstOrDefault(r => r.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));
                    if (req != null)
                    {
                        PendingResetRequests.Remove(req);
                        PendingResetCount = PendingResetRequests.Count;
                        HasPendingResets = IsAdmin && PendingResetCount > 0;
                    }

                    await AppShell.DisplayToastAsync($"Đã reset mật khẩu cho {user.FullName} về: {newPw}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Lỗi", message, "Đóng");
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

        // ==================== CỘNG / TRỪ ĐIỂM RÈN LUYỆN ====================

        [RelayCommand]
        public async Task AdjustPointsForUser(User user)
        {
            if (user == null) return;

            if (!_authService.IsAdmin)
            {
                await Shell.Current.DisplayAlert("Quyền bị từ chối", "Chỉ Admin mới có quyền điều chỉnh điểm rèn luyện trực tiếp!", "Đóng");
                return;
            }

            string action = await Shell.Current.DisplayActionSheet(
                $"⚡ Quản lý Điểm Rèn Luyện: {user.FullName} (Hiện có: {user.TrainingPoints} ĐRL)",
                "Hủy",
                null,
                "➕ Cộng +5 ĐRL (Khen thưởng)",
                "➕ Cộng +10 ĐRL (Đoạt giải / Tích cực)",
                "➖ Trừ -5 ĐRL (Điểm danh không hợp lệ)",
                "➖ Trừ -10 ĐRL (Vi phạm quy chế)",
                "✍️ Nhập số điểm tùy chỉnh (+/-)");

            if (string.IsNullOrEmpty(action) || action == "Hủy") return;

            int delta = 0;
            string defaultReason = string.Empty;

            switch (action)
            {
                case "➕ Cộng +5 ĐRL (Khen thưởng)":
                    delta = 5;
                    defaultReason = "Khen thưởng cán bộ / hoạt động tích cực";
                    break;
                case "➕ Cộng +10 ĐRL (Đoạt giải / Tích cực)":
                    delta = 10;
                    defaultReason = "Đoạt giải thưởng / Đóng góp xuất sắc";
                    break;
                case "➖ Trừ -5 ĐRL (Điểm danh không hợp lệ)":
                    delta = -5;
                    defaultReason = "Minh chứng điểm danh không hợp lệ";
                    break;
                case "➖ Trừ -10 ĐRL (Vi phạm quy chế)":
                    delta = -10;
                    defaultReason = "Vi phạm nội quy trường / vắng không phép";
                    break;
                case "✍️ Nhập số điểm tùy chỉnh (+/-)":
                    string deltaStr = await Shell.Current.DisplayPromptAsync(
                        "Nhập số điểm cần điều chỉnh",
                        "Nhập số điểm (ví dụ: 5 để cộng 5, hoặc -5 để trừ 5):",
                        "Tiếp tục",
                        "Hủy",
                        keyboard: Keyboard.Numeric);
                    if (string.IsNullOrWhiteSpace(deltaStr) || !int.TryParse(deltaStr.Trim(), out delta))
                    {
                        return;
                    }
                    break;
            }

            string reason = await Shell.Current.DisplayPromptAsync(
                "Lý do điều chỉnh điểm",
                "Nhập lý do lưu vào biên bản kiểm toán ĐRL:",
                "Xác nhận",
                "Bỏ qua",
                initialValue: defaultReason);

            try
            {
                IsBusy = true;
                var (success, msg) = await _userService.AdjustTrainingPointsWithAuditAsync(
                    user.Username,
                    delta,
                    reason,
                    _authService.CurrentUser?.Username ?? "admin",
                    CurrentUserName);

                if (success)
                {
                    user.TrainingPoints = Math.Max(0, user.TrainingPoints + delta);
                    await AppShell.DisplayToastAsync(msg);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Lỗi", msg, "Đóng");
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

        // ==================== PHÂN QUYỀN ====================

        [RelayCommand]
        public async Task SelectUserForRoleAssignment(User user)
        {
            if (user == null)
                return;

            if (!_authService.IsAdmin)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert(
                        "Quyền truy cập bị từ chối",
                        "Chỉ tài khoản Quản trị viên (Admin) mới có quyền phân quyền người dùng!",
                        "Đóng");
                }
                return;
            }

            SelectedUser = user;
            SelectedNewRole = user.Role;
            IsRoleModalVisible = true;
        }

        [RelayCommand]
        private void CloseRoleModal()
        {
            IsRoleModalVisible = false;
        }

        [RelayCommand]
        private void SelectRole(string roleString)
        {
            if (Enum.TryParse<Role>(roleString, true, out var role))
            {
                SelectedNewRole = role;
            }
        }

        [RelayCommand]
        public async Task SaveRoleAssignment()
        {
            if (SelectedUser == null)
                return;

            try
            {
                IsBusy = true;
                var userId = SelectedUser.Id;
                var userName = SelectedUser.FullName;
                var targetRole = SelectedNewRole;

                var success = await _userService.UpdateUserRoleAsync(userId, targetRole);
                if (success)
                {
                    SelectedUser.Role = targetRole;

                    if (_authService.CurrentUser?.Id == userId)
                    {
                        await _authService.RefreshCurrentUserAsync();
                        SyncCurrentUserState();
                    }

                    UpdateStatistics();
                    ApplyFilter();
                    IsRoleModalVisible = false;

                    await AppShell.DisplayToastAsync(
                        $"Phân quyền thành công! Tài khoản {userName} đã được cập nhật vai trò: {targetRole.GetShortName()}");
                }
                else
                {
                    if (Shell.Current != null)
                    {
                        await Shell.Current.DisplayAlert("Lỗi", "Không thể cập nhật vai trò vào cơ sở dữ liệu. Vui lòng thử lại!", "Đóng");
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
        public async Task QuickChangeRoleActionSheet(User user)
        {
            if (user == null)
                return;

            if (!_authService.IsAdmin)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert(
                        "Quyền truy cập bị từ chối",
                        "Chỉ tài khoản Quản trị viên (Admin) mới có quyền phân quyền!",
                        "Đóng");
                }
                return;
            }

            if (Shell.Current == null)
                return;

            var action = await Shell.Current.DisplayActionSheet(
                $"⚡ Đổi vai trò: {user.FullName} (@{user.Username})",
                "Hủy",
                null,
                "Admin (Quản trị viên)",
                "Manager (Quản lý sự kiện)",
                "Lecturer (Giảng viên / Cố vấn học tập)",
                "Secretary (Bí thư chi đoàn)",
                "User (Sinh viên / Học sinh)",
                "Guest (Khách vãng lai)");

            if (string.IsNullOrEmpty(action) || action == "Hủy")
                return;

            Role chosenRole = action switch
            {
                "Admin (Quản trị viên)" => Role.Admin,
                "Manager (Quản lý sự kiện)" => Role.Manager,
                "Lecturer (Giảng viên / Cố vấn học tập)" => Role.Lecturer,
                "Secretary (Bí thư chi đoàn)" => Role.Secretary,
                "User (Sinh viên / Học sinh)" => Role.User,
                "Guest (Khách vãng lai)" => Role.Guest,
                _ => user.Role
            };

            if (chosenRole == user.Role)
                return;

            var success = await _userService.UpdateUserRoleAsync(user.Id, chosenRole);
            if (success)
            {
                user.Role = chosenRole;

                if (_authService.CurrentUser?.Id == user.Id)
                {
                    await _authService.RefreshCurrentUserAsync();
                    SyncCurrentUserState();
                }

                UpdateStatistics();
                ApplyFilter();

                await AppShell.DisplayToastAsync(
                    $"Phân quyền thành công! Tài khoản {user.FullName} đã chuyển sang vai trò: {chosenRole.GetShortName()}");
            }
        }

        [RelayCommand]
        public async Task DeleteUser(User user)
        {
            if (user == null)
                return;

            if (!_authService.IsAdmin)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Quyền bị từ chối", "Chỉ Admin mới có quyền xóa người dùng!", "Đóng");
                }
                return;
            }

            if (_authService.CurrentUser?.Id == user.Id)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert(
                        "Cảnh báo",
                        "Bạn không thể tự xóa tài khoản Quản trị viên đang đăng nhập!",
                        "Đóng");
                }
                return;
            }

            if (Shell.Current == null)
                return;

            var confirm = await Shell.Current.DisplayAlert(
                "Xác nhận xóa tài khoản",
                $"Bạn có chắc chắn muốn xóa vĩnh viễn người dùng '{user.FullName}' (@{user.Username}) khỏi hệ thống?",
                "Xóa",
                "Hủy");

            if (!confirm)
                return;

            var success = await _userService.DeleteUserAsync(user.Id);
            if (success)
            {
                UsersList.Remove(user);
                UpdateStatistics();
                ApplyFilter();
                await AppShell.DisplayToastAsync($"Đã xóa tài khoản {user.FullName}");
            }
        }

        [RelayCommand]
        public async Task ToggleUserStatus(User user)
        {
            if (user == null)
                return;

            if (!_authService.IsAdmin)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Quyền bị từ chối", "Chỉ Admin mới có quyền khóa/mở tài khoản!", "Đóng");
                }
                return;
            }

            if (_authService.CurrentUser?.Id == user.Id)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Cảnh báo", "Không thể tự khóa tài khoản của chính mình!", "Đóng");
                }
                return;
            }

            var success = await _userService.ToggleUserStatusAsync(user.Id);
            if (success)
            {
                user.IsActive = !user.IsActive;
                UpdateStatistics();
                ApplyFilter();
                await AppShell.DisplayToastAsync(
                    user.IsActive ? $"Đã kích hoạt tài khoản {user.FullName}" : $"Đã khóa tài khoản {user.FullName}");
            }
        }

        [RelayCommand]
        public async Task NavigateToCreateUserPage()
        {
            if (!_authService.IsAdmin)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Quyền bị từ chối", "Chỉ Admin mới có quyền tạo người dùng mới!", "Đóng");
                }
                return;
            }

            await Shell.Current.GoToAsync(nameof(CampusActivitiesManager.Pages.CreateUserPage));
        }

        [RelayCommand]
        public async Task NavigateToEditRolePage(User user)
        {
            if (user == null)
                return;

            if (!_authService.IsAdmin)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Quyền bị từ chối", "Chỉ Admin mới có quyền truy cập trang phân quyền!", "Đóng");
                }
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(CampusActivitiesManager.Pages.EditUserRolePage)}?UserId={user.Id}");
        }

        [RelayCommand]
        public void ToggleMenu()
        {
            if (Shell.Current != null)
            {
                Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
            }
        }
    }

    public class UserManagementViewModel : UserManagementPageModel
    {
        public UserManagementViewModel(
            IUserService userService,
            IAuthenticationService authService,
            ModalErrorHandler errorHandler)
            : base(userService, authService, errorHandler)
        {
        }
    }
}
