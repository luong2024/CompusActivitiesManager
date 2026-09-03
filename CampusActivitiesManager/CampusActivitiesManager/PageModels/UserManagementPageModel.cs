using System.Collections.ObjectModel;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    /// <summary>
    /// ViewModel quản lý người dùng và thực thi nghiệp vụ phân quyền truy cập (RBAC).
    /// </summary>
    public partial class UserManagementPageModel : BaseViewModel
    {
        private readonly IUserService<User> _userService;
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

        public UserManagementPageModel(
            IUserService<User> userService,
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

        /// <summary>
        /// Mở Modal phân quyền cho tài khoản được chọn (kèm Authorization Guard kiểm tra quyền Admin).
        /// </summary>
        [RelayCommand]
        public async Task SelectUserForRoleAssignment(User user)
        {
            if (user == null)
                return;

            // Authorization Guard: Nếu không phải Admin, chặn hành động và hiển thị cảnh báo
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

        /// <summary>
        /// Lưu vai trò phân quyền mới vào SQLite Database (Trọng tâm AC 05.3.1).
        /// </summary>
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

                    // Nếu Admin tự phân quyền lại chính mình, đồng bộ phiên làm việc tức thì
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

        /// <summary>
        /// Menu tác vụ nhanh DisplayActionSheet cho phép đổi quyền trực tiếp từ bảng chọn.
        /// </summary>
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
                "Manager (Quản lý hoạt động)",
                "User (Sinh viên / Thành viên)",
                "Guest (Khách vãng lai)");

            if (string.IsNullOrEmpty(action) || action == "Hủy")
                return;

            Role chosenRole = action switch
            {
                "Admin (Quản trị viên)" => Role.Admin,
                "Manager (Quản lý hoạt động)" => Role.Manager,
                "User (Sinh viên / Thành viên)" => Role.User,
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

        /// <summary>
        /// Xóa người dùng (Ngăn chặn Admin tự xóa chính mình qua canExecute / validation).
        /// </summary>
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

            // Ngăn chặn Admin tự xóa chính mình
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

        /// <summary>
        /// Khóa hoặc Mở khóa tài khoản người dùng.
        /// </summary>
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
                await AppShell.DisplayToastAsync(
                    user.IsActive ? $"Đã kích hoạt tài khoản {user.FullName}" : $"Đã khóa tài khoản {user.FullName}");
            }
        }

        /// <summary>
        /// Điều hướng sang trang chỉnh sửa phân quyền EditUserRolePage qua Shell và QueryProperty.
        /// </summary>
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

            await Shell.Current.GoToAsync($"editrole?UserId={user.Id}");
        }
    }

    /// <summary>
    /// Alias UserManagementViewModel tương thích với định danh trong Task.md
    /// </summary>
    public class UserManagementViewModel : UserManagementPageModel
    {
        public UserManagementViewModel(
            IUserService<User> userService,
            IAuthenticationService authService,
            ModalErrorHandler errorHandler)
            : base(userService, authService, errorHandler)
        {
        }
    }
}
