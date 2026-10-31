namespace UITests.Windows
{
    [TestFixture]
    public class WindowsDesktopAppConfigTests
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
        public void Windows_TargetFramework_ConfiguredForWinUI10()
        {
            var csprojPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "CampusActivitiesManager.csproj");
            Assert.That(File.Exists(csprojPath), Is.True);

            var content = File.ReadAllText(csprojPath);
            Assert.That(content, Does.Contain("net9.0-windows10.0.19041.0"),
                "Dự án phải hỗ trợ TargetFramework WinUI net9.0-windows10.0.19041.0");
        }

        [Test]
        public void Windows_PackageType_IsConfiguredAsUnpackagedDesktop()
        {
            var csprojPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "CampusActivitiesManager.csproj");
            var content = File.ReadAllText(csprojPath);

            Assert.That(content, Does.Contain("<WindowsPackageType Condition=\"$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'\">None</WindowsPackageType>"),
                "Cấu hình đóng gói desktop Windows phải là unpackaged (None) để khởi chạy linh hoạt.");
        }

        [Test]
        public void Windows_SupportedOSPlatformVersion_IsAtLeast17763()
        {
            var csprojPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "CampusActivitiesManager.csproj");
            var content = File.ReadAllText(csprojPath);

            Assert.That(content, Does.Contain("<SupportedOSPlatformVersion Condition=\"$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'\">10.0.17763.0</SupportedOSPlatformVersion>"));
        }
    }
}
