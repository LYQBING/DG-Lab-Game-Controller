## 在开发之前
本程序是基于 DG-Lab-Coyote-Game-Hub 的 API 进行开发，因此可以根据您的不同需求进行开发：

### 何时选择 DG-Lab-Coyote-Game-Hub ？

它为您提供了网页 API 及控制器功能，若您不需要其他功能封装或统一管理模块时，您可以直接采用它进行开发。
适用场景建议：
- 您可以使用任意语言进行开发
- 您可以通过它为支持 MOD 功能的游戏开发 MOD
- 您可以直接通过 URL 进行惩罚控制

### 何时选择 DG-Lab-Game-Controller ？

它在原有功能之上，直接为您提供了 C# 的脚本代码，您可以直接通过代码控制郊狼进行惩罚。且支持统一的模块管理，您可以更加方便的处理用户所设置的参数。
适用场景建议：
- 您可以使用 C# 进行开发
- 您可以通过它对不支持 MOD 的游戏进行注入
- 您可以使用它快速开发通用惩罚功能

## 模块开发相关
欢迎使用 DG-Lab-Game-Controller 进行您的模块开发，同时也感谢您对 DG-LAB 游戏生态所做出的一切贡献！这里将为您介绍常用函数，完整内容请参考主程序源码。

**模块开发入口相关**
- 让我们开发一个创意十足的模块吧
- [前往 Module 手册](ModuleApi/Module.md)

**控制及获取郊狼 API**
- 提供控制郊狼的相关方法
- 提供获取服务器状态的相关方法
- 提供获取郊狼状态的相关方法
- [前往 DGLabApi 手册](ModuleApi/DGLabApi.md)

**日志输出 API**
- 提供日志输出的相关方法
- [前往 DebugApi 手册](ModuleApi/Debug.md)

**弹窗展示 API**
- 提供展示弹窗方法
- 提取文本输入弹窗方法
- [前往 DialogApi 手册](ModuleApi/Dialog.md)

**网络请求 API**
- 提供网络请求相关方法
- [前往 Network 手册](ModuleApi/Network.md)

## 内存脚本开发相关
欢迎使用 GameValueDetector 创建您的郊狼游戏脚本。它将给您带来简单迅速且强大的脚本编辑体验！但内存脚本存在局限性，如果您的技术足够，这里推荐模块开发哦。

**内存脚本开发相关**
- 让我们快速适配您的游戏吧
- [前往 GameValueDetector 手册](GameValueDetector/Manual.md)