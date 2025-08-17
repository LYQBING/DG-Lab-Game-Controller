using DGLabGameController.Core.Config;
using DGLabGameController.Core.Debug;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace DGLabGameController.Core.Module
{
	/// <summary>
	/// 模块管理器
	/// <para>负责管理主程序模块</para>
	/// <para>除非有特殊需求，否则你不应该调用此类中的任何函数</para>
	/// </summary>
	public static class ModuleManager
	{
		public static ObservableCollection<ModuleInfo> LoadModules()
		{
			int errorCount = 0;
			ObservableCollection<ModuleInfo> modules = [];
			string modulesPath = AppConfig.ModulesPath;

			if (!Directory.Exists(modulesPath))Directory.CreateDirectory(modulesPath);
			foreach (string modDir in Directory.GetDirectories(modulesPath))
			{
				string folderName = Path.GetFileName(modDir);
				string dllPath = Path.Combine(modDir, $"{folderName}.dll");
				if (!File.Exists(dllPath))
				{
					DebugHub.Error("模块丢失", $"在 {modDir} 中不存在 {folderName}.dll 文件：此模块未正确安装", true);
					continue;
				}

				try
				{
					Assembly asm = Assembly.LoadFrom(dllPath);
					var types = asm.GetTypes().Where(t => typeof(ModuleBase).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract).ToList();
					foreach (var type in types)
					{
						if (Activator.CreateInstance(type) is not ModuleBase module)
						{
							DebugHub.Error("模块实例化失败", $"无法实例化模块类型：{type.FullName}", true);
							errorCount++;
							continue;
						}

						if (module.ModuleId != folderName)
						{
							DebugHub.Warning("模块结构异常", $" {module.Name} 实际标识为：{module.ModuleId}，但文件夹名称却为：{folderName}。", true);
						}

						modules.Add(new ModuleInfo
						{
							Name = module.Name,
							Description = module.Description,
							Info = $"{module.Version} 来自 {module.Author}",
							ModuleInstance = module
						});
					}
				}
				catch (Exception ex)
				{
					DebugHub.Error("模块加载异常", $"尝试加载 {folderName} 时发生了意料之外的错误：\r\n{ex}", true);
					errorCount++;
				}
			}
			if (errorCount > 0) DebugHub.Warning("模块加载完成", $"共计 {modules.Count} 项模块，其中 {errorCount} 项加载失败");
			else DebugHub.Log("模块加载完成", $"已成功加载所有模块");

			return modules;
		}
	}
}