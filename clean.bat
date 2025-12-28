@echo off
echo ========================================
echo Cleaning TSFM build artifacts
echo ========================================
echo.

if exist "bin" (
    echo Removing bin folder...
    rmdir /s /q bin
)

if exist "obj" (
    echo Removing obj folder...
    rmdir /s /q obj
)

echo.
echo ========================================
echo Clean complete!
echo ========================================
echo.

pause
