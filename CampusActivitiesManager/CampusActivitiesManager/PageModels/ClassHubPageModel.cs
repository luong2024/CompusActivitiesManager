#nullable disable
using System.Collections.ObjectModel;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class ClassHubPageModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly IEventService _eventService;
        private readonly IAuthenticationService _authService;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isAdmin;

        [ObservableProperty]
        private bool _canCreateClassEvent;

        [ObservableProperty]
        private bool _canViewClassmatesDrl;

        [ObservableProperty]
        private string _studentDrlSectionTitle = "Danh sách Sinh viên & Tiến độ ĐRL";

        [ObservableProperty]
        private string _studentDrlSectionSubtitle = "Điểm ban đầu: 0 ĐRL. Mục tiêu: Kỳ ≥ 50 ĐRL, Năm ≥ 250 ĐRL.";

        [ObservableProperty]
        private string _userRoleTitle = "Sinh viên";

        [ObservableProperty]
        private List<ClassRoom> _availableClasses = [];

        [ObservableProperty]
        private ClassRoom _selectedClass;

        [ObservableProperty]
        private ObservableCollection<User> _studentsList = [];

        [ObservableProperty]
        private ObservableCollection<CampusEvent> _classEventsList = [];

        [ObservableProperty]
        private int _totalStudents;

        [ObservableProperty]
        private int _qualifiedSemesterCount;

        [ObservableProperty]
        private int _qualifiedYearCount;

        [ObservableProperty]
        private string _semesterQualificationRate = "0%";

        public ClassHubPageModel(
            IUserService userService,
            IEventService eventService,
            IAuthenticationService authService)
        {
            _userService = userService;
            _eventService = eventService;
            _authService = authService;
        }

        [RelayCommand]
        public async Task AppearingAsync()
        {
            await InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                var currentUser = _authService.CurrentUser;
                IsAdmin = _authService.IsAdmin;
                bool isSecretary = _authService.IsSecretary;
                bool isLecturer = currentUser?.Role == Role.Lecturer || (currentUser?.Role == Role.Manager && !string.IsNullOrEmpty(currentUser?.AssignedClass));

                // QUY ĐỊNH BẢO MẬT ĐRL: Chỉ Bí thư, Giảng viên và Admin mới được xem ĐRL của các sinh viên khác trong lớp
                CanViewClassmatesDrl = IsAdmin || isSecretary || isLecturer;
                CanCreateClassEvent = IsAdmin || isSecretary || isLecturer;

                if (!CanViewClassmatesDrl)
                {
                    StudentDrlSectionTitle = "Tiến độ Điểm Rèn Luyện của bạn";
                    StudentDrlSectionSubtitle = $"Theo quy định, sinh viên chỉ được xem điểm rèn luyện của chính mình.";
                }
                else
                {
                    StudentDrlSectionTitle = "Danh sách Sinh viên & Tiến độ ĐRL";
                    StudentDrlSectionSubtitle = "Điểm ban đầu: 0 ĐRL. Mục tiêu: Kỳ ≥ 50 ĐRL, Năm ≥ 250 ĐRL.";
                }

                UserRoleTitle = currentUser?.Role switch
                {
                    Role.Admin => "Quản trị viên (Toàn quyền quản lý)",
                    Role.Lecturer => "Giảng viên / Cố vấn học tập",
                    Role.Secretary => "Bí thư chi đoàn lớp",
                    Role.Manager => "Ban tổ chức / Cố vấn",
                    _ => "Sinh viên lớp"
                };

                AvailableClasses = await _userService.GetClassesAsync();

                if (SelectedClass == null)
                {
                    string targetClassName = currentUser?.AssignedClass ?? currentUser?.ClassName ?? "D20CNTT1";
                    SelectedClass = AvailableClasses.FirstOrDefault(c => c.ClassName.Equals(targetClassName, StringComparison.OrdinalIgnoreCase))
                                   ?? AvailableClasses.FirstOrDefault();
                }

                await LoadClassDetailsAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSelectedClassChanged(ClassRoom value)
        {
            if (value != null)
            {
                _ = LoadClassDetailsAsync();
            }
        }

        public async Task LoadClassDetailsAsync()
        {
            if (SelectedClass == null) return;

            try
            {
                IsLoading = true;
                var currentUser = _authService.CurrentUser;

                // 1. Tải danh sách sinh viên của lớp
                var students = await _userService.GetStudentsByClassAsync(SelectedClass.ClassName);
                StudentsList.Clear();

                if (!CanViewClassmatesDrl)
                {
                    // Sinh viên thường chỉ xem được bản ghi ĐRL của chính mình
                    var myCode = currentUser?.Username;
                    var myself = students.FirstOrDefault(s => string.Equals(s.Username, myCode, StringComparison.OrdinalIgnoreCase))
                                 ?? currentUser;
                    if (myself != null)
                    {
                        StudentsList.Add(myself);
                    }
                }
                else
                {
                    // Bí thư, Giảng viên, Admin xem toàn bộ sinh viên trong lớp
                    foreach (var s in students)
                    {
                        StudentsList.Add(s);
                    }
                }

                TotalStudents = students.Count;
                QualifiedSemesterCount = students.Count(s => s.IsSemesterQualified);
                QualifiedYearCount = students.Count(s => s.IsYearQualified);

                int rate = TotalStudents > 0 ? (int)((double)QualifiedSemesterCount / TotalStudents * 100) : 0;
                SemesterQualificationRate = $"{rate}%";

                // 2. Tải hoạt động riêng của lớp (Chỉ hoạt động đã duyệt mới hiện cho sinh viên thường)
                bool isSecretary = _authService.IsSecretary;
                bool isLecturer = currentUser?.Role == Role.Lecturer || (currentUser?.Role == Role.Manager && !string.IsNullOrEmpty(currentUser?.AssignedClass));
                bool canSeePendingClassEvents = IsAdmin || isSecretary || isLecturer;
                var allEvents = await _eventService.GetEventsAsync(includePending: canSeePendingClassEvents);
                var classEvents = allEvents.Where(e =>
                    e.Scope == "ClassOnly" &&
                    string.Equals(e.TargetClass, SelectedClass.ClassName, StringComparison.OrdinalIgnoreCase) &&
                    (canSeePendingClassEvents || (e.IsApproved && e.ApprovalStatus == "Approved"))
                ).ToList();

                ClassEventsList.Clear();
                foreach (var evt in classEvents)
                {
                    ClassEventsList.Add(evt);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task CreateClassEventAsync()
        {
            if (!CanCreateClassEvent)
            {
                await Shell.Current.DisplayAlert("Phân quyền", "Chỉ Giảng viên cố vấn, Bí thư lớp hoặc Quản trị viên mới có thể tạo hoạt động riêng cho lớp.", "Đã hiểu");
                return;
            }

            string targetClass = SelectedClass?.ClassName ?? "D20CNTT1";
            string title = await Shell.Current.DisplayPromptAsync(
                "Tạo Hoạt Động Lớp",
                $"Nhập tên hoạt động nội bộ cho lớp {targetClass}:",
                "Tiếp tục", "Hủy",
                placeholder: "Sinh hoạt lớp chuyên đề");

            if (string.IsNullOrWhiteSpace(title)) return;

            string pointsStr = await Shell.Current.DisplayPromptAsync(
                "Điểm Rèn Luyện Thưởng",
                "Nhập số điểm rèn luyện thưởng (+ĐRL):",
                "Xác nhận", "Hủy",
                initialValue: "10",
                keyboard: Keyboard.Numeric);

            int points = int.TryParse(pointsStr, out int p) ? p : 10;

            var newEvent = new CampusEvent
            {
                ID = new Random().Next(100, 999),
                Name = title.Trim(),
                CategoryID = 4,
                Category = new EventCategoryDto { ID = 4, Title = "Hoạt động Lớp", Color = "#7C3AED" },
                Location = $"Phòng học lớp {targetClass}",
                StartDate = DateTime.UtcNow.AddDays(3).ToString("dd/MM/yyyy"),
                EndDate = DateTime.UtcNow.AddDays(3).ToString("dd/MM/yyyy"),
                StartTime = "19:00",
                EndTime = "21:00",
                CreatedByName = _authService.CurrentUser?.FullName ?? "Ban cán sự lớp",
                CreatedByUsername = _authService.CurrentUser?.Username ?? "bithu",
                CreatedByRole = _authService.CurrentUser?.Role.ToString() ?? "Secretary",
                Description = $"Hoạt động rèn luyện nội bộ của chi đoàn/lớp {targetClass}. Chỉ sinh viên trong lớp mới có thể tham gia và ghi nhận điểm rèn luyện.",
                Scope = "ClassOnly",
                TargetClass = targetClass,
                RegistrationType = "Auto",
                PointsReward = points,
                PointsPenalty = 5,
                IsApproved = IsAdmin,
                ApprovalStatus = IsAdmin ? "Approved" : "Pending", // Admin tạo thì duyệt luôn, Bí thư tạo thì chờ Admin duyệt
                MaxParticipants = TotalStudents > 0 ? TotalStudents : 50,
                ImageUrl = "https://images.unsplash.com/photo-1523240795612-9a054b0db644?w=800&q=80"
            };

            bool success = await _eventService.CreateEventAsync(newEvent);
            if (success)
            {
                await LoadClassDetailsAsync();
                await Shell.Current.DisplayAlert(
                    "Thành công 🎉",
                    IsAdmin ? $"Đã tạo hoạt động nội bộ lớp {targetClass} thành công!"
                            : $"Đã gửi đề xuất hoạt động cho lớp {targetClass}. Đang chờ Quản trị viên (Admin) phê duyệt.",
                    "OK");
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
        public async Task OpenEventDetailAsync(CampusEvent evt)
        {
            if (evt != null && Shell.Current != null)
            {
                await Shell.Current.GoToAsync($"eventdetail?eventId={evt.ID}");
            }
        }

        [RelayCommand]
        public async Task ViewStudentProgressAsync(User student)
        {
            if (student == null || Shell.Current == null) return;

            string msg = $"Họ tên: {student.FullName}\n" +
                         $"Mã SV: {student.Username}\n" +
                         $"Lớp: {student.ClassName}\n" +
                         $"Chức vụ: {(student.IsSecretary ? "Bí thư chi đoàn" : "Sinh viên")}\n\n" +
                         $"• Điểm hiện tại: {student.TrainingPoints} ĐRL (Ban đầu: 0 ĐRL)\n" +
                         $"• Chỉ tiêu kỳ (50đ): {student.SemesterProgressPercent}% - {(student.IsSemesterQualified ? "✅ ĐÃ ĐẠT" : "❌ CHƯA ĐẠT")}\n" +
                         $"• Chỉ tiêu năm (250đ): {student.YearProgressPercent}% - {(student.IsYearQualified ? "✅ ĐÃ ĐẠT" : "❌ CHƯA ĐẠT")}";

            await Shell.Current.DisplayAlert("Tiến độ Điểm Rèn Luyện", msg, "Đóng");
        }
    }
}
