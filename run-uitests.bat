@echo off
chcp 65001 >nul
cd /d "%~dp0"
echo ===============================================================================
echo   Campus Activity Manager - Chay UI Test Android (NUnit + Appium)
echo ===============================================================================
echo.
echo Thu muc hien tai: %CD%
echo Dang chay bo test: UITests.Android...
echo.

dotnet test "UITests.Android\UITests.Android.csproj" --logger "console;verbosity=normal"

echo.
echo ===============================================================================
echo Hoan tat kiem thu! Nhan phim bat ky de dong cua so nay...
pause >nul
