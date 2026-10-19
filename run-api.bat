@echo off
chcp 65001 >nul
echo ===============================================================================
echo   Campus Activity Manager - Khoi Dong Backend API
echo ===============================================================================
echo API se lang nghe tai:
echo   - HTTPS: https://localhost:7258
echo   - HTTP:  http://localhost:5073 (va http://0.0.0.0:5073 cho Android Emulator)
echo ===============================================================================
dotnet run --project CampusActivitiesManager.Api
