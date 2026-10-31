using CampusActivitiesManager.Utilities;
#nullable disable
using System.Collections.ObjectModel;
using CampusActivitiesManager.BusinessLogic.Services;
using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        private readonly IEventService _eventService;
        private readonly IAuthenticationService _authService;
        private readonly IUserService _userService;
        private readonly ModalErrorHandler _errorHandler;
        private readonly IQrCodeService _qrCodeService;
        private readonly IAttendanceService _attendanceService;

        [ObservableProperty]
        private Account _currentStudent;

        [ObservableProperty]
        private string _studentGreeting = "Xin chào 👋";

        [ObservableProperty]
        private string _roleSubtitle = string.Empty;

        // Role Indicators
        [ObservableProperty]
        private bool _isAdmin;

        [ObservableProperty]
        private bool _isManager;

        [ObservableProperty]
        private bool _isLecturer;

        [ObservableProperty]
        private bool _isSecretary;

        [ObservableProperty]
        private bool _isStudent;

        [ObservableProperty]
        private bool _isStudentOrSecretary;

        [ObservableProperty]
        private bool _isAdminOrManager;

        // Student DRL Progress
        [ObservableProperty]
        private double _trainingPointsProgress = 0.0;

        [ObservableProperty]
        private double _semesterProgress = 0.0;

        [ObservableProperty]
        private double _yearProgress = 0.0;

        [ObservableProperty]
        private string _semesterGoalDisplay = "0/50 ĐRL";

        [ObservableProperty]
        private string _yearGoalDisplay = "0/250 ĐRL";

        [ObservableProperty]
        private bool _isSemesterQualified;

        [ObservableProperty]
        private bool _isYearQualified;

        // Role-Specific Metrics
        [ObservableProperty]
        private int _totalUsersCount;

        [ObservableProperty]
        private int _myCreatedEventsCount;

        [ObservableProperty]
        private int _pendingRegistrationsCount;

        [ObservableProperty]
        private string _assignedClassDisplay = "D20CNTT1";

        [ObservableProperty]
        private int _classStudentsCount;

        [ObservableProperty]
        private int _pendingApprovalCount;

        [ObservableProperty]
        private bool _hasPendingApprovals;

        // Events Data
        [ObservableProperty]
        private List<CampusEvent> _allEvents = [];

        [ObservableProperty]
        private List<CampusEvent> _displayEvents = [];

        [ObservableProperty]
        private List<CampusEvent> _myRegisteredEvents = [];

        [ObservableProperty]
        private List<CategoryChartData> _todoCategoryData = [];

        [ObservableProperty]
        private List<Brush> _todoCategoryColors = [];

        [ObservableProperty]
        private string _selectedCategoryFilter = "Tất cả";

        [ObservableProperty]
        private int _upcomingCount;

        [ObservableProperty]
        private int _registeredCount;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private string _today = DateTime.Now.ToString("dddd, dd/MM/yyyy");

        // QR Attendance Ticket Modal
        [ObservableProperty]
        private bool _isQrModalVisible;

        [ObservableProperty]
        private string _selectedActivityForQr = "Hội thảo AI & Công nghệ Thông tin 2026";

        [ObservableProperty]
        private string _qrTicketCode = "CAMPUS-TK-B20DCCN001-71965";

        [ObservableProperty]
        private ImageSource _qrImageSource;

        [ObservableProperty]
        private ObservableCollection<CampusNotificationDto> _notifications = [];

        [ObservableProperty]
        private int _unreadNotificationsCount;

        [ObservableProperty]
        private bool _hasUnreadNotifications;

        [ObservableProperty]
        private bool _isNotificationsModalVisible;

        [ObservableProperty]
        private bool _isCreateNotificationModalVisible;

        [ObservableProperty]
        private string _selectedNotificationFilter = "ALL";

        public Color FilterAllBg => SelectedNotificationFilter == "ALL" ? Color.FromArgb("#2563EB") : Color.FromArgb("#F1F5F9");
        public Color FilterAllText => SelectedNotificationFilter == "ALL" ? Colors.White : Color.FromArgb("#475569");

        public Color FilterSchoolBg => SelectedNotificationFilter == "SCHOOL" ? Color.FromArgb("#2563EB") : Color.FromArgb("#F1F5F9");
        public Color FilterSchoolText => SelectedNotificationFilter == "SCHOOL" ? Colors.White : Color.FromArgb("#475569");

        public Color FilterClassBg => SelectedNotificationFilter == "CLASS" ? Color.FromArgb("#2563EB") : Color.FromArgb("#F1F5F9");
        public Color FilterClassText => SelectedNotificationFilter == "CLASS" ? Colors.White : Color.FromArgb("#475569");

        public Color FilterPersonalBg => SelectedNotificationFilter == "PERSONAL" ? Color.FromArgb("#2563EB") : Color.FromArgb("#F1F5F9");
        public Color FilterPersonalText => SelectedNotificationFilter == "PERSONAL" ? Colors.White : Color.FromArgb("#475569");

        private List<CampusNotificationDto> _allNotifications = new();

        [RelayCommand]
        public void SetNotificationFilter(string filter)
        {
            SelectedNotificationFilter = filter ?? "ALL";
            OnPropertyChanged(nameof(FilterAllBg));
            OnPropertyChanged(nameof(FilterAllText));
            OnPropertyChanged(nameof(FilterSchoolBg));
            OnPropertyChanged(nameof(FilterSchoolText));
            OnPropertyChanged(nameof(FilterClassBg));
            OnPropertyChanged(nameof(FilterClassText));
            OnPropertyChanged(nameof(FilterPersonalBg));
            OnPropertyChanged(nameof(FilterPersonalText));
            ApplyNotificationFilter();
        }

        private void ApplyNotificationFilter()
        {
            var currentUser = _authService.CurrentUser;
            string myUser = currentUser?.Username ?? "";
            string myClass = currentUser?.ClassName ?? "";

            IEnumerable<CampusNotificationDto> filtered = _allNotifications;

            if (SelectedNotificationFilter == "SCHOOL")
            {
                filtered = _allNotifications.Where(n => string.IsNullOrWhiteSpace(n.TargetUsername) || n.TargetUsername == "ALL");
            }
            else if (SelectedNotificationFilter == "CLASS")
            {
                filtered = _allNotifications.Where(n =>
                    !string.IsNullOrWhiteSpace(n.TargetUsername) &&
                    n.TargetUsername != "ALL" &&
                    (string.Equals(n.TargetUsername, myClass, StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(n.TargetUsername, "CLASS:" + myClass, StringComparison.OrdinalIgnoreCase) ||
                     (!n.TargetUsername.Equals(myUser, StringComparison.OrdinalIgnoreCase) && n.Type != "Points")));
            }
            else if (SelectedNotificationFilter == "PERSONAL")
            {
                filtered = _allNotifications.Where(n =>
                    n.Type == "Points" ||
                    string.Equals(n.TargetUsername, myUser, StringComparison.OrdinalIgnoreCase));
            }

            Notifications.Clear();
            foreach (var item in filtered.OrderByDescending(x => x.CreatedAt))
            {
                Notifications.Add(item);
            }
        }

        [ObservableProperty]
        private string _newNotifTitle = string.Empty;

        [ObservableProperty]
        private string _newNotifMessage = string.Empty;

        [ObservableProperty]
        private string _newNotifType = "Info";

        [ObservableProperty]
        private string _newNotifTarget = "ALL (Toàn trường)";

        [ObservableProperty]
        private List<string> _availableNotifTargets = new() { "ALL (Toàn trường)" };

        public MainPageModel(
            IEventService eventService,
            IAuthenticationService authService,
            IUserService userService,
            ModalErrorHandler errorHandler,
            IQrCodeService qrCodeService,
            IAttendanceService attendanceService)
        {
            _eventService = eventService;
            _authService = authService;
            _userService = userService;
            _errorHandler = errorHandler;
            _qrCodeService = qrCodeService;
            _attendanceService = attendanceService;
        }

        [RelayCommand]
        private async Task Appearing()
        {
            if (_authService?.CurrentUser == null)
            {
                await _authService.TryRestoreSessionAsync();
            }

            // Bắt buộc đăng nhập nếu thực sự không có phiên người dùng
            if (_authService?.CurrentUser == null)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync("//login");
                }
                return;
            }

            UpdateStudentProfile();
            await LoadEventsData();
            await LoadNotificationsAsync();
        }

        [RelayCommand]
        private async Task Refresh()
        {
            try
            {
                IsRefreshing = true;
                UpdateStudentProfile();
                await LoadEventsData();
                await LoadNotificationsAsync();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private void UpdateStudentProfile()
        {
            var u = _authService.CurrentUser;
            if (u == null) return;

            IsAdmin = u.Role == Role.Admin;
            IsManager = u.Role == Role.Manager;
            IsLecturer = u.Role == Role.Lecturer;
            IsSecretary = u.Role == Role.Secretary || u.IsSecretary;
            IsStudent = !IsAdmin && !IsManager && !IsLecturer && !IsSecretary;
            IsAdminOrManager = IsAdmin || IsManager;
            IsStudentOrSecretary = IsStudent || IsSecretary;

            int currentPts = u.TrainingPoints;

            var role = u.Role switch
            {
                Role.Admin => AccountRole.Admin,
                Role.Manager => AccountRole.BanChuNhiem,
                _ => AccountRole.SinhVien
            };

            CurrentStudent = new Account
            {
                FullName = u.FullName,
                StudentCode = string.IsNullOrWhiteSpace(u.Username) ? (u.Role == Role.Admin ? "ADMIN" : "MANAGER") : u.Username.ToUpper(),
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                ClassName = u.Role == Role.Admin ? "Ban Quản Trị Hệ Thống" : (u.Role == Role.Manager ? "Ban Tổ Chức Sự Kiện" : (u.ClassName ?? "D20CNTT1")),
                AcademicYear = u.Role == Role.Admin ? "Quản trị viên" : (u.Role == Role.Manager ? "Quản lý sự kiện" : (u.IsSecretary ? "Bí thư chi đoàn" : (u.Role == Role.Lecturer ? "Cố vấn học tập" : "K20 (2020–2025)"))),
                Role = role,
                Status = AccountStatus.DangHoc,
                TrainingPoints = currentPts
            };

            StudentGreeting = u.Role switch
            {
                Role.Admin => $"Xin chào, Quản trị viên {u.FullName} 🛡️",
                Role.Manager => $"Xin chào, Ban tổ chức {u.FullName} 🎪",
                Role.Lecturer => $"Xin chào, Giảng viên {u.FullName} 👨‍🏫",
                Role.Secretary => $"Xin chào, Bí thư {u.FullName} 🎓",
                _ => $"Xin chào, {u.FullName} 👋"
            };

            RoleSubtitle = u.Role switch
            {
                Role.Admin => "Bảng Điều Khiển Quản Trị Toàn Quyền",
                Role.Manager => "Bảng Điều Khiển Quản Lý Sự Kiện & Duyệt Check-in",
                Role.Lecturer => $"Cố Vấn Học Tập • Lớp {u.AssignedClass ?? u.ClassName}",
                Role.Secretary => $"Bí Thư Chi Đoàn • Lớp {u.ClassName}",
                _ => $"Sinh Viên Lớp {u.ClassName} • MSV: {u.Username}"
            };

            AssignedClassDisplay = u.AssignedClass ?? u.ClassName ?? "D20CNTT1";

            TrainingPointsProgress = Math.Clamp((double)currentPts / 50.0, 0.0, 1.0);
            SemesterProgress = Math.Clamp((double)currentPts / 50.0, 0.0, 1.0);
            YearProgress = Math.Clamp((double)currentPts / 250.0, 0.0, 1.0);
            SemesterGoalDisplay = $"{currentPts}/50 ĐRL";
            YearGoalDisplay = $"{currentPts}/250 ĐRL";
            IsSemesterQualified = currentPts >= 50;
            IsYearQualified = currentPts >= 250;

            QrTicketCode = $"CAMPUS-TK-{CurrentStudent.StudentCode}-71965";
        }

        private async Task LoadEventsData()
        {
            try
            {
                IsBusy = true;
                var currentUser = _authService.CurrentUser;
                if (currentUser == null) return;

                // Sự kiện được lọc chuẩn theo quyền của role
                var events = await _eventService.GetEventsForUserAsync(currentUser);
                AllEvents = events ?? [];

                var allRaw = await _eventService.GetEventsAsync(includePending: true);
                PendingApprovalCount = allRaw.Count(e => e.ApprovalStatus == "Pending");
                HasPendingApprovals = IsAdmin && PendingApprovalCount > 0;

                UpcomingCount = AllEvents.Count;

                // Thống kê riêng cho Manager
                if (IsManager)
                {
                    var myEvents = allRaw.Where(e => e.CreatedByUsername == currentUser.Username || e.CreatedByName == currentUser.FullName).ToList();
                    MyCreatedEventsCount = myEvents.Count;
                    PendingRegistrationsCount = myEvents.SelectMany(e => e.Registrations ?? []).Count(r => r.Status == "Pending");
                }

                // Thống kê riêng cho Admin
                if (IsAdmin)
                {
                    var allUsers = await _userService.GetUsersListAsync();
                    TotalUsersCount = allUsers?.Count ?? 0;
                }

                // Thống kê riêng cho Giảng viên
                if (IsLecturer)
                {
                    var classUsers = await _userService.GetUsersListAsync();
                    ClassStudentsCount = classUsers?.Count(u => u.ClassName == AssignedClassDisplay && u.Role == Role.User) ?? 0;
                }

                string myCode = currentUser.Username ?? "B20DCCN001";
                MyRegisteredEvents = AllEvents.Where(e => e.Registrations != null &&
                    e.Registrations.Any(r => r.StudentCode.Equals(myCode, StringComparison.OrdinalIgnoreCase) && r.Status != "Cancelled")).ToList();
                RegisteredCount = MyRegisteredEvents.Count;

                ApplyCategoryFilter();
                UpdateChartData();
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

        private void UpdateChartData()
        {
            var chartList = new List<CategoryChartData>
            {
                new("Học thuật", AllEvents.Count(e => e.CategoryTitle == "Học thuật")),
                new("Thể thao", AllEvents.Count(e => e.CategoryTitle == "Thể thao")),
                new("Tình nguyện", AllEvents.Count(e => e.CategoryTitle == "Tình nguyện")),
                new("Kỹ năng", AllEvents.Count(e => e.CategoryTitle == "Kỹ năng mềm" || e.CategoryTitle == "Kỹ năng"))
            };

            var colorList = new List<Brush>
            {
                new SolidColorBrush(Color.FromArgb("#2563EB")),
                new SolidColorBrush(Color.FromArgb("#10B981")),
                new SolidColorBrush(Color.FromArgb("#EF4444")),
                new SolidColorBrush(Color.FromArgb("#7C3AED"))
            };

            TodoCategoryData = chartList;
            TodoCategoryColors = colorList;
        }

        [RelayCommand]
        private void FilterCategory(string category)
        {
            SelectedCategoryFilter = category;
            ApplyCategoryFilter();
        }

        private void ApplyCategoryFilter()
        {
            if (string.IsNullOrWhiteSpace(SelectedCategoryFilter) || SelectedCategoryFilter == "Tất cả")
            {
                DisplayEvents = new List<CampusEvent>(AllEvents ?? []);
            }
            else
            {
                DisplayEvents = (AllEvents ?? []).Where(e => string.Equals(e.CategoryTitle, SelectedCategoryFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        [RelayCommand]
        private void ShowQrTicket(CampusEvent evt)
        {
            if (evt != null)
            {
                SelectedActivityForQr = evt.Name;
                QrTicketCode = _qrCodeService.GenerateTicketPayload(evt.ID, CurrentStudent.StudentCode, evt.Name);
            }
            else
            {
                QrTicketCode = _qrCodeService.GenerateTicketPayload(1, CurrentStudent.StudentCode, SelectedActivityForQr);
            }
            QrImageSource = _qrCodeService.GenerateQrCodeImageSource(QrTicketCode, 20);
            IsQrModalVisible = true;
        }

        [RelayCommand]
        private void CloseQrTicket()
        {
            IsQrModalVisible = false;
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
                    await Shell.Current.DisplayAlert("Video giới thiệu", $"Đường dẫn video hoạt động:\n{evt.VideoUrl}", "Đóng");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
        }

        [RelayCommand]
        private async Task CheckInWithPhoto(CampusEvent evt)
        {
            if (evt == null) return;
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync($"photoattendance?eventId={evt.ID}");
            }
        }

        [RelayCommand]
        private async Task ViewEventReport(CampusEvent evt)
        {
            if (evt == null) return;
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync($"attendancereport?eventId={evt.ID}");
            }
        }

        [RelayCommand]
        private async Task NavigateToQrScanner()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//qrTab/qrscan");
            }
        }

        [RelayCommand]
        private async Task NavigateToUsers()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("users");
            }
        }

        [RelayCommand]
        private async Task NavigateToClassHub()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//classTab/classhub");
            }
        }

        [RelayCommand]
        private async Task NavigateToEvents()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//eventsTab/events");
            }
        }

        [RelayCommand]
        private async Task NavigateToProfile()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//profileTab/profile");
            }
        }

        [RelayCommand]
        private async Task NavigateToAttendanceReport()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//attendancereport");
            }
        }

        [RelayCommand]
        private async Task Logout()
        {
            if (Shell.Current != null)
            {
                bool confirm = await Shell.Current.DisplayAlert("Đăng xuất", "Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?", "Đăng xuất", "Hủy");
                if (confirm)
                {
                    _authService?.Logout();
                    await Shell.Current.GoToAsync("//login");
                }
            }
            else
            {
                _authService?.Logout();
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
        public async Task OpenEventDetail(CampusEvent evt)
        {
            if (evt != null && Shell.Current != null)
            {
                await Shell.Current.GoToAsync($"eventdetail?eventId={evt.ID}");
            }
        }
    
        public async Task LoadNotificationsAsync()
        {
            try
            {
                var currentUser = _authService.CurrentUser;
                var list = await _attendanceService.GetNotificationsAsync(currentUser?.Username, currentUser?.ClassName);
                _allNotifications = list;
                ApplyNotificationFilter();
                UnreadNotificationsCount = _allNotifications.Count(n => !n.IsRead);
                HasUnreadNotifications = UnreadNotificationsCount > 0;
            }
            catch { }
        }

        [RelayCommand]
        public void ToggleNotificationsModal()
        {
            IsNotificationsModalVisible = !IsNotificationsModalVisible;
        }

        [RelayCommand]
        public void CloseNotificationsModal()
        {
            IsNotificationsModalVisible = false;
        }

        [RelayCommand]
        public async Task MarkNotificationAsReadAsync(CampusNotificationDto notif)
        {
            if (notif == null) return;
            notif.IsRead = true;
            await _attendanceService.MarkNotificationAsReadAsync(notif.Id);
            UnreadNotificationsCount = Notifications.Count(n => !n.IsRead);
            HasUnreadNotifications = UnreadNotificationsCount > 0;
        }

        [RelayCommand]
        public async Task MarkAllNotificationsAsReadAsync()
        {
            var currentUser = _authService.CurrentUser;
            await _attendanceService.MarkAllNotificationsAsReadAsync(currentUser?.Username);
            foreach (var n in _allNotifications)
            {
                n.IsRead = true;
            }
            ApplyNotificationFilter();
            UnreadNotificationsCount = 0;
            HasUnreadNotifications = false;
        }

        [RelayCommand]
        public async Task OpenCreateNotificationModalAsync()
        {
            if (!IsAdmin)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Phân quyền ⚠️", "Chức năng tạo thông báo chung chỉ dành cho Quản trị viên (Admin)!", "Đã hiểu");
                }
                return;
            }

            NewNotifTitle = string.Empty;
            NewNotifMessage = string.Empty;
            NewNotifType = "Info";
            NewNotifTarget = "ALL (Toàn trường)";

            try
            {
                var classes = await _userService.GetClassesAsync();
                var targets = new List<string> { "ALL (Toàn trường)" };
                if (classes != null && classes.Any())
                {
                    targets.AddRange(classes.Select(c => c.ClassName).Where(n => !string.IsNullOrWhiteSpace(n)).Distinct());
                }
                AvailableNotifTargets = targets;
            }
            catch { }

            IsCreateNotificationModalVisible = true;
        }

        [RelayCommand]
        public void CloseCreateNotificationModal()
        {
            IsCreateNotificationModalVisible = false;
        }

        [RelayCommand]
        public async Task SendNotificationAsync()
        {
            if (string.IsNullOrWhiteSpace(NewNotifTitle))
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Thiếu thông tin ⚠️", "Vui lòng nhập tiêu đề thông báo.", "Đóng");
                }
                return;
            }

            if (string.IsNullOrWhiteSpace(NewNotifMessage))
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Thiếu thông tin ⚠️", "Vui lòng nhập nội dung thông báo.", "Đóng");
                }
                return;
            }

            string target = string.IsNullOrWhiteSpace(NewNotifTarget) || NewNotifTarget.StartsWith("ALL", StringComparison.OrdinalIgnoreCase)
                ? "ALL"
                : NewNotifTarget.Trim();

            var notif = new CampusNotificationDto
            {
                Id = "NOTIF-" + Guid.NewGuid().ToString("N")[..6].ToUpper(),
                Title = NewNotifTitle.Trim(),
                Message = NewNotifMessage.Trim(),
                Type = string.IsNullOrWhiteSpace(NewNotifType) ? "Info" : NewNotifType,
                TargetUsername = target,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _attendanceService.AddNotificationAsync(notif);
            IsCreateNotificationModalVisible = false;
            await LoadNotificationsAsync();

            string targetDisplay = target == "ALL" ? "Toàn trường" : $"Lớp/Mục tiêu: {target}";
            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert(
                    "Đã phát thông báo thành công 📢",
                    $"Thông báo '{notif.Title}' đã được gửi đến {targetDisplay} và tự động đồng bộ hóa lên Firebase Cloud!",
                    "OK");
            }
        }
    }
}
