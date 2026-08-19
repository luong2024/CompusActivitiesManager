using System.Collections.ObjectModel;
using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class UserManagementPageModel : ObservableObject
    {
        private readonly UserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly ModalErrorHandler _errorHandler;

        [ObservableProperty]
        private ObservableCollection<User> _users = [];

        [ObservableProperty]
        private ObservableCollection<User> _filteredUsers = [];

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _selectedRoleFilter = "Tất cả";

        [ObservableProperty]
        private List<string> _filterRoles = ["Tất cả", UserRoles.Admin, UserRoles.Manager, UserRoles.Student];

        [ObservableProperty]
        private List<string> _availableRoles = [UserRoles.Admin, UserRoles.Manager, UserRoles.Student];

        [ObservableProperty]
        private User? _selectedUser;

        [ObservableProperty]
        private string _selectedRole = UserRoles.Student;

        [ObservableProperty]
        private bool _isEditingRole;

        [ObservableProperty]
        private bool _isAddingUser;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private User? _currentUser;

        [ObservableProperty]
        private bool _isAdmin;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private int _adminCount;

        [ObservableProperty]
        private int _managerCount;

        [ObservableProperty]
        private int _studentCount;

        // Form fields for adding new user
        [ObservableProperty]
        private string _newUsername = string.Empty;

        [ObservableProperty]
        private string _newFullName = string.Empty;

        [ObservableProperty]
        private string _newEmail = string.Empty;

        [ObservableProperty]
        private string _newRole = UserRoles.Student;

        [ObservableProperty]
        private string _newPhoneNumber = string.Empty;

        [ObservableProperty]
        private string _newDepartment = string.Empty;

        public UserManagementPageModel(UserRepository userRepository, IAuthService authService, ModalErrorHandler errorHandler)
        {
            _userRepository = userRepository;
            _authService = authService;
            _errorHandler = errorHandler;

            _authService.CurrentUserChanged += OnCurrentUserChanged;
            UpdateAuthStatus();
        }

        private void OnCurrentUserChanged(object? sender, EventArgs e)
        {
            UpdateAuthStatus();
        }

        private void UpdateAuthStatus()
        {
            CurrentUser = _authService.CurrentUser;
            IsAdmin = _authService.IsAdmin;
        }

        [RelayCommand]
        private async Task Appearing()
        {
            await _authService.InitializeAsync();
            UpdateAuthStatus();
            await LoadData();
        }

        [RelayCommand]
        private async Task Refresh()
        {
            try
            {
                IsRefreshing = true;
                await LoadData();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        public async Task LoadData()
        {
            try
            {
                IsBusy = true;
                var userList = await _userRepository.ListAsync();
                Users = new ObservableCollection<User>(userList);
                UpdateCounts();
                ApplyFilter();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void UpdateCounts()
        {
            TotalCount = Users.Count;
            AdminCount = Users.Count(u => u.Role == UserRoles.Admin);
            ManagerCount = Users.Count(u => u.Role == UserRoles.Manager);
            StudentCount = Users.Count(u => u.Role == UserRoles.Student);
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        partial void OnSelectedRoleFilterChanged(string value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var query = Users.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SelectedRoleFilter) && SelectedRoleFilter != "Tất cả")
            {
                query = query.Where(u => u.Role.Equals(SelectedRoleFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string search = SearchText.Trim().ToLowerInvariant();
                query = query.Where(u =>
                    (u.FullName?.ToLowerInvariant().Contains(search) ?? false) ||
                    (u.Username?.ToLowerInvariant().Contains(search) ?? false) ||
                    (u.Email?.ToLowerInvariant().Contains(search) ?? false) ||
                    (u.Department?.ToLowerInvariant().Contains(search) ?? false));
            }

            FilteredUsers = new ObservableCollection<User>(query.ToList());
        }

        [RelayCommand]
        private void SelectFilterRole(string role)
        {
            SelectedRoleFilter = role;
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchText = string.Empty;
        }

        [RelayCommand]
        private void SelectRoleForEdit(string role)
        {
            SelectedRole = role;
        }

        [RelayCommand]
        private async Task SelectUserForRoleAssignment(User user)
        {
            if (user == null) return;

            if (!IsAdmin)
            {
                await AppShell.DisplaySnackbarAsync("Bạn không có quyền thực hiện. Chỉ tài khoản Admin mới có thể phân quyền!");
                return;
            }

            SelectedUser = user;
            SelectedRole = user.Role;
            IsEditingRole = true;
        }

        [RelayCommand]
        private void CancelRoleAssignment()
        {
            IsEditingRole = false;
            SelectedUser = null;
        }

        /// <summary>
        /// Acceptance Criteria: Hệ thống cho phép tài khoản Admin cập nhật vai trò/quyền hạn cho tài khoản khác và lưu thành công vào CSDL (Phân quyền thành công).
        /// </summary>
        [RelayCommand]
        private async Task UpdateRole()
        {
            if (SelectedUser == null)
            {
                await AppShell.DisplaySnackbarAsync("Vui lòng chọn tài khoản cần phân quyền!");
                return;
            }

            if (!IsAdmin)
            {
                await AppShell.DisplaySnackbarAsync("Quyền truy cập bị từ chối: Chỉ tài khoản Admin mới có quyền phân quyền!");
                IsEditingRole = false;
                return;
            }

            try
            {
                IsBusy = true;
                string oldRole = SelectedUser.Role;
                string newRole = SelectedRole;

                // 1. Lưu thay đổi trực tiếp vào CSDL SQLite
                bool isSuccess = await _userRepository.UpdateRoleAsync(SelectedUser.ID, newRole);

                if (isSuccess)
                {
                    SelectedUser.Role = newRole;

                    // Nếu Admin đang phân quyền lại chính tài khoản của mình
                    if (CurrentUser != null && CurrentUser.ID == SelectedUser.ID)
                    {
                        await _authService.RefreshCurrentUserAsync();
                        UpdateAuthStatus();
                    }

                    // Reload & Refresh danh sách
                    await LoadData();
                    IsEditingRole = false;

                    // 2. Hiển thị thông báo thành công (Acceptance Criteria)
                    string successMsg = $"Phân quyền thành công!\nTài khoản '{SelectedUser.FullName}' đã được cập nhật vai trò: {newRole}.";
                    await AppShell.DisplaySnackbarAsync(successMsg);
                    await AppShell.DisplayToastAsync("Phân quyền thành công!");
                }
                else
                {
                    await AppShell.DisplaySnackbarAsync("Cập nhật vai trò thất bại. Vui lòng thử lại!");
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
        private async Task OpenAddUserDialog()
        {
            if (!IsAdmin)
            {
                await AppShell.DisplaySnackbarAsync("Chỉ Admin mới có quyền thêm tài khoản mới!");
                return;
            }

            NewUsername = string.Empty;
            NewFullName = string.Empty;
            NewEmail = string.Empty;
            NewRole = UserRoles.Student;
            NewPhoneNumber = string.Empty;
            NewDepartment = string.Empty;
            IsAddingUser = true;
        }

        [RelayCommand]
        private void CancelAddUser()
        {
            IsAddingUser = false;
        }

        [RelayCommand]
        private async Task SaveNewUser()
        {
            if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(NewFullName))
            {
                await AppShell.DisplaySnackbarAsync("Vui lòng nhập tên đăng nhập và họ tên!");
                return;
            }

            try
            {
                IsBusy = true;
                var existing = await _userRepository.GetByUsernameAsync(NewUsername.Trim());
                if (existing != null)
                {
                    await AppShell.DisplaySnackbarAsync($"Tên đăng nhập '{NewUsername}' đã tồn tại!");
                    return;
                }

                var newUser = new User
                {
                    Username = NewUsername.Trim(),
                    FullName = NewFullName.Trim(),
                    Email = NewEmail.Trim(),
                    Role = NewRole,
                    PhoneNumber = NewPhoneNumber.Trim(),
                    Department = NewDepartment.Trim()
                };

                await _userRepository.SaveItemAsync(newUser);
                await LoadData();
                IsAddingUser = false;
                await AppShell.DisplayToastAsync("Thêm tài khoản thành công!");
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
        private async Task DeleteUser(User user)
        {
            if (user == null) return;

            if (!IsAdmin)
            {
                await AppShell.DisplaySnackbarAsync("Chỉ Admin mới có quyền xóa tài khoản!");
                return;
            }

            if (CurrentUser != null && CurrentUser.ID == user.ID)
            {
                await AppShell.DisplaySnackbarAsync("Không thể xóa tài khoản đang đăng nhập hiện tại!");
                return;
            }

            try
            {
                await _userRepository.DeleteItemAsync(user);
                await LoadData();
                await AppShell.DisplayToastAsync($"Đã xóa tài khoản {user.Username}!");
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
        }

        [RelayCommand]
        private async Task SwitchUser(User user)
        {
            if (user == null) return;
            await _authService.SwitchUserAsync(user);
            UpdateAuthStatus();
            ApplyFilter();
            await AppShell.DisplayToastAsync($"Đã chuyển sang tài khoản: {user.FullName} ({user.Role})");
        }
    }
}
