using System.IO.Compression;
using System.Xml.Linq;

namespace UITests.Android
{
    [TestFixture]
    public class AndroidAppManifestTests
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
        public void Android_AppId_MatchesExpectedPackageName()
        {
            var csprojPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "CampusActivitiesManager.csproj");
            Assert.That(File.Exists(csprojPath), Is.True, $"Không tìm thấy file csproj tại {csprojPath}");

            var content = File.ReadAllText(csprojPath);
            Assert.That(content, Does.Contain("<ApplicationId>com.companyname.campusactivitiesmanager</ApplicationId>"));
            Assert.That(content, Does.Contain("<ApplicationTitle>CampusActivitiesManager</ApplicationTitle>"));
        }

        [Test]
        public void Android_SupportedOSPlatformVersion_IsAtLeast21()
        {
            var csprojPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "CampusActivitiesManager.csproj");
            var content = File.ReadAllText(csprojPath);

            Assert.That(content, Does.Contain("<SupportedOSPlatformVersion Condition=\"$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'\">21.0</SupportedOSPlatformVersion>"));
        }

        [Test]
        public void Android_SignedApk_ExistsAndHasValidContent()
        {
            var apkPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "bin", "Debug", "net9.0-android", "com.companyname.campusactivitiesmanager-Signed.apk");

            if (!File.Exists(apkPath))
            {
                Assert.Ignore($"Tệp APK đã ký chưa tồn tại tại {apkPath} (cần chạy dotnet build -f net9.0-android để tạo).");
                return;
            }

            var fileInfo = new FileInfo(apkPath);
            Assert.That(fileInfo.Length, Is.GreaterThan(10 * 1024 * 1024), "Dung lượng APK phải lớn hơn 10MB");

            // Kiểm tra cấu trúc tệp APK (Zip archive)
            using var zip = ZipFile.OpenRead(apkPath);
            var manifestEntry = zip.GetEntry("AndroidManifest.xml");
            Assert.That(manifestEntry, Is.Not.Null, "APK phải chứa tệp AndroidManifest.xml");

            var classesDex = zip.Entries.Any(e => e.Name.EndsWith(".dex", StringComparison.OrdinalIgnoreCase));
            Assert.That(classesDex, Is.True, "APK phải chứa tệp bytecode Dalvik/ART (.dex)");
        }

        [Test]
        public void Android_RequiredPermissions_AreDeclaredInManifest()
        {
            var manifestPath = Path.Combine(_solutionRoot, "CampusActivitiesManager", "CampusActivitiesManager", "Platforms", "Android", "AndroidManifest.xml");

            if (!File.Exists(manifestPath))
            {
                Assert.Ignore($"Không tìm thấy file AndroidManifest.xml tại {manifestPath}");
                return;
            }

            var xml = XDocument.Load(manifestPath);
            var permissions = xml.Descendants("uses-permission")
                                 .Select(e => e.Attribute(XName.Get("name", "http://schemas.android.com/apk/res/android"))?.Value)
                                 .Where(v => !string.IsNullOrEmpty(v))
                                 .ToList();

            // Rà soát các quyền cốt lõi cho Điểm Danh Ảnh & Đồng Bộ Mạng
            Assert.That(permissions, Does.Contain("android.permission.INTERNET"), "Thiếu quyền INTERNET");
            Assert.That(permissions, Does.Contain("android.permission.ACCESS_NETWORK_STATE"), "Thiếu quyền ACCESS_NETWORK_STATE");
        }
    }
}
