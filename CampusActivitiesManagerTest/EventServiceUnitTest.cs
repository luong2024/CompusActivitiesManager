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
    }
}
