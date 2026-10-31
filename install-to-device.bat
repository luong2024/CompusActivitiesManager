@echo off
chcp 65001 >nul
title Cai dat Campus Activity Manager len dien thoai Android
cls
echo ======================================================================
echo    CAI DAT UNG DUNG CAMPUS ACTIVITY MANAGER VAO DIEN THOAI ANDROID
echo ======================================================================
echo.

set ADB="%LOCALAPPDATA%\Android\Sdk\platform-tools\adb.exe"
if not exist %ADB% (
    set ADB="C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe"
)

if not exist %ADB% (
    echo [LOI] Khong tim thay cong cu adb.exe tren may tinh!
    echo Ban hay copy file 'CampusActivityManager.apk' tren Desktop vao dien thoai qua Zalo/Drive/USB.
    echo.
    pause
    exit /b 1
)

set APK="CampusActivitiesManager\CampusActivitiesManager\bin\Debug\net9.0-android\com.companyname.campusactivitiesmanager-Signed.apk"

if not exist %APK% (
    set APK="%USERPROFILE%\Desktop\CampusActivityManager.apk"
)

if not exist %APK% (
    echo [LOI] Khong tim thay file APK da ky!
    pause
    exit /b 1
)

echo [1/3] Kiem tra ket noi dien thoai qua cap USB...
echo.
%ADB% devices
echo.
echo LUU Y TRUOC KHI TIEP TUC:
echo  1. Ban da cam cap USB noi dien thoai voi may tinh.
echo  2. Tren dien thoai da bat 'Go loi USB' (USB Debugging) trong Tuy chon nha phat trien.
echo  3. Neu man hinh dien thoai hien 'Cho phep go loi USB?', hay tich 'Luon cho phep' va bam OK.
echo.
pause

echo.
echo [2/3] Dang tien hanh cai dat file APK vao dien thoai...
echo (Vui long cho khoang 10 - 20 giay...)
echo.
%ADB% install -r %APK%

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ======================================================================
    echo   [THANH CONG] DA CAI DAT XONG APP LEN DIEN THOAI CUA BAN!
    echo ======================================================================
    echo Ban co the mo ung dung 'Campus Activity Manager' tren dien thoai de test ngay.
) else (
    echo.
    echo [LOI] Cai dat khong thanh cong!
    echo Goi y:
    echo  - Kiem tra lai xem dien thoai co dang khoa man hinh khong.
    echo  - Hoac copy truc tiep file 'CampusActivityManager.apk' tu Desktop vao dien thoai de cai thu cong.
)

echo.
pause
