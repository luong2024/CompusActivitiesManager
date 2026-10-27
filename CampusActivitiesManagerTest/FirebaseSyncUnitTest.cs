using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CampusActivitiesManager.Data;

namespace CampusActivitiesManagerTest
{
    [TestFixture]
    public class FirebaseSyncUnitTest
    {
        [Test]
        public void FirebaseConfig_Parameters_AreConfiguredForCloudFirestore()
        {
            Assert.That(FirebaseConfig.ProjectId, Is.EqualTo("campusacmanage"));
            Assert.That(FirebaseConfig.ClientEmail, Does.Contain("campusacmanage.iam.gserviceaccount.com"));
            Assert.That(FirebaseConfig.FirestoreBaseUrl, Does.Contain("campusacmanage/databases/(default)/documents"));
            Assert.That(FirebaseConfig.PrivateKey, Does.StartWith("-----BEGIN PRIVATE KEY-----"));
            Assert.That(FirebaseConfig.TokenUri, Does.Contain("oauth2.googleapis.com"));
        }

        [Test]
        public async Task TestFirestore_SyncDefaultUsersToFirebaseAuthAndFirestore()
        {
            var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<CampusActivitiesManager.BusinessLogic.Services.FirebaseSyncService>.Instance;
            var syncService = new CampusActivitiesManager.BusinessLogic.Services.FirebaseSyncService(logger);

            var usersToSync = new List<CampusActivitiesManager.Models.User>
            {
                new CampusActivitiesManager.Models.User
                {
                    Id = "admin-sys-01",
                    Username = "admin",
                    FullName = "Quản Trị Viên Hệ Thống",
                    Email = "admin@campus.edu.vn",
                    Role = CampusActivitiesManager.Models.Role.Admin,
                    Password = "Admin@123",
                    ClassName = "Ban Quản Trị",
                    IsActive = true
                },
                new CampusActivitiesManager.Models.User
                {
                    Id = "manager-sys-02",
                    Username = "manager",
                    FullName = "ThS. Nguyễn Văn Quản",
                    Email = "manager@campus.edu.vn",
                    Role = CampusActivitiesManager.Models.Role.Manager,
                    Password = "Manager@123",
                    ClassName = "Khoa CNTT",
                    IsActive = true
                },
                new CampusActivitiesManager.Models.User
                {
                    Id = "lecturer-sys-03",
                    Username = "giangvien",
                    FullName = "TS. Trần Văn Minh",
                    Email = "giangvien@campus.edu.vn",
                    Role = CampusActivitiesManager.Models.Role.Lecturer,
                    Password = "Lecturer@123",
                    ClassName = "Khoa CNTT",
                    IsActive = true
                },
                new CampusActivitiesManager.Models.User
                {
                    Id = "secretary-sys-04",
                    Username = "bithu",
                    FullName = "Lê Hoàng Bí Thư",
                    Email = "bithu@campus.edu.vn",
                    Role = CampusActivitiesManager.Models.Role.Secretary,
                    Password = "Bithu@123",
                    ClassName = "D20CNTT1",
                    IsActive = true
                },
                new CampusActivitiesManager.Models.User
                {
                    Id = "student-sys-05",
                    Username = "sv123456",
                    FullName = "Nguyễn Văn Sinh Viên",
                    Email = "sinhvien@gmail.com",
                    Role = CampusActivitiesManager.Models.Role.User,
                    Password = "Student@123",
                    ClassName = "D20CNTT1",
                    IsActive = true
                }
            };

            foreach (var user in usersToSync)
            {
                bool ok = await syncService.PushUserAsync(user);
                Assert.That(ok, Is.True, $"PushUserAsync failed for {user.Email}");
            }
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}
