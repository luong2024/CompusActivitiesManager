using System.Collections.ObjectModel;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
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

        [RelayCommand]
        public async Task CapturePhotoAsync()
        {
            try
            {
                FileResult? photo = null;

                if (MediaPicker.Default.IsCaptureSupported)
                {
                    photo = await MediaPicker.Default.CapturePhotoAsync();
                }
                else
                {
                    // Fallback to PickPhotoAsync on emulators / Windows without physical cameras
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
            using var stream = await photo.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            CapturedPhotoBytes = ms.ToArray();
            CapturedPhotoSource = ImageSource.FromStream(() => new MemoryStream(CapturedPhotoBytes));
            HasCapturedPhoto = true;

            if (string.IsNullOrWhiteSpace(Notes))
            {
                Notes = $"Điểm danh ảnh thực tế tại {SelectedEvent?.Location ?? "hội trường"} lúc {DateTime.Now:HH:mm dd/MM/yyyy}";
            }
        }

        [RelayCommand]
        public void LoadSampleProofPhoto()
        {
            // Sample photo simulation for quick testing in emulator / desktop
            CapturedPhotoBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
            CapturedPhotoSource = "dotnet_bot.png";
            HasCapturedPhoto = true;
            Notes = $"Hình ảnh minh chứng check-in tham gia tại {SelectedEvent?.Location ?? "Sân trường"} (Bản mẫu kiểm thử)";
        }

        [RelayCommand]
        public async Task SubmitPhotoAttendanceAsync()
        {
            if (SelectedEvent == null)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Thông báo", "Vui lòng chọn sự kiện tham dự", "Đóng");
                }
                return;
            }

            // Kiểm tra ràng buộc thời gian: Sau giờ kết thúc KHÔNG THỂ ĐIỂM DANH
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

            // Kiểm tra ràng buộc thời gian: Chưa tới giờ bắt đầu KHÔNG THỂ ĐIỂM DANH
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

            // Ràng buộc phê duyệt: Hoạt động chưa được Quản trị viên duyệt không được điểm danh
            if (!SelectedEvent.IsApproved)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert(
                        "Chưa được phê duyệt ⏳",
                        $"Hoạt động \"{SelectedEvent.Name}\" đang chờ Quản trị viên (Admin) duyệt trước khi mở điểm danh.",
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
                string base64 = Convert.ToBase64String(CapturedPhotoBytes);
                var (success, msg, record) = await _attendanceService.CheckInWithPhotoAsync(SelectedEvent.ID, StudentCode, base64, Notes);

                IsSuccess = success;
                StatusMessage = msg;

                if (success && Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert(
                        "Điểm danh thành công! 🎉",
                        $"Ảnh minh chứng của bạn đã được ghi nhận vào hệ thống!\n" +
                        $"Sự kiện: {SelectedEvent.Name}\n" +
                        $"Quyền lợi: +{SelectedEvent.PointsReward} Điểm rèn luyện (khi duyệt)\n" +
                        $"Thời gian: {DateTime.Now:HH:mm dd/MM/yyyy}",
                        "Tuyệt vời");

                    ClearPhoto();
                }
                else if (!success && Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Không thể điểm danh ⚠️", msg, "Đóng");
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
