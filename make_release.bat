@echo off
echo ====================================
echo 打包 Death99TauntMod
echo ====================================

set MOD_NAME=Death99TauntMod
set BUILD_DIR=bin\Release\net35
set RELEASE_DIR=Release

echo.
echo 正在编译...
dotnet build -c Release

if %errorlevel% neq 0 (
    echo 编译失败！
    pause
    exit /b 1
)

echo.
echo 正在创建发布文件夹...
if exist %RELEASE_DIR% rmdir /s /q %RELEASE_DIR%
mkdir %RELEASE_DIR%
mkdir %RELEASE_DIR%\%MOD_NAME%

echo.
echo 正在复制文件...
copy %BUILD_DIR%\%MOD_NAME%.dll %RELEASE_DIR%\%MOD_NAME%\
copy Info.json %RELEASE_DIR%\%MOD_NAME%\
copy README.md %RELEASE_DIR%\

echo.
echo 正在创建ZIP压缩包...
powershell Compress-Archive -Path "%RELEASE_DIR%\%MOD_NAME%" -DestinationPath "%RELEASE_DIR%\%MOD_NAME%.zip" -Force

echo.
echo ====================================
echo 打包完成！
echo 输出文件: %RELEASE_DIR%\%MOD_NAME%.zip
echo ====================================
pause
