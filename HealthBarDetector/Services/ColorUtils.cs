using System.Drawing;

namespace HealthBarDetector.Services
{
	/// <summary>
	/// 颜色处理工具类
	/// </summary>
	public static class ColorUtils
	{
		/// <summary>
		/// 获取指定区域内出现次数最多的颜色
		/// </summary>
		/// <param name="area">屏幕坐标</param>
		/// <returns></returns>
		public static Color GetMostFrequentColor(Rectangle area)
		{
			using var bmp = new Bitmap(area.Width, area.Height);
			using var g = Graphics.FromImage(bmp);
			g.CopyFromScreen(area.X, area.Y, 0, 0, area.Size);

			// 使用字典统计颜色出现次数
			Dictionary<int, int> colorCount = [];
			for (int x = 0; x < bmp.Width; x++)
			{
				for (int y = 0; y < bmp.Height; y++)
				{
					var color = bmp.GetPixel(x, y).ToArgb();
					if (colorCount.TryGetValue(color, out int value))
						colorCount[color] = ++value;
					else
						colorCount[color] = 1;
				}
			}

			// 如果没有颜色，直接返回黑色
			if (colorCount.Count == 0) return Color.Black;

			// 找到出现次数最多的颜色
			int maxColor = 0, maxCount = 0;
			foreach (var kv in colorCount)
			{
				if (kv.Value > maxCount)
				{
					maxColor = kv.Key;
					maxCount = kv.Value;
				}
			}
			return Color.FromArgb(maxColor);
		}

		/// <summary>
		/// 获取指定区域内指定颜色的占比
		/// </summary>
		/// <param name="area">屏幕坐标</param>
		/// <param name="targetColor">目标颜色</param>
		/// <param name="tolerance">容差范围</param>
		/// <returns></returns>
		public static double CalculateColorPercent(Rectangle area, Color targetColor, int tolerance)
		{
			using var bmp = new Bitmap(area.Width, area.Height);
			using var g = Graphics.FromImage(bmp);
			g.CopyFromScreen(area.X, area.Y, 0, 0, area.Size);

			int matchCount = 0, total = bmp.Width * bmp.Height;
			for (int x = 0; x < bmp.Width; x++)
			{
				for (int y = 0; y < bmp.Height; y++)
				{
					var pixel = bmp.GetPixel(x, y);
					if (IsColorMatch(pixel, targetColor, tolerance)) matchCount++;
				}
			}
			return total == 0 ? 0 : (double)matchCount / total;
		}

		/// <summary>
		/// 颜色是否匹配
		/// </summary>
		/// <param name="a">目标颜色</param>
		/// <param name="b">检测颜色</param>
		/// <param name="tolerance">容差范围</param>
		/// <returns></returns>
		public static bool IsColorMatch(Color a, Color b, int tolerance)
		{
			return Math.Abs(a.R - b.R) <= tolerance &&
				   Math.Abs(a.G - b.G) <= tolerance &&
				   Math.Abs(a.B - b.B) <= tolerance;
		}
	}
}