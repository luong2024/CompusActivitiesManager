using System.Collections.ObjectModel;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
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
                    await Shell.Current.DisplayAlert("Thông báo", "Vui lòng nhập hoặc quét mã QR hợp lệ", "Đóng");
                }
                return;
            }

            try
            {
                IsBusy = true;
                HasResult = false;

                int eventId = SelectedEvent?.ID ?? 1;

                // If QR code contains metadata format "CAMPUS|EVT:{id}|STU:{code}..."
                if (code.Contains("EVT:"))
                {
                    var parts = code.Split('|');
                    foreach (var part in parts)
                    {
                        if (part.StartsWith("EVT:") && int.TryParse(part.Replace("EVT:", ""), out int parsedId))
                        {
                            eventId = parsedId;
                        }
                    }
                }

                var (success, msg, record) = await _attendanceService.CheckInWithQrAsync(eventId, StudentCode, code.Trim());
                IsSuccess = success;
                StatusMessage = msg;
                LastRecord = record;
                HasResult = true;

                if (success && Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert(
                        "Điểm danh thành công! 🎉",
                        $"{msg}\nSự kiện: {record?.EventName ?? SelectedEvent?.Name}\nThời gian: {DateTime.Now:HH:mm dd/MM/yyyy}\nĐiểm rèn luyện: +5 ĐRL",
                        "Xác nhận");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SimulateQuickScanAsync()
        {
            int eventId = SelectedEvent?.ID ?? 1;
            var ticketCode = _qrCodeService.GenerateTicketPayload(eventId, StudentCode, SelectedEvent?.Name ?? "Sự kiện");
            QrInput = ticketCode;
            await ProcessQrCodeAsync(ticketCode);
        }

        [RelayCommand]
        public void ResetScan()
        {
            HasResult = false;
            LastRecord = null;
            QrInput = string.Empty;
            StatusMessage = string.Empty;
        }
    }
}
