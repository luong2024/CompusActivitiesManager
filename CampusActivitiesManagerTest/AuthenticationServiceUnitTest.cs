using CampusActivitiesManager.Models;

namespace CampusActivitiesManagerTest
{
    [TestFixture]
    public class AuthenticationServiceUnitTest
    {
        [Test]
        public void User_DefaultRole_IsUser()
        {
            var user = new User();
            Assert.That(user.Role, Is.EqualTo(Role.User));
            Assert.That(user.RoleDisplayName, Does.Contain("Sinh viên"));
        }

        [Test]
        public void User_AdminRole_CalculatesPropertiesCorrectly()
        {
            var user = new User
            {
                Username = "admin",
                FullName = "Quản Trị Viên",
                Role = Role.Admin,
                IsActive = true
            };

            Assert.That(user.IsAdminRole, Is.True);
            Assert.That(user.RoleDisplayName, Does.Contain("Admin"));
            Assert.That(user.RoleShortName, Is.EqualTo("Admin"));
            Assert.That(user.Initials, Is.EqualTo("QV"));
        }

        [Test]
        public void User_ManagerRole_CalculatesPropertiesCorrectly()
        {
            var user = new User
            {
                Username = "manager",
                FullName = "Nguyễn Văn Quản",
                Role = Role.Manager,
                IsActive = true
            };

            Assert.That(user.IsAdminRole, Is.False);
            Assert.That(user.IsManagerOrLecturer, Is.True);
            Assert.That(user.RoleDisplayName, Does.Contain("Quản lý"));
            Assert.That(user.RoleShortName, Is.EqualTo("Quản lý"));
        }

        [Test]
        public void User_LecturerRole_CanManageClasses()
        {
            var user = new User
            {
                Username = "gv_an",
                FullName = "TS. Nguyễn An",
                Role = Role.Lecturer,
                AssignedClass = "D20CNTT1",
                ManagedClasses = "D20CNTT1, D20CNTT2"
            };

            Assert.That(user.IsManagerOrLecturer, Is.True);
            Assert.That(user.IsInChargeOfClass("D20CNTT1"), Is.True);
            Assert.That(user.IsInChargeOfClass("D20CNTT2"), Is.True);
            Assert.That(user.IsInChargeOfClass("D20CNTT3"), Is.False);

            // Thêm lớp mới
            user.AddManagedClass("D20CNTT3");
            Assert.That(user.IsInChargeOfClass("D20CNTT3"), Is.True);
        }

        [Test]
        public void User_SecretaryRole_CalculatesSecretaryFlag()
        {
            var user = new User
            {
                Username = "bithu01",
                FullName = "Trần Bí Thư",
                Role = Role.Secretary,
                IsSecretary = true,
                ClassName = "D20CNTT1"
            };

            Assert.That(user.IsSecretaryRole, Is.True);
            Assert.That(user.RoleDisplayName, Does.Contain("Bí thư"));
            Assert.That(user.RoleShortName, Does.Contain("Bí thư"));
        }

        [TestCase("admin", Role.Admin)]
        [TestCase("manager", Role.Manager)]
        [TestCase("lecturer", Role.Lecturer)]
        [TestCase("secretary", Role.Secretary)]
        [TestCase("student", Role.Student)]
        public void Role_Enum_MatchesExpectedValue(string name, Role expected)
        {
            Assert.That(Enum.TryParse<Role>(name, true, out var parsed), Is.True);
            Assert.That(parsed, Is.EqualTo(expected));
        }
    }
}
