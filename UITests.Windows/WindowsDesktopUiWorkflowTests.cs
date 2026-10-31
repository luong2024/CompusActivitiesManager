namespace UITests.Windows
{
    [TestFixture]
    public class WindowsDesktopUiWorkflowTests
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
        public void Windows_EventManagement_TwoTierLayout_StructuresPrimaryAndManagerToolbar()
        {
            var xamlPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "Pages", "EventManagementPage.xaml");
            Assert.That(File.Exists(xamlPath), Is.True);

            var content = File.ReadAllText(xamlPath);

            // Kiểm tra nút hành động chính full-width: Xem Chi Tiết & Đăng Ký
            Assert.That(content, Does.Contain("Xem Chi Tiết &amp; Đăng Ký Tham Gia"));

            // Kiểm tra toolbar quản trị chỉ hiển thị khi có quyền quản lý
            Assert.That(content, Does.Contain("Manager & Admin Toolbar"));
            Assert.That(content, Does.Contain("IsVisible=\"{Binding CanUserManage}\""));
            Assert.That(content, Does.Contain("ViewAttendanceReportCommand"));
            Assert.That(content, Does.Contain("OpenEditModalCommand"));
            Assert.That(content, Does.Contain("DeleteEventCommand"));
        }

        [Test]
        public void Windows_UserManagement_RestrictsAdminActionButtonsToAdminRole()
        {
            var xamlPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "Pages", "UserManagementPage.xaml");
            var content = File.ReadAllText(xamlPath);

            // Rà soát cụm nút thao tác quản trị có ràng buộc IsVisible="{Binding IsAdmin}"
            Assert.That(content, Does.Contain("IsVisible=\"{Binding Source={RelativeSource AncestorType={x:Type pageModels:UserManagementPageModel}}, Path=IsAdmin, x:DataType=pageModels:UserManagementPageModel}\""),
                "Cụm 7 nút quản trị nhạy cảm phải được bảo vệ bằng IsVisible IsAdmin.");
        }

        [Test]
        public void Windows_ProfilePage_SecurityPasswordEyeToggles_ConfiguredCorrectly()
        {
            var xamlPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "Pages", "ProfilePage.xaml");
            var content = File.ReadAllText(xamlPath);

            // Kiểm tra 3 nút toggle ẩn/hiện mật khẩu
            Assert.That(content, Does.Contain("ToggleCurrentPasswordVisibilityCommand"));
            Assert.That(content, Does.Contain("ToggleNewPasswordVisibilityCommand"));
            Assert.That(content, Does.Contain("ToggleConfirmPasswordVisibilityCommand"));
            Assert.That(content, Does.Contain("eye_off.png"));
            Assert.That(content, Does.Contain("eye_on.png"));
        }

        [Test]
        public void Windows_ProfilePage_StaffDrlExemption_ConfiguredCorrectly()
        {
            var xamlPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "Pages", "ProfilePage.xaml");
            var content = File.ReadAllText(xamlPath);

            // Kiểm tra huy hiệu miễn tính ĐRL cho cán bộ quản lý
            Assert.That(content, Does.Contain("Training Points for Students vs Exemption Badge for Staff"));
            Assert.That(content, Does.Contain("Miễn tính ĐRL (Cán bộ)"));
            Assert.That(content, Does.Contain("IsVisible=\"{Binding IsStudent, Converter={toolkit:InvertedBoolConverter}}\""));
        }
    }
}
