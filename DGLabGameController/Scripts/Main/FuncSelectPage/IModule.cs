using System.Windows.Controls;

namespace DGLabGameController
{
	public interface IModule
	{
		/// <summary>
		/// 模块名称
		/// </summary>
		string Name { get; }
		/// <summary>
		/// 模块信息
		/// </summary>
		string Info { get; }
		/// <summary>
		/// 模块描述
		/// </summary>
		string Description { get; }
		/// <summary>
		/// 当模块运行时 (获取模块页面及模块初始化)
		/// </summary>
		/// <returns></returns>
		UserControl GetPage();
		/// <summary>
		/// 当模块页面关闭时 (用于释放资源等操作)
		/// </summary>
		void OnModulePageClosed();
	}

	/// <summary>
	/// 模块基类，所有模块都应继承此类
	/// </summary>
	public abstract class ModuleBase : IModule
	{
		public abstract string Name { get; }
		public abstract string Info { get; }
		public abstract string Description { get; }

		protected abstract UserControl CreatePage();

		protected UserControl? _page;
		public UserControl GetPage()
		{
			_page ??= CreatePage();
			return _page;
		}

		public virtual void OnModulePageClosed()
		{
			if (_page is IDisposable disposable) disposable.Dispose();
			_page = null;
		}
	}
}

