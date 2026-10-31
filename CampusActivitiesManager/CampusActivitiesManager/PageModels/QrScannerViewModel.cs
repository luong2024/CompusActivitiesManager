using ZXing.Net.Maui;
using System.Collections.ObjectModel;
using CampusActivitiesManager.BusinessLogic.Services;
using CampusActivitiesManager.Models;

using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;



namespace CampusActivitiesManager.PageModels

{

    public partial class QrScannerViewModel : ObservableObject, IQueryAttributable

    {

        private readonly IAttendanceService _attendanceService;

        private readonly IEventService _eventService;

        private readonly IAuthenticationService _authService;

        private readonly IQrCodeService _qrCodeService;

        private int? _targetEventId;



        [ObservableProperty]

        private string _studentCode = "B20DCCN001";



        [ObservableProperty]

        private string _studentName = "Nguyễn An Cương";



        [ObservableProperty]

        private string _qrInput = string.Empty;



        [ObservableProperty]

        private bool _isBusy;



        [ObservableProperty]

        private bool _hasResult;



        [ObservableProperty]

        private bool _isSuccess;



        [ObservableProperty]

        private string _statusMessage = string.Empty;



        [ObservableProperty]

        private AttendanceRecord? _lastRecord;



        [ObservableProperty]

        private ObservableCollection<CampusEvent> _activeEvents = [];



        [ObservableProperty]

        private CampusEvent? _selectedEvent;

        [ObservableProperty]
        private bool _isCameraPermissionGranted;

        [ObservableProperty]
        private bool _isDetecting = true;

        [ObservableProperty]
        private bool _isTorchOn;

        [ObservableProperty]
        private ZXing.Net.Maui.CameraLocation _selectedCameraLocation = ZXing.Net.Maui.CameraLocation.Rear;

        [ObservableProperty]
        private string _torchButtonText = "⚡ Bật Flash";

        private DateTime _lastScanTime = DateTime.MinValue;
        private string _lastScannedValue = string.Empty;




        public QrScannerViewModel(

            IAttendanceService attendanceService,

            IEventService eventService,

            IAuthenticationService authService,

            IQrCodeService qrCodeService)

        {

            _attendanceService = attendanceService;

            _eventService = eventService;

            _authService = authService;

            _qrCodeService = qrCodeService;



            InitializeStudentInfo();

        }



        public void ApplyQueryAttributes(IDictionary<string, object> query)

        {

            if (query.TryGetValue("eventId", out var eventIdObj) && eventIdObj != null)

            {

                if (int.TryParse(eventIdObj.ToString(), out int id))

                {

                    _targetEventId = id;

                    SelectTargetEventIfLoaded();

                }

            }

        }



        private void SelectTargetEventIfLoaded()

        {

            if (_targetEventId.HasValue && ActiveEvents.Count > 0)

            {

                var match = ActiveEvents.FirstOrDefault(e => e.ID == _targetEventId.Value);

                if (match != null)

                {

                    SelectedEvent = match;

                }

            }

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

                if (ActiveEvents.Count > 0)

                {

                    if (_targetEventId.HasValue)

                    {

                        var match = ActiveEvents.FirstOrDefault(e => e.ID == _targetEventId.Value);

                        SelectedEvent = match ?? ActiveEvents[0];

                    }

                    else if (SelectedEvent == null)

                    {

                        SelectedEvent = ActiveEvents[0];

                    }

                }

            }

            finally

            {

                IsBusy = false;

            }

        }



        [RelayCommand]

        public async Task ProcessQrCodeAsync(string? payload = null)

        {

            var code = !string.IsNullOrWhiteSpace(payload) ? payload : QrInput;

            if (string.IsNullOrWhiteSpace(code))

            {

                if (Shell.Current != null)

                {

                    await Shell.Current.DisplayAlert("Thông báo", "Vui lòng nhập hoặc quét mã QR đường dẫn sự kiện hợp lệ.", "Đóng");

                }

                return;

            }



            try

            {

                IsBusy = true;

                HasResult = false;



                int eventId = SelectedEvent?.ID ?? 1;



                // Parse eventId from code (supports: "1", "EVT:1", "event/1", "campus://event/1", "https://campus.edu.vn/events/1")

                var cleanCode = code.Trim();

                if (int.TryParse(cleanCode, out int directId))

                {

                    eventId = directId;

                }

                else if (cleanCode.Contains("EVT:"))

                {

                    var parts = cleanCode.Split('|');

                    foreach (var part in parts)

                    {

                        if (part.StartsWith("EVT:") && int.TryParse(part.Replace("EVT:", ""), out int parsedId))

                        {

                            eventId = parsedId;

                        }

                    }

                }

                else if (cleanCode.Contains("event/"))

                {

                    var segment = cleanCode.Substring(cleanCode.LastIndexOf("event/") + 6);

                    if (int.TryParse(segment, out int parsedId))

                    {

                        eventId = parsedId;

                    }

                }



                if (Shell.Current != null)

                {

                    await Shell.Current.GoToAsync($"eventdetail?eventId={eventId}");

                }

            }

            finally

            {

                IsBusy = false;

            }

        }



        [RelayCommand]

        public async Task OpenSelectedEventDetailAsync()

        {

            if (SelectedEvent != null && Shell.Current != null)

            {

                await Shell.Current.GoToAsync($"eventdetail?eventId={SelectedEvent.ID}");

            }

        }



        [RelayCommand]

        public async Task SimulateQuickScanAsync()

        {

            int eventId = SelectedEvent?.ID ?? 1;

            QrInput = $"campus://event/{eventId}";

            await ProcessQrCodeAsync(QrInput);

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

        public void ResetScan()

        {

            HasResult = false;

            LastRecord = null;

            QrInput = string.Empty;

            StatusMessage = string.Empty;

        }

[RelayCommand]
        public async Task InitializeCameraPermissionAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                }

                IsCameraPermissionGranted = (status == PermissionStatus.Granted);
                IsDetecting = IsCameraPermissionGranted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Camera permission error: {ex.Message}");
                IsCameraPermissionGranted = false;
                IsDetecting = false;
            }
        }

        [RelayCommand]
        public void ToggleTorch()
        {
            IsTorchOn = !IsTorchOn;
            TorchButtonText = IsTorchOn ? "💡 Tắt Flash" : "⚡ Bật Flash";
        }

        [RelayCommand]
        public void SwitchCameraLocation()
        {
            SelectedCameraLocation = (SelectedCameraLocation == ZXing.Net.Maui.CameraLocation.Rear) ? ZXing.Net.Maui.CameraLocation.Front : ZXing.Net.Maui.CameraLocation.Rear;
        }

        public async Task ProcessScannedBarcodeAsync(string rawBarcode)
        {
            if (string.IsNullOrWhiteSpace(rawBarcode)) return;

            // Debounce: prevent duplicate scan within 2.5 seconds
            var now = DateTime.UtcNow;
            if (_lastScannedValue == rawBarcode && (now - _lastScanTime).TotalSeconds < 2.5)
            {
                return;
            }

            _lastScanTime = now;
            _lastScannedValue = rawBarcode;

            try
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
            catch { }

            QrInput = rawBarcode;
            await ProcessQrCodeAsync(rawBarcode);
    }
}
}
