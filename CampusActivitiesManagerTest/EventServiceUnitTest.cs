using CampusActivitiesManager.Models;

namespace CampusActivitiesManagerTest
{
    [TestFixture]
    public class EventServiceUnitTest
    {
        [Test]
        public void CampusEvent_PendingStatus_IsPendingApprovalReturnsTrue()
        {
            var evt = new CampusEvent
            {
                ID = 101,
                Name = "Hội thảo Công Nghệ 2026",
                ApprovalStatus = "Pending",
                IsApproved = false
            };

            Assert.That(evt.IsPendingApproval, Is.True);
            Assert.That(evt.ApprovalStatusDisplay, Does.Contain("Chờ Admin duyệt"));
        }

        [Test]
        public void CampusEvent_ApprovedStatus_IsPendingApprovalReturnsFalse()
        {
            var evt = new CampusEvent
            {
                ID = 102,
                Name = "Giải Chạy Việt Dã",
                ApprovalStatus = "Approved",
                IsApproved = true
            };

            Assert.That(evt.IsPendingApproval, Is.False);
            Assert.That(evt.ApprovalStatusDisplay, Does.Contain("Đã duyệt"));
        }

        [Test]
        public void CampusEvent_PastEvent_IsEventEndedReturnsTrue()
        {
            var evt = new CampusEvent
            {
                ID = 103,
                Name = "Sự kiện tuần trước",
                StartDate = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy"),
                StartTime = "08:00",
                EndTime = "11:00",
                IsApproved = true,
                ApprovalStatus = "Approved"
            };

            Assert.That(evt.IsEventEnded, Is.True);
            Assert.That(evt.IsAttendanceOpen, Is.False);
            Assert.That(evt.CanCheckIn, Is.False);
            Assert.That(evt.AttendanceStatusBadge, Does.Contain("Hết hạn"));
        }

        [Test]
        public void CampusEvent_FutureEvent_IsEventNotStartedReturnsTrue()
        {
            var evt = new CampusEvent
            {
                ID = 104,
                Name = "Hội thao tháng tới",
                StartDate = DateTime.Now.AddDays(30).ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.AddDays(30).ToString("dd/MM/yyyy"),
                StartTime = "08:00",
                EndTime = "17:00",
                IsApproved = true,
                ApprovalStatus = "Approved"
            };

            Assert.That(evt.IsEventNotStarted, Is.True);
            Assert.That(evt.IsEventEnded, Is.False);
            Assert.That(evt.IsAttendanceOpen, Is.False);
            Assert.That(evt.CanCheckIn, Is.False);
            Assert.That(evt.AttendanceStatusBadge, Does.Contain("Chưa mở"));
        }

        [Test]
        public void CampusEvent_ActiveWindow_AttendanceIsOpen()
        {
            var evt = new CampusEvent
            {
                ID = 105,
                Name = "Sự kiện đang diễn ra hôm nay",
                StartDate = DateTime.Now.ToString("dd/MM/yyyy"),
                EndDate = DateTime.Now.ToString("dd/MM/yyyy"),
                StartTime = DateTime.Now.AddHours(-1).ToString("HH:mm"),
                EndTime = DateTime.Now.AddHours(2).ToString("HH:mm"),
                IsApproved = true,
                ApprovalStatus = "Approved"
            };

            Assert.That(evt.IsEventEnded, Is.False);
            Assert.That(evt.IsEventNotStarted, Is.False);
            Assert.That(evt.IsAttendanceOpen, Is.True);
            Assert.That(evt.CanCheckIn, Is.True);
            Assert.That(evt.AttendanceStatusBadge, Does.Contain("Đang mở"));
        }

        [Test]
        public void CampusEvent_CategoryAndLocationDisplay_FormattedCorrectly()
        {
            var cat = new EventCategoryDto { ID = 1, Title = "Học thuật", Color = "#2563EB" };
            var evt = new CampusEvent
            {
                ID = 1,
                Name = "Workshop AI",
                Category = cat,
                CategoryID = 1,
                Location = "Hội trường A2",
                StartDate = "20/11/2026",
                StartTime = "08:00",
                MaxParticipants = 100
            };

            Assert.That(evt.CategoryTitle, Is.EqualTo("Học thuật"));
            Assert.That(evt.Category?.Color, Is.EqualTo("#2563EB"));
            Assert.That(evt.ParticipantsDisplay, Does.Contain("100"));
        }

