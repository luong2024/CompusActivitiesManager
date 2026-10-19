using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CampusActivitiesManager.Models
{
    /// <summary>
    /// Model User đại diện cho thực thể người dùng trong hệ thống.
    /// Kế thừa ObservableObject (INotifyPropertyChanged) để tự động cập nhật UI.
    /// </summary>
    public partial class User : ObservableObject
    {
        [ObservableProperty]
        private string _id = string.Empty;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _passwordHash = string.Empty;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        [ObservableProperty]
        private string _department = "Công nghệ Thông tin";

        [ObservableProperty]
        private string _className = "D20CNTT1"; // Lớp sinh hoạt

        [ObservableProperty]
        private string _assignedClass = "D20CNTT1"; // Lớp chủ nhiệm (đối với Giảng viên)

        [ObservableProperty]
        private bool _isSecretary = false; // Đánh dấu Bí thư lớp

        [ObservableProperty]
        private string _academicYear = "K20 (2020-2025)";

        // Điểm rèn luyện khởi điểm ban đầu là 0 theo yêu cầu người dùng
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SemesterProgress))]
        [NotifyPropertyChangedFor(nameof(YearProgress))]
        [NotifyPropertyChangedFor(nameof(IsSemesterQualified))]
        [NotifyPropertyChangedFor(nameof(IsYearQualified))]
        [NotifyPropertyChangedFor(nameof(PointsStatusDisplay))]
        private int _trainingPoints = 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RoleDisplayName))]
        [NotifyPropertyChangedFor(nameof(RoleShortName))]
        [NotifyPropertyChangedFor(nameof(RoleBadgeColor))]
        [NotifyPropertyChangedFor(nameof(RoleDescription))]
        [NotifyPropertyChangedFor(nameof(IsAdminRole))]
        [NotifyPropertyChangedFor(nameof(IsManagerOrLecturer))]
        [NotifyPropertyChangedFor(nameof(IsSecretaryRole))]
        private Role _role = Role.User;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StatusText))]
        [NotifyPropertyChangedFor(nameof(StatusColorHex))]
        private bool _isActive = true;

        [JsonIgnore]
        public int IntId
        {
            get => int.TryParse(Id, out var result) ? result : 0;
            set => Id = value.ToString();
        }

        public string RoleDisplayName => Role == Role.Secretary || IsSecretary
            ? "Bí thư Lớp"
            : Role.GetDisplayName();

        public string RoleShortName => Role == Role.Secretary || IsSecretary
            ? "Bí thư"
            : Role.GetShortName();

        public string RoleBadgeColor => Role == Role.Secretary || IsSecretary
            ? "#7C3AED"
            : Role.GetBadgeColorHex();

        public string RoleDescription => Role.GetDescription();

        public bool IsAdminRole => Role == Role.Admin;

        public bool IsManagerOrLecturer => Role == Role.Manager || Role == Role.Lecturer;

        public bool IsSecretaryRole => Role == Role.Secretary || IsSecretary;

        public string StatusText => IsActive ? "Đang hoạt động" : "Đã khóa";

        public string StatusColorHex => IsActive ? "#16A34A" : "#DC2626";

        // Chỉ tiêu rèn luyện: 50 điểm / kỳ, 250 điểm / năm
        public const int SemesterMinGoal = 50;
        public const int YearMinGoal = 250;

        public double SemesterProgress => Math.Min(1.0, (double)TrainingPoints / SemesterMinGoal);

        public double YearProgress => Math.Min(1.0, (double)TrainingPoints / YearMinGoal);

        public int SemesterProgressPercent => (int)(SemesterProgress * 100);

        public int YearProgressPercent => (int)(YearProgress * 100);

        public bool IsSemesterQualified => TrainingPoints >= SemesterMinGoal;

        public bool IsYearQualified => TrainingPoints >= YearMinGoal;

        public string PointsStatusDisplay => IsSemesterQualified 
            ? "Đạt chuẩn kỳ này 🎉" 
            : $"Cần thêm {Math.Max(0, SemesterMinGoal - TrainingPoints)} điểm để đạt kỳ";

        public string AvatarInitials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullName))
                {
                    return string.IsNullOrWhiteSpace(Username) ? "?" : Username[..1].ToUpper();
                }

                var parts = FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                    return parts[0][..1].ToUpper();

                return $"{parts[0][..1]}{parts[^1][..1]}".ToUpper();
            }
        }

        public override string ToString() => $"{FullName} (@{Username}) - {RoleDisplayName} [{ClassName}]";
    }
}
