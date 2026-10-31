using CampusActivitiesManager.Models;

namespace CampusActivitiesManagerTest
{
    [TestFixture]
    public class AttendanceServiceUnitTest
    {
        [Test]
        public void AttendanceRecord_DefaultValues_AreConsistent()
        {
            var record = new AttendanceRecord
            {
                Id = "ATT-001",
                EventId = 5,
                EventName = "Hội nghị Nghiên cứu Khoa học",
                StudentCode = "B21DCCN045",
                StudentName = "Lê Văn Cường",
                Method = "QR",
                TrainingPointsAwarded = 10,
                Status = "Approved"
            };

            Assert.That(record.Id, Is.EqualTo("ATT-001"));
            Assert.That(record.EventId, Is.EqualTo(5));
            Assert.That(record.StudentCode, Is.EqualTo("B21DCCN045"));
            Assert.That(record.Method, Is.EqualTo("QR"));
            Assert.That(record.TrainingPointsAwarded, Is.EqualTo(10));
            Assert.That(record.Status, Is.EqualTo("Approved"));
            Assert.That(record.FormattedCheckInTime, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void QrCheckInRequest_Properties_SetCorrectly()
        {
            var req = new QrCheckInRequest
            {
                EventId = 12,
                StudentCode = "B20DCCN001",
                QrTicketCode = "CAMPUS|EVT:12|STU:B20DCCN001|SIG:ABCDEF12"
            };

            Assert.That(req.EventId, Is.EqualTo(12));
            Assert.That(req.StudentCode, Is.EqualTo("B20DCCN001"));
            Assert.That(req.QrTicketCode, Does.Contain("CAMPUS|EVT:12"));
        }

        [Test]
        public void PhotoCheckInRequest_Properties_SetCorrectly()
        {
            var req = new PhotoCheckInRequest
            {
                EventId = 8,
                StudentCode = "B20DCCN002",
                PhotoBase64 = "data:image/jpeg;base64,samplephoto123",
                Notes = "Tham gia trực tiếp tại phòng A2"
            };

            Assert.That(req.EventId, Is.EqualTo(8));
            Assert.That(req.StudentCode, Is.EqualTo("B20DCCN002"));
            Assert.That(req.PhotoBase64, Is.Not.Null.And.Not.Empty);
            Assert.That(req.Notes, Does.Contain("trực tiếp"));
        }

        [Test]
        public void CampusNotificationDto_PropertiesAndUnread_AreAccurate()
        {
            var notif = new CampusNotificationDto
            {
                Id = "NOTIF-001",
                Title = "Cộng Điểm Rèn Luyện",
                Message = "Bạn vừa được cộng +10 ĐRL",
                Type = "Points",
                TargetUsername = "B20DCCN001",
                IsRead = false
            };

            Assert.That(notif.IsRead, Is.False);
            Assert.That(notif.IsUnread, Is.True);
            Assert.That(notif.TypeIcon, Is.EqualTo("🏆"));
            Assert.That(notif.FormattedTime, Is.Not.Null.And.Not.Empty);

            notif.IsRead = true;
            Assert.That(notif.IsUnread, Is.False);

            notif.Type = "Reminder";
            Assert.That(notif.TypeIcon, Is.EqualTo("⏰"));

            notif.Type = "Success";
            Assert.That(notif.TypeIcon, Is.EqualTo("🎫"));

            notif.Type = "Urgent";
            Assert.That(notif.TypeIcon, Is.EqualTo("⚠️"));
        }

        [Test]
        public void AttendanceRecord_PenaltyAndPoints_DisplayCorrectly()
        {
            var record = new AttendanceRecord
            {
                Id = "ATT-123",
                EventId = 10,
                StudentCode = "B20DCCN001",
                Status = "Rejected",
                PointsDeducted = 15,
                ReviewNotes = "Minh chứng ảnh không đúng địa điểm"
            };

            Assert.That(record.Status, Is.EqualTo("Rejected"));
            Assert.That(record.PointsDeducted, Is.EqualTo(15));
            Assert.That(record.PointsDisplay, Is.EqualTo("-15 ĐRL"));
            Assert.That(record.StatusDisplay, Does.Contain("Bị từ chối"));
        }

        [Test]
        public void CampusNotificationDto_AdminCustomNotification_CreatedWithTargetAndType()
        {
            var adminNotif = new CampusNotificationDto
            {
                Id = "NOTIF-ADMIN-01",
                Title = "Thông báo khẩn từ Ban Quản Trị",
                Message = "Tất cả sinh viên hoàn tất điểm danh trước 17:00 hôm nay",
                Type = "Urgent",
                TargetUsername = "ALL",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            Assert.That(adminNotif.Id, Is.EqualTo("NOTIF-ADMIN-01"));
            Assert.That(adminNotif.Title, Does.Contain("Ban Quản Trị"));
            Assert.That(adminNotif.TargetUsername, Is.EqualTo("ALL"));
            Assert.That(adminNotif.Type, Is.EqualTo("Urgent"));
            Assert.That(adminNotif.IsRead, Is.False);
            Assert.That(adminNotif.IsUnread, Is.True);
            Assert.That(adminNotif.FormattedTime, Is.Not.Null.And.Not.Empty);
        }
    }
}
