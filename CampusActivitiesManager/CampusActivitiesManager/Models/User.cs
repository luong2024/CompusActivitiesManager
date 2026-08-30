using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CampusActivitiesManager.Models
{
    /// <summary>
    /// Model User đại diện cho thực thể người dùng trong hệ thống.
    /// Kế thừa ObservableObject (INotifyPropertyChanged) để tự động kích hoạt cập nhật giao diện
    /// khi trường Role, IsActive hoặc các trường thông tin khác bị thay đổi.
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
        private string _department = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RoleDisplayName))]
        [NotifyPropertyChangedFor(nameof(RoleShortName))]
        [NotifyPropertyChangedFor(nameof(RoleBadgeColor))]
        [NotifyPropertyChangedFor(nameof(RoleDescription))]
        [NotifyPropertyChangedFor(nameof(IsAdminRole))]
        private Role _role = Role.User;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StatusText))]
        [NotifyPropertyChangedFor(nameof(StatusColorHex))]
        private bool _isActive = true;

        /// <summary>
        /// ID dạng số nguyên tương thích với cấu trúc khóa chính SQLite AUTOINCREMENT nếu có.
        /// </summary>
        [JsonIgnore]
        public int IntId
        {
            get => int.TryParse(Id, out var result) ? result : 0;
            set => Id = value.ToString();
        }

        public string RoleDisplayName => Role.GetDisplayName();

        public string RoleShortName => Role.GetShortName();

        public string RoleBadgeColor => Role.GetBadgeColorHex();

        public string RoleDescription => Role.GetDescription();

        public bool IsAdminRole => Role == Role.Admin;

        public string StatusText => IsActive ? "Đang hoạt động" : "Đã khóa";

        public string StatusColorHex => IsActive ? "#16A34A" : "#DC2626";

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

        public override string ToString() => $"{FullName} (@{Username}) - {Role}";
    }
}

