# ADOFAI 死亡95%嘲讽Mod

当你在《A Dance of Fire and Ice》中完成度达到95%以上时死亡，自动打开浏览器播放嘲讽视频！

## ✨ 功能特性

- 🎯 **自定义进度阈值**：可设置50%-99.9%的触发阈值（默认95%）
- 🎮 **判定窗口自定义**：根据严格/标准/宽容判定窗口播放不同的嘲讽视频
- 🌐 **自定义URL**：可以设置任意网页作为嘲讽内容
- ⚙️ **图形化配置**：通过Mod管理器界面轻松配置

## 📦 安装方法

### 前置要求
- 已安装 [Unity Mod Manager](https://www.nexusmods.com/site/mods/21)
- 游戏版本：A Dance of Fire and Ice

### 安装步骤

1. 下载最新版本的 `Death99TauntMod_v1.0.zip` 
2. 解压压缩包
3. 将 `Death99TauntMod` 文件夹复制到游戏的 `Mods` 目录：
   ```
   <游戏安装目录>/Mods/Death99TauntMod/
   ```
4. 启动游戏
5. 按 `Ctrl+F10` 打开Unity Mod Manager
6. 找到并启用 **死亡95%嘲讽Mod**

## 🎮 使用说明

1. **打开设置**：在游戏中按 `Ctrl+F10`，找到"死亡95%嘲讽Mod"
2. **调整进度阈值**：拖动滑块设置触发死亡嘲讽的最低进度（默认95%）
3. **自定义URL**（可选）：
   - 勾选"根据判定窗口自定义嘲讽URL"
   - 为严格/标准/宽容判定分别设置不同的URL
4. **开始游戏**：当你在设定进度以上死亡时，浏览器会自动打开嘲讽你！

## ⚙️ 默认配置

- **触发阈值**：95%
- **严格判定URL**：https://www.bilibili.com/video/BV1zSM46AE7w/
- **标准判定URL**：https://www.bilibili.com/video/BV1uT4y1P7CX/ (Never Gonna Give You Up)
- **宽容判定URL**：https://www.bilibili.com/video/BV1uN4y1d7Js/

## 🔧 浏览器设置（重要！）

由于浏览器的自动播放限制，您需要配置浏览器以允许B站自动播放，示例：

1. 打开Edge浏览器
2. 访问 `edge://settings/content/mediaAutoplay`
3. 找到"控制音频和视频是否在网站上自动播放"
4. 选择以下之一：
   - 将设置改为"允许"（所有网站都能自动播放）
   - 在"允许"列表中添加 `bilibili.com`

这样嘲讽视频就能自动播放且有声音了！

## 🐛 常见问题

### Q: 视频自动播放但没有声音？
A: 这是浏览器的自动播放策略导致的。请参考上面的"浏览器设置"章节配置Edge。

### Q: 可以使用其他视频网站吗？
A: 可以！在mod设置中输入任意URL即可，比如YouTube、抖音等。

### Q: 如何完全关闭这个功能？
A: 在Mod管理器中禁用此mod即可。

## 🛠️ 开发构建

```bash
# 克隆仓库
git clone https://github.com/Nico6719/ADOFAI_Death99TauntMod.git
cd ADOFAI_Death99TauntMod

# 编译
dotnet build -c Release

# 输出位置
bin/Release/net48/Death99TauntMod.dll
```

## 📝 更新日志

### v1.0.0 (2024-08-18)
- 🎉 首次发布
- ✅ 支持进度阈值自定义
- ✅ 支持根据判定窗口自定义URL
- ✅ 图形化配置界面

## 📄 许可证

本项目采用 MIT 许可证。详见 [LICENSE](LICENSE) 文件。

## 👤 作者

**Nico6719**

## 🌟 致谢

感谢 [JipperResourcePack](https://github.com/Jongye0l/JipperResourcePack) 提供的参考实现。

---

⚠️ **免责声明**：本mod仅供娱乐使用，请勿在严肃场合使用。作者不对因使用本mod造成的任何心理伤害负责！😄
