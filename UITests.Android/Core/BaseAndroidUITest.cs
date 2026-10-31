using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using UITests.Android.Configuration;

namespace UITests.Android.Core
{
    /// <summary>
    /// Base class cho toàn bộ kịch bản kiểm thử giao diện Android.
    /// Quản lý vòng đời kết nối Appium Driver, cơ chế kiểm tra server và các tiện ích định vị phần tử.
    /// </summary>
    public abstract class BaseAndroidUITest
    {
        public AndroidDriver? Driver { get; private set; }

        [SetUp]
        public virtual void BaseSetUp()
        {
            if (!AppiumDriverFactory.IsServerAvailable())
            {
                Assert.Ignore($"Appium Server không khả dụng tại {AppiumConfig.ServerUri}. " +
                              $"Vui lòng khởi động Appium (chạy lệnh 'appium') và mở máy ảo Android để chạy kiểm thử UI thực tế.");
            }

            try
            {
                Driver = AppiumDriverFactory.CreateDriver();
            }
            catch (Exception ex)
            {
                Assert.Ignore($"Không thể kết nối đến Appium Server / Thiết bị Android: {ex.Message}");
            }
        }

        [TearDown]
        public virtual void BaseTearDown()
        {
            if (Driver != null)
            {
                try
                {
                    if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
                    {
                        TakeScreenshot(TestContext.CurrentContext.Test.Name);
                    }
                }
                catch { }

                try
                {
                    Driver.Quit();
                    Driver.Dispose();
                }
                catch { }
                finally
                {
                    Driver = null;
                }
            }
        }

        /// <summary>
        /// Tìm phần tử theo AccessibilityId (tương ứng với AutomationId trong .NET MAUI) với cơ chế chờ hiển thị.
        /// </summary>
        public AppiumElement? FindByAutomationId(string automationId, TimeSpan? timeout = null)
        {
            var wait = new DefaultWait<AndroidDriver>(Driver!)
            {
                Timeout = timeout ?? AppiumConfig.ExplicitWait,
                PollingInterval = TimeSpan.FromMilliseconds(400)
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));

            try
            {
                return wait.Until(d =>
                {
                    // 1. Tìm qua AccessibilityId
                    try
                    {
                        var el = d.FindElement(MobileBy.AccessibilityId(automationId));
                        if (el != null && el.Displayed) return el;
                    }
                    catch { }

                    // 2. Fallback tìm qua ID tài nguyên Android
                    try
                    {
                        var el = d.FindElement(By.Id($"{AppiumConfig.AppPackage}:id/{automationId}"));
                        if (el != null && el.Displayed) return el;
                    }
                    catch { }

                    // 3. Fallback tìm qua XPath content-desc hoặc resource-id
                    try
                    {
                        var el = d.FindElement(By.XPath($"//*[@content-desc='{automationId}' or @resource-id='{automationId}']"));
                        if (el != null && el.Displayed) return el;
                    }
                    catch { }

                    return null;
                });
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
        }

        /// <summary>
        /// Tìm phần tử theo nội dung văn bản (Text / Label) hiển thị trên màn hình.
        /// </summary>
        public AppiumElement? FindByText(string text, TimeSpan? timeout = null)
        {
            var wait = new DefaultWait<AndroidDriver>(Driver!)
            {
                Timeout = timeout ?? AppiumConfig.ExplicitWait,
                PollingInterval = TimeSpan.FromMilliseconds(400)
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));

            try
            {
                return wait.Until(d =>
                {
                    try
                    {
                        var el = d.FindElement(MobileBy.AndroidUIAutomator($"new UiSelector().text(\"{text}\")"));
                        if (el != null && el.Displayed) return el;
                    }
                    catch { }

                    try
                    {
                        var el = d.FindElement(By.XPath($"//*[@text='{text}' or contains(@text, '{text}') or @content-desc='{text}' or contains(@content-desc, '{text}')]"));
                        if (el != null && el.Displayed) return el;
                    }
                    catch { }

                    return null;
                });
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
        }

