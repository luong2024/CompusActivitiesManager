namespace CampusActivitiesManager.Models
{
    public class ClassRoom
    {
        public string ClassName { get; set; } = "D20CNTT1";
        public string Department { get; set; } = "Khoa Công nghệ Thông tin";
        public string AdvisorName { get; set; } = "ThS. Nguyễn Văn Quản";
        public string SecretaryName { get; set; } = "Lê Hoàng Bí Thư";
        public string AcademicYear { get; set; } = "K20 (2020 - 2025)";
        public int StudentCount { get; set; } = 45;
        public string Description { get; set; } = "Lớp Kỹ thuật Phần mềm & Trí tuệ Nhân tạo";

        public override string ToString() => ClassName;
    }
}
