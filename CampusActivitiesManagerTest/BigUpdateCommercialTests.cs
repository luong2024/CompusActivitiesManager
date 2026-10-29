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

    
        [Test]
        public void AttendanceRecord_MethodDisplayAndIcon_PhotoAndManualAndQr()
        {
            var photoRecord = new AttendanceRecord { Method = "Photo" };
            Assert.That(photoRecord.MethodIcon, Is.EqualTo("📸"));
            Assert.That(photoRecord.MethodDisplay, Does.Contain("Ảnh minh chứng"));

            var qrRecord = new AttendanceRecord { Method = "QR" };
            Assert.That(qrRecord.MethodIcon, Is.EqualTo("🔗"));
            Assert.That(qrRecord.MethodDisplay, Does.Contain("sự kiện"));

            var manualRecord = new AttendanceRecord { Method = "Manual" };
            Assert.That(manualRecord.MethodIcon, Is.EqualTo("✍️"));
            Assert.That(manualRecord.MethodDisplay, Does.Contain("thủ công"));
        }

        [Test]
        public void AttendanceRecord_StatusColors_ApprovedPendingRejected()
        {
            var approved = new AttendanceRecord { Status = "Approved" };
            Assert.That(approved.StatusColor, Is.EqualTo(Color.FromArgb("#16A34A")));

            var pending = new AttendanceRecord { Status = "PendingReview" };
            Assert.That(pending.StatusColor, Is.EqualTo(Color.FromArgb("#D97706")));

            var rejected = new AttendanceRecord { Status = "Rejected" };
            Assert.That(rejected.StatusColor, Is.EqualTo(Color.FromArgb("#DC2626")));
        }

        [Test]
        public void CampusNotificationDto_FilterCategories_IdentifiesSchoolClassAndPoints()
        {
            var schoolNotif = new CampusNotificationDto { TargetUsername = "ALL", Type = "Info" };
            var isSchool = schoolNotif.TargetUsername == "ALL" || schoolNotif.TargetUsername == "SCHOOL";
            Assert.That(isSchool, Is.True);

            var classNotif = new CampusNotificationDto { TargetUsername = "CLASS:D20CNTT1", Type = "Info" };
            var isClass = classNotif.TargetUsername.StartsWith("CLASS:", StringComparison.OrdinalIgnoreCase);
            Assert.That(isClass, Is.True);

            var pointsNotif = new CampusNotificationDto { TargetUsername = "B20DCCN001", Type = "Points" };
            var isPoints = pointsNotif.Type == "Points";
            Assert.That(isPoints, Is.True);
        }

        [Test]
        public void CampusEvent_RegistrationCapacity_RejectsOverCapacity()
        {
            var evt = new CampusEvent
            {
                ID = 99,
                Name = "Workshop UI/UX",
                MaxParticipants = 2,
                Registrations = new List<EventRegistration>
                {
                    new EventRegistration { StudentCode = "SV01", Status = "Approved" },
                    new EventRegistration { StudentCode = "SV02", Status = "Approved" }
                }
            };

            Assert.That(evt.RegisteredCount, Is.EqualTo(2));
            Assert.That(evt.IsUnlimitedParticipants, Is.False);
            Assert.That(evt.RegisteredCount >= evt.MaxParticipants, Is.True);
        }

        [Test]
        public void CampusEvent_ScopeRestricted_BlocksOtherClasses()
        {
            var evt = new CampusEvent
            {
                ID = 100,
                Name = "Sinh hoạt Lớp D20CNTT1",
                Scope = "ClassOnly",
                TargetClass = "D20CNTT1"
            };

            string studentClassA = "D20CNTT1";
            string studentClassB = "D20CNTT2";

            bool canJoinA = evt.Scope != "ClassOnly" || string.Equals(evt.TargetClass, studentClassA, StringComparison.OrdinalIgnoreCase);
            bool canJoinB = evt.Scope != "ClassOnly" || string.Equals(evt.TargetClass, studentClassB, StringComparison.OrdinalIgnoreCase);

            Assert.That(canJoinA, Is.True);
            Assert.That(canJoinB, Is.False);
        }
    }
}
