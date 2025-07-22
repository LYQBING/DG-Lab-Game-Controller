using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.IO;
namespace DGLabGameController
{
	public class ModuleInfo
	{
		public string? Name { get; set; } // 模块名称
		public string? Description { get; set; } // 模块描述
		public string? Info { get; set; } // 模块信息（版本号、作者等）
		public ICommand? OperateCommand { get; set; } // 启动模块命令
		public IModule? ModuleInstance { get; set; } // 模块实例
	}

	public partial class FuncSelectPage : UserControl
	{
		public ObservableCollection<ModuleInfo> Modules { get; set; } = [];

		public FuncSelectPage()
		{
			InitializeComponent();
			DataContext = this;
			LoadModules();
		}

		/// <summary>
		/// 加载模块列表
		/// </summary>
		private void LoadModules()
		{
			Modules = ModuleManager.LoadModules(ConfigManager.ModulesPath);
			foreach (var info in Modules)
			{
				info.OperateCommand = new RelayCommand(_ => StartModule(info));
			}
			DataContext = this;
		}

		/// <summary>
		/// 启动模块按钮
		/// </summary>
		private static void StartModule(ModuleInfo info)
		{
			if (Application.Current.MainWindow is MainWindow mw && info.ModuleInstance != null)
			{
				int apiVersion = info.ModuleInstance.CompatibleApiVersion;
				if (apiVersion != App.ApiVersion)
				{
					new MessageDialog("不兼容的模块", $"此模块的 API 版本：{apiVersion}\r主程序的 API 版本：{App.ApiVersion}\r是否继续启动？这可能出现兼容性问题！","继续",
					(data) =>
					{
                        mw.ShowModulePage(info.ModuleInstance);
                        data.Close();
					}, "取消",
					(data) => data.Close()).ShowDialog();
				}
				else mw.ShowModulePage(info.ModuleInstance);
			}
			else new MessageDialog("模块创建异常", "此模块不存在独立界面或内部错误", "确定", (data) => data.Close()).ShowDialog();
        }

		/// <summary>
		/// 编辑模块按钮
		/// </summary>
		private void Edit_Click(object sender, RoutedEventArgs e)
		{
			// 打开模块目录
			if (!Directory.Exists(ConfigManager.ModulesPath)) Directory.CreateDirectory(ConfigManager.ModulesPath);
			new MessageDialog("程序即将关闭", "添加或卸载模块需在程序关闭状态下进行，是否继续操作？","继续", 
			(data) =>
			{
				// 打开文件夹
				System.Diagnostics.Process.Start("explorer.exe", ConfigManager.ModulesPath);

				Application.Current.Shutdown();
				data.Close();
			},"取消", 
			(data) => data.Close()).ShowDialog();
		}

		/// <summary>
		/// 按钮操作
		/// </summary>
		public class RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null) : ICommand
		{
			public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;
			public void Execute(object? parameter) => execute(parameter);
			public event EventHandler? CanExecuteChanged;
			public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}