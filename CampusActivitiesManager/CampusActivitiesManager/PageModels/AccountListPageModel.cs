#nullable disable
using System.Collections.ObjectModel;
using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CampusActivitiesManager.PageModels
{
    public partial class AccountListPageModel : ObservableObject
    {
        private readonly IUserService<Account> _accountService;
        private readonly AccountRepository _accountRepository;
        private readonly AuthService _authService;
        private CancellationTokenSource _searchCts;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError = false;

        [ObservableProperty]
        private string _errorType = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Account> _accounts = [];

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private bool _hasSearchText;

        [ObservableProperty]
        private AccountStatus? _selectedStatusFilter = null;

        [ObservableProperty]
        private bool _isAllSelected = true;

        [ObservableProperty]
        private bool _isDangHocSelected = false;

        [ObservableProperty]
        private bool _isBaoLuuSelected = false;

        [ObservableProperty]
        private bool _isBiKhoaSelected = false;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private int _dangHocCount;

        [ObservableProperty]
        private int _baoLuuCount;

        [ObservableProperty]
        private int _biKhoaCount;

        [ObservableProperty]
        private int _filteredCount;

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 10;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private bool _isLoadingMore;

        [ObservableProperty]
        private bool _hasMoreItems = true;

        [ObservableProperty]
        private int _remainingItemsThreshold = 2;

        [ObservableProperty]
        private int _columnSpan = 1;

        [ObservableProperty]
        private bool _isFilterPopupVisible = false;

        [ObservableProperty]
        private List<string> _classOptions = ["Tất cả"];

        [ObservableProperty]
        private List<string> _academicYearOptions = ["Tất cả"];

        [ObservableProperty]
        private List<string> _roleOptions = ["Tất cả", "Sinh viên", "Lớp trưởng", "Giảng viên", "Ban chủ nhiệm", "Quản trị viên"];

        [ObservableProperty]
        private string _selectedClassFilter = "Tất cả";

        [ObservableProperty]
        private string _selectedAcademicYearFilter = "Tất cả";

        [ObservableProperty]
        private string _selectedRoleFilter = "Tất cả";

        [ObservableProperty]
        private int _activeFilterCount = 0;

        [ObservableProperty]
        private bool _hasActiveFilter = false;

        // Strongly-typed Chip Styles
        public Brush ChipAllBg => IsAllSelected ? new SolidColorBrush(Color.FromArgb("#512BD4")) : new SolidColorBrush(Colors.Transparent);
        public Color ChipAllText => IsAllSelected ? Colors.White : Color.FromArgb("#512BD4");
        public Color ChipAllBorder => IsAllSelected ? Color.FromArgb("#512BD4") : Color.FromArgb("#C8C8C8");

        public Brush ChipDangHocBg => IsDangHocSelected ? new SolidColorBrush(Color.FromArgb("#107C41")) : new SolidColorBrush(Colors.Transparent);
        public Color ChipDangHocText => IsDangHocSelected ? Colors.White : Color.FromArgb("#107C41");
        public Color ChipDangHocBorder => IsDangHocSelected ? Color.FromArgb("#107C41") : Color.FromArgb("#C8C8C8");

        public Brush ChipBaoLuuBg => IsBaoLuuSelected ? new SolidColorBrush(Color.FromArgb("#D83B01")) : new SolidColorBrush(Colors.Transparent);
        public Color ChipBaoLuuText => IsBaoLuuSelected ? Colors.White : Color.FromArgb("#D83B01");
        public Color ChipBaoLuuBorder => IsBaoLuuSelected ? Color.FromArgb("#D83B01") : Color.FromArgb("#C8C8C8");

        public Brush ChipBiKhoaBg => IsBiKhoaSelected ? new SolidColorBrush(Color.FromArgb("#A80000")) : new SolidColorBrush(Colors.Transparent);
        public Color ChipBiKhoaText => IsBiKhoaSelected ? Colors.White : Color.FromArgb("#A80000");
        public Color ChipBiKhoaBorder => IsBiKhoaSelected ? Color.FromArgb("#A80000") : Color.FromArgb("#C8C8C8");

        public AccountFilterCriteria FilterCriteria { get; } = new();

        public AccountListPageModel(IUserService<Account> accountService, AuthService authService, AccountRepository accountRepository)
        {
            _accountService = accountService;
            _authService = authService;
            _accountRepository = accountRepository;
        }

        [RelayCommand]
        public void ClearError()
        {
            ErrorMessage = string.Empty;
            HasError = false;
            ErrorType = string.Empty;
        }

        [RelayCommand]
        public async Task Appearing()
        {
            await LoadFilterOptionsAsync();
            await RefreshDataAsync();
        }

        private async Task LoadFilterOptionsAsync()
        {
            try
            {
                var classes = await _accountRepository.GetUniqueClassesAsync();
                var classList = new List<string> { "Tất cả" };
                classList.AddRange(classes);
                ClassOptions = classList;

                var years = await _accountRepository.GetUniqueAcademicYearsAsync();
                var yearList = new List<string> { "Tất cả" };
                yearList.AddRange(years);
                AcademicYearOptions = yearList;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading filter options: {ex.Message}");
            }
        }

        public async Task RefreshDataAsync()
        {
            CurrentPage = 1;
            HasMoreItems = true;
            RemainingItemsThreshold = 2;
            await LoadAccountsAsync(isNewSearch: true);
            await UpdateCountsAsync();
            NotifyChipStylesChanged();
        }

        private void NotifyChipStylesChanged()
        {
            OnPropertyChanged(nameof(ChipAllBg));
            OnPropertyChanged(nameof(ChipAllText));
            OnPropertyChanged(nameof(ChipAllBorder));

            OnPropertyChanged(nameof(ChipDangHocBg));
            OnPropertyChanged(nameof(ChipDangHocText));
            OnPropertyChanged(nameof(ChipDangHocBorder));

            OnPropertyChanged(nameof(ChipBaoLuuBg));
            OnPropertyChanged(nameof(ChipBaoLuuText));
            OnPropertyChanged(nameof(ChipBaoLuuBorder));

            OnPropertyChanged(nameof(ChipBiKhoaBg));
            OnPropertyChanged(nameof(ChipBiKhoaText));
            OnPropertyChanged(nameof(ChipBiKhoaBorder));
        }

        private async Task LoadAccountsAsync(bool isNewSearch = false)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                FilterCriteria.SearchText = SearchText;
                FilterCriteria.SelectedStatus = SelectedStatusFilter;
                FilterCriteria.SelectedClass = SelectedClassFilter == "Tất cả" ? string.Empty : SelectedClassFilter;
                FilterCriteria.SelectedAcademicYear = SelectedAcademicYearFilter == "Tất cả" ? string.Empty : SelectedAcademicYearFilter;
                FilterCriteria.SelectedRole = ParseRoleFilter(SelectedRoleFilter);

                List<Account> items;
                int total;
                int filtered;

                // Thử gọi API lấy danh sách tài khoản qua IUserService (GET /api/accounts)
                try
                {
                    var apiList = await _accountService.GetAllAsync();

                    var query = apiList.AsEnumerable();

                    if (!string.IsNullOrWhiteSpace(FilterCriteria.SearchText))
                    {
                        var st = FilterCriteria.SearchText.Trim();
                        query = query.Where(a =>
                            (!string.IsNullOrEmpty(a.FullName) && a.FullName.Contains(st, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(a.StudentCode) && a.StudentCode.Contains(st, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(a.Email) && a.Email.Contains(st, StringComparison.OrdinalIgnoreCase)));
                    }

                    if (FilterCriteria.SelectedStatus.HasValue)
                    {
                        query = query.Where(a => a.Status == FilterCriteria.SelectedStatus.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(FilterCriteria.SelectedClass) && FilterCriteria.SelectedClass != "Tất cả")
                    {
                        query = query.Where(a => a.ClassName == FilterCriteria.SelectedClass);
                    }

                    if (!string.IsNullOrWhiteSpace(FilterCriteria.SelectedAcademicYear) && FilterCriteria.SelectedAcademicYear != "Tất cả")
                    {
                        query = query.Where(a => a.AcademicYear == FilterCriteria.SelectedAcademicYear);
                    }

                    if (FilterCriteria.SelectedRole.HasValue)
                    {
                        query = query.Where(a => a.Role == FilterCriteria.SelectedRole.Value);
                    }

                    var filteredQueryList = query.ToList();
                    total = apiList.Count;
                    filtered = filteredQueryList.Count;

                    var offset = Math.Max(0, (CurrentPage - 1) * PageSize);
                    items = filteredQueryList.Skip(offset).Take(PageSize).ToList();

                    // Cập nhật thống kê trạng thái trực tiếp từ dữ liệu API
                    TotalCount = apiList.Count;
                    DangHocCount = apiList.Count(a => a.Status == AccountStatus.DangHoc);
                    BaoLuuCount = apiList.Count(a => a.Status == AccountStatus.BaoLuu);
                    BiKhoaCount = apiList.Count(a => a.Status == AccountStatus.BiKhoa);

                    ClearError();
                }
                catch (NetworkException netEx)
                {
                    ErrorMessage = netEx.Message;
                    HasError = true;
                    ErrorType = "Network";
                    System.Diagnostics.Debug.WriteLine($"Network error: {netEx.Message}");

                    var result = await _accountRepository.GetPagedAsync(CurrentPage, PageSize, FilterCriteria);
                    items = result.Items;
                    total = result.TotalCount;
                    filtered = result.FilteredCount;

                    var counts = await _accountRepository.GetStatusCountsAsync(SearchText);
                    TotalCount = counts.Total;
                    DangHocCount = counts.DangHocCount;
                    BaoLuuCount = counts.BaoLuuCount;
                    BiKhoaCount = counts.BiKhoaCount;
                }
                catch (ApiException apiEx)
                {
                    ErrorMessage = apiEx.Message;
                    HasError = true;
                    ErrorType = "API";
                    System.Diagnostics.Debug.WriteLine($"API error: {apiEx.Message}");

                    var result = await _accountRepository.GetPagedAsync(CurrentPage, PageSize, FilterCriteria);
                    items = result.Items;
                    total = result.TotalCount;
                    filtered = result.FilteredCount;

                    var counts = await _accountRepository.GetStatusCountsAsync(SearchText);
                    TotalCount = counts.Total;
                    DangHocCount = counts.DangHocCount;
                    BaoLuuCount = counts.BaoLuuCount;
                    BiKhoaCount = counts.BiKhoaCount;
                }
                catch (ValidationException valEx)
                {
                    ErrorMessage = valEx.Message;
                    HasError = true;
                    ErrorType = "Validation";

                    var result = await _accountRepository.GetPagedAsync(CurrentPage, PageSize, FilterCriteria);
                    items = result.Items;
                    total = result.TotalCount;
                    filtered = result.FilteredCount;

                    var counts = await _accountRepository.GetStatusCountsAsync(SearchText);
                    TotalCount = counts.Total;
                    DangHocCount = counts.DangHocCount;
                    BaoLuuCount = counts.BaoLuuCount;
                    BiKhoaCount = counts.BiKhoaCount;
                }

                TotalCount = total;
                FilteredCount = filtered;

                if (isNewSearch)
                {
                    Accounts.Clear();
                }

                foreach (var item in items)
                {
                    Accounts.Add(item);
                }

                HasMoreItems = Accounts.Count < FilteredCount;
                RemainingItemsThreshold = HasMoreItems ? 2 : -1;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi không xác định: {ex.Message}";
                HasError = true;
                ErrorType = "API";
                System.Diagnostics.Debug.WriteLine($"Error loading accounts: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
                IsLoadingMore = false;
            }
        }

        private async Task UpdateCountsAsync()
        {
            try
            {
                // Chỉ cập nhật từ SQLite nếu chưa có dữ liệu đếm hoặc đang ở chế độ offline
                if (TotalCount == 0)
                {
                    var (total, dangHoc, baoLuu, biKhoa) = await _accountRepository.GetStatusCountsAsync(SearchText);
                    TotalCount = total;
                    DangHocCount = dangHoc;
                    BaoLuuCount = baoLuu;
                    BiKhoaCount = biKhoa;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating counts: {ex.Message}");
            }
        }

        private AccountRole? ParseRoleFilter(string roleName)
        {
            return roleName switch
            {
                "Sinh viên" => AccountRole.SinhVien,
                "Lớp trưởng" => AccountRole.LopTruong,
                "Giảng viên" => AccountRole.GiangVien,
                "Ban chủ nhiệm" => AccountRole.BanChuNhiem,
                "Quản trị viên" => AccountRole.Admin,
                _ => null
            };
        }

        // AC 1.2: Debounced Search Mechanism
        partial void OnSearchTextChanged(string value)
        {
            HasSearchText = !string.IsNullOrWhiteSpace(value);

            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(350, token);
                    if (!token.IsCancellationRequested)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await RefreshDataAsync();
                        });
                    }
                }
                catch (TaskCanceledException)
                {
                }
            }, token);
        }

        // AC 1.2: Instant Clear Search
        [RelayCommand]
        private async Task ClearSearch()
        {
            SearchText = string.Empty;
            HasSearchText = false;
            await RefreshDataAsync();
        }

        // AC 1.3: Quick Status Chip Filtering
        [RelayCommand]
        private async Task FilterByStatus(string statusKey)
        {
            switch (statusKey)
            {
                case "DangHoc":
                    SelectedStatusFilter = AccountStatus.DangHoc;
                    IsAllSelected = false;
                    IsDangHocSelected = true;
                    IsBaoLuuSelected = false;
                    IsBiKhoaSelected = false;
                    break;
                case "BaoLuu":
                    SelectedStatusFilter = AccountStatus.BaoLuu;
                    IsAllSelected = false;
                    IsDangHocSelected = false;
                    IsBaoLuuSelected = true;
                    IsBiKhoaSelected = false;
                    break;
                case "BiKhoa":
                    SelectedStatusFilter = AccountStatus.BiKhoa;
                    IsAllSelected = false;
                    IsDangHocSelected = false;
                    IsBaoLuuSelected = false;
                    IsBiKhoaSelected = true;
                    break;
                default: // "All"
                    SelectedStatusFilter = null;
                    IsAllSelected = true;
                    IsDangHocSelected = false;
                    IsBaoLuuSelected = false;
                    IsBiKhoaSelected = false;
                    break;
            }

            NotifyChipStylesChanged();
            await RefreshDataAsync();
        }

        // AC 1.3: Advanced Filter Popup Controls
        [RelayCommand]
        private void OpenFilterPopup()
        {
            IsFilterPopupVisible = true;
        }

        [RelayCommand]
        private void CloseFilterPopup()
        {
            IsFilterPopupVisible = false;
        }

        [RelayCommand]
        private async Task ApplyAdvancedFilter()
        {
            IsFilterPopupVisible = false;
            UpdateActiveFilterCount();
            await RefreshDataAsync();
        }

        [RelayCommand]
        private async Task ResetAdvancedFilter()
        {
            SelectedClassFilter = "Tất cả";
            SelectedAcademicYearFilter = "Tất cả";
            SelectedRoleFilter = "Tất cả";
            UpdateActiveFilterCount();
            IsFilterPopupVisible = false;
            await RefreshDataAsync();
        }

        private void UpdateActiveFilterCount()
        {
            int count = 0;
            if (SelectedClassFilter != "Tất cả") count++;
            if (SelectedAcademicYearFilter != "Tất cả") count++;
            if (SelectedRoleFilter != "Tất cả") count++;

            ActiveFilterCount = count;
            HasActiveFilter = count > 0;
        }

        // AC 1.4: Pull-to-Refresh
        [RelayCommand]
        private async Task Refresh()
        {
            IsRefreshing = true;
            await RefreshDataAsync();
        }

        // AC 1.4: Infinite Scroll / Load More
        [RelayCommand]
        private async Task LoadMore()
        {
            if (IsLoadingMore || IsBusy || !HasMoreItems)
                return;

            IsLoadingMore = true;
            CurrentPage++;
            await LoadAccountsAsync(isNewSearch: false);
        }

        // AC 1.1: Action Menu for Account
        [RelayCommand]
        private async Task AccountAction(Account account)
        {
            if (account == null)
                return;

            string statusAction = account.Status switch
            {
                AccountStatus.DangHoc => "Chuyển sang Bảo lưu",
                AccountStatus.BaoLuu => "Mở lại Đang học",
                AccountStatus.BiKhoa => "Mở khóa tài khoản",
                _ => "Đổi trạng thái"
            };

            string lockAction = account.Status == AccountStatus.BiKhoa ? "Mở khóa tài khoản" : "Khóa tài khoản";

            var choice = await Shell.Current.DisplayActionSheet(
                $"Thao tác: {account.FullName} ({account.StudentCode})",
                "Hủy",
                "Xóa tài khoản",
                "Xem thông tin chi tiết",
                statusAction,
                lockAction,
                "Phân lại quyền hạn"
            );

            if (choice == "Xem thông tin chi tiết")
            {
                Account detail = account;
                try
                {
                    var apiDetail = await _accountService.GetByIdAsync(account.ID);
                    if (apiDetail != null)
                    {
                        detail = apiDetail;
                    }
                }
                catch
                {
                    // Fallback to current object if API fails
                }

                await Shell.Current.DisplayAlert(
                    "Thông tin tài khoản",
                    $"Họ và tên: {detail.FullName}\n" +
                    $"Mã số: {detail.StudentCode}\n" +
                    $"Email: {detail.Email}\n" +
                    $"Số điện thoại: {(string.IsNullOrEmpty(detail.PhoneNumber) ? "Chưa cập nhật" : detail.PhoneNumber)}\n" +
                    $"Lớp: {detail.ClassName}\n" +
                    $"Khóa: {detail.AcademicYear}\n" +
                    $"Quyền: {detail.RoleName}\n" +
                    $"Trạng thái: {detail.StatusName}\n" +
                    $"Ngày tạo: {detail.CreatedAt:dd/MM/yyyy HH:mm}",
                    "Đóng"
                );
            }
            else if (choice == statusAction)
            {
                AccountStatus newStatus = account.Status switch
                {
                    AccountStatus.DangHoc => AccountStatus.BaoLuu,
                    AccountStatus.BaoLuu => AccountStatus.DangHoc,
                    AccountStatus.BiKhoa => AccountStatus.DangHoc,
                    _ => AccountStatus.DangHoc
                };

                account.Status = newStatus;
                try
                {
                    ClearError();
                    await _accountService.UpdateAsync(account.ID, account);
                }
                catch (NetworkException netEx)
                {
                    ErrorMessage = netEx.Message;
                    HasError = true;
                    ErrorType = "Network";
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    HasError = true;
                    ErrorType = "API";
                }

                await _accountRepository.UpdateStatusAsync(account.ID, newStatus);
                await RefreshDataAsync();
            }
            else if (choice == lockAction)
            {
                if (account.Status != AccountStatus.BiKhoa && !_authService.CanLockAccount(account))
                {
                    await Shell.Current.DisplayAlert("Cảnh báo", "Bạn không thể tự khóa tài khoản Quản trị viên của chính mình!", "Đã hiểu");
                    return;
                }

                AccountStatus newStatus = account.Status == AccountStatus.BiKhoa ? AccountStatus.DangHoc : AccountStatus.BiKhoa;
                account.Status = newStatus;

                try
                {
                    ClearError();
                    await _accountService.UpdateAsync(account.ID, account);
                }
                catch (NetworkException netEx)
                {
                    ErrorMessage = netEx.Message;
                    HasError = true;
                    ErrorType = "Network";
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    HasError = true;
                    ErrorType = "API";
                }

                await _accountRepository.UpdateStatusAsync(account.ID, newStatus);
                await RefreshDataAsync();
            }
            else if (choice == "Phân lại quyền hạn")
            {
                var roleChoice = await Shell.Current.DisplayActionSheet(
                    "Chọn quyền hạn mới",
                    "Hủy",
                    null,
                    "Sinh viên",
                    "Lớp trưởng",
                    "Giảng viên",
                    "Ban chủ nhiệm",
                    "Quản trị viên"
                );

                if (!string.IsNullOrEmpty(roleChoice) && roleChoice != "Hủy")
                {
                    var newRole = ParseRoleFilter(roleChoice);
                    if (newRole.HasValue)
                    {
                        account.Role = newRole.Value;
                        try
                        {
                            ClearError();
                            await _accountService.UpdateAsync(account.ID, account);
                        }
                        catch (NetworkException netEx)
                        {
                            ErrorMessage = netEx.Message;
                            HasError = true;
                            ErrorType = "Network";
                        }
                        catch (Exception ex)
                        {
                            ErrorMessage = ex.Message;
                            HasError = true;
                            ErrorType = "API";
                        }

                        await _accountRepository.SaveItemAsync(account);
                        await RefreshDataAsync();
                    }
                }
            }
            else if (choice == "Xóa tài khoản")
            {
                bool confirm = await Shell.Current.DisplayAlert(
                    "Xác nhận xóa",
                    $"Bạn có chắc chắn muốn xóa tài khoản {account.FullName} ({account.StudentCode})?",
                    "Xóa",
                    "Hủy"
                );

                if (confirm)
                {
                    try
                    {
                        ClearError();
                        await _accountService.DeleteAsync(account.ID);
                    }
                    catch (NetworkException netEx)
                    {
                        ErrorMessage = netEx.Message;
                        HasError = true;
                        ErrorType = "Network";
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = ex.Message;
                        HasError = true;
                        ErrorType = "API";
                    }

                    await _accountRepository.DeleteItemAsync(account);
                    Accounts.Remove(account);
                    await UpdateCountsAsync();
                }
            }
        }

        // Add Account Prompt (AC 1.1 / CRUD API)
        [RelayCommand]
        private async Task AddAccount()
        {
            string fullName = await Shell.Current.DisplayPromptAsync("Thêm tài khoản mới", "Nhập họ và tên:", "Tiếp tục", "Hủy");
            if (string.IsNullOrWhiteSpace(fullName))
            {
                ErrorMessage = "Lỗi xác thực (Validation Error): Họ và tên không được để trống!";
                HasError = true;
                ErrorType = "Validation";
                return;
            }

            string studentCode = await Shell.Current.DisplayPromptAsync("Thêm tài khoản mới", "Nhập Mã sinh viên / Mã cán bộ:", "Tiếp tục", "Hủy");
            if (string.IsNullOrWhiteSpace(studentCode))
            {
                ErrorMessage = "Lỗi xác thực (Validation Error): Mã sinh viên/cán bộ không được để trống!";
                HasError = true;
                ErrorType = "Validation";
                return;
            }

            string email = await Shell.Current.DisplayPromptAsync("Thêm tài khoản mới", "Nhập Email:", "Tiếp tục", "Hủy", initialValue: $"{studentCode.ToLower()}@campus.edu.vn");
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                ErrorMessage = "Lỗi xác thực (Validation Error): Địa chỉ Email không hợp lệ!";
                HasError = true;
                ErrorType = "Validation";
                return;
            }

            string className = await Shell.Current.DisplayPromptAsync("Thêm tài khoản mới", "Nhập Lớp:", "Hoàn tất", "Hủy", initialValue: "D20CNTT1");
            if (string.IsNullOrWhiteSpace(className))
                className = "D20CNTT1";

            var newAccount = new Account
            {
                FullName = fullName.Trim(),
                StudentCode = studentCode.Trim(),
                Email = email.Trim(),
                ClassName = className.Trim(),
                AcademicYear = "K20",
                Status = AccountStatus.DangHoc,
                Role = AccountRole.SinhVien,
                CreatedAt = DateTime.Now
            };

            IsBusy = true;
            try
            {
                ClearError();
                Account createdAccount;
                try
                {
                    createdAccount = await _accountService.CreateAsync(newAccount);
                }
                catch (NetworkException netEx)
                {
                    ErrorMessage = netEx.Message;
                    HasError = true;
                    ErrorType = "Network";
                    createdAccount = newAccount;
                }
                catch (ValidationException valEx)
                {
                    ErrorMessage = valEx.Message;
                    HasError = true;
                    ErrorType = "Validation";
                    await Shell.Current.DisplayAlert("Lỗi dữ liệu", valEx.Message, "OK");
                    return;
                }
                catch (ApiException apiEx)
                {
                    ErrorMessage = apiEx.Message;
                    HasError = true;
                    ErrorType = "API";
                    createdAccount = newAccount;
                }

                await _accountRepository.SaveItemAsync(createdAccount);
                await LoadFilterOptionsAsync();
                IsBusy = false;
                await RefreshDataAsync();
                await Shell.Current.DisplayAlert("Thành công", $"Đã tạo mới tài khoản cho {createdAccount.FullName}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // CRUD RelayCommands (001.md / nvu01.md: AddCommand, UpdateCommand, DeleteCommand)
        [RelayCommand]
        public async Task Add() => await AddAccount();

        [RelayCommand]
        public async Task Update(Account account)
        {
            if (account == null) return;
            await AccountAction(account);
        }

        [RelayCommand]
        public async Task Delete(Account account)
        {
            if (account == null) return;
            bool confirm = await Shell.Current.DisplayAlert(
                "Xác nhận xóa",
                $"Bạn có chắc chắn muốn xóa tài khoản {account.FullName} ({account.StudentCode})?",
                "Xóa",
                "Hủy"
            );

            if (confirm)
            {
                try
                {
                    ClearError();
                    await _accountService.DeleteAsync(account.ID);
                }
                catch (NetworkException netEx)
                {
                    ErrorMessage = netEx.Message;
                    HasError = true;
                    ErrorType = "Network";
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    HasError = true;
                    ErrorType = "API";
                }

                await _accountRepository.DeleteItemAsync(account);
                Accounts.Remove(account);
                await UpdateCountsAsync();
            }
        }

        // Logout Command
        [RelayCommand]
        private async Task Logout()
        {
            bool confirm = await Shell.Current.DisplayAlert("Đăng xuất", "Bạn có chắc chắn muốn đăng xuất khỏi tài khoản Quản trị viên?", "Đăng xuất", "Hủy");
            if (confirm)
            {
                await _authService.LogoutAsync();
                await Shell.Current.GoToAsync("//login");
            }
        }

        public void UpdateResponsiveLayout(double width)
        {
            if (width < 600)
            {
                ColumnSpan = 1; // Mobile
            }
            else if (width < 1000)
            {
                ColumnSpan = 2; // Tablet
            }
            else
            {
                ColumnSpan = 3; // Desktop
            }
        }
    }
}
