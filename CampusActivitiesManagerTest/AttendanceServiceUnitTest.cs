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
    }
}
