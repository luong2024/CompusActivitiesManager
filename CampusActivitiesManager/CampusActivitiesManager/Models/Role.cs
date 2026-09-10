namespace CampusActivitiesManager.Models
{
    /// <summary>
    /// Định nghĩa các cấp bậc vai trò người dùng trong hệ thống.
    /// Admin: Toàn quyền quản trị và phân quyền
    /// Manager: Quản lý dự án, hoạt động
    /// User: Sinh viên / người dùng tiêu chuẩn
    /// Guest: Khách xem nội dung
    /// </summary>
    public enum Role
    {
        Admin,
        Manager,
        User,
        Guest
    }

    public static class RoleExtensions
    {
        public static string GetDisplayName(this Role role) => role switch
        {
            Role.Admin => "Admin (Quản trị viên)",
            Role.Manager => "Manager (Quản lý)",
            Role.User => "User (Sinh viên / Thành viên)",
            Role.Guest => "Guest (Khách vãng lai)",
            _ => role.ToString()
        };

        public static string GetShortName(this Role role) => role switch
        {
            Role.Admin => "Admin",
            Role.Manager => "Manager",
            Role.User => "User",
            Role.Guest => "Guest",
            _ => role.ToString()
        };

        public static string GetBadgeColorHex(this Role role) => role switch
        {
            Role.Admin => "#DC2626",    // Đỏ tím nổi bật cho Admin
            Role.Manager => "#2563EB",  // Xanh dương cho Manager
            Role.User => "#16A34A",     // Xanh lá cho User / Student
            Role.Guest => "#6B7280",    // Xám cho Guest
            _ => "#6B7280"
        };

        public static string GetDescription(this Role role) => role switch
        {
            Role.Admin => "Toàn quyền trên hệ thống, phân quyền và quản lý tài khoản người dùng.",
            Role.Manager => "Quản lý các hoạt động, dự án và nhiệm vụ. Chế độ chỉ xem đối với người dùng.",
            Role.User => "Xem hoạt động, nhận nhiệm vụ và đánh dấu hoàn thành nhiệm vụ cá nhân.",
            Role.Guest => "Chỉ có quyền xem thông tin cơ bản, không được chỉnh sửa dữ liệu.",
            _ => string.Empty
        };
    }
}

