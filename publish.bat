@echo off
chcp 65001 >nul
echo === ADOFAI Death99TauntMod GitHub发布助手 ===
echo.

cd /d "%~dp0"

REM 检查是否已经初始化git
if not exist .git (
    echo 初始化Git仓库...
    git init
    git branch -M main
) else (
    echo Git仓库已存在
)

REM 配置git用户信息
set /p username="请输入您的GitHub用户名 (Nico6719): "
if "%username%"=="" set username=Nico6719

set /p email="请输入您的GitHub邮箱: "

git config user.name "%username%"
git config user.email "%email%"

echo.
echo Git配置完成：
git config user.name
git config user.email
echo.

REM 添加所有文件
echo 添加文件到Git...
git add .

REM 提交
set /p commit_msg="请输入提交信息 (默认: Initial commit): "
if "%commit_msg%"=="" set commit_msg=Initial commit

git commit -m "%commit_msg%"

REM 添加远程仓库
echo.
echo 请先在GitHub上创建一个名为 'ADOFAI_Death99TauntMod' 的public仓库
echo 然后输入仓库URL
echo 例如: https://github.com/Nico6719/ADOFAI_Death99TauntMod.git
echo.
set /p repo_url="仓库URL: "

if not "%repo_url%"=="" (
    git remote remove origin 2>nul
    git remote add origin "%repo_url%"

    echo.
    echo 准备推送到GitHub...
    echo 如果需要认证，请输入您的GitHub Personal Access Token
    echo.

    git push -u origin main

    echo.
    echo ✅ 推送完成！
    echo 访问您的仓库: %repo_url%
) else (
    echo 未输入仓库URL，跳过推送
)

echo.
echo === 完成 ===
echo.
pause
