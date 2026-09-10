using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CampusActivitiesManager.Models
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
        public int TrainingPoints { get; set; } = 85; // Điểm rèn luyện
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // UI Helpers
        public bool HasAvatar => !string.IsNullOrWhiteSpace(AvatarUrl);

        public string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullName))
                    return "U";

                var parts = FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                    return parts[0][..1].ToUpper();

                return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
            }
        }

        public string StatusName => Status switch
        {
            AccountStatus.DangHoc => "Đang học",
            AccountStatus.BaoLuu => "Bảo lưu",
            AccountStatus.BiKhoa => "Bị khóa",
            _ => "Không xác định"
        };

        public Color StatusColor => Status switch
        {
            AccountStatus.DangHoc => Color.FromArgb("#107C41"),  // Success Green
            AccountStatus.BaoLuu => Color.FromArgb("#D83B01"),   // Warning Orange
            AccountStatus.BiKhoa => Color.FromArgb("#A80000"),   // Error Red
            _ => Color.FromArgb("#6E6E6E")
        };

        public Brush StatusBgBrush => Status switch
        {
            AccountStatus.DangHoc => new SolidColorBrush(Color.FromArgb("#E8F5E9")),
            AccountStatus.BaoLuu => new SolidColorBrush(Color.FromArgb("#FFF3E0")),
            AccountStatus.BiKhoa => new SolidColorBrush(Color.FromArgb("#FFEBEE")),
            _ => new SolidColorBrush(Color.FromArgb("#F0F0F0"))
        };

        public string RoleName => Role switch
        {
            AccountRole.SinhVien => "Sinh viên",
            AccountRole.LopTruong => "Lớp trưởng",
            AccountRole.GiangVien => "Giảng viên",
            AccountRole.BanChuNhiem => "Ban chủ nhiệm",
            AccountRole.Admin => "Quản trị viên",
            _ => "Thành viên"
        };

        public Color RoleColor => Role switch
        {
            AccountRole.Admin => Color.FromArgb("#7F22FE"),
            AccountRole.GiangVien => Color.FromArgb("#0078D4"),
            AccountRole.LopTruong => Color.FromArgb("#008272"),
            AccountRole.BanChuNhiem => Color.FromArgb("#D13438"),
            _ => Color.FromArgb("#512BD4")
        };

        public Brush RoleBgBrush => Role switch
        {
            AccountRole.Admin => new SolidColorBrush(Color.FromArgb("#F3E8FF")),
            AccountRole.GiangVien => new SolidColorBrush(Color.FromArgb("#E0F2FE")),
            AccountRole.LopTruong => new SolidColorBrush(Color.FromArgb("#E6F4F1")),
            AccountRole.BanChuNhiem => new SolidColorBrush(Color.FromArgb("#FEE2E2")),
            _ => new SolidColorBrush(Color.FromArgb("#DFD8F7"))
        };

        public string PointRank => TrainingPoints switch
        {
            >= 90 => "Xuất sắc",
            >= 80 => "Tốt",
            >= 65 => "Khá",
            >= 50 => "Trung bình",
            _ => "Yếu"
        };

        public Color PointRankColor => TrainingPoints switch
        {
            >= 90 => Color.FromArgb("#107C41"),
            >= 80 => Color.FromArgb("#0078D4"),
            >= 65 => Color.FromArgb("#D83B01"),
            _ => Color.FromArgb("#A80000")
        };
    }
}
