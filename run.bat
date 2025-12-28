@echo off
echo Running TSFM...
echo.

if not exist "bin\Release\net8.0-windows\TSFM.exe" (
    echo ERROR: Application not built!
    echo Please run build.bat first.
    pause
    exit /b 1
)

cd bin\Release\net8.0-windows
TSFM.exe
cd ..\..\..

pause
