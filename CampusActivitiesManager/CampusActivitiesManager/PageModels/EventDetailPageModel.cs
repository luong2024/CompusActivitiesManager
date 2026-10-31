#nullable disable
using System.Collections.ObjectModel;
using CampusActivitiesManager.BusinessLogic.Services;
using CampusActivitiesManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class EventDetailPageModel : ObservableObject, IQueryAttributable
    {
        private readonly IEventService _eventService;
        private readonly IAuthenticationService _authService;
        private readonly IAttendanceService _attendanceService;

        [ObservableProperty]
        private CampusEvent _eventItem;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isNotLoading = true;

        partial void OnIsLoadingChanged(bool value)
        {
            IsNotLoading = !value;
        }

        [ObservableProperty]
        private bool _isAdmin;

        [ObservableProperty]
        private bool _isManager;

        [ObservableProperty]
        private bool _isManagerOrAdmin;

        [ObservableProperty]
        private bool _canManageEvent;

        [ObservableProperty]
        private bool _canRegister;

        [ObservableProperty]
        private bool _canCancel;

        [ObservableProperty]
        private bool _canCheckInPhoto;

        [ObservableProperty]
        private bool _canAdminApprove;

        [ObservableProperty]
        private bool _hasVideo;

        [ObservableProperty]
        private string _userRegistrationStatus = "Chưa đăng ký";

        [ObservableProperty]
        private Color _registrationStatusColor = Color.FromArgb("#64748B");

        [ObservableProperty]
        private ObservableCollection<EventRegistration> _registrationsList = [];

        [ObservableProperty]
        private int _totalRegistrations;

        public EventDetailPageModel(
            IEventService eventService,
            IAuthenticationService authService,
            IAttendanceService attendanceService)
        {
            _eventService = eventService;
            _authService = authService;
            _attendanceService = attendanceService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            object idObj = null;
            if (query.TryGetValue("eventId", out idObj) ||
                query.TryGetValue("EventId", out idObj) ||
                query.TryGetValue("id", out idObj) ||
                query.TryGetValue("ID", out idObj))
            {
                if (idObj != null && int.TryParse(idObj.ToString(), out int eventId))
                {
                    _ = LoadEventAsync(eventId);
                }
            }
        }

        [RelayCommand]
        public async Task LoadEventAsync(int eventId)
        {
            try
            {
                IsLoading = true;
                var evt = await _eventService.GetEventByIdAsync(eventId);
                if (evt == null)
                {
                    if (Shell.Current != null)
                    {
                        await Shell.Current.DisplayAlert("Lỗi", "Không tìm thấy thông tin hoạt động.", "Đóng");
                        await Shell.Current.GoToAsync("..");
                    }
                    return;
                }

                EventItem = evt;
                HasVideo = !string.IsNullOrWhiteSpace(evt.VideoUrl);

                var currentUser = _authService.CurrentUser;
                IsAdmin = _authService.IsAdmin;
                IsManager = _authService.IsManager;
                IsManagerOrAdmin = IsAdmin || IsManager;

                bool isCreator = string.Equals(evt.CreatedByUsername, currentUser?.Username, StringComparison.OrdinalIgnoreCase);
                bool isStaff = IsAdmin || IsManager || currentUser?.Role == Role.Lecturer || currentUser?.Role == Role.Secretary;

                // Sinh viên thường không được xem hoạt động chưa duyệt (trừ người tạo, ban tổ chức, giảng viên, bí thư)
                if (!isStaff && !isCreator && (!evt.IsApproved || evt.ApprovalStatus != "Approved"))
                {
                    if (Shell.Current != null)
                    {
                        await Shell.Current.DisplayAlert("Thông báo 🛡️", "Hoạt động này đang chờ Quản trị viên (Admin) phê duyệt và chưa được công khai cho sinh viên.", "Quay lại");
                        await Shell.Current.GoToAsync("..");
                    }
                    return;
                }

                UpdateRegistrationState();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateRegistrationState()
        {
            if (EventItem == null) return;

            var currentUser = _authService.CurrentUser;
            string studentId = currentUser?.Username ?? "B20DCCN001";

            var myReg = EventItem.GetUserRegistration(studentId);

            // Quyền quản lý sự kiện: Admin có toàn quyền, Quản lý sự kiện chỉ có quyền trong sự kiện do mình tạo
            CanManageEvent = IsAdmin || (IsManager && (
                string.Equals(EventItem.CreatedByUsername, currentUser?.Username, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(EventItem.CreatedByName, currentUser?.FullName, StringComparison.OrdinalIgnoreCase)
            ));

            // Nút duyệt của Admin chỉ hiển thị khi người dùng là Admin VÀ sự kiện đang ở trạng thái Pending
            CanAdminApprove = IsAdmin && EventItem.IsPendingApproval;

            RegistrationsList.Clear();
            foreach (var reg in EventItem.Registrations)
            {
                RegistrationsList.Add(reg);
            }
            TotalRegistrations = EventItem.Registrations.Count(r => r.Status != "Cancelled");

            if (myReg == null || myReg.Status == "Cancelled")
            {
                CanCancel = false;
                CanCheckInPhoto = false;

                // 1. Kiểm tra nếu hoạt động đã kết thúc
                if (EventItem.IsEventEnded)
                {
                    UserRegistrationStatus = "Hoạt động đã kết thúc (Đóng đăng ký)";
                    RegistrationStatusColor = Color.FromArgb("#94A3B8");
                    CanRegister = false;
                }
                // 2. Kiểm tra nếu là hoạt động nội bộ lớp (ClassOnly) và sinh viên khác lớp
                else if (string.Equals(EventItem.Scope, "ClassOnly", StringComparison.OrdinalIgnoreCase) &&
                         !string.IsNullOrWhiteSpace(EventItem.TargetClass) &&
                         !IsAdmin &&
                         !string.Equals(currentUser?.AssignedClass, EventItem.TargetClass, StringComparison.OrdinalIgnoreCase))
                {
                    UserRegistrationStatus = $"Hoạt động nội bộ chỉ dành riêng cho lớp {EventItem.TargetClass}";
                    RegistrationStatusColor = Color.FromArgb("#DC2626");
                    CanRegister = false;
                }
                // 3. Kiểm tra số lượng người tham gia tối đa (Capacity check)
                else if (!EventItem.IsUnlimitedParticipants && EventItem.MaxParticipants > 0 && EventItem.RegisteredCount >= EventItem.MaxParticipants)
                {
                    UserRegistrationStatus = $"Hoạt động đã đủ số lượng tham gia ({EventItem.RegisteredCount}/{EventItem.MaxParticipants})";
                    RegistrationStatusColor = Color.FromArgb("#EF4444");
                    CanRegister = false;
                }
                // 4. Kiểm tra sự kiện chưa được Admin duyệt (nếu không phải Admin)
                else if (EventItem.ApprovalStatus != "Approved" && !IsAdmin)
                {
                    UserRegistrationStatus = "Hoạt động đang chờ Quản trị viên phê duyệt";
                    RegistrationStatusColor = Color.FromArgb("#D97706");
                    CanRegister = false;
                }
                // 5. Thỏa mãn mọi điều kiện
                else
                {
                    UserRegistrationStatus = "Chưa đăng ký tham gia";
                    RegistrationStatusColor = Color.FromArgb("#64748B");
                    CanRegister = true;
                }
            }
            else if (myReg.Status == "Pending")
            {
                UserRegistrationStatus = "⏳ Đang chờ Ban tổ chức phê duyệt";
                RegistrationStatusColor = Color.FromArgb("#D97706");
                CanRegister = false;
                CanCancel = true; // Người dùng có thể hủy đăng ký trước khi được duyệt!
                CanCheckInPhoto = false;
            }
            else if (myReg.Status == "Approved")
            {
                UserRegistrationStatus = "✅ Đã phê duyệt tham gia";
                RegistrationStatusColor = Color.FromArgb("#16A34A");
                CanRegister = false;
                CanCancel = false;
                // Điểm danh chỉ khả dụng khi sự kiện đã duyệt và CHƯA kết thúc
                CanCheckInPhoto = !EventItem.IsEventEnded && !EventItem.IsEventNotStarted;
            }
            else if (myReg.Status == "Rejected")
            {
                UserRegistrationStatus = "❌ Yêu cầu tham gia bị từ chối";
                RegistrationStatusColor = Color.FromArgb("#DC2626");
                CanRegister = false;
                CanCancel = false;
                CanCheckInPhoto = false;
            }
        }

        [RelayCommand]
        public async Task RegisterAsync()
        {
            if (EventItem == null || _authService.CurrentUser == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Đăng ký tham gia hoạt động",
                $"Bạn muốn đăng ký tham gia \"{EventItem.Title}\"?\n\n" +
                $"• Điểm thưởng khi điểm danh đạt: +{EventItem.PointsReward} ĐRL\n" +
                $"• Điểm phạt nếu vắng/từ chối: -{EventItem.PointsPenalty} ĐRL\n" +
                $"• Chế độ: {(EventItem.RegistrationType == EventRegistrationType.Auto ? "Tự động duyệt" : "Cần Ban tổ chức xét duyệt")}",
                "Xác nhận", "Hủy");

            if (!confirm) return;

            try
            {
                IsLoading = true;
                var (success, msg, status) = await _eventService.RegisterUserForEventAsync(EventItem.ID, _authService.CurrentUser);

                await Shell.Current.DisplayAlert(success ? "Thành công 🎉" : "Thông báo", msg, "Đồng ý");
                await LoadEventAsync(EventItem.ID);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task CancelRegistrationAsync()
        {
            if (EventItem == null || _authService.CurrentUser == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Hủy đăng ký",
                "Bạn có chắc chắn muốn hủy đăng ký tham gia hoạt động này?",
                "Hủy đăng ký", "Quay lại");

            if (!confirm) return;

            try
            {
                IsLoading = true;
                string studentId = _authService.CurrentUser.Username;
                var (success, msg) = await _eventService.CancelRegistrationAsync(EventItem.ID, studentId);

                await Shell.Current.DisplayAlert(success ? "Đã hủy" : "Thông báo", msg, "Đồng ý");
                await LoadEventAsync(EventItem.ID);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task GoToPhotoCheckInAsync()
        {
            if (EventItem == null) return;

            if (EventItem.IsEventEnded)
            {
                await Shell.Current.DisplayAlert(
                    "Hết hạn điểm danh ⛔",
                    $"Sự kiện \"{EventItem.Name}\" đã kết thúc vào lúc {EventItem.EndTime} ngày {EventItem.EndDate}.\n\nBạn không thể điểm danh sau thời gian kết thúc sự kiện!",
                    "Đóng");
                return;
            }

            if (EventItem.IsEventNotStarted)
            {
                await Shell.Current.DisplayAlert(
                    "Chưa mở điểm danh ⏳",
                    $"Sự kiện \"{EventItem.Name}\" chưa bắt đầu.\n\nKhung giờ tổ chức: {EventItem.StartTime} - {EventItem.EndTime} ngày {EventItem.StartDate}.\nVui lòng quay lại trong khung giờ sự kiện diễn ra!",
                    "Đóng");
                return;
            }

            await Shell.Current.GoToAsync($"photoattendance?eventId={EventItem.ID}");
        }

        [RelayCommand]
        public async Task OpenVideoLinkAsync()
        {
            if (EventItem == null || string.IsNullOrWhiteSpace(EventItem.VideoUrl)) return;

            try
            {
                var uri = new Uri(EventItem.VideoUrl);
                await Launcher.Default.OpenAsync(uri);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Thông báo", $"Không thể mở liên kết video: {ex.Message}", "Đóng");
            }
        }

        [RelayCommand]
        public async Task ApproveEventPostAsync()
        {
            if (EventItem == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Phê duyệt bài đăng sự kiện 🛡️",
                $"Bạn muốn duyệt hoạt động \"{EventItem.Title}\" xuất hiện chính thức trên ứng dụng?",
                "Duyệt bài", "Hủy");

            if (!confirm) return;

            try
            {
                IsLoading = true;
                string adminName = _authService.CurrentUser?.FullName ?? "Admin";
                bool ok = await _eventService.ApproveEventAsync(EventItem.ID, true, adminName);
                if (ok)
                {
                    await Shell.Current.DisplayAlert("Thành công", "Đã phê duyệt hoạt động xuất bản thành công!", "OK");
                    await LoadEventAsync(EventItem.ID);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task RejectEventPostAsync()
        {
            if (EventItem == null) return;

            string reason = await Shell.Current.DisplayPromptAsync(
                "Từ chối & Xóa hoạt động 🗑️",
                "Nhập lý do từ chối (hoạt động này sẽ bị XÓA vĩnh viễn):",
                "Từ chối & Xóa", "Hủy",
                placeholder: "Chưa đạt tiêu chuẩn tổ chức");

            if (reason == null) return;

            try
            {
                IsLoading = true;
                string adminName = _authService.CurrentUser?.FullName ?? "Admin";
                bool ok = await _eventService.ApproveEventAsync(EventItem.ID, false, adminName, reason);
                if (ok)
                {
                    await Shell.Current.DisplayAlert("Đã xóa hoạt động 🗑️", "Hoạt động đã bị từ chối và xóa vĩnh viễn khỏi hệ thống.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task ApproveParticipantAsync(EventRegistration reg)
        {
            if (reg == null || EventItem == null) return;

            if (!CanManageEvent)
            {
                await Shell.Current.DisplayAlert("Phân quyền ⚠️", "Bạn chỉ có quyền xét duyệt sinh viên cho sự kiện do chính bạn quản lý!", "Đã hiểu");
                return;
            }

            bool ok = await _eventService.ApproveRegistrationAsync(EventItem.ID, reg.Id, true);
            if (ok)
            {
                await Shell.Current.DisplayAlert("Thành công", $"Đã duyệt sinh viên {reg.StudentName} ({reg.StudentCode}) tham gia hoạt động.", "OK");
                await LoadEventAsync(EventItem.ID);
            }
        }

        [RelayCommand]
        public async Task RejectParticipantAsync(EventRegistration reg)
        {
            if (reg == null || EventItem == null) return;

            if (!CanManageEvent)
            {
                await Shell.Current.DisplayAlert("Phân quyền ⚠️", "Bạn chỉ có quyền xét duyệt sinh viên cho sự kiện do chính bạn quản lý!", "Đã hiểu");
                return;
            }

            bool ok = await _eventService.ApproveRegistrationAsync(EventItem.ID, reg.Id, false);
            if (ok)
            {
                await Shell.Current.DisplayAlert("Đã từ chối", $"Đã từ chối đăng ký của sinh viên {reg.StudentName}.", "OK");
                await LoadEventAsync(EventItem.ID);
            }
        }

        [RelayCommand]
        public async Task GoToAttendanceReportAsync()
        {
            if (EventItem == null) return;

            if (!CanManageEvent)
            {
                await Shell.Current.DisplayAlert("Phân quyền ⚠️", "Bạn chỉ có quyền xem điểm danh của sự kiện do chính bạn quản lý!", "Đã hiểu");
                return;
            }

            await Shell.Current.GoToAsync($"attendancereport?eventId={EventItem.ID}");
        }

        [RelayCommand]
        public async Task GoBackAsync()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
