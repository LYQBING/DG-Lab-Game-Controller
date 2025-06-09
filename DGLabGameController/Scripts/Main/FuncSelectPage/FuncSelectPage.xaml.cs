using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Diagnostics;
namespace DGLabGameController
{
	public class ModuleInfo
	{
		/// <summary>
		/// 模块名称：与列表中的名称绑定
		/// </summary>
		public string? Name { get; set; }
		/// <summary>
		/// 模块信息：与列表中的信息绑定
		/// </summary>
		public string? Info { get; set; }
		/// <summary>
		/// 模块描述：与列表中的描述绑定
		/// </summary>
		public string? Description { get; set; }
		/// <summary>
		/// 创建事件：与列表中的操作按钮绑定
		/// </summary>
		public ICommand? OperateCommand { get; set; }

		/// <summary>
		/// 模块实例：实际的模块实例，用于获取页面等操作
		/// </summary>
		public IModule? ModuleInstance { get; set; }
		/// <summary>
		/// DLL 路径：模块所在的 DLL 文件路径
		/// </summary>
		public string? DllPath { get; set; }
	}

	public partial class FuncSelectPage : UserControl
	{
		public ObservableCollection<ModuleInfo> Modules { get; set; } = new();

		public FuncSelectPage()
		{
			InitializeComponent();
			DataContext = this;
			LoadModules();
		}

		private void Edit_Click(object sender, RoutedEventArgs e)
		{
			// 打开模块目录
			string pluginDir = Path.Combine(ConfigManager.DataPath, "Modules");
			if (!Directory.Exists(pluginDir)) Directory.CreateDirectory(pluginDir);

			new MessageDialog("程序即将关闭", "添加或卸载模块需在程序关闭状态下进行，是否继续操作？","继续", 
			(data) =>
			{
				// 打开文件夹
				System.Diagnostics.Process.Start("explorer.exe", pluginDir);

				Application.Current.Shutdown();
				data.Close();
			},"取消", 
			(data) => data.Close()).ShowDialog();
		}

		/// <summary>
		/// 加载模块列表
		/// </summary>
		private void LoadModules()
		{
			string pluginDir = Path.Combine(ConfigManager.DataPath, "Modules");
			if (!Directory.Exists(pluginDir)) Directory.CreateDirectory(pluginDir);
			Modules.Clear();

			foreach (var dll in Directory.GetFiles(pluginDir, "*.dll"))
			{
				try
				{
					// 加载 DLL 并寻找 IModule 接口
					var asm = Assembly.LoadFrom(dll);
					var types = asm.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

					// 创建模块实例并添加到列表
					foreach (var type in types)
					{
						IModule module = (IModule)Activator.CreateInstance(type)!;
						ModuleInfo info = new()
						{
							Name = module.Name,
							Info = module.Info,
							Description = module.Description,

							ModuleInstance = module,
							DllPath = dll
						};
						info.OperateCommand = new RelayCommand(_ => StartModule(info));
						Modules.Add(info);
					}
				}
				catch (Exception ex)
				{
					DebugHub.Warning("模块加载失败", $"尝试加载 {dll} 时发生了意外：{ex}");
				}
			}
		}

		/// <summary>
		/// 启动模块
		/// </summary>
		private static void StartModule(ModuleInfo info)
		{
			if (Application.Current.MainWindow is MainWindow mw && info.ModuleInstance != null)
			{
				mw.ShowModulePage(info.ModuleInstance);
			}
			else _ = new MessageDialog("模块创建异常", "此模块不存在独立页面或程序内部错误。", "确定", (data) => data.Close());
		}

		/// <summary>
		/// 按钮操作
		/// </summary>
		public class RelayCommand : ICommand
		{
			private readonly Action<object?> _execute;
			private readonly Func<object?, bool>? _canExecute;
			public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
			{
				_execute = execute;
				_canExecute = canExecute;
			}
			public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
			public void Execute(object? parameter) => _execute(parameter);
			public event EventHandler? CanExecuteChanged;
			public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}