using System.Collections.ObjectModel;
using CampusActivitiesManager.BusinessLogic.Services;
using CampusActivitiesManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class PhotoAttendanceViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IEventService _eventService;
        private readonly IAuthenticationService _authService;
        private int? _targetEventId;

        [ObservableProperty]
        private string _studentCode = "B20DCCN001";

        [ObservableProperty]
        private string _studentName = "Nguyễn An Cương";

        [ObservableProperty]
        private ObservableCollection<CampusEvent> _activeEvents = [];

        [ObservableProperty]
        private CampusEvent? _selectedEvent;

        partial void OnSelectedEventChanged(CampusEvent? value)
        {
            _ = CheckExistingAttendanceAsync();
        }

        [ObservableProperty]
        private ImageSource? _capturedPhotoSource;

        [ObservableProperty]
        private byte[]? _capturedPhotoBytes;

        [ObservableProperty]
        private bool _hasCapturedPhoto;

        [ObservableProperty]
        private bool _hasNoCapturedPhoto = true;

        partial void OnHasCapturedPhotoChanged(bool value)
        {
            HasNoCapturedPhoto = !value;
        }

        [ObservableProperty]
        private string _notes = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isSuccess;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        // Trạng thái Điểm danh thành công & Phê duyệt
        [ObservableProperty]
        private bool _isAlreadyCheckedIn;

        [ObservableProperty]
        private AttendanceRecord? _currentAttendanceRecord;

        [ObservableProperty]
        private string _approvalStatusBadge = string.Empty;

        [ObservableProperty]
        private string _approvalStatusText = string.Empty;

        [ObservableProperty]
        private string _approvalStatusSubtitle = string.Empty;

        [ObservableProperty]
        private Color _approvalStatusColor = Color.FromArgb("#16A34A");

        [ObservableProperty]
        private Color _approvalStatusBg = Color.FromArgb("#DCFCE7");

        [ObservableProperty]
        private bool _canReSubmitProof;

        public PhotoAttendanceViewModel(
            IAttendanceService attendanceService,
            IEventService eventService,
            IAuthenticationService authService)
        {
            _attendanceService = attendanceService;
            _eventService = eventService;
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
            await LoadEventsAsync();
            await CheckExistingAttendanceAsync();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("eventId", out var idObj) && idObj != null)
            {
                if (int.TryParse(idObj.ToString(), out int eventId))
                {
                    _targetEventId = eventId;
                    if (ActiveEvents.Count > 0)
                    {
                        var match = ActiveEvents.FirstOrDefault(e => e.ID == eventId);
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
                var list = await _eventService.GetEventsAsync();
                ActiveEvents.Clear();
                foreach (var evt in list)
                {
                    ActiveEvents.Add(evt);
                }

                if (_targetEventId.HasValue)
                {
                    var match = ActiveEvents.FirstOrDefault(e => e.ID == _targetEventId.Value);
                    if (match != null)
                    {
                        SelectedEvent = match;
                        return;
                    }
                }

                if (ActiveEvents.Count > 0 && SelectedEvent == null)
                {
                    SelectedEvent = ActiveEvents[0];
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task CheckExistingAttendanceAsync()
        {
            if (SelectedEvent == null)
            {
                IsAlreadyCheckedIn = false;
                CurrentAttendanceRecord = null;
                return;
            }

            try
            {
                var history = await _attendanceService.GetStudentAttendanceHistoryAsync(StudentCode);
                var match = history.FirstOrDefault(r => r.EventId == SelectedEvent.ID);
                if (match != null)
                {
                    CurrentAttendanceRecord = match;
                    IsAlreadyCheckedIn = true;
                    UpdateApprovalStatusDisplay(match);
                }
                else
                {
                    CurrentAttendanceRecord = null;
                    IsAlreadyCheckedIn = false;
                }
            }
            catch
            {
                // Fallback gracefully
            }
        }

        public void UpdateApprovalStatusDisplay(AttendanceRecord record)
        {
            if (record.Status == "Approved")
            {
                ApprovalStatusBadge = $"✅ ĐÃ PHÊ DUYỆT (+{record.TrainingPointsAwarded} ĐRL)";
                ApprovalStatusText = "Điểm danh của bạn đã được phê duyệt thành công!";
                ApprovalStatusSubtitle = $"Đã cộng +{record.TrainingPointsAwarded} Điểm Rèn Luyện vào hồ sơ sinh viên của bạn.";
                ApprovalStatusColor = Color.FromArgb("#16A34A");
                ApprovalStatusBg = Color.FromArgb("#DCFCE7");
                CanReSubmitProof = false;
            }
            else if (record.Status == "Rejected")
            {
                ApprovalStatusBadge = "❌ MINH CHỨNG BỊ TỪ CHỐI";
                ApprovalStatusText = "Minh chứng điểm danh không được chấp thuận";
                ApprovalStatusSubtitle = string.IsNullOrWhiteSpace(record.ReviewNotes)
                    ? "Hình ảnh minh chứng không rõ ràng hoặc không hợp lệ. Vui lòng chụp lại ảnh mới để gửi xét duyệt lại."
                    : $"Lý do từ chối: {record.ReviewNotes}. Bạn có thể chụp ảnh mới và gửi lại.";
                ApprovalStatusColor = Color.FromArgb("#DC2626");
                ApprovalStatusBg = Color.FromArgb("#FEE2E2");
                CanReSubmitProof = true;
            }
            else // "PendingReview"
            {
                ApprovalStatusBadge = "⏳ ĐANG CHỜ PHÊ DUYỆT";
                ApprovalStatusText = "Đã gửi minh chứng thành công! Đang chờ duyệt";
                ApprovalStatusSubtitle = $"Hệ thống đã ghi nhận ảnh chụp lúc {record.FormattedCheckInTime}. Ban tổ chức / Quản trị viên sẽ kiểm tra và cộng +{record.TrainingPointsAwarded} ĐRL khi đạt yêu cầu.";
                ApprovalStatusColor = Color.FromArgb("#D97706");
                ApprovalStatusBg = Color.FromArgb("#FEF3C7");
                CanReSubmitProof = true;
            }
        }

        [RelayCommand]
        public void ReSubmitProof()
        {
            IsAlreadyCheckedIn = false;
            CapturedPhotoBytes = null;
            CapturedPhotoSource = null;
            HasCapturedPhoto = false;
            Notes = $"Cập nhật lại ảnh minh chứng cho {SelectedEvent?.Name ?? "sự kiện"}";
        }

        [RelayCommand]
        public async Task GoToHistoryAsync()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("attendancehistory");
            }
        }

        [RelayCommand]
        public async Task BackToHomeAsync()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//homeTab/main");
            }
        }

        [RelayCommand]
        public async Task CapturePhotoAsync()
        {
            if (IsBusy) return;

            try
            {
                FileResult? photo = null;

                if (MediaPicker.Default.IsCaptureSupported)
                {
                    photo = await MediaPicker.Default.CapturePhotoAsync();
                }
                else
                {
                    photo = await MediaPicker.Default.PickPhotoAsync();
                }

                if (photo != null)
                {
                    await LoadPhotoFileAsync(photo);
                }
            }
            catch (Exception ex)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Thông báo", $"Không thể mở máy ảnh: {ex.Message}. Bạn có thể chọn ảnh từ thư viện.", "Đồng ý");
                }
            }
        }

        [RelayCommand]
        public async Task PickPhotoAsync()
        {
            if (IsBusy) return;

            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo != null)
                {
                    await LoadPhotoFileAsync(photo);
                }
            }
            catch (Exception ex)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Thông báo", $"Không thể chọn ảnh: {ex.Message}", "Đồng ý");
                }
            }
        }

        private async Task LoadPhotoFileAsync(FileResult photo)
        {
            try
            {
                IsBusy = true;
                var bytes = await Task.Run(async () =>
                {
                    using var stream = await photo.OpenReadAsync();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    return ms.ToArray();
                });

                CapturedPhotoBytes = bytes;
                CapturedPhotoSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                HasCapturedPhoto = true;

                if (string.IsNullOrWhiteSpace(Notes))
                {
                    Notes = $"Điểm danh ảnh thực tế tại {SelectedEvent?.Location ?? "hội trường"} lúc {DateTime.Now:HH:mm dd/MM/yyyy}";
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SubmitPhotoAttendanceAsync()
        {
            if (IsBusy) return;

            if (SelectedEvent == null)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Thông báo", "Vui lòng chọn sự kiện tham dự", "Đóng");
                }
                return;
            }

            if (SelectedEvent.IsEventEnded)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert(
                        "Hết hạn điểm danh ⛔",
                        $"Sự kiện \"{SelectedEvent.Name}\" đã kết thúc vào lúc {SelectedEvent.EndTime} ngày {SelectedEvent.EndDate}.\n\nBạn không thể điểm danh sau thời gian kết thúc sự kiện!",
                        "Đóng");
                }
                return;
            }

            if (SelectedEvent.IsEventNotStarted)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert(
                        "Chưa mở điểm danh ⏳",
                        $"Sự kiện \"{SelectedEvent.Name}\" chưa bắt đầu.\n\nKhung giờ tổ chức: {SelectedEvent.StartTime} - {SelectedEvent.EndTime} ngày {SelectedEvent.StartDate}.\nVui lòng quay lại trong khung giờ sự kiện diễn ra!",
                        "Đóng");
                }
                return;
            }

            if (!SelectedEvent.IsApproved)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert(
                        "Chưa được phê duyệt ⏳",
                        $"Hoạt động \"{SelectedEvent.Name}\" đang chờ Quản trị viên duyệt trước khi mở điểm danh.",
                        "Đóng");
                }
                return;
            }

            if (!HasCapturedPhoto || CapturedPhotoBytes == null)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Thông báo", "Vui lòng chụp hoặc chọn ảnh minh chứng có mặt tại sự kiện", "Đóng");
                }
                return;
            }

            try
            {
                IsBusy = true;
                string base64 = await Task.Run(() => Convert.ToBase64String(CapturedPhotoBytes));
                var (success, msg, record) = await _attendanceService.CheckInWithPhotoAsync(SelectedEvent.ID, StudentCode, base64, Notes);

                IsSuccess = success;
                StatusMessage = msg;

                if (success && record != null)
                {
                    CurrentAttendanceRecord = record;
                    UpdateApprovalStatusDisplay(record);
                    IsAlreadyCheckedIn = true;

                    CapturedPhotoBytes = null;
                    CapturedPhotoSource = null;
                    HasCapturedPhoto = false;
                    Notes = string.Empty;

                    if (Shell.Current != null)
                    {
                        await Shell.Current.DisplayAlert(
                            "Điểm danh thành công! 🎉",
                            $"Minh chứng hình ảnh đã được gửi lên hệ thống.\n\n• Sự kiện: {SelectedEvent.Name}\n• Trạng thái: Đang chờ phê duyệt (+{SelectedEvent.PointsReward} ĐRL khi duyệt)\n• Thời gian: {DateTime.Now:HH:mm dd/MM/yyyy}",
                            "Xem trạng thái duyệt");
                    }
                }
                else if (!success && Shell.Current != null)
                {
                    if (record != null)
                    {
                        CurrentAttendanceRecord = record;
                        UpdateApprovalStatusDisplay(record);
                        IsAlreadyCheckedIn = true;
                    }
                    await Shell.Current.DisplayAlert("Thông báo ⚠️", msg, "Đóng");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void ClearPhoto()
        {
            CapturedPhotoBytes = null;
            CapturedPhotoSource = null;
            HasCapturedPhoto = false;
            Notes = string.Empty;
            StatusMessage = string.Empty;
        }
    }
}
