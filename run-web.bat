@echo off
chcp 65001 >nul
echo =================================================================
echo   ĐANG KHỞI CHẠY BẢN WEB - CAMPUS ACTIVITIES MANAGER
echo =================================================================
start http://localhost:5000
cd CampusActivitiesManager.Web
dotnet run
