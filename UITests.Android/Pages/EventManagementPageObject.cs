using OpenQA.Selenium.Appium;
using UITests.Android.Core;

namespace UITests.Android.Pages
{
    /// <summary>
    /// Page Object Model cho màn hình Quản lý & Tổ chức Sự kiện (EventManagementPage).
    /// </summary>
    public class EventManagementPageObject
    {
        private readonly BaseAndroidUITest _test;

        public EventManagementPageObject(BaseAndroidUITest test)
        {
            _test = test;
        }

        public AppiumElement? TitleHeader => _test.FindByText("Quản lý & Tổ chức Sự kiện");
        public AppiumElement? SearchInput => _test.FindByAutomationId("txtSearchEvent");
        public AppiumElement? CreateEventButton => _test.FindByAutomationId("btnCreateEvent") ?? _test.FindByText("+ Thêm Sự Kiện");
        public AppiumElement? FilterAllChip => _test.FindByText("Tất cả sự kiện");
        public AppiumElement? FilterParticipatingChip => _test.FindByText("🎯 Đang tham gia");
        public AppiumElement? FilterPendingChip => _test.FindByText("⏳ Chờ duyệt");

        /// <summary>
        /// Kiểm tra màn hình Quản lý Sự kiện đã hiển thị.
        /// </summary>
        public bool IsLoaded()
        {
            return TitleHeader != null || SearchInput != null;
        }

        /// <summary>
        /// Tìm kiếm sự kiện theo từ khóa.
        /// </summary>
        public void Search(string query)
        {
            _test.SafeType(SearchInput, query);
        }

        /// <summary>
        /// Nhấn nút tạo mới sự kiện (+ Thêm Sự Kiện).
        /// </summary>
        public void ClickCreateEvent()
        {
            _test.SafeClick(CreateEventButton);
        }

        /// <summary>
        /// Chọn danh mục lọc sự kiện.
        /// </summary>
        public void SelectCategory(string category)
        {
            var chip = _test.FindByText(category);
            _test.SafeClick(chip);
        }
    }
}
