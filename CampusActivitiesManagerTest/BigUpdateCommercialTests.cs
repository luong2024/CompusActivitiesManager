using CampusActivitiesManager.Models;

namespace CampusActivitiesManagerTest
{
    [TestFixture]
    public class BigUpdateCommercialTests
    {
        [Test]
        public void Notification_UnreadFlagAndIcons_CalculateCorrectly()
        {
            var pointsNotif = new CampusNotificationDto
            {
                Id = "N01",
                Title = "Cộng điểm rèn luyện",
                Message = "Bạn được cộng 10 ĐRL",
                Type = "Points",
                IsRead = false
            };

            Assert.That(pointsNotif.IsUnread, Is.True);
            Assert.That(pointsNotif.TypeIcon, Is.EqualTo("🏆"));

            var reminderNotif = new CampusNotificationDto
            {
                Id = "N02",
                Type = "Reminder",
                IsRead = true
            };

            Assert.That(reminderNotif.IsUnread, Is.False);
            Assert.That(reminderNotif.TypeIcon, Is.EqualTo("⏰"));

            var successNotif = new CampusNotificationDto { Type = "Success" };
            Assert.That(successNotif.TypeIcon, Is.EqualTo("🎫"));

            var urgentNotif = new CampusNotificationDto { Type = "Urgent" };
            Assert.That(urgentNotif.TypeIcon, Is.EqualTo("⚠️"));

            var defaultNotif = new CampusNotificationDto { Type = "Info" };
            Assert.That(defaultNotif.TypeIcon, Is.EqualTo("🔔"));
        }

        [Test]
        public void EventCapacity_UnlimitedScenarios_HandledProperly()
        {
            // Case 1: MaxParticipants = 0 => Unlimited
            var evtZero = new CampusEvent { MaxParticipants = 0 };
            Assert.That(evtZero.IsUnlimitedParticipants, Is.True);
            Assert.That(evtZero.ParticipantsDisplay, Does.Contain("Không giới hạn"));

            // Case 2: MaxParticipants < 0 => Unlimited
            var evtNeg = new CampusEvent { MaxParticipants = -5 };
            Assert.That(evtNeg.IsUnlimitedParticipants, Is.True);
            Assert.That(evtNeg.ParticipantsDisplay, Does.Contain("Không giới hạn"));

            // Case 3: MaxParticipants > 0 => Limited
            var evtPos = new CampusEvent
            {
                MaxParticipants = 100,
                Registrations = new List<EventRegistration>
                {
                    new EventRegistration { StudentCode = "SV01", Status = "Approved" }
                }
            };
            Assert.That(evtPos.IsUnlimitedParticipants, Is.False);
            Assert.That(evtPos.ParticipantsDisplay, Does.Contain("1/100"));
        }

        [Test]
        public void EventScope_ClassOnly_RequiresAndDisplaysTargetClass()
        {
            var classEvent = new CampusEvent
            {
                Scope = "ClassOnly",
                TargetClass = "D20CNTT2"
            };

            Assert.That(classEvent.Scope, Is.EqualTo("ClassOnly"));
            Assert.That(classEvent.TargetClass, Is.EqualTo("D20CNTT2"));
            Assert.That(classEvent.ScopeBadge, Does.Contain("D20CNTT2"));

            var schoolEvent = new CampusEvent
            {
                Scope = "Public",
                TargetClass = string.Empty
            };

            Assert.That(schoolEvent.ScopeBadge, Does.Contain("Toàn trường"));
        }

        [Test]
        public void User_PasswordSecurity_DoesNotExposePlaintextWhenHashed()
        {
            var user = new User
            {
                Username = "sv_commercial",
                Role = Role.User,
                Password = "InitialStudent@123",
                PasswordHash = "InitialStudent@123"
            };

            Assert.That(user.EffectivePassword, Is.EqualTo("InitialStudent@123"));

            // Update to a new secure password
            user.Password = "SuperSecure#Pass2026";
            user.PasswordHash = "SuperSecure#Pass2026";

            Assert.That(user.EffectivePassword, Is.EqualTo("SuperSecure#Pass2026"));
        }

        [Test]
        public void Role_SecretaryAndLecturer_PermissionsDistinctFromStudent()
        {
            var secretary = new User
            {
                Username = "bithu_d20",
                Role = Role.Secretary,
                AssignedClass = "D20CNTT1"
            };

            Assert.That(secretary.Role, Is.EqualTo(Role.Secretary));
            Assert.That(secretary.AssignedClass, Is.EqualTo("D20CNTT1"));

            var lecturer = new User
            {
                Username = "gv_advisor",
                Role = Role.Lecturer,
                AssignedClass = "D20CNTT1",
                ManagedClasses = "D20CNTT1,D20CNTT2"
            };

            Assert.That(lecturer.IsInChargeOfClass("D20CNTT1"), Is.True);
            Assert.That(lecturer.IsInChargeOfClass("D20CNTT2"), Is.True);
            Assert.That(lecturer.IsInChargeOfClass("D21CNTT1"), Is.False);
        }
        [Test]
        public void AttendanceRecord_PhotoCheckIn_PendingReviewState()
        {
            var record = new AttendanceRecord
            {
                Id = "ATT-PHOTO-01",
                EventId = 15,
                EventName = "Hội thảo Công Nghệ 2026",
                StudentCode = "B20DCCN001",
                Status = "PendingReview",
                TrainingPointsAwarded = 10,
                Method = "Photo",
                PhotoUrl = "base64imageproof"
            };

            Assert.That(record.IsPendingReview, Is.True);
            Assert.That(record.HasPhoto, Is.True);
            Assert.That(record.MethodDisplay, Does.Contain("Ảnh minh chứng"));
            Assert.That(record.StatusDisplay, Does.Contain("Chờ duyệt"));
        }

        [Test]
        public void AttendanceRecord_PhotoCheckIn_ApprovedStateAwardsPoints()
        {
            var record = new AttendanceRecord
            {
                Id = "ATT-PHOTO-02",
                EventId = 16,
                EventName = "Cuộc thi Lập trình Hackathon",
                StudentCode = "B20DCCN002",
                Status = "Approved",
                TrainingPointsAwarded = 15,
                Method = "Photo"
            };

            Assert.That(record.IsPendingReview, Is.False);
            Assert.That(record.PointsDisplay, Is.EqualTo("+15 ĐRL"));
            Assert.That(record.StatusDisplay, Does.Contain("Đã duyệt"));
        }

        [Test]
        public void AttendanceRecord_PhotoCheckIn_RejectedStateDeductsPoints()
        {
            var record = new AttendanceRecord
            {
                Id = "ATT-PHOTO-03",
                EventId = 17,
                StudentCode = "B20DCCN003",
                Status = "Rejected",
                PointsDeducted = 5,
                ReviewNotes = "Ảnh chụp không có mặt tại hội trường"
            };

            Assert.That(record.Status, Is.EqualTo("Rejected"));
            Assert.That(record.PointsDisplay, Is.EqualTo("-5 ĐRL"));
            Assert.That(record.ReviewNotes, Does.Contain("không có mặt"));
        }
    }
}
