#nullable disable
using System.Collections.ObjectModel;
using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        private readonly IEventService _eventService;
        private readonly IAuthenticationService _authService;
        private readonly ModalErrorHandler _errorHandler;

        [ObservableProperty]
        private Account _currentStudent;

        [ObservableProperty]
        private string _studentGreeting = "Xin chào, Sinh viên 👋";

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

        [ObservableProperty]
        private int _pendingApprovalCount;

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
        private int _upcomingCount = 5;

        [ObservableProperty]
        private int _registeredCount = 2;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private string _today = DateTime.Now.ToString("dddd, dd/MM/yyyy");

        [ObservableProperty]
        private bool _isAdminOrManager;

        // QR Attendance Ticket Modal (US 17)
        [ObservableProperty]
        private bool _isQrModalVisible;

        [ObservableProperty]
        private string _selectedActivityForQr = "Hội thảo AI & Công nghệ Thông tin 2026";

        [ObservableProperty]
        private string _qrTicketCode = "CAMPUS-TK-B20DCCN001-71965";

        [ObservableProperty]
        private ImageSource _qrImageSource;

        private readonly IQrCodeService _qrCodeService;

        public MainPageModel(
            IEventService eventService,
            IAuthenticationService authService,
            ModalErrorHandler errorHandler,
            IQrCodeService qrCodeService)
        {
            _eventService = eventService;
            _authService = authService;
            _errorHandler = errorHandler;
            _qrCodeService = qrCodeService;

            UpdateStudentProfile();
        }

        private void UpdateStudentProfile()
        {
            if (_authService?.CurrentUser != null)
            {
                var u = _authService.CurrentUser;
                var role = u.Role switch
                {
                    Role.Admin => AccountRole.Admin,
                    Role.Manager => AccountRole.BanChuNhiem,
                    _ => AccountRole.SinhVien
                };

                IsAdminOrManager = u.Role == Role.Admin || u.Role == Role.Manager;

                int currentPts = u.TrainingPoints;

                CurrentStudent = new Account
                {
                    FullName = u.FullName,
                    StudentCode = string.IsNullOrWhiteSpace(u.Username) ? (u.Role == Role.Admin ? "ADMIN" : "MANAGER") : u.Username.ToUpper(),
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    ClassName = u.Role == Role.Admin ? "Ban Quản Trị Hệ Thống" : (u.Role == Role.Manager ? "Ban Tổ Chức Sự Kiện" : (u.ClassName ?? "D20CNTT1")),
                    AcademicYear = u.Role == Role.Admin ? "Quản trị viên" : (u.Role == Role.Manager ? "Quản lý sự kiện" : (u.IsSecretary ? "Bí thư chi đoàn" : "K20 (2020–2025)")),
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

                TrainingPointsProgress = Math.Clamp((double)currentPts / 50.0, 0.0, 1.0);
                SemesterProgress = Math.Clamp((double)currentPts / 50.0, 0.0, 1.0);
                YearProgress = Math.Clamp((double)currentPts / 250.0, 0.0, 1.0);
                SemesterGoalDisplay = $"{currentPts}/50 ĐRL";
                YearGoalDisplay = $"{currentPts}/250 ĐRL";
                IsSemesterQualified = currentPts >= 50;
                IsYearQualified = currentPts >= 250;
            }
            else
            {
                IsAdminOrManager = false;
                CurrentStudent = new Account
                {
                    FullName = "Nguyễn An Cương",
                    StudentCode = "B20DCCN001",
                    Email = "cuong.na@campus.edu.vn",
                    PhoneNumber = "0987654321",
                    ClassName = "D20CNTT1",
                    AcademicYear = "K20 (2020–2025)",
                    Role = AccountRole.SinhVien,
                    Status = AccountStatus.DangHoc,
                    TrainingPoints = 0
                };
                StudentGreeting = "Xin chào, Nguyễn An Cương 👋";
                TrainingPointsProgress = 0.0;
                SemesterProgress = 0.0;
                YearProgress = 0.0;
                SemesterGoalDisplay = "0/50 ĐRL";
                YearGoalDisplay = "0/250 ĐRL";
                IsSemesterQualified = false;
                IsYearQualified = false;
            }

            QrTicketCode = $"CAMPUS-TK-{CurrentStudent.StudentCode}-71965";
        }

        [RelayCommand]
        private async Task Appearing()
        {
            UpdateStudentProfile();
            await LoadEventsData();
        }

        [RelayCommand]
        private async Task Refresh()
        {
            try
            {
                IsRefreshing = true;
                UpdateStudentProfile();
                await LoadEventsData();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task LoadEventsData()
        {
            try
            {
                IsBusy = true;
                // Chỉ lấy sự kiện hợp lệ theo role và lớp học của user (Admin xem được cả pending)
                var events = await _eventService.GetEventsForUserAsync(_authService.CurrentUser);

                AllEvents = events ?? [];

                var allRaw = await _eventService.GetEventsAsync(includePending: true);
                PendingApprovalCount = allRaw.Count(e => e.ApprovalStatus == "Pending");

                UpcomingCount = AllEvents.Count;

                string myCode = _authService.CurrentUser?.Username ?? "B20DCCN001";
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

        // US 17 & US 01: Quick Register or View QR Ticket for Campus Event
        [RelayCommand]
        private async Task RegisterEvent(CampusEvent evt)
        {
            if (evt == null) return;

            if (evt.IsRegistered)
            {
                SelectedActivityForQr = evt.Name;
                QrTicketCode = _qrCodeService.GenerateTicketPayload(evt.ID, CurrentStudent.StudentCode, evt.Name);
                QrImageSource = _qrCodeService.GenerateQrCodeImageSource(QrTicketCode, 20);
                IsQrModalVisible = true;
                return;
            }

            evt.IsRegistered = true;
            MyRegisteredEvents = (AllEvents ?? []).Where(e => e.IsRegistered).ToList();
            RegisteredCount = MyRegisteredEvents.Count;
            AllEvents = new List<CampusEvent>(AllEvents ?? []);
            ApplyCategoryFilter();

            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert(
                    "Đăng ký thành công 🎉",
                    $"Bạn đã đăng ký tham gia: {evt.Name}\n" +
                    $"Địa điểm: {evt.Location}\n" +
                    $"Quyền lợi: +{evt.TrainingPointsReward} Điểm rèn luyện\n\n" +
                    "Vé điện tử QR của bạn đã sẵn sàng!",
                    "Xem vé ngay");
            }

            SelectedActivityForQr = evt.Name;
            QrTicketCode = _qrCodeService.GenerateTicketPayload(evt.ID, CurrentStudent.StudentCode, evt.Name);
            QrImageSource = _qrCodeService.GenerateQrCodeImageSource(QrTicketCode, 20);
            IsQrModalVisible = true;
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
                await Shell.Current.GoToAsync($"//photoattendance?eventId={evt.ID}");
            }
        }

        [RelayCommand]
        private async Task ViewEventReport(CampusEvent evt)
        {
            if (evt == null) return;
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync($"//attendancereport?eventId={evt.ID}");
            }
        }

        [RelayCommand]
        private async Task NavigateToQrScanner()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//qrscan");
            }
        }

        [RelayCommand]
        private async Task NavigateToPhotoAttendance()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//photoattendance");
            }
        }

        [RelayCommand]
        private async Task NavigateToAttendanceHistory()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//attendancehistory");
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

        // Navigation Commands
        [RelayCommand]
        private async Task NavigateToEvents()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//events");
            }
        }

        [RelayCommand]
        private async Task NavigateToProfile()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//profile");
            }
        }

        [RelayCommand]
        private async Task NavigateToLogin()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//login");
            }
        }

        [RelayCommand]
        private async Task Logout()
        {
            if (Shell.Current != null)
            {
                bool confirm = await Shell.Current.DisplayAlert("Đăng xuất", "Bạn có chắc chắn muốn đăng xuất khỏi tài khoản Sinh viên?", "Đăng xuất", "Hủy");
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
        public async Task OpenEventDetail(CampusEvent evt)
        {
            if (evt != null && Shell.Current != null)
            {
                await Shell.Current.GoToAsync($"eventdetail?eventId={evt.ID}");
            }
        }
    }
}
