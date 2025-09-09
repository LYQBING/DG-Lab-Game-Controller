## 模块开发相关帮助

### 主程序结构
- **Assets 目录** 这里存放着所有主程序资源内容，内容结构可能会变动
- **Core 目录** 这里存放着所有开放接口，内容会增加，现有结构一般不会变动
- **Main 目录** 这里存放着主程序内容，通常您不应该使用这里的内容
- **Themes 目录** 这里存放着开放的主题文件，你可以使用它快速开发程序

### 模块入口参考
```CS
// 模块入口脚本
internal class Main : ModuleBase
{
	public override string ModuleId => "模块唯一ID";
	public override string Name => "模块名称";
	public override string Description => "模块介绍";
	public override string Author => "模块作者";
	public override string Version => "模块版本号";
	public override int CompatibleApiVersion => 10087; // 兼容的 API 版本号
	// 返回模块的主页面，入口方法
	protected override UserControl CreatePage()
	{
		return new GameValueDetectorPage();
	}
}
```

### 注意事项
- 您应该在项目的 ”引用管理器“ 中框选 ”DGLabGameController“，否则可能无法正常工作
- 您必须为您的模块创建一个界面作为项目主页面
- 您可以使用 ”Themes“ 文件夹中的内置主题，以便统一界面
- 在 Core/Config/AppConfig.cs 中存放当前主程序相关信息，其中包括 API 版本号

## 获取模块文件夹
### 函数展示
```CS
// 获取模块文件夹路径
Path.Combine(AppConfig.ModulesPath, ModuleId);
```

### 注意事项
- 每个模块的独立文件夹名称就是他的 ModuleId 名称
- 模块的文件夹与模块DLL应该与ModuleId保持一致