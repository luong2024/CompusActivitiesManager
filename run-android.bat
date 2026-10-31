@echo off
chcp 65001 >nul
echo ===============================================================================
echo   Campus Activity Manager - Khoi Dong Android (API + .NET MAUI Android)
echo ===============================================================================
echo 1. Dang khoi dong Backend API tai http://0.0.0.0:5073 cho Android Emulator (10.0.2.2:5073)...
start "Campus Activities API Server" cmd /k "dotnet run --project CampusActivitiesManager.Api"

echo 2. Cho API san sang trong 4 giay...
timeout /t 4 /nobreak >nul

echo 3. Dang build va deploy ung dung len Android Emulator / Thiet bi Android...
dotnet build CampusActivitiesManager\CampusActivitiesManager\CampusActivitiesManager.csproj -t:Run -f net9.0-android
