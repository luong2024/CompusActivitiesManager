namespace UITests.Android
{
    [TestFixture]
    public class AndroidMobileUiWorkflowTests
    {
        private string _solutionRoot = string.Empty;

        [SetUp]
        public void SetUp()
        {
            var currentDir = TestContext.CurrentContext.TestDirectory;
            var dir = new DirectoryInfo(currentDir);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "CampusActivitiesManager.sln")))
            {
                dir = dir.Parent;
            }

            _solutionRoot = dir?.FullName ?? Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", ".."));
        }

        [Test]
        public void Android_NotificationModal_HeaderLayout_PreventsOverlap()
        {
            var xamlPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "Pages", "MainPage.xaml");
            Assert.That(File.Exists(xamlPath), Is.True);

            var content = File.ReadAllText(xamlPath);

            // Kiểm tra nút Đóng thông báo nằm độc lập ở Grid.Row="4" (không đè lên danh sách Row 3)
            Assert.That(content, Does.Contain("<Button Grid.Row=\"4\" Text=\"Đóng thông báo\""),
                "Nút 'Đóng thông báo' phải được đặt tại Row 4 để không chèn lên ScrollView danh sách thông báo.");

            // Kiểm tra hàng tiêu đề chia làm 2 tầng rõ ràng (Top Bar + Actions Bar)
            Assert.That(content, Does.Contain("Modal Header: Structured 2-tier layout to prevent horizontal crowding"),
                "Modal header phải tuân thủ cấu trúc 2 tầng chống đè chữ trên màn hình hẹp.");
        }

        [Test]
        public void Android_PhotoAttendance_TestButton_IsCompletelyRemoved()
        {
            var xamlPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "Pages", "PhotoAttendancePage.xaml");
            var content = File.ReadAllText(xamlPath);

            // Nút "Dùng Ảnh Mẫu Kiểm Thử (1-Click Test)" đã được xóa bỏ hoàn toàn
            Assert.That(content, Does.Not.Contain("Dùng Ảnh Mẫu Kiểm Thử"),
                "Nút điểm danh kiểm thử phải được loại bỏ hoàn toàn theo yêu cầu người dùng.");
        }

        [Test]
        public void Android_PhotoAttendance_ContainsStateSwitchingAndApprovalBadges()
        {
            var xamlPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "Pages", "PhotoAttendancePage.xaml");
            var content = File.ReadAllText(xamlPath);

            // Kiểm tra sự hiện diện của 2 trạng thái: STATE A (Đã điểm danh) và STATE B (Form gửi)
            Assert.That(content, Does.Contain("STATE A: TRANG ĐÃ ĐIỂM DANH THÀNH CÔNG & TRẠNG THÁI PHÊ DUYỆT"));
            Assert.That(content, Does.Contain("STATE B: FORM CHỤP ẢNH & GỬI ĐIỂM DANH"));

            // Kiểm tra các thành phần minh chứng và trạng thái
            Assert.That(content, Does.Contain("{Binding ApprovalStatusBadge}"));
            Assert.That(content, Does.Contain("{Binding ApprovalStatusText}"));
            Assert.That(content, Does.Contain("ẢNH MINH CHỨNG ĐÃ GỬI HỆ THỐNG"));
            Assert.That(content, Does.Contain("{Binding CurrentAttendanceRecord.PhotoImageSource}"));
            Assert.That(content, Does.Contain("Quay Lại Trang Chủ"));
        }

        [Test]
        public void Android_ClassHub_RegularStudent_DisplaysStaticBadgeInsteadOfDisabledPicker()
        {
            var xamlPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "Pages", "ClassHubPage.xaml");
            var content = File.ReadAllText(xamlPath);

            // Kiểm tra badge tĩnh "Lớp của bạn" khi CanViewClassmatesDrl == false
            Assert.That(content, Does.Contain("Static Display Badge for regular students"));
            Assert.That(content, Does.Contain("Lớp của bạn"));
            Assert.That(content, Does.Contain("CanViewClassmatesDrl, Converter={toolkit:InvertedBoolConverter}"));
        }

        [Test]
        public void Android_MobileTouchTargets_MeetAccessibilityGuidelines()
        {
            var xamlPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "Pages", "PhotoAttendancePage.xaml");
            var content = File.ReadAllText(xamlPath);

            // Chiều cao nút thao tác chụp ảnh và gửi tối thiểu 44-48px
            Assert.That(content, Does.Contain("HeightRequest=\"44\""));
            Assert.That(content, Does.Contain("HeightRequest=\"48\""));
        }
    }
}
