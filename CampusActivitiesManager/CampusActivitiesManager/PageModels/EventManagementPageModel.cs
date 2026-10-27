#nullable disable
using System.Collections.ObjectModel;
using CampusActivitiesManager.BusinessLogic.Services;
using CampusActivitiesManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class EventManagementPageModel : ObservableObject
    {
        private readonly IEventService _eventService;
        private readonly IAuthenticationService _authService;
        private readonly ModalErrorHandler _errorHandler;

        [ObservableProperty]
        private List<CampusEvent> _events = [];

        [ObservableProperty]
        private List<CampusEvent> _filteredEvents = [];

        [ObservableProperty]
        private List<EventCategoryDto> _categories = [];

        [ObservableProperty]
        private List<string> _categoryNames = [];

        [ObservableProperty]
        private string _selectedCategoryFilter = "Tất cả";

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private bool _isAdmin;

        [ObservableProperty]
        private bool _isManagerOrAdmin = true;

        [ObservableProperty]
        private string _currentUserRoleName = "Quản trị viên";

        // Thống kê Metrics
        [ObservableProperty]
        private int _totalEventsCount;

        [ObservableProperty]
        private int _academicCount;

        [ObservableProperty]
        private int _sportsCount;

        [ObservableProperty]
        private int _volunteerCount;

        [ObservableProperty]
        private int _pendingCount;

        [ObservableProperty]
        private int _approvedCount;

        [ObservableProperty]
        private int _participatingCount;

        // Modal Tạo / Chỉnh sửa
        [ObservableProperty]
        private bool _isModalVisible;

        [ObservableProperty]
        private bool _isEditing;

        [ObservableProperty]
        private string _modalTitle = "Tạo Sự Kiện Mới";

        [ObservableProperty]
        private int _editingId;

        [ObservableProperty]
        private string _formName = string.Empty;

        [ObservableProperty]
        private string _formDescription = string.Empty;

        [ObservableProperty]
        private string _formLocation = "Hội trường A2";

        [ObservableProperty]
        private string _formStartDate = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");

        [ObservableProperty]
        private string _formEndDate = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");

        [ObservableProperty]
        private string _formStartTime = "08:00";

        [ObservableProperty]
        private string _formEndTime = "11:30";

        [ObservableProperty]
        private int _formMaxParticipants = 150;

        [ObservableProperty]
        private string _formStatus = "Sắp diễn ra";

        [ObservableProperty]
        private string _formIcon = "📅";

        [ObservableProperty]
        private string _formImageUrl = "https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&q=80";

        [ObservableProperty]
        private string _formVideoUrl = string.Empty;

        [ObservableProperty]
        private int _formPointsReward = 10;

        [ObservableProperty]
        private int _formPointsPenalty = 5;

        [ObservableProperty]
        private string _formScope = "Public";

        [ObservableProperty]
        private string _formRegistrationType = "Auto";

        [ObservableProperty]
        private int _selectedCategoryIndex = 0;

        [ObservableProperty]
        private bool _isSaving;

        [ObservableProperty]
        private bool _canCreateEvent;

        public EventManagementPageModel(
            IEventService eventService,
            IAuthenticationService authService,
            ModalErrorHandler errorHandler)
        {
            _eventService = eventService;
            _authService = authService;
            _errorHandler = errorHandler;

            UpdateRoleState();
        }

        private void UpdateRoleState()
        {
            if (_authService?.CurrentUser != null)
            {
                var user = _authService.CurrentUser;
                IsAdmin = user.Role == Role.Admin;
                IsManagerOrAdmin = user.Role == Role.Admin || user.Role == Role.Manager;
                CanCreateEvent = IsAdmin || user.Role == Role.Manager || user.Role == Role.Secretary || user.IsSecretary;
                CurrentUserRoleName = user.Role switch
                {
                    Role.Admin => "Quản trị viên (Admin)",
                    Role.Manager => "Ban tổ chức (Manager)",
                    Role.Lecturer => "Giảng viên (Lecturer)",
                    Role.Secretary => "Bí thư chi đoàn",
                    _ => "Sinh viên (Student)"
                };
            }
            else
            {
                IsAdmin = false;
                IsManagerOrAdmin = false;
                CanCreateEvent = false;
                CurrentUserRoleName = "Khách (Chưa đăng nhập)";
            }
        }

        [RelayCommand]
        private async Task Appearing()
        {
            UpdateRoleState();
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

                // Load categories
                Categories = await _eventService.GetCategoriesAsync();
                CategoryNames = Categories.Select(c => c.Title).ToList();

                var currentUser = _authService?.CurrentUser;
                bool isAdmin = currentUser?.Role == Role.Admin;
                bool isManager = currentUser?.Role == Role.Manager;

                if (isAdmin)
                {
                    Events = await _eventService.GetEventsAsync(includePending: true);
                }
                else if (isManager)
                {
                    // Quản lý sự kiện: xem các sự kiện đã duyệt + sự kiện do chính mình tạo (kể cả pending của mình)
                    var all = await _eventService.GetEventsAsync(includePending: true);
                    Events = all.Where(e =>
                        (e.IsApproved && e.ApprovalStatus == "Approved") ||
                        string.Equals(e.CreatedByUsername, currentUser?.Username, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(e.CreatedByName, currentUser?.FullName, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }
                else
                {
                    // Học sinh / Sinh viên thường: CHỈ XEM CÁC HOẠT ĐỘNG ĐÃ ĐƯỢC DUYỆT!
                    Events = await _eventService.GetEventsForUserAsync(currentUser);
                }

                // Cập nhật quyền quản lý và duyệt cho từng sự kiện
                if (Events != null)
                {
                    foreach (var evt in Events)
                    {
                        evt.CanAdminApprove = isAdmin && evt.IsPendingApproval;
                        evt.CanUserManage = isAdmin ||
                            (!string.IsNullOrEmpty(currentUser?.Username) && string.Equals(evt.CreatedByUsername, currentUser.Username, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(currentUser?.FullName) && string.Equals(evt.CreatedByName, currentUser.FullName, StringComparison.OrdinalIgnoreCase));
                    }
                }

                UpdateMetrics();
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

        private void UpdateMetrics()
        {
            var currentUser = _authService?.CurrentUser;
            var activeEvents = Events.Where(e => e.ApprovalStatus != "Rejected" && e.Status != "Đã từ chối").ToList();
            TotalEventsCount = activeEvents.Count;
            AcademicCount = activeEvents.Count(e => e.CategoryTitle == "Học thuật");
            SportsCount = activeEvents.Count(e => e.CategoryTitle == "Thể thao");
            VolunteerCount = activeEvents.Count(e => e.CategoryTitle == "Tình nguyện");
            PendingCount = activeEvents.Count(e => e.ApprovalStatus == "Pending");
            ApprovedCount = activeEvents.Count(e => e.IsApproved && e.ApprovalStatus == "Approved");
            ParticipatingCount = activeEvents.Count(e =>
                e.IsRegistered ||
                (e.Registrations != null && e.Registrations.Any(r =>
                    (string.Equals(r.StudentCode, currentUser?.Username, StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(r.UserId, currentUser?.Id, StringComparison.OrdinalIgnoreCase)) &&
                    r.Status != "Cancelled")));
        }

        [RelayCommand]
        private void FilterCategory(string category)
        {
            SelectedCategoryFilter = category;
            ApplyFilter();
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        [RelayCommand]
        private void PerformSearch(string query)
        {
            SearchText = query;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var currentUser = _authService?.CurrentUser;
            bool isAdmin = currentUser?.Role == Role.Admin;

            // Thiết lập quyền duyệt và quản lý cho từng sự kiện
            if (Events != null)
            {
                foreach (var evt in Events)
                {
                    evt.CanAdminApprove = isAdmin && evt.IsPendingApproval;
                    evt.CanUserManage = isAdmin ||
                        (!string.IsNullOrEmpty(currentUser?.Username) && string.Equals(evt.CreatedByUsername, currentUser.Username, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(currentUser?.FullName) && string.Equals(evt.CreatedByName, currentUser.FullName, StringComparison.OrdinalIgnoreCase));
                }
            }

            IEnumerable<CampusEvent> query = (Events ?? []).Where(e => e.ApprovalStatus != "Rejected" && e.Status != "Đã từ chối");

            if (SelectedCategoryFilter == "Đang tham gia")
            {
                query = query.Where(e =>
                    e.IsRegistered ||
                    (e.Registrations != null && e.Registrations.Any(r =>
                        (string.Equals(r.StudentCode, currentUser?.Username, StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(r.UserId, currentUser?.Id, StringComparison.OrdinalIgnoreCase)) &&
                        r.Status != "Cancelled")));
            }
            else if (SelectedCategoryFilter == "Chờ duyệt")
            {
                query = query.Where(e => e.ApprovalStatus == "Pending");
            }
            else if (SelectedCategoryFilter == "Đã duyệt")
            {
                query = query.Where(e => e.IsApproved && e.ApprovalStatus == "Approved");
            }
            else if (!string.IsNullOrWhiteSpace(SelectedCategoryFilter) && SelectedCategoryFilter != "Tất cả")
            {
                query = query.Where(e => string.Equals(e.CategoryTitle, SelectedCategoryFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var lower = SearchText.Trim();
                query = query.Where(e =>
                    (!string.IsNullOrEmpty(e.Name) && e.Name.Contains(lower, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(e.Description) && e.Description.Contains(lower, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(e.Location) && e.Location.Contains(lower, StringComparison.OrdinalIgnoreCase)));
            }

            FilteredEvents = query.OrderBy(e => e.ID).ToList();
        }

        [RelayCommand]
        private void OpenCreateModal()
        {
            if (!CanCreateEvent)
            {
                Shell.Current?.DisplayAlert("Phân quyền ⚠️", "Chỉ Ban tổ chức sự kiện hoặc Quản trị viên mới có thể tạo sự kiện mới.", "Đã hiểu");
                return;
            }

            IsEditing = false;
            ModalTitle = "Tạo Sự Kiện Mới";
            EditingId = 0;
            FormName = string.Empty;
            FormDescription = string.Empty;
            FormLocation = "Hội trường A2";
            FormStartDate = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
            FormEndDate = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
            FormStartTime = "08:00";
            FormEndTime = "11:30";
            FormMaxParticipants = 150;
            FormStatus = "Sắp diễn ra";
            FormIcon = "📅";
            FormImageUrl = "https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&q=80";
            FormVideoUrl = string.Empty;
            FormPointsReward = 10;
            FormPointsPenalty = 5;
            FormScope = "Public";
            FormRegistrationType = "Auto";
            SelectedCategoryIndex = 0;

            IsModalVisible = true;
        }

        [RelayCommand]
        private async Task OpenEditModal(CampusEvent evt)
        {
            if (evt == null) return;

            if (!IsAdmin && !evt.CanUserManage)
            {
                await Shell.Current.DisplayAlert("Phân quyền ⚠️", "Bạn chỉ có quyền chỉnh sửa hoạt động do chính bạn tạo ra!", "Đã hiểu");
                return;
            }

            IsEditing = true;
            ModalTitle = "Chỉnh Sửa Sự Kiện";
            EditingId = evt.ID;
            FormName = evt.Name;
            FormDescription = evt.Description;
            FormLocation = evt.Location;
            FormStartDate = evt.StartDate;
            FormEndDate = evt.EndDate;
            FormStartTime = string.IsNullOrWhiteSpace(evt.StartTime) ? "08:00" : evt.StartTime;
            FormEndTime = string.IsNullOrWhiteSpace(evt.EndTime) ? "11:30" : evt.EndTime;
            FormMaxParticipants = evt.MaxParticipants;
            FormStatus = evt.Status;
            FormIcon = string.IsNullOrWhiteSpace(evt.Icon) ? "📅" : evt.Icon;
            FormImageUrl = evt.ImageUrl;
            FormVideoUrl = evt.VideoUrl;
            FormPointsReward = evt.PointsReward > 0 ? evt.PointsReward : 10;
            FormPointsPenalty = evt.PointsPenalty >= 0 ? evt.PointsPenalty : 5;
            FormScope = string.IsNullOrWhiteSpace(evt.Scope) ? "Public" : evt.Scope;
            FormRegistrationType = string.IsNullOrWhiteSpace(evt.RegistrationType) ? "Auto" : evt.RegistrationType;

            int catIdx = Categories.FindIndex(c => c.ID == evt.CategoryID || c.Title == evt.CategoryTitle);
            SelectedCategoryIndex = catIdx >= 0 ? catIdx : 0;

            IsModalVisible = true;
        }

        [RelayCommand]
        private void CloseModal()
        {
            IsModalVisible = false;
        }

        [RelayCommand]
        private void QuickPickIcon(string icon)
        {
            if (!string.IsNullOrEmpty(icon))
            {
                FormIcon = icon;
            }
        }

        [RelayCommand]
        private async Task PickCoverPhoto()
        {
            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo != null)
                {
                    FormImageUrl = photo.FullPath;
                    await AppShell.DisplayToastAsync("Đã chọn ảnh đại diện hoạt động! 🖼️");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
        }

        [RelayCommand]
        private async Task PickVideo()
        {
            try
            {
                var video = await MediaPicker.Default.PickVideoAsync();
                if (video != null)
                {
                    FormVideoUrl = video.FullPath;
                    await AppShell.DisplayToastAsync("Đã chọn video cho hoạt động! 🎬");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
        }

        [RelayCommand]
        private async Task SaveEvent()
        {
            if (string.IsNullOrWhiteSpace(FormName))
            {
                await Shell.Current.DisplayAlert("Lỗi nhập liệu", "Vui lòng nhập tên sự kiện.", "Đóng");
                return;
            }

            if (string.IsNullOrWhiteSpace(FormDescription))
            {
                await Shell.Current.DisplayAlert("Lỗi nhập liệu", "Vui lòng nhập mô tả sự kiện.", "Đóng");
                return;
            }

            try
            {
                IsSaving = true;

                int categoryId = 1;
                EventCategoryDto selectedCat = null;
                if (SelectedCategoryIndex >= 0 && SelectedCategoryIndex < Categories.Count)
                {
                    selectedCat = Categories[SelectedCategoryIndex];
                    categoryId = selectedCat.ID;
                }

                bool isUserAdmin = _authService?.CurrentUser?.Role == Role.Admin;
                bool isApproved = isUserAdmin;
                string approvalStatus = isUserAdmin ? "Approved" : "Pending";
                string createdRole = isUserAdmin ? "Admin" : (_authService?.CurrentUser?.Role.ToString() ?? "Manager");
                string createdName = _authService?.CurrentUser?.FullName ?? (isUserAdmin ? "Admin" : "Ban tổ chức");
                string createdUsername = _authService?.CurrentUser?.Username ?? (isUserAdmin ? "admin" : "manager");

                var evtModel = new CampusEvent
                {
                    ID = EditingId,
                    Name = FormName.Trim(),
                    Description = FormDescription.Trim(),
                    Icon = string.IsNullOrWhiteSpace(FormIcon) ? "📅" : FormIcon.Trim(),
                    CategoryID = categoryId,
                    Category = selectedCat,
                    Location = string.IsNullOrWhiteSpace(FormLocation) ? "Hội trường A2" : FormLocation.Trim(),
                    StartDate = FormStartDate,
                    EndDate = FormEndDate,
                    StartTime = string.IsNullOrWhiteSpace(FormStartTime) ? "08:00" : FormStartTime.Trim(),
                    EndTime = string.IsNullOrWhiteSpace(FormEndTime) ? "11:30" : FormEndTime.Trim(),
                    MaxParticipants = FormMaxParticipants > 0 ? FormMaxParticipants : 100,
                    PointsReward = FormPointsReward > 0 ? FormPointsReward : 10,
                    PointsPenalty = FormPointsPenalty >= 0 ? FormPointsPenalty : 5,
                    Scope = string.IsNullOrWhiteSpace(FormScope) ? "Public" : FormScope,
                    RegistrationType = string.IsNullOrWhiteSpace(FormRegistrationType) ? "Auto" : FormRegistrationType,
                    Status = FormStatus,
                    ImageUrl = FormImageUrl?.Trim() ?? "",
                    VideoUrl = FormVideoUrl?.Trim() ?? "",
                    IsApproved = IsEditing ? (Events.FirstOrDefault(e => e.ID == EditingId)?.IsApproved ?? isApproved) : isApproved,
                    ApprovalStatus = IsEditing ? (Events.FirstOrDefault(e => e.ID == EditingId)?.ApprovalStatus ?? approvalStatus) : approvalStatus,
                    CreatedByRole = createdRole,
                    CreatedByName = createdName,
                    CreatedByUsername = createdUsername
                };

                bool success;
                if (IsEditing)
                {
                    success = await _eventService.UpdateEventAsync(evtModel);
                }
                else
                {
                    success = await _eventService.CreateEventAsync(evtModel);
                }

                if (success)
                {
                    IsModalVisible = false;
                    await LoadData();

                    if (!isUserAdmin && !IsEditing)
                    {
                        await Shell.Current.DisplayAlert(
                            "Đã gửi yêu cầu tạo hoạt động 📝",
                            "Hoạt động đã được gửi lên hệ thống và đang chờ Quản trị viên (Admin) phê duyệt trước khi xuất hiện chính thức trên Cổng sinh viên.",
                            "Đã hiểu");
                    }
                    else
                    {
                        await AppShell.DisplayToastAsync(IsEditing ? "Cập nhật sự kiện thành công! 🎉" : "Tạo sự kiện mới thành công! 🎉");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Thông báo", "Không thể lưu sự kiện lên Firebase. Vui lòng kiểm tra kết nối mạng.", "Đóng");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            finally
            {
                IsSaving = false;
            }
        }

        [RelayCommand]
        private async Task ApproveEvent(CampusEvent evt)
        {
            if (evt == null) return;

            if (!IsAdmin)
            {
                await Shell.Current.DisplayAlert("Phân quyền 🛡️", "Chỉ Quản trị viên (Admin) mới có quyền duyệt hoạt động.", "Đóng");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert(
                "Phê duyệt hoạt động 🛡️",
                $"Bạn có chắc chắn muốn duyệt hoạt động \"{evt.Name}\" để xuất hiện trên toàn hệ thống?",
                "Duyệt ngay",
                "Hủy");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                string adminName = _authService?.CurrentUser?.FullName ?? "Admin";
                bool ok = await _eventService.ApproveEventAsync(evt.ID, true, adminName);
                if (ok)
                {
                    evt.IsApproved = true;
                    evt.ApprovalStatus = "Approved";
                    evt.CanAdminApprove = false;
                    await LoadData();
                    await AppShell.DisplayToastAsync("Đã phê duyệt hoạt động thành công! ✅");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Lỗi", "Không thể cập nhật trạng thái duyệt trên máy chủ.", "Đóng");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task RejectEvent(CampusEvent evt)
        {
            if (evt == null) return;

            if (!IsAdmin)
            {
                await Shell.Current.DisplayAlert("Phân quyền 🛡️", "Chỉ Quản trị viên (Admin) mới có quyền từ chối hoạt động.", "Đóng");
                return;
            }

            string reason = await Shell.Current.DisplayPromptAsync(
                "Từ chối & Xóa hoạt động 🗑️",
                "Nhập lý do từ chối (hoạt động này sẽ bị XÓA vĩnh viễn):",
                "Từ chối & Xóa",
                "Hủy",
                "Chưa đáp ứng tiêu chí tổ chức");

            if (reason == null) return;

            try
            {
                IsBusy = true;
                string adminName = _authService?.CurrentUser?.FullName ?? "Admin";
                bool ok = await _eventService.ApproveEventAsync(evt.ID, false, adminName, reason);
                if (ok)
                {
                    Events.Remove(evt);
                    FilteredEvents.Remove(evt);
                    await LoadData();
                    await AppShell.DisplayToastAsync("Đã từ chối và xóa hoạt động khỏi hệ thống 🗑️");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Lỗi", "Không thể cập nhật trạng thái trên máy chủ.", "Đóng");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteEvent(CampusEvent evt)
        {
            if (evt == null) return;

            if (!IsAdmin && !evt.CanUserManage)
            {
                await Shell.Current.DisplayAlert("Phân quyền ⚠️", "Bạn chỉ có quyền xóa hoạt động do chính bạn tạo ra!", "Đã hiểu");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert(
                "Xác nhận xóa sự kiện",
                $"Bạn có chắc chắn muốn xóa sự kiện \"{evt.Name}\"? Hành động này không thể hoàn tác.",
                "Xóa ngay",
                "Hủy");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                bool deleted = await _eventService.DeleteEventAsync(evt.ID);
                if (deleted)
                {
                    await LoadData();
                    await AppShell.DisplayToastAsync("Đã xóa sự kiện khỏi hệ thống!");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Lỗi", "Không thể xóa sự kiện trên Firebase.", "Đóng");
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
        private async Task ViewAttendanceReport(CampusEvent evt)
        {
            if (evt == null) return;

            if (!IsAdmin && !evt.CanUserManage)
            {
                await Shell.Current.DisplayAlert("Phân quyền ⚠️", "Bạn chỉ có quyền xem báo cáo điểm danh của hoạt động do chính bạn quản lý!", "Đã hiểu");
                return;
            }

            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync($"//attendancereport?eventId={evt.ID}");
            }
        }

        [RelayCommand]
        private async Task OpenVideo(CampusEvent evt)
        {
            if (evt == null || string.IsNullOrWhiteSpace(evt.VideoUrl))
            {
                await Shell.Current.DisplayAlert("Thông báo", "Hoạt động này chưa có video đính kèm.", "Đóng");
                return;
            }

            try
            {
                if (Uri.TryCreate(evt.VideoUrl, UriKind.Absolute, out var uri))
                {
                    await Launcher.Default.OpenAsync(uri);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Video hoạt động", $"Đường dẫn video:\n{evt.VideoUrl}", "Đóng");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
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
        private async Task OpenEventDetail(CampusEvent evt)
        {
            if (evt == null || Shell.Current == null) return;
            await Shell.Current.GoToAsync($"eventdetail?eventId={evt.ID}");
        }
    }
}
