using System.Text.RegularExpressions;
using CampusActivitiesManager.BusinessLogic.Services;
using Microsoft.Maui.Controls;

namespace CampusActivitiesManagerTest
{
    [TestFixture]
    public class QrCodeServiceUnitTest
    {
        private static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        private const string DefaultPayload = "CAMPUS-ACTIVITIES-MANAGER";

        private QrCodeService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _service = new QrCodeService();
        }

        /// <summary>Đọc chiều rộng ảnh từ chunk IHDR của file PNG (byte 16-19, big-endian).</summary>
        private static int ReadPngWidth(byte[] png)
        {
            return (png[16] << 24) | (png[17] << 16) | (png[18] << 8) | png[19];
        }

        private static bool StartsWithPngSignature(byte[] data)
        {
            return data.Length >= PngSignature.Length && data.Take(PngSignature.Length).SequenceEqual(PngSignature);
        }

        // ---------- GenerateQrCodePngBytes ----------

        [Test]
        public void GenerateQrCodePngBytes_ValidPayload_ReturnsNonEmptyPng()
        {
            var bytes = _service.GenerateQrCodePngBytes("CAMPUS|EVT:1|STU:B21DCCN001");

            Assert.That(bytes, Is.Not.Null.And.Not.Empty);
            Assert.That(StartsWithPngSignature(bytes), Is.True, "Dữ liệu trả về phải có chữ ký PNG hợp lệ.");
        }

        [Test]
        public void GenerateQrCodePngBytes_SamePayload_IsDeterministic()
        {
            var first = _service.GenerateQrCodePngBytes("hello");
            var second = _service.GenerateQrCodePngBytes("hello");

            Assert.That(second, Is.EqualTo(first));
        }

