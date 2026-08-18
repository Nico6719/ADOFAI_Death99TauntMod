#!/bin/bash

# ADOFAI Death99TauntMod - GitHub发布脚本
# 作者: Nico6719

echo "=== ADOFAI Death99TauntMod GitHub发布助手 ==="
echo ""

# 检查是否已经初始化git
if [ ! -d .git ]; then
    echo "初始化Git仓库..."
    git init
    git branch -M main
else
    echo "Git仓库已存在"
fi

# 配置git用户信息（如果需要）
read -p "请输入您的GitHub用户名: " username
read -p "请输入您的GitHub邮箱: " email

git config user.name "$username"
git config user.email "$email"

echo ""
echo "Git配置完成："
echo "  用户名: $(git config user.name)"
echo "  邮箱: $(git config user.email)"
echo ""

# 添加所有文件
echo "添加文件到Git..."
git add .

# 提交
read -p "请输入提交信息 (默认: Initial commit): " commit_msg
commit_msg=${commit_msg:-"Initial commit"}

git commit -m "$commit_msg"

# 添加远程仓库
echo ""
echo "请先在GitHub上创建一个名为 'ADOFAI_Death99TauntMod' 的public仓库"
echo "然后输入仓库URL (例如: https://github.com/Nico6719/ADOFAI_Death99TauntMod.git)"
read -p "仓库URL: " repo_url

if [ -n "$repo_url" ]; then
    git remote remove origin 2>/dev/null
    git remote add origin "$repo_url"

    echo ""
    echo "准备推送到GitHub..."
    echo "如果需要认证，请输入您的GitHub Personal Access Token"

    git push -u origin main

    echo ""
    echo "✅ 推送完成！"
    echo "访问您的仓库: $repo_url"
else
    echo "未输入仓库URL，跳过推送"
fi

echo ""
echo "=== 完成 ==="