        [Test]
        public void CampusEvent_RejectedEvent_ExcludedFromActiveMetricsAndQuery()
        {
            var events = new List<CampusEvent>
            {
                new() { ID = 1, Name = "Event 1", ApprovalStatus = "Approved", IsApproved = true },
                new() { ID = 2, Name = "Event 2", ApprovalStatus = "Pending", IsApproved = false },
                new() { ID = 3, Name = "Event 3 (Rejected)", ApprovalStatus = "Rejected", IsApproved = false },
                new() { ID = 4, Name = "Event 4 (Dismissed)", Status = "Đã từ chối", IsApproved = false }
            };

            // Business rule: Any rejected activity is excluded and deleted from display
            var activeEvents = events.Where(e => e.ApprovalStatus != "Rejected" && e.Status != "Đã từ chối").ToList();
            Assert.That(activeEvents.Count, Is.EqualTo(2));
            Assert.That(activeEvents.Any(e => e.ApprovalStatus == "Rejected"), Is.False);
            Assert.That(activeEvents.Any(e => e.Status == "Đã từ chối"), Is.False);

            // Pending metrics only count active pending events
            var pendingCount = activeEvents.Count(e => e.ApprovalStatus == "Pending");
            Assert.That(pendingCount, Is.EqualTo(1));
        }

        [Test]
        public void CampusEvent_PointsConfiguration_SetsRewardAndPenaltyCorrectly()
        {
            var evt = new CampusEvent
            {
                ID = 201,
                Name = "Cuộc thi Lập trình Olympic Tin học",
                PointsReward = 25,
                PointsPenalty = 10,
                Scope = "Public",
                RegistrationType = "ApprovalRequired"
            };

            Assert.That(evt.PointsReward, Is.EqualTo(25));
            Assert.That(evt.PointsPenalty, Is.EqualTo(10));
            Assert.That(evt.PointsRewardDisplay, Does.Contain("+25"));
            Assert.That(evt.PointsPenaltyDisplay, Does.Contain("-10"));
            Assert.That(evt.TrainingPointsReward, Is.EqualTo(25));
            Assert.That(evt.TrainingPointsRewardDisplay, Does.Contain("+25"));
        }

        [Test]
        public void CampusEvent_DefaultPoints_InitializedWithValidDefaults()
        {
            var evt = new CampusEvent();

            Assert.That(evt.PointsReward, Is.EqualTo(10));
            Assert.That(evt.PointsPenalty, Is.EqualTo(5));
            Assert.That(evt.PointsRewardDisplay, Does.Contain("+10"));
            Assert.That(evt.PointsPenaltyDisplay, Does.Contain("-5"));
        }

        [Test]
        public void CampusEvent_CapacityLimit_UnlimitedVersusLimited()
        {
            var unlimitedEvt = new CampusEvent { MaxParticipants = 0 };
            Assert.That(unlimitedEvt.IsUnlimitedParticipants, Is.True);
            Assert.That(unlimitedEvt.ParticipantsDisplay, Does.Contain("Không giới hạn"));

            var negativeEvt = new CampusEvent { MaxParticipants = -1 };
            Assert.That(negativeEvt.IsUnlimitedParticipants, Is.True);

            var limitedEvt = new CampusEvent { MaxParticipants = 50 };
            Assert.That(limitedEvt.IsUnlimitedParticipants, Is.False);
            Assert.That(limitedEvt.ParticipantsDisplay, Does.Contain("/50"));
        }

        [Test]
        public void CampusEvent_Scope_ClassOnlyVersusPublicBadge()
        {
            var classEvt = new CampusEvent
            {
                Scope = "ClassOnly",
                TargetClass = "D20CNTT1"
            };
            Assert.That(classEvt.ScopeBadgeText, Does.Contain("Lớp D20CNTT1"));
            Assert.That(classEvt.ScopeBadge, Does.Contain("Lớp D20CNTT1"));

            var publicEvt = new CampusEvent
            {
                Scope = "Public"
            };
            Assert.That(publicEvt.ScopeBadgeText, Does.Contain("Toàn trường"));
        }

        [Test]
        public void CampusEvent_RegistrationCapacity_CalculationMatchesCount()
        {
            var evt = new CampusEvent
            {
                MaxParticipants = 2,
                Registrations = new List<EventRegistration>
                {
                    new EventRegistration { StudentCode = "SV01", Status = "Approved" },
                    new EventRegistration { StudentCode = "SV02", Status = "Pending" },
                    new EventRegistration { StudentCode = "SV03", Status = "Rejected" }
                }
            };

            // Active registrations count (Approved or Pending) is 2
            Assert.That(evt.RegisteredCount, Is.EqualTo(2));
            Assert.That(evt.ParticipantsDisplay, Does.Contain("2/2"));
            Assert.That(evt.IsUnlimitedParticipants, Is.False);
        }
    }
}
