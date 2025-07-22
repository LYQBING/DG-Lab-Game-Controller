using System.IO;
using System.Windows.Controls;

namespace DGLabGameController
{
	public interface IModule
	{
		string ModuleId { get; } // 模块标识：必须与模块所在文件夹名称一致
		string Name { get; } // 模块名称
		string Description { get; } // 模块描述：简要介绍模块功能
		string Author { get; } // 模块作者：模块的开发者或维护者
		string Version { get; } // 模块版本号：若想要支持模块更新功能，请自行实现
		int CompatibleApiVersion { get; } // 模块兼容的应用版本：用于判断模块是否与当前应用版本兼容

		/// <summary>
		/// 获取模块页面
		/// </summary>
		UserControl GetPage();
		/// <summary>
		/// 当模块页面关闭时
		/// </summary>
		void OnModulePageClosed();
	}

	/// <summary>
	/// 模块基类，所有模块都应继承此类
	/// </summary>
	public abstract class ModuleBase : IModule
	{
		public abstract string ModuleId { get; }
		public abstract string Name { get; }
		public abstract string Description { get; }
		public abstract string Author { get; }
		public abstract string Version { get; }
		public abstract int CompatibleApiVersion { get; }

        /// <summary>
        /// 当页面创建时
        /// </summary>
        protected abstract UserControl CreatePage();

        /// <summary>
        /// 当页面关闭时
        /// </summary>
        public virtual void OnModulePageClosed()
		{
			if (_page is IDisposable disposable) disposable.Dispose();
			_page = null;
		}

        protected UserControl? _page;
        public UserControl GetPage()
        {
            _page ??= CreatePage();
            return _page;
        }
    }
}