        /// <summary>
        /// Chờ phần tử xuất hiện và click an toàn.
        /// </summary>
        public bool SafeClick(AppiumElement? element)
        {
            if (element == null) return false;
            try
            {
                element.Click();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Nhập chuỗi văn bản vào ô Entry với tùy chọn xóa dữ liệu cũ trước khi gõ.
        /// </summary>
        public bool SafeType(AppiumElement? element, string text, bool clearFirst = true)
        {
            if (element == null) return false;
            try
            {
                if (clearFirst) element.Clear();
                element.SendKeys(text);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Cuộn màn hình tìm kiếm phần tử chứa văn bản chỉ định.
        /// </summary>
        public AppiumElement? ScrollToText(string text)
        {
            try
            {
                return Driver?.FindElement(MobileBy.AndroidUIAutomator(
                    $"new UiScrollable(new UiSelector().scrollable(true)).scrollIntoView(new UiSelector().textContains(\"{text}\"))"));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Cuộn xuống trang.
        /// </summary>
        public void ScrollDown()
        {
            try
            {
                Driver?.FindElement(MobileBy.AndroidUIAutomator("new UiScrollable(new UiSelector().scrollable(true)).scrollForward()"));
            }
            catch { }
        }

        /// <summary>
        /// Cuộn lên đầu trang.
        /// </summary>
                /// <summary>
        /// Cuộn lên đầu trang (to beginning).
        /// </summary>
        public void ScrollToEnd()
        {
            try { Driver?.FindElement(MobileBy.AndroidUIAutomator("new UiScrollable(new UiSelector().scrollable(true)).scrollToEnd(5)")); } catch { }
        }

        public void ScrollToBeginning()
        {
            try
            {
                Driver?.FindElement(MobileBy.AndroidUIAutomator("new UiScrollable(new UiSelector().scrollable(true)).scrollToBeginning(5)"));
            }
            catch { }
        }

        public void ScrollUp()
        {
            try
            {
                Driver?.FindElement(MobileBy.AndroidUIAutomator("new UiScrollable(new UiSelector().scrollable(true)).scrollBackward()"));
            }
            catch { }
        }

        /// <summary>
        /// Thực hiện thao tác vuốt màn hình (Swipe/Scroll) theo tọa độ.
        /// </summary>
        /// <summary>
        /// Chạm vào tọa độ màn hình chỉ định.
        /// </summary>
        public void TapCoordinate(int x, int y)
        {
            try
            {
                var adbPath = @"C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe";
                if (System.IO.File.Exists(adbPath))
                {
                    var p = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = adbPath,
                        Arguments = $"-s emulator-5554 shell input tap {x} {y}",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                    p?.WaitForExit(2000);
                }
            }
            catch { }
        }

        /// <summary>
        /// Thực hiện thao tác vuốt màn hình (Swipe/Scroll) theo tọa độ.
        /// </summary>
        public void Swipe(int startX, int startY, int endX, int endY, int durationMs = 300)
        {
            try
            {
                var adbPath = @"C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe";
                if (System.IO.File.Exists(adbPath))
                {
                    var p = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = adbPath,
                        Arguments = $"-s emulator-5554 shell input swipe {startX} {startY} {endX} {endY} {durationMs}",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                    p?.WaitForExit(2000);
                    return;
                }
            }
            catch { }

            if (Driver == null) return;
            try
            {
                var touch = new OpenQA.Selenium.Interactions.PointerInputDevice(OpenQA.Selenium.Interactions.PointerKind.Touch, "finger");
                var sequence = new OpenQA.Selenium.Interactions.ActionSequence(touch);
                sequence.AddAction(touch.CreatePointerMove(OpenQA.Selenium.Interactions.CoordinateOrigin.Viewport, startX, startY, TimeSpan.Zero));
                sequence.AddAction(touch.CreatePointerDown(OpenQA.Selenium.Interactions.MouseButton.Left));
                sequence.AddAction(touch.CreatePointerMove(OpenQA.Selenium.Interactions.CoordinateOrigin.Viewport, endX, endY, TimeSpan.FromMilliseconds(durationMs)));
                sequence.AddAction(touch.CreatePointerUp(OpenQA.Selenium.Interactions.MouseButton.Left));
                Driver.PerformActions(new[] { sequence });
            }
            catch { }
        }

        /// <summary>
        /// Vuốt từ dưới lên trên để cuộn trang xuống.
        /// </summary>
        public void SwipeDown()
        {
            Swipe(500, 1800, 500, 300, 300);
        }

        /// <summary>
        /// Vuốt từ trên xuống dưới để cuộn trang lên đầu.
        /// </summary>
        public void SwipeUp()
        {
            Swipe(500, 300, 500, 1800, 300);
        }

        /// <summary>
        /// Tìm phần tử theo ClassName và chỉ số index (ví dụ: android.widget.EditText).
        /// </summary>
        public AppiumElement? FindByClassName(string className, int index = 0)
        {
            try
            {
                // XPath index is 1-based: (//className)[index+1]
                return Driver?.FindElement(By.XPath($"(//{className})[{index + 1}]"));
            }
            catch { }

            try
            {
                var elements = Driver?.FindElements(By.XPath($"//{className}"));
                if (elements != null && elements.Count > index)
                {
                    return (AppiumElement)elements.ElementAt(index);
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Chụp màn hình khi kiểm thử thất bại để phục vụ điều tra lỗi.
        /// </summary>
        public void TakeScreenshot(string testName)
        {
            if (Driver == null) return;
            try
            {
                var screenshot = Driver.GetScreenshot();
                var screenshotDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");
                Directory.CreateDirectory(screenshotDir);
                var filePath = Path.Combine(screenshotDir, $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                screenshot.SaveAsFile(filePath);
                TestContext.AddTestAttachment(filePath, $"Screenshot lỗi: {testName}");
            }
            catch { }
        }
    }
}
