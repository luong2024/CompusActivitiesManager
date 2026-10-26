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
    }
}
