using System.Windows.Controls;

namespace DGLabGameController.Core.Module
{
	/// <summary>
	/// 模块基类：所有模块都应继承自此类
	/// </summary>
	public abstract class ModuleBase
	{
		/// <summary>
		/// 模块标识符
		/// <para>必须与模块所在文件夹名称及 DLL 模块名称保持一致</para>
		/// </summary>
		public abstract string ModuleId { get; }
		/// <summary>
		/// 模块名称
		/// <para>在模块列表中所显示的名称</para>
		/// </summary>
		public abstract string Name { get; }
		/// <summary>
		/// 模块描述
		/// <para>简要描述模块的功能和用途</para>
		/// </summary>
		public abstract string Description { get; }
		/// <summary>
		/// 模块作者
		/// <para>模块的开发者或维护者</para>
		/// </summary>
		public abstract string Author { get; }
		/// <summary>
		/// 模块版本号
		/// <para>若想要支持模块更新功能，请自行实现</para>
		/// </summary>
		public abstract string Version { get; }
		/// <summary>
		/// 模块兼容的 API 版本号
		/// <para>用于确保模块与 DGLabApi 的兼容性</para>
		/// </summary>
		public abstract int CompatibleApiVersion { get; }

		/// <summary>
		/// 当页面创建时
		/// </summary>
		protected abstract UserControl CreatePage();

		/// <summary>
		/// 当模块被关闭时
		/// </summary>
		public virtual void OnModulePageClosed()
		{
			if (_page is IDisposable disposable) disposable.Dispose();
			_page = null;
		}

		/// <summary>
		/// 缓存的模块页面
		/// </summary>
		protected UserControl? _page;
		public UserControl Page
		{
			get
			{
				_page ??= CreatePage();
				return _page;
			}
		}
	}
}

