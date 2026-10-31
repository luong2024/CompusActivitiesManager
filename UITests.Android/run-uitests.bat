@echo off
chcp 65001 >nul
echo ===============================================================================
echo   Campus Activity Manager - Chay Android UI Test (Appium + NUnit)
echo ===============================================================================
echo.

echo 1. Kiem tra thiet bi Android qua adb...
adb devices
echo.

echo 2. Thuc thi kiem thu giao dien Android...
dotnet test "%~dp0UITests.Android.csproj" --logger "console;verbosity=normal"

echo.
echo Hoan tat chay test. Nhan phim bat ky de thoat...
pause >nul
