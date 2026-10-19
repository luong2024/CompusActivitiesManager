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
                bool isLecturer = currentUser?.Role == Role.Lecturer || currentUser?.Role == Role.Manager;

                CanCreateClassEvent = IsAdmin || isSecretary || isLecturer;

                UserRoleTitle = currentUser?.Role switch
                {
                    Role.Admin => "Quản trị viên (Toàn quyền quản lý)",
                    Role.Lecturer => "Giảng viên / Cố vấn học tập",
                    Role.Secretary => "Bí thư chi đoàn lớp",
                    _ => "Sinh viên lớp"
                };

                AvailableClasses = await _userService.GetClassesAsync();

                if (SelectedClass == null)
                {
                    string targetClassName = currentUser?.ClassName ?? "D20CNTT1";
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

                // 1. Tải danh sách sinh viên của lớp
                var students = await _userService.GetStudentsByClassAsync(SelectedClass.ClassName);
                StudentsList.Clear();
                foreach (var s in students)
                {
                    StudentsList.Add(s);
                }

                TotalStudents = students.Count;
                QualifiedSemesterCount = students.Count(s => s.IsSemesterQualified);
                QualifiedYearCount = students.Count(s => s.IsYearQualified);

                int rate = TotalStudents > 0 ? (int)((double)QualifiedSemesterCount / TotalStudents * 100) : 0;
                SemesterQualificationRate = $"{rate}%";

                // 2. Tải hoạt động riêng của lớp
                var allEvents = await _eventService.GetEventsAsync(includePending: IsAdmin);
                var classEvents = allEvents.Where(e =>
                    e.Scope == EventScope.ClassOnly &&
                    string.Equals(e.TargetClass, SelectedClass.ClassName, StringComparison.OrdinalIgnoreCase)).ToList();

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
                Title = title.Trim(),
                CategoryID = 4,
                Category = new EventCategoryDto { ID = 4, Title = "Hoạt động Lớp", Color = "#7C3AED" },
                Location = $"Phòng học lớp {targetClass}",
                StartDate = DateTime.UtcNow.AddDays(3).ToString("dd/MM/yyyy HH:mm"),
                EndDate = DateTime.UtcNow.AddDays(3).AddHours(2).ToString("dd/MM/yyyy HH:mm"),
                OrganizerName = _authService.CurrentUser?.FullName ?? "Ban cán sự lớp",
                Description = $"Hoạt động rèn luyện nội bộ của chi đoàn/lớp {targetClass}. Chỉ sinh viên trong lớp mới có thể tham gia và ghi nhận điểm rèn luyện.",
                Scope = EventScope.ClassOnly,
                TargetClass = targetClass,
                RegistrationType = EventRegistrationType.Auto,
                PointsReward = points,
                PointsPenalty = 5,
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
