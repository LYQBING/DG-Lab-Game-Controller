## 在开发之前
本程序是基于 DG-Lab-Coyote-Game-Hub 的 API 进行开发，因此可以根据您的不同需求进行开发：

**何时选择 DG-Lab-Coyote-Game-Hub ？**

它为您提供了网页 API 及控制器功能，若您不需要其他功能封装或统一管理模块时，您可以直接采用它进行开发。
适用场景建议：
- 您可以通过它为支持 MOD 功能的游戏开发 MOD
- 您可以直接通过 URL 进行惩罚控制

**何时选择 DG-Lab-Game-Controller ？**

它在原有功能之上，直接为您提供了 C# 的脚本代码，您可以直接通过代码控制郊狼进行惩罚。且支持统一的模块管理，您可以更加方便的处理用户所设置的参数。
适用场景建议：
- 您可以通过它对不支持 MOD 的游戏进行注入
- 您可以使用它快速开发通用惩罚功能

## 郊狼的 API 目录
欢迎使用 DG-Lab-Game-Controller 进行您的模块开发，同时也感谢您对 DG-LAB 游戏生态所做出的一切贡献。

**游戏惩罚数据 API**
- 根据当前情景所增加/减少或设置指定的惩罚数据
- 获取当前正在进行的惩罚数据
- [前往 游戏惩罚数据 API 手册](StrengthAPI.md)

**一键开火 API**
- 当触发特殊事件时向 DG-LAB 设备所发送的开火惩罚数据
- [前往 一键开火 API 手册](FireApi.md)

**游戏波形 API**
- 为您的 DG-LAB 设备设置符合惩罚情景的波形数据
- 获取当前正在播放的波形数据
- [前往 游戏波形 API 手册](PulseApi.md)

**响应数据 API**
- 获取当前服务器的状态及数据
- 获取当前游戏的状态及数据
- [前往 响应数据 API 手册](ResponseApi.md)

## 模块开发注意事件（待完善）
模块的入口应该继承自 ModuleBase，并且您还需要为您的模块设置一个独立的 xaml 页面。

**模块文件夹**
- 每个模块的独立文件夹名称就是他的 ModuleId 名称。
- 模块的文件夹与模块DLL应该与ModuleId保持一致。
- 您可以通过 Path.Combine(ConfigManager.ModulesPath, ModuleId); 获取到它的文件夹路径。

**项目结构**
- DGLabGameController 主程序：若您想要开发模块，仅下载此包即可进行开发
- GamepadVibrationProcessor 手柄注入功能模块：**你可以参考此模块进行开发**
- HealthBarDetector 颜色检测功能模块：**你可以参考此模块进行开发**
- GamepadVibrationHook 手柄注入功能模块的DLL文件：用于与主程序通讯

**主程序 API**
- DebugHub 向控制台发送日志
- InputDialog 界面弹窗
