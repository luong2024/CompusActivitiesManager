using System;

namespace CampusActivitiesManager.Api.Models
{
    public enum AccountStatus
    {
        DangHoc = 0,    // Đang học
        BaoLuu = 1,     // Bảo lưu
        BiKhoa = 2      // Bị khóa
    }

    public enum AccountRole
    {
        SinhVien = 0,       // Sinh viên
        LopTruong = 1,      // Lớp trưởng
        GiangVien = 2,      // Giảng viên
        BanChuNhiem = 3,    // Ban chủ nhiệm CLB
        Admin = 4           // Quản trị viên
    }

    public class Account
    {
        public int ID { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public AccountStatus Status { get; set; } = AccountStatus.DangHoc;
        public AccountRole Role { get; set; } = AccountRole.SinhVien;
        public string ClassName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public int TrainingPoints { get; set; } = 85;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
