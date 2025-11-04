using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DGLabGameController.Views
{
	/// <summary>
	/// Dock 图标数据类
	/// </summary>
	public class DockButton
	{
		/// <summary> 默认图标项 </summary>
		public object? IconNormal { get; set; }

		/// <summary> 选中图标项 </summary>
		public object? IconActive { get; set; }

		/// <summary> 界面标题 </summary>
		public string Title { get; set; } = "Hello World";

		/// <summary> 页面工厂 </summary>
		public Func<object> PageFactory { get; set; } = () => "Hello World";

		/// <summary> 缓存的页面 </summary>
		private object? _cachedPage = null;

		/// <summary> 获取页面实例 </summary>
		public object GetPage()
		{
			_cachedPage ??= PageFactory.Invoke();
			return _cachedPage;
		}
	}

	/// <summary>
	/// Dock 图标数据转换器
	/// </summary>
	public class DockButtonIconDataConverter : IMultiValueConverter
	{
		public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
		{
			if (values[0] is bool isActive)
			{
				return isActive ? values[1] : values[2];
			}
			return null;
		}
	}
}