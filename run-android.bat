@echo off
echo =================================================================
echo   KHOI CHAY CAMPUS ACTIVITIES MANAGER TREN MAY AO PIXEL 7
echo =================================================================

set ADB="C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe"
set EMULATOR="C:\Program Files (x86)\Android\android-sdk\emulator\emulator.exe"
set APK="CampusActivitiesManager\CampusActivitiesManager\bin\Debug\net9.0-android\com.companyname.campusactivitiesmanager-Signed.apk"

echo [1/3] Kiem tra may ao Android Pixel 7...
%ADB% devices | findstr /R "emulator-[0-9]*" >nul
if errorlevel 1 (
    echo [!] May ao chua mo. Dang bat may ao Pixel 7...
    start "" %EMULATOR% -avd pixel_7_-_api_35_0 -no-snapshot-load -gpu host
    echo [!] Dang cho may ao khoi dong xong...
    %ADB% wait-for-device
    timeout /t 10 /nobreak >nul
) else (
    echo [OK] May ao Pixel 7 dang ket noi san sang!
)

echo [2/3] Cai dat app vao may ao Pixel 7...
%ADB% install -r %APK%

echo [3/3] Mo ung dung Campus Activities Manager...
%ADB% shell monkey -p com.companyname.campusactivitiesmanager -c android.intent.category.LAUNCHER 1 >nul 2>&1

echo =================================================================
echo   DA MO UNG DUNG THANH CONG TREN MAN HINH PIXEL 7!
echo =================================================================