        [Test]
        public void GenerateQrCodePngBytes_DifferentPayloads_ProduceDifferentImages()
        {
            var a = _service.GenerateQrCodePngBytes("payload-A");
            var b = _service.GenerateQrCodePngBytes("payload-B");

            Assert.That(b, Is.Not.EqualTo(a));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GenerateQrCodePngBytes_NullOrWhitespacePayload_FallsBackToDefaultPayload(string? payload)
        {
            var expected = _service.GenerateQrCodePngBytes(DefaultPayload);

            var actual = _service.GenerateQrCodePngBytes(payload!);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GenerateQrCodePngBytes_LargerPixelsPerModule_ProducesWiderImage()
        {
            var small = _service.GenerateQrCodePngBytes("size-test", 5);
            var large = _service.GenerateQrCodePngBytes("size-test", 20);

            Assert.That(ReadPngWidth(large), Is.GreaterThan(ReadPngWidth(small)));
        }

        [Test]
        public void GenerateQrCodePngBytes_DefaultPixelsPerModule_EqualsExplicit20()
        {
            var implicitDefault = _service.GenerateQrCodePngBytes("default-size");
            var explicit20 = _service.GenerateQrCodePngBytes("default-size", 20);

            Assert.That(implicitDefault, Is.EqualTo(explicit20));
        }

        [Test]
        public void GenerateQrCodePngBytes_LongPayload_StillGeneratesPng()
        {
            var payload = new string('A', 500);

            var bytes = _service.GenerateQrCodePngBytes(payload, 4);

            Assert.That(StartsWithPngSignature(bytes), Is.True);
        }

        // ---------- GenerateQrCodeImageSource ----------

        /// <summary>
        /// ImageSource là BindableObject của MAUI nên khi khởi tạo cần WinUI DispatcherQueue.
        /// Trên test host thông thường (không có app MAUI) sẽ ném COMException -> bỏ qua có chủ đích.
        /// </summary>
        private StreamImageSource CreateImageSourceOrIgnore(string payload)
        {
            try
            {
                return (StreamImageSource)_service.GenerateQrCodeImageSource(payload);
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                Assert.Ignore("Cần runtime WinUI/MAUI (DispatcherQueue) để tạo ImageSource; không khả dụng trong test host này.");
                throw; // không bao giờ tới đây
            }
        }

        [Test]
        public void GenerateQrCodeImageSource_ValidPayload_ReturnsStreamImageSource()
        {
            var source = CreateImageSourceOrIgnore("CAMPUS|EVT:2");

            Assert.That(source, Is.InstanceOf<StreamImageSource>());
        }

        [Test]
        public async Task GenerateQrCodeImageSource_Stream_ContainsPngData()
        {
            var source = CreateImageSourceOrIgnore("stream-test");

            using var stream = await source.Stream(CancellationToken.None);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            Assert.That(StartsWithPngSignature(ms.ToArray()), Is.True);
        }

        [Test]
        public async Task GenerateQrCodeImageSource_Stream_CanBeReadMultipleTimes()
        {
            var source = CreateImageSourceOrIgnore("reread-test");

            using var s1 = await source.Stream(CancellationToken.None);
            using var s2 = await source.Stream(CancellationToken.None);

            Assert.That(s2.Length, Is.EqualTo(s1.Length));
            Assert.That(s1.Length, Is.GreaterThan(0));
        }

        // ---------- GenerateTicketPayload ----------

        [Test]
        public void GenerateTicketPayload_ValidInput_MatchesExpectedFormat()
        {
            var payload = _service.GenerateTicketPayload(12, "b21dccn045", "Hội thảo AI");

            Assert.That(payload, Does.Match(@"^CAMPUS\|EVT:12\|STU:B21DCCN045\|DATE:\d{8}\|SIG:[0-9A-F]{8}$"));
        }

        [Test]
        public void GenerateTicketPayload_StudentCode_IsTrimmedAndUppercased()
        {
            var payload = _service.GenerateTicketPayload(1, "  b21dccn001  ", "Event");

            Assert.That(payload, Does.Contain("|STU:B21DCCN001|"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GenerateTicketPayload_EmptyStudentCode_UsesDefaultStudent(string? studentCode)
        {
            var payload = _service.GenerateTicketPayload(5, studentCode!, "Event");

            Assert.That(payload, Does.Contain("|STU:STUDENT|"));
        }

        [Test]
        public void GenerateTicketPayload_DateSegment_IsTodayUtc()
        {
            var before = DateTime.UtcNow.ToString("yyyyMMdd");
            var payload = _service.GenerateTicketPayload(1, "B21DCCN001", "Event");
            var after = DateTime.UtcNow.ToString("yyyyMMdd");

            var date = Regex.Match(payload, @"\|DATE:(\d{8})\|").Groups[1].Value;

            // Cho phép trường hợp đổi ngày UTC giữa hai lần đọc đồng hồ.
            Assert.That(date, Is.EqualTo(before).Or.EqualTo(after));
        }

        [Test]
        public void GenerateTicketPayload_SameInputSameDay_IsDeterministic()
        {
            var first = _service.GenerateTicketPayload(3, "B21DCCN010", "Event");
            var second = _service.GenerateTicketPayload(3, "B21DCCN010", "Event");

            Assert.That(second, Is.EqualTo(first));
        }

        [Test]
        public void GenerateTicketPayload_EventName_DoesNotAffectPayload()
        {
            var a = _service.GenerateTicketPayload(3, "B21DCCN010", "Event A");
            var b = _service.GenerateTicketPayload(3, "B21DCCN010", "Event B");

            Assert.That(b, Is.EqualTo(a));
        }

        [Test]
        public void GenerateTicketPayload_DifferentEventId_ChangesSignature()
        {
            var a = _service.GenerateTicketPayload(1, "B21DCCN010", "Event");
            var b = _service.GenerateTicketPayload(2, "B21DCCN010", "Event");

            Assert.That(SignatureOf(b), Is.Not.EqualTo(SignatureOf(a)));
        }

        [Test]
        public void GenerateTicketPayload_DifferentStudent_ChangesSignature()
        {
            var a = _service.GenerateTicketPayload(1, "B21DCCN010", "Event");
            var b = _service.GenerateTicketPayload(1, "B21DCCN011", "Event");

            Assert.That(SignatureOf(b), Is.Not.EqualTo(SignatureOf(a)));
        }

        [Test]
        public void GenerateTicketPayload_CaseInsensitiveStudentCode_SameSignature()
        {
            var lower = _service.GenerateTicketPayload(1, "b21dccn010", "Event");
            var upper = _service.GenerateTicketPayload(1, "B21DCCN010", "Event");

            Assert.That(lower, Is.EqualTo(upper));
        }

        [Test]
        public void GenerateTicketPayload_CanBeEncodedAsQrCode()
        {
            var payload = _service.GenerateTicketPayload(9, "B21DCCN099", "Event");

            var png = _service.GenerateQrCodePngBytes(payload);

            Assert.That(StartsWithPngSignature(png), Is.True);
        }

        private static string SignatureOf(string payload)
        {
            return Regex.Match(payload, @"\|SIG:([0-9A-F]{8})$").Groups[1].Value;
        }
    }
}
