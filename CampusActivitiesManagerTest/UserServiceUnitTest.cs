using CampusActivitiesManager.Models;

namespace CampusActivitiesManagerTest
{
    [TestFixture]
    public class UserServiceUnitTest
    {
        [Test]
        public void User_TrainingPoints_CalculatesProgressProperly()
        {
            var user = new User
            {
                Username = "B21DCCN099",
                FullName = "Phạm Thị Mai",
                TrainingPoints = 40
            };

            // Mục tiêu học kỳ: 50 điểm
            Assert.That(user.SemesterProgress, Is.EqualTo(40.0 / 50.0).Within(0.001));
            Assert.That(user.SemesterProgressPercent, Is.EqualTo(80));
            Assert.That(user.IsSemesterQualified, Is.False);

            // Tăng lên 55 điểm -> hoàn thành mục tiêu học kỳ
            user.TrainingPoints = 55;
            Assert.That(user.SemesterProgress, Is.EqualTo(1.0));
            Assert.That(user.SemesterProgressPercent, Is.EqualTo(100));
            Assert.That(user.IsSemesterQualified, Is.True);
        }

        [Test]
        public void ClassRoom_PropertiesAndString_AreCorrect()
        {
            var classRoom = new ClassRoom
            {
                ClassName = "D20CNTT1",
                Department = "Khoa CNTT",
                AdvisorName = "ThS. Nguyễn Văn Quản",
                AdvisorUsername = "manager",
                StudentCount = 48
            };

            Assert.That(classRoom.ClassName, Is.EqualTo("D20CNTT1"));
            Assert.That(classRoom.ToString(), Is.EqualTo("D20CNTT1"));
            Assert.That(classRoom.AdvisorDisplay, Does.Contain("ThS. Nguyễn Văn Quản"));
            Assert.That(classRoom.AdvisorDisplay, Does.Contain("@manager"));
        }

        [Test]
        public void User_LecturerMultiClassAssignment_WorksAccurately()
        {
            var lecturer = new User
            {
                Username = "gv_hung",
                FullName = "ThS. Đỗ Tuấn Hùng",
                Role = Role.Lecturer,
                AssignedClass = "D20CNTT1",
                ManagedClasses = "D20CNTT1"
            };

            Assert.That(lecturer.IsInChargeOfClass("D20CNTT1"), Is.True);
            Assert.That(lecturer.IsInChargeOfClass("D20CNTT2"), Is.False);

            // Gán thêm lớp D20CNTT2 và D20CNTT3
            lecturer.AddManagedClass("D20CNTT2");
            lecturer.AddManagedClass("D20CNTT3");

            Assert.That(lecturer.IsInChargeOfClass("D20CNTT2"), Is.True);
            Assert.That(lecturer.IsInChargeOfClass("D20CNTT3"), Is.True);
            Assert.That(lecturer.IsInChargeOfClass("D20KTPM1"), Is.False);

            // Tránh thêm trùng lặp
            lecturer.AddManagedClass("D20CNTT2");
            var classCount = lecturer.ManagedClasses.Split(',').Length;
            Assert.That(classCount, Is.EqualTo(3));
        }

        [Test]
        public void RoleExtensions_DefaultPasswords_MatchExactRequirements()
        {
            Assert.That(Role.Admin.GetDefaultPassword(), Is.EqualTo("Admin@123"));
            Assert.That(Role.Manager.GetDefaultPassword(), Is.EqualTo("Manager@123"));
            Assert.That(Role.Lecturer.GetDefaultPassword(), Is.EqualTo("Gv@123"));
            Assert.That(Role.Secretary.GetDefaultPassword(), Is.EqualTo("Bithu@123"));
            Assert.That(Role.User.GetDefaultPassword(), Is.EqualTo("Student@123"));
        }

        [Test]
        public void User_EffectivePassword_FallsBackToRoleDefaultOrCustom()
        {
            var student = new User { Role = Role.User, Password = string.Empty };
            Assert.That(student.EffectivePassword, Is.EqualTo("Student@123"));

            student.Password = "MyCustomPassword!999";
            Assert.That(student.EffectivePassword, Is.EqualTo("MyCustomPassword!999"));

            var lecturer = new User { Role = Role.Lecturer };
            Assert.That(lecturer.EffectivePassword, Is.EqualTo("Gv@123"));

            var admin = new User { Role = Role.Admin };
            Assert.That(admin.EffectivePassword, Is.EqualTo("Admin@123"));
        }

        [Test]
        public void PasswordResetRequest_CreationAndStatus_AreCorrect()
        {
            var req = new PasswordResetRequest
            {
                Id = "RESET-001",
                Username = "B20DCCN001",
                FullName = "Nguyễn Văn An",
                Email = "an@student.ptit.edu.vn",
                Role = Role.User,
                DefaultPasswordAssigned = Role.User.GetDefaultPassword(),
                Status = "Pending",
                Notes = "Quên mật khẩu đăng nhập"
            };

            Assert.That(req.Id, Is.EqualTo("RESET-001"));
            Assert.That(req.DefaultPasswordAssigned, Is.EqualTo("Student@123"));
            Assert.That(req.IsPending, Is.True);
            Assert.That(req.StatusDisplay, Does.Contain("Chờ Admin"));

            req.Status = "Resolved";
            req.ResolvedBy = "admin";
            req.ResolvedAt = DateTime.UtcNow;
            Assert.That(req.IsPending, Is.False);
            Assert.That(req.StatusDisplay, Does.Contain("Đã reset"));
        }

        [Test]
        public void PointAuditRecord_PropertiesAndDisplay_AreAccurate()
        {
            var audit = new PointAuditRecord
            {
                Id = "AUDIT-001",
                StudentCode = "B20DCCN001",
                Delta = 10,
                Reason = "Đạt giải Nhất cuộc thi Sáng tạo Công nghệ",
                OperatorUsername = "admin",
                OperatorName = "Quản trị viên",
                Timestamp = DateTime.UtcNow
            };

            Assert.That(audit.Delta, Is.EqualTo(10));
            Assert.That(audit.DeltaDisplay, Is.EqualTo("+10 ĐRL"));
            Assert.That(audit.OperatorUsername, Is.EqualTo("admin"));

            audit.Delta = -5;
            audit.Reason = "Vắng sinh hoạt lớp không phép";
            Assert.That(audit.DeltaDisplay, Is.EqualTo("-5 ĐRL"));
        }
    }
}
