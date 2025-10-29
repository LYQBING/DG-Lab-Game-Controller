

namespace DGLabGameController.Views
{
	using Avalonia.Data.Converters;
	using Avalonia.Media.Imaging;
	using Avalonia.Platform;
	using System;
	using System.IO;
	using System.Net.Http;
	using System.Threading.Tasks;
	public class DockButton
	{
		/// <summary> 默认图标项 </summary>
		public string IconNormal { get; set; } = "/Assets/icons/back.png";

		public Bitmap? IconNormalBitmap { get => ImageHelper.LoadFromResource(new Uri(IconNormal)); }

		/// <summary> 选中图标项 </summary>
		public string IconActive { get; set; } = "/Assets/icons/back.png";

		public Bitmap? IconSelectedBitmap { get => ImageHelper.LoadFromResource(new Uri(IconActive)); }

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
	public static class ImageHelper
	{
		public static Bitmap LoadFromResource(Uri resourceUri)
		{
			return new Bitmap(AssetLoader.Open(resourceUri));
		}

		public static async Task<Bitmap?> LoadFromWeb(Uri url)
		{
			using var httpClient = new HttpClient();
			try
			{
				var response = await httpClient.GetAsync(url);
				response.EnsureSuccessStatusCode();
				var data = await response.Content.ReadAsByteArrayAsync();
				return new Bitmap(new MemoryStream(data));
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
				return null;
			}
		}
	}
}

namespace DGLabGameController.Views
{
	using Avalonia.Data.Converters;
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	public class DockButtonIconConverter : IMultiValueConverter
	{
		/// <summary>
		/// 这是一个多值转换器，根据是否选中状态返回不同的图标
		/// </summary>
		/// <param name="values"></param>// values 是一个包含多个绑定值的列表
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) 
		{
			// 弹窗调试信息
			System.Diagnostics.Debug.WriteLine($"DockButtonIconConverter.Convert called with values: {string.Join(", ", values)}");

			if (values is [bool isSelected, Avalonia.Media.Imaging.Bitmap normal, Avalonia.Media.Imaging.Bitmap selected]) 
				return isSelected ? selected : normal; return null; 
		}

	}
}