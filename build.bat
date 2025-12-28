@echo off
echo ========================================
echo Building TSFM (WPF .NET 8)
echo ========================================
echo.

REM Check if .NET 8 SDK is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET 8 SDK is not installed!
    echo Please download from: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo .NET SDK version:
dotnet --version
echo.

REM Restore NuGet packages
echo ========================================
echo Restoring NuGet packages...
echo ========================================
dotnet restore
if %errorlevel% neq 0 (
    echo Package restore failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Building Release configuration...
echo ========================================
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo Build failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Build successful!
echo ========================================
echo Executable: bin\Release\net8.0-windows\TSFM.exe
echo.

pause
