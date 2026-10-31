using System.Collections.ObjectModel;
using System.Text;
using CampusActivitiesManager.BusinessLogic.Services;
using CampusActivitiesManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class EventAttendanceReportViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IEventService _eventService;
        private readonly IAuthenticationService _authService;
        private int? _targetEventId;

        [ObservableProperty]
        private ObservableCollection<CampusEvent> _events = [];

        [ObservableProperty]
        private CampusEvent? _selectedEvent;

        [ObservableProperty]
        private bool _canManageReport = true;

        [ObservableProperty]
        private int _totalCheckedIn;

        [ObservableProperty]
        private int _qrCheckInCount;

        [ObservableProperty]
        private int _photoCheckInCount;

        [ObservableProperty]
        private int _pendingReviewCount;

        [ObservableProperty]
        private int _manualCheckInCount;

        [ObservableProperty]
        private string _attendanceRateDisplay = "0%";

        [ObservableProperty]
        private double _attendanceRateValue = 0;

        [ObservableProperty]
        private string _searchKeyword = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private ObservableCollection<AttendanceRecord> _records = [];

        // Photo Preview Modal
        [ObservableProperty]
        private bool _isPhotoPreviewVisible;

        [ObservableProperty]
        private AttendanceRecord? _previewRecord;

        private List<AttendanceRecord> _allRecords = [];

        public EventAttendanceReportViewModel(
            IAttendanceService attendanceService,
            IEventService eventService,
            IAuthenticationService authService)
        {
            _attendanceService = attendanceService;
            _eventService = eventService;
            _authService = authService;
        }

        [RelayCommand]
        public async Task AppearingAsync()
        {
            await LoadEventsAsync();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("eventId", out var idObj) && idObj != null)
            {
                if (int.TryParse(idObj.ToString(), out int eventId))
                {
                    _targetEventId = eventId;
                    if (Events.Count > 0)
                    {
                        var match = Events.FirstOrDefault(e => e.ID == eventId);
                        if (match != null)
                        {
                            SelectedEvent = match;
                        }
                    }
                }
            }
        }

        private async Task LoadEventsAsync()
        {
            try
            {
                IsBusy = true;
                var list = await _eventService.GetEventsAsync(includePending: _authService.IsAdmin);
                Events.Clear();
                foreach (var evt in list)
                {
                    Events.Add(evt);
                }

                if (_targetEventId.HasValue)
                {
                    var match = Events.FirstOrDefault(e => e.ID == _targetEventId.Value);
                    if (match != null)
                    {
                        SelectedEvent = match;
                        return;
                    }
                }

                if (Events.Count > 0 && SelectedEvent == null)
                {
                    SelectedEvent = Events[0];
                }
                else if (SelectedEvent != null)
                {
                    await LoadAttendanceDataAsync();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnSelectedEventChanged(CampusEvent? value)
        {
            if (value != null)
            {
                var currentUser = _authService.CurrentUser;
                CanManageReport = _authService.IsAdmin ||
                    (_authService.IsManager && (
                        string.Equals(value.CreatedByUsername, currentUser?.Username, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(value.CreatedByName, currentUser?.FullName, StringComparison.OrdinalIgnoreCase)));

                _ = LoadAttendanceDataAsync();
            }
        }

        [RelayCommand]
        public async Task RefreshAsync()
        {
            try
            {
                IsRefreshing = true;
                await LoadAttendanceDataAsync();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        public async Task LoadAttendanceDataAsync()
        {
            if (SelectedEvent == null) return;

            try
            {
                IsBusy = true;
                var list = await _attendanceService.GetEventAttendanceAsync(SelectedEvent.ID);
                _allRecords = list;

                TotalCheckedIn = list.Count(r => r.Status == "Approved" || r.Status == "Present");
                QrCheckInCount = list.Count(r => r.Method == "QR");
                PhotoCheckInCount = list.Count(r => r.Method == "Photo");
                PendingReviewCount = list.Count(r => r.Status == "PendingReview");
                ManualCheckInCount = list.Count(r => r.Method == "Manual");

                int capacity = SelectedEvent.MaxParticipants > 0 ? SelectedEvent.MaxParticipants : 100;
                double rate = Math.Clamp((double)TotalCheckedIn / capacity, 0.0, 1.0);
                AttendanceRateValue = rate;
                AttendanceRateDisplay = $"{TotalCheckedIn}/{capacity} ({rate * 100:0.0}%)";

                ApplySearch();
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnSearchKeywordChanged(string value)
        {
            ApplySearch();
        }

        private void ApplySearch()
        {
            Records.Clear();
            var query = _allRecords.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                var kw = SearchKeyword.Trim();
                query = query.Where(r => r.StudentCode.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         r.StudentName.Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var r in query)
            {
                Records.Add(r);
            }
        }

        [RelayCommand]
        public async Task ApproveAttendanceAsync(AttendanceRecord record)
        {
            if (record == null) return;

            if (!CanManageReport)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Phân quyền ⚠️", "Chỉ Quản trị viên (Admin) hoặc Quản lý của sự kiện này mới có quyền duyệt điểm danh!", "Đã hiểu");
                }
                return;
            }

            string reviewer = _authService.CurrentUser?.FullName ?? "Ban tổ chức";
            var (success, msg) = await _attendanceService.ReviewAttendanceAsync(record.Id, true, reviewer);

            if (success)
            {
                if (PreviewRecord?.Id == record.Id)
                {
                    IsPhotoPreviewVisible = false;
                    PreviewRecord = null;
                }
                await AppShell.DisplayToastAsync(msg);
                await LoadAttendanceDataAsync();
            }
            else if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Lỗi", msg, "Đóng");
            }
        }

        [RelayCommand]
        public async Task RejectAttendanceAsync(AttendanceRecord record)
        {
            if (record == null) return;

            if (!CanManageReport)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Phân quyền ⚠️", "Chỉ Quản trị viên (Admin) hoặc Quản lý của sự kiện này mới có quyền từ chối điểm danh!", "Đã hiểu");
                }
                return;
            }

            string reason = "Ảnh minh chứng không hợp lệ";
            if (Shell.Current != null)
            {
                var input = await Shell.Current.DisplayPromptAsync(
                    "Từ chối điểm danh ❌",
                    $"Nhập lý do từ chối điểm danh cho sinh viên {record.StudentCode} (sẽ trừ -{record.PointsDeducted} ĐRL):",
                    "Xác nhận",
                    "Hủy",
                    placeholder: "Ảnh không rõ mặt hoặc không tại sự kiện");

                if (input == null) return;
                if (!string.IsNullOrWhiteSpace(input)) reason = input.Trim();
            }

            string reviewer = _authService.CurrentUser?.FullName ?? "Ban tổ chức";
            var (success, msg) = await _attendanceService.ReviewAttendanceAsync(record.Id, false, reviewer, reason);

            if (success)
            {
                if (PreviewRecord?.Id == record.Id)
                {
                    IsPhotoPreviewVisible = false;
                    PreviewRecord = null;
                }
                await AppShell.DisplayToastAsync(msg);
                await LoadAttendanceDataAsync();
            }
            else if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Lỗi", msg, "Đóng");
            }
        }

        [RelayCommand]
        public void ViewProofPhoto(AttendanceRecord record)
        {
            if (record == null) return;
            PreviewRecord = record;
            IsPhotoPreviewVisible = true;
        }

        [RelayCommand]
        public void ClosePhotoPreview()
        {
            IsPhotoPreviewVisible = false;
            PreviewRecord = null;
        }

        [RelayCommand]
        public async Task ExportCsvAsync()
        {
            if (SelectedEvent == null) return;

            try
            {
                IsBusy = true;
                string csv = await _attendanceService.ExportAttendanceCsvAsync(SelectedEvent.ID);

                string fileName = $"DiemDanh_SuKien_{SelectedEvent.ID}_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                var preamble = Encoding.UTF8.GetPreamble();
                var csvBytes = Encoding.UTF8.GetBytes(csv);
                var fullBytes = new byte[preamble.Length + csvBytes.Length];
                Buffer.BlockCopy(preamble, 0, fullBytes, 0, preamble.Length);
                Buffer.BlockCopy(csvBytes, 0, fullBytes, preamble.Length, csvBytes.Length);

                await File.WriteAllBytesAsync(filePath, fullBytes);

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = $"Xuất Báo Cáo Điểm Danh: {SelectedEvent.Name}",
                    File = new ShareFile(filePath)
                });
            }
            catch (Exception ex)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Thông báo", $"Đã xuất file: {ex.Message}", "Đồng ý");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AddManualAttendanceAsync()
        {
            if (SelectedEvent == null || Shell.Current == null) return;

            if (!CanManageReport)
            {
                await Shell.Current.DisplayAlert("Phân quyền ⚠️", "Chỉ Quản trị viên (Admin) hoặc Quản lý của sự kiện này mới có quyền điểm danh bổ sung!", "Đã hiểu");
                return;
            }

            string studentCode = await Shell.Current.DisplayPromptAsync(
                "Điểm danh bổ sung",
                "Nhập Mã số sinh viên (VD: B21DCCN045):",
                "Xác nhận",
                "Hủy",
                placeholder: "B21DCCN045");

            if (string.IsNullOrWhiteSpace(studentCode)) return;

            var (success, msg, record) = await _attendanceService.MarkManualAttendanceAsync(
                SelectedEvent.ID,
                studentCode.Trim().ToUpperInvariant(),
                "Approved",
                SelectedEvent.TrainingPointsReward > 0 ? SelectedEvent.TrainingPointsReward : 10,
                "Điểm danh bổ sung bởi Ban tổ chức");

            if (success)
            {
                await LoadAttendanceDataAsync();
                await Shell.Current.DisplayAlert("Thành công 🎉", $"Đã ghi nhận điểm danh (+{SelectedEvent.TrainingPointsReward} ĐRL) cho sinh viên {studentCode}", "Đóng");
            }
            else
            {
                await Shell.Current.DisplayAlert("Lỗi", msg, "Đóng");
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
    }
}
