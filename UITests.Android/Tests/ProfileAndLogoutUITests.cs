using NUnit.Framework;
using UITests.Android.Core;
using UITests.Android.Pages;

namespace UITests.Android.Tests
{
    /// <summary>
    /// Bộ kiểm thử tự động giao diện Android: Hồ sơ cá nhân (ProfilePage) & Luồng Đăng xuất (Logout).
    /// </summary>
    [TestFixture]
    [Category("Android_UI")]
    [Category("US07_US28_Profile_Logout")]
    public class ProfileAndLogoutUITests : BaseAndroidUITest
    {
        private LoginPageObject _loginPage = null!;
        private MainPageObject _mainPage = null!;

        public override void BaseSetUp()
        {
            base.BaseSetUp();
            _loginPage = new LoginPageObject(this);
            _mainPage = new MainPageObject(this);

            // Đăng nhập tài khoản sinh viên trước khi vào hồ sơ
            _loginPage.EnterCredentials("B20DCCN001", "Student@123");
            _loginPage.ClickLogin();
        }

        [Test]
        [Description("TC_UI_14: Màn hình Hồ sơ cá nhân hiển thị chính xác thông tin sinh viên")]
        public void TC_UI_14_ProfilePage_ShouldDisplayUserInfo()
        {
            _mainPage.SwitchTab("Cá nhân");

            var fullNameInput = FindByAutomationId("txtProfileFullName");
            var phoneInput = FindByAutomationId("txtProfilePhoneNumber");
            var saveButton = FindByAutomationId("btnSaveProfile");

            Assert.Multiple(() =>
            {
                Assert.That(fullNameInput, Is.Not.Null, "Ô hiển thị họ và tên không tồn tại.");
                Assert.That(phoneInput, Is.Not.Null, "Ô hiển thị số điện thoại không tồn tại.");
                Assert.That(saveButton, Is.Not.Null, "Nút Lưu thay đổi thông tin không tồn tại.");
            });
        }

        [Test]
        [Description("TC_UI_15: Thực hiện đăng xuất từ Flyout Menu đưa người dùng quay lại màn hình Đăng nhập")]
        public void TC_UI_15_Logout_ShouldReturnUserToLoginPage()
        {
            // Mở thanh điều hướng Flyout Menu
            _mainPage.OpenMenu();

            var logoutItem = _mainPage.LogoutMenuItem;
            Assert.That(logoutItem, Is.Not.Null, "Mục Đăng xuất không có trong Flyout Menu.");

            SafeClick(logoutItem);

            // Xác nhận hộp thoại DisplayAlert xác nhận đăng xuất
            var confirmBtn = FindByText("Đăng xuất") ?? FindByText("OK");
            if (confirmBtn != null)
            {
                SafeClick(confirmBtn);
            }

            // Kiểm tra đã quay về màn hình Đăng nhập
            Assert.Multiple(() =>
            {
                Assert.That(_loginPage.IsLoaded(), Is.True, "Sau khi đăng xuất không quay về màn hình Đăng nhập.");
                Assert.That(_loginPage.LoginButton, Is.Not.Null, "Màn hình đăng nhập thiếu nút Đăng nhập.");
            });
        }
    }
}
