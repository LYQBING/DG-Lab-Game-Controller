using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace DGLabGameController
{
    public static class ModuleManager
    {
        public static ObservableCollection<ModuleInfo> LoadModules(string pluginDir)
        {
            var modules = new ObservableCollection<ModuleInfo>();
            if (!Directory.Exists(pluginDir)) Directory.CreateDirectory(pluginDir);

            foreach (string modDir in Directory.GetDirectories(pluginDir))
            {
                string folderName = Path.GetFileName(modDir);
                string dllPath = Path.Combine(modDir, $"{folderName}.dll");
                if (!File.Exists(dllPath))
                {
                    DebugHub.Error("模块加载失败", $"在 {modDir} 中未找到 {folderName}.dll 文件，可能是模块未正确安装。");
                    continue;
                }

                try
                {
                    Assembly asm = Assembly.LoadFrom(dllPath);
                    var types = asm.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var type in types)
                    {
                        IModule module = (IModule)Activator.CreateInstance(type)!;
                        if (module.ModuleId != folderName)
                        {
                            DebugHub.Warning("模块数据不匹配", $"模块 {dllPath} 的实际标识为：{module.ModuleId}，但文件夹名称却为：{folderName}。");
                        }

                        modules.Add(new ModuleInfo
                        {
                            Name = module.Name,
                            Description = module.Description,
                            Info = module.Version + " 来自 " + module.Author,
                            ModuleInstance = module
                        });
                    }
                }
                catch (Exception ex)
                {
                    DebugHub.Error("模块加载失败", $"尝试加载 {dllPath} 时发生了意外：{ex}");
                }
            }
            return modules;
        }
    }
}