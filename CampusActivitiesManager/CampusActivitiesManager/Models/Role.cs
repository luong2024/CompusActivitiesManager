namespace CampusActivitiesManager.Models
{
    /// <summary>
    /// Định nghĩa các cấp bậc vai trò người dùng trong hệ thống:
    /// - Admin: Toàn quyền quản trị, duyệt tất cả bài đăng, quản lý tài khoản và lớp.
    /// - Manager / Lecturer: Quản lý sự kiện / Giảng viên chủ nhiệm lớp.
    /// - Secretary: Bí thư lớp (sinh viên có quyền tạo hoạt động cho lớp).
    /// - User / Student: Học sinh / Sinh viên tham gia hoạt động và tích lũy điểm rèn luyện.
    /// - Guest: Khách vãng lai.
    /// </summary>
    public enum Role
    {
        Admin = 0,
        Manager = 1,
        Lecturer = 2,
        Secretary = 3,
        User = 4,
        Student = 4,
        Guest = 5
    }

    public static class RoleExtensions
    {
        public static string GetDisplayName(this Role role) => role switch
        {
            Role.Admin => "Admin (Quản trị viên)",
            Role.Manager => "Quản lý sự kiện",
            Role.Lecturer => "Giảng viên / Cố vấn học tập",
            Role.Secretary => "Bí thư Lớp (Ban cán sự)",
            Role.User => "Sinh viên / Học sinh",
            Role.Guest => "Khách vãng lai",
            _ => role.ToString()
        };

        public static string GetShortName(this Role role) => role switch
        {
            Role.Admin => "Admin",
            Role.Manager => "Quản lý",
            Role.Lecturer => "Giảng viên",
            Role.Secretary => "Bí thư",
            Role.User => "Sinh viên",
            Role.Guest => "Khách",
            _ => role.ToString()
        };

        public static string GetBadgeColorHex(this Role role) => role switch
        {
            Role.Admin => "#DC2626",      // Đỏ cho Admin
            Role.Manager => "#0284C7",    // Xanh dương sáng cho Quản lý
            Role.Lecturer => "#2563EB",   // Xanh dương đậm cho Giảng viên
            Role.Secretary => "#7C3AED",  // Tím cho Bí thư
            Role.User => "#16A34A",       // Xanh lá cho Sinh viên
            Role.Guest => "#6B7280",      // Xám cho Guest
            _ => "#6B7280"
        };

        public static string GetDescription(this Role role) => role switch
        {
            Role.Admin => "Toàn quyền quản trị hệ thống: duyệt tất cả bài đăng hoạt động, duyệt điểm danh, quản lý tài khoản và lớp học.",
            Role.Manager => "Quản lý sự kiện: tạo hoạt động, duyệt đăng ký tham gia và duyệt điểm danh (+/- điểm).",
            Role.Lecturer => "Giảng viên chủ nhiệm: quản lý sinh viên lớp mình, tạo hoạt động cho lớp, theo dõi điểm rèn luyện của lớp.",
            Role.Secretary => "Bí thư lớp: kiểm soát học sinh trong lớp, tạo hoạt động riêng cho lớp, theo dõi điểm rèn luyện các bạn.",
            Role.User => "Học sinh / Sinh viên: xem thông tin sự kiện, đăng ký tham gia, điểm danh bằng ảnh, tích lũy điểm rèn luyện đạt chuẩn kỳ/năm.",
            Role.Guest => "Chỉ có quyền xem thông tin cơ bản.",
            _ => string.Empty
        };
    }
}
