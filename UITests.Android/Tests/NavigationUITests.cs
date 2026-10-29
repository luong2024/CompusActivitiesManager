using NUnit.Framework;
using UITests.Android.Core;
using UITests.Android.Pages;

namespace UITests.Android.Tests
{
    /// <summary>
    /// Bộ kiểm thử tự động giao diện Android: Điều hướng Bottom TabBar & Quản lý Sự kiện.
    /// </summary>
    [TestFixture]
    [Category("Android_UI")]
    [Category("US13_US31_Navigation")]
    public class NavigationUITests : BaseAndroidUITest
    {
        private LoginPageObject _loginPage = null!;
        private MainPageObject _mainPage = null!;

        public override void BaseSetUp()
        {
            base.BaseSetUp();
            _loginPage = new LoginPageObject(this);
            _mainPage = new MainPageObject(this);

            // Đăng nhập tài khoản sinh viên trước khi kiểm thử điều hướng
            _loginPage.EnterCredentials("B20DCCN001", "Student@123");
            _loginPage.ClickLogin();
        }

        [Test]
        [Description("TC_UI_11: Chuyển đổi qua lại giữa các Tab trên thanh điều hướng dưới cùng")]
        public void TC_UI_11_TabNavigation_ShouldSwitchBetweenTabs()
        {
            Assert.That(_mainPage.IsLoaded(), Is.True, "Trang chủ chưa tải sau khi đăng nhập.");

            // 1. Chuyển sang Tab Sự kiện
            _mainPage.SwitchTab("Sự kiện");
            var eventPage = new EventManagementPageObject(this);
            Assert.That(eventPage.IsLoaded(), Is.True, "Không chuyển được sang màn hình Sự kiện.");

            // 2. Chuyển sang Tab Cá nhân
            _mainPage.SwitchTab("Cá nhân");
            var profileTitle = FindByText("Chi tiết Hồ sơ") ?? FindByText("Hồ sơ cá nhân");
            Assert.That(profileTitle, Is.Not.Null, "Không chuyển được sang màn hình Hồ sơ cá nhân.");

            // 3. Quay lại Tab Trang chủ
            _mainPage.SwitchTab("Trang chủ");
            Assert.That(_mainPage.IsLoaded(), Is.True, "Không quay trở lại được Trang chủ.");
        }

        [Test]
        [Description("TC_UI_12: Tìm kiếm sự kiện theo từ khóa trên trang Quản lý sự kiện")]
        public void TC_UI_12_EventManagement_SearchEvents_ShouldFilterList()
        {
            _mainPage.SwitchTab("Sự kiện");
            var eventPage = new EventManagementPageObject(this);
            Assert.That(eventPage.SearchInput, Is.Not.Null, "Ô tìm kiếm sự kiện không hiển thị.");

            eventPage.Search("Hội Thảo AI");
            // Xác nhận có sự kiện tương ứng hiển thị
            var result = FindByText("Hội Thảo AI & Chuyển Đổi Số 2026") ?? FindByText("Hội Thảo AI");
            Assert.That(result, Is.Not.Null, "Không tìm thấy sự kiện phù hợp với từ khóa.");
        }

        [Test]
        [Description("TC_UI_13: Bấm chọn các chip lọc danh mục sự kiện (Tất cả, Đang tham gia)")]
        public void TC_UI_13_EventManagement_FilterChips_ShouldSelectCategories()
        {
            _mainPage.SwitchTab("Sự kiện");
            var eventPage = new EventManagementPageObject(this);

            Assert.Multiple(() =>
            {
                Assert.That(eventPage.FilterAllChip, Is.Not.Null, "Chip Tất cả sự kiện không tồn tại.");
                Assert.That(eventPage.FilterParticipatingChip, Is.Not.Null, "Chip Đang tham gia không tồn tại.");
            });

            // Chọn chip Đang tham gia
            SafeClick(eventPage.FilterParticipatingChip);
            // Quay lại chip Tất cả
            SafeClick(eventPage.FilterAllChip);
        }
    }
}
