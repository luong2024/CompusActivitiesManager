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
        private bool _isDataLoaded;

        [ObservableProperty]
        private Account _currentStudent;

        [ObservableProperty]
        private string _studentGreeting = "Xin chào, Sinh viên 👋";

        [ObservableProperty]
        private double _trainingPointsProgress = 0.92;

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

        // Promotional Event Slider (US 16 / US 01)
        [ObservableProperty]
        private List<CampusEvent> _promotionalEvents = [];

        // Campus Articles / News (US 22)
        [ObservableProperty]
        private List<CampusPost> _campusPosts = [];

        [ObservableProperty]
        private CampusPost _selectedPost;

        [ObservableProperty]
        private bool _isPostModalVisible;

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
                CurrentStudent = new Account
                {
                    FullName = u.FullName,
                    StudentCode = string.IsNullOrWhiteSpace(u.Username) ? "B20DCCN001" : u.Username.ToUpper(),
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    ClassName = "D20CNTT1",
                    AcademicYear = "K20 (2020–2025)",
                    Role = AccountRole.SinhVien,
                    Status = AccountStatus.DangHoc,
                    TrainingPoints = 92
                };
            }
            else
            {
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
                    TrainingPoints = 92
                };
            }

            StudentGreeting = $"Xin chào, {CurrentStudent.FullName} 👋";
            TrainingPointsProgress = Math.Clamp(CurrentStudent.TrainingPoints / 100.0, 0.0, 1.0);
            QrTicketCode = $"CAMPUS-TK-{CurrentStudent.StudentCode}-71965";
        }

        [RelayCommand]
        private async Task Appearing()
        {
            UpdateStudentProfile();
            if (!_isDataLoaded)
            {
                _isDataLoaded = true;
                await LoadEventsData();
            }
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
                var events = await _eventService.GetEventsAsync();

                if (events != null && events.Count > 0)
                {
                    // Mark the first 2 events as registered for demo realism
                    for (int i = 0; i < events.Count; i++)
                    {
                        if (i == 0 || i == 1)
                        {
                            events[i].IsRegistered = true;
                        }
                    }

                    AllEvents = events;
                }

                UpcomingCount = AllEvents.Count;
                MyRegisteredEvents = AllEvents.Where(e => e.IsRegistered).ToList();
                RegisteredCount = MyRegisteredEvents.Count;

                PromotionalEvents = (AllEvents ?? []).Take(4).ToList();

                CampusPosts =
                [
                    new CampusPost
                    {
                        Id = "POST-01",
                        Title = "Chiến dịch Tình nguyện Mùa Hè Xanh 2026 chính thức khởi động toàn trường",
                        Excerpt = "Đoàn trường phát động chiến dịch tình nguyện hè với hơn 500 đoàn viên thanh niên tham gia tại các mặt trận trọng điểm.",
                        Content = "Sáng ngày 15/10/2026, tại Quảng trường Trung tâm trường, Ban Chấp hành Đoàn trường đã long trọng tổ chức Lễ ra quân Chiến dịch Tình nguyện Mùa Hè Xanh 2026.\n\nChiến dịch năm nay quy tụ hơn 500 đoàn viên, sinh viên tiêu biểu đến từ các khoa, viện trong toàn trường. Các đội hình chuyên sẽ triển khai thực hiện các công trình thanh niên: xây dựng cầu dân sinh, phổ cập tin học cho thiếu nhi, chuyển đổi số cộng đồng và hướng dẫn kỹ năng phòng chống tai nạn thương tích.\n\nĐồng chí Bí thư Đoàn trường nhấn mạnh: 'Mùa Hè Xanh không chỉ là môi trường rèn luyện thực tiễn quý báu mà còn là cơ hội để mỗi sinh viên thể hiện trách nhiệm xã hội và cống hiến sức trẻ cho cộng đồng.'",
                        Category = "Hoạt động",
                        Author = "Ban Truyền thông Đoàn trường",
                        PublishDate = DateTime.Now.AddDays(-1),
                        ReadTimeMinutes = 4,
                        LikesCount = 128
                    },
                    new CampusPost
                    {
                        Id = "POST-02",
                        Title = "Sinh viên Khoa CNTT xuất sắc giành giải Nhất Cuộc thi Hackathon AI 2026",
                        Excerpt = "Dự án Campus Activity Manager tích hợp Firebase và AI nhận diện điểm danh thông minh đã thuyết phục toàn bộ hội đồng giám khảo.",
                        Content = "Vượt qua hơn 40 đội thi đến từ các trường Đại học công nghệ hàng đầu, nhóm sinh viên Khoa Công nghệ Thông tin đã xuất sắc giành ngôi vị Quán quân tại Chung kết Hackathon AI Innovation 2026.\n\nGiải pháp ứng dụng di động đa nền tảng .NET MAUI kết hợp nền tảng đám mây Firebase giúp tự động hóa quy trình quản lý hoạt động ngoại khóa, tạo mã QR bảo mật động và điểm danh minh chứng ảnh bằng thị giác máy tính đã nhận được sự đánh giá rất cao từ các chuyên gia đầu ngành.\n\nGiải thưởng bao gồm Cúp vô địch, tiền thưởng 50.000.000 VNĐ cùng cơ hội ươm tạo khởi nghiệp tại Trung tâm Đổi mới Sáng tạo Quốc gia.",
                        Category = "Gương sáng",
                        Author = "CLB Sáng tạo Trẻ",
                        PublishDate = DateTime.Now.AddDays(-3),
                        ReadTimeMinutes = 3,
                        LikesCount = 256
                    },
                    new CampusPost
                    {
                        Id = "POST-03",
                        Title = "Cẩm nang hướng dẫn tích lũy trọn vẹn 100 Điểm rèn luyện trong năm học",
                        Excerpt = "Bí quyết cân bằng giữa học tập trên lớp và tham gia các hoạt động ngoại khóa, tình nguyện, nghiên cứu khoa học đạt kết quả Xuất sắc.",
                        Content = "Điểm rèn luyện là một trong những tiêu chí quan trọng quyết định học bổng khuyến khích học tập cũng như danh hiệu 'Sinh viên 5 Tốt' các cấp.\n\nĐể đạt mức xếp loại Xuất sắc (từ 90 - 100 điểm), sinh viên cần lưu ý:\n1. Tham gia tối thiểu 3 hoạt động học thuật và hội thảo chuyên môn (+15 đến +20 điểm).\n2. Góp mặt trong các giải đấu thể thao, ngày hội văn hóa nghệ thuật của trường (+10 đến +15 điểm).\n3. Tích cực tham gia các chiến dịch tình nguyện, hiến máu nhân đạo (+15 điểm).\n4. Chấp hành tốt quy chế đào tạo, nội quy ký túc xá và an ninh trật tự (+40 đến +50 điểm).\n\nHãy thường xuyên theo dõi Cổng Sinh Viên để không bỏ lỡ bất kỳ sự kiện hấp dẫn nào!",
                        Category = "Hướng dẫn",
                        Author = "Phòng Công tác Sinh viên",
                        PublishDate = DateTime.Now.AddDays(-5),
                        ReadTimeMinutes = 5,
                        LikesCount = 340
                    }
                ];

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

        [RelayCommand]
        private void OpenPostDetail(CampusPost post)
        {
            if (post != null)
            {
                SelectedPost = post;
                IsPostModalVisible = true;
            }
        }

        [RelayCommand]
        private void ClosePostDetail()
        {
            IsPostModalVisible = false;
        }

        [RelayCommand]
        private void LikePost(CampusPost post)
        {
            if (post == null) return;
            post.IsLiked = !post.IsLiked;
            post.LikesCount += post.IsLiked ? 1 : -1;
            CampusPosts = new List<CampusPost>(CampusPosts);
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
    }
}
