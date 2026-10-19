@echo off
chcp 65001 >nul
echo ===============================================================================
echo   Campus Activity Manager - Khoi Dong Desktop (API + .NET MAUI Windows)
echo ===============================================================================
echo 1. Dang khoi dong Backend API tai https://localhost:7258 (va http://localhost:5073)...
start "Campus Activities API Server" cmd /k "dotnet run --project CampusActivitiesManager.Api"

echo 2. Cho API san sang trong 4 giay...
timeout /t 4 /nobreak >nul

echo 3. Dang khoi dong ung dung .NET MAUI Windows...
dotnet run --project CampusActivitiesManager\CampusActivitiesManager\CampusActivitiesManager.csproj -f net9.0-windows10.0.19041.0
