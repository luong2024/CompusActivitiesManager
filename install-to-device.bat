@echo off
chcp 65001 >nul
echo ======================================================================
echo   CÀI ĐẶT ỨNG DỤNG CAMPUS ACTIVITY MANAGER VÀO ĐIỆN THOẠI ANDROID
echo ======================================================================
echo.

set ADB="%LOCALAPPDATA%\Android\Sdk\platform-tools\adb.exe"
if not exist %ADB% (
    set ADB="C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe"
)

if not exist %ADB% (
    echo [LỖI] Không tìm thấy công cụ adb.exe trên máy tính!
    echo Vui lòng cài Android SDK hoặc chuyển file APK thủ công qua Zalo/Drive.
    pause
    exit /b 1
)

set APK="CampusActivitiesManager\CampusActivitiesManager\bin\Debug\net9.0-android\com.companyname.campusactivitiesmanager-Signed.apk"

if not exist %APK% (
    echo [LỖI] Không tìm thấy file APK đã ký tại:
    echo %APK%
    echo Vui lòng chạy 'dotnet build -f net9.0-android' trước.
    pause
    exit /b 1
)

echo [1/3] Đang kiểm tra thiết bị Android kết nối qua USB...
%ADB% devices
echo.

echo Hãy chắc chắn rằng:
echo  1. Bạn đã cắm cáp USB nối điện thoại với máy tính.
echo  2. Trên điện thoại đã bật 'Tùy chọn nhà phát triển' -^> 'Gỡ lỗi USB' (USB Debugging).
echo  3. Nếu màn hình điện thoại hiện 'Cho phép gỡ lỗi USB từ máy tính này?', hãy bấm CHO PHÉP (ALLOW).
echo.
pause

echo.
echo [2/3] Đang tiến hành cài đặt file APK vào điện thoại...
%ADB% install -r %APK%

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ======================================================================
    echo   [THÀNH CÔNG] ĐÃ CÀI ĐẶT ỨNG DỤNG LÊN ĐIỆN THOẠI CỦA BẠN!
    echo ======================================================================
    echo Bạn có thể mở ứng dụng 'Campus Activity Manager' trên điện thoại để test.
) else (
    echo.
    echo [LỖI] Cài đặt thất bại! Vui lòng kiểm tra lại cáp kết nối và quyền USB Debugging.
)

echo.
pause
