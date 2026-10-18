using System.Collections.ObjectModel;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class AttendanceHistoryViewModel : ObservableObject
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IAuthenticationService _authService;

        [ObservableProperty]
        private string _studentCode = "B20DCCN001";

        [ObservableProperty]
        private string _studentName = "Nguyễn An Cương";

        [ObservableProperty]
        private int _totalTrainingPoints = 92;

        [ObservableProperty]
        private string _pointRank = "Xuất Sắc";

        [ObservableProperty]
        private double _progressValue = 0.92;

        [ObservableProperty]
        private int _totalAttendedEvents = 0;

        [ObservableProperty]
        private string _selectedFilter = "Tất cả";

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private ObservableCollection<AttendanceRecord> _records = [];

        private List<AttendanceRecord> _allRecords = [];

        public AttendanceHistoryViewModel(
            IAttendanceService attendanceService,
            IAuthenticationService authService)
        {
            _attendanceService = attendanceService;
            _authService = authService;

            InitializeStudentInfo();
        }

        private void InitializeStudentInfo()
        {
            if (_authService?.CurrentUser != null)
            {
                var u = _authService.CurrentUser;
                StudentCode = string.IsNullOrWhiteSpace(u.Username) ? "B20DCCN001" : u.Username.ToUpperInvariant();
                StudentName = u.FullName;
            }
        }

        [RelayCommand]
        public async Task AppearingAsync()
        {
            InitializeStudentInfo();
            await LoadHistoryAsync();
        }

        [RelayCommand]
        public async Task RefreshAsync()
        {
            try
            {
                IsRefreshing = true;
                InitializeStudentInfo();
                await LoadHistoryAsync();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task LoadHistoryAsync()
        {
            try
            {
                IsBusy = true;
                var list = await _attendanceService.GetStudentAttendanceHistoryAsync(StudentCode);
                _allRecords = list;

                // Calculate total points and rank
                int points = 80; // Baseline points
                foreach (var r in list)
                {
                    if (r.Status == "Present")
                    {
                        points += r.TrainingPointsAwarded;
                    }
                }

                TotalTrainingPoints = Math.Min(points, 100);
                ProgressValue = TotalTrainingPoints / 100.0;
                TotalAttendedEvents = list.Count(r => r.Status == "Present");

                PointRank = TotalTrainingPoints switch
                {
                    >= 90 => "Xuất Sắc 🏆",
                    >= 80 => "Tốt 🌟",
                    >= 65 => "Khá 👍",
                    >= 50 => "Trung Bình ⚠️",
                    _ => "Yếu ❌"
                };

                ApplyFilter();
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void Filter(string filterMethod)
        {
            SelectedFilter = filterMethod;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            Records.Clear();
            var query = _allRecords.AsEnumerable();

            if (SelectedFilter == "Mã QR")
            {
                query = query.Where(r => r.Method == "QR");
            }
            else if (SelectedFilter == "Ảnh minh chứng")
            {
                query = query.Where(r => r.Method == "Photo");
            }
            else if (SelectedFilter == "Thủ công")
            {
                query = query.Where(r => r.Method == "Manual");
            }

            foreach (var r in query)
            {
                Records.Add(r);
            }
        }
    }
}
