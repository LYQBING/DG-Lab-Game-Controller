using System.Windows;
using System.Windows.Input;

namespace HealthBarDetector
{
	public partial class AreaSelectorWindow : Window
	{
		public Rect SelectedRect { get; private set; } // 选择的矩形区域
		private Point startPoint; // 选择区域的起始点
		private bool isSelecting = false; // 是否开始选择区域

		public AreaSelectorWindow()
		{
			InitializeComponent();
			MouseLeftButtonDown += AreaSelectorWindow_MouseLeftButtonDown;
			MouseMove += AreaSelectorWindow_MouseMove;
			MouseLeftButtonUp += AreaSelectorWindow_MouseLeftButtonUp;
		}

		/// <summary>
		/// 鼠标左键按下事件处理器，用于开始选择区域
		/// </summary>
		private void AreaSelectorWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			// 标记为正在选择状态，并记录起始点
			isSelecting = true;
			startPoint = e.GetPosition(canvas);

			// 初始化页面上的矩形选框，并将其可见
			System.Windows.Controls.Canvas.SetLeft(selectionRect, startPoint.X);
			System.Windows.Controls.Canvas.SetTop(selectionRect, startPoint.Y);
			selectionRect.Width = 0;
			selectionRect.Height = 0;
			selectionRect.Visibility = Visibility.Visible;

			// 捕获鼠标事件，以便在鼠标移动时可以继续更新选择矩形
			CaptureMouse();
		}

		/// <summary>
		/// 鼠标移动事件处理器，用于更新选择矩形的大小和位置
		/// </summary>
		private void AreaSelectorWindow_MouseMove(object sender, MouseEventArgs e)
		{
			// 确保正在选择状态，如果不是则直接返回
			if (!isSelecting) return;

			// 获取当前鼠标位置，并计算选择矩形的大小和位置
			var pos = e.GetPosition(canvas);
			var x = Math.Min(pos.X, startPoint.X);
			var y = Math.Min(pos.Y, startPoint.Y);
			var w = Math.Abs(pos.X - startPoint.X);
			var h = Math.Abs(pos.Y - startPoint.Y);
			System.Windows.Controls.Canvas.SetLeft(selectionRect, x);
			System.Windows.Controls.Canvas.SetTop(selectionRect, y);
			selectionRect.Width = w;
			selectionRect.Height = h;
		}

		/// <summary>
		/// 鼠标左键释放事件处理器，用于结束选择区域并返回结果
		/// </summary>
		private void AreaSelectorWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			// 确保正在选择状态，如果不是则直接返回
			if (!isSelecting) return;
			isSelecting = false;

			// 释放鼠标捕获，结束选择
			ReleaseMouseCapture();

			// 获取结束点位置，并计算最终选择的矩形区域
			var endPoint = e.GetPosition(canvas);
			var x = Math.Min(startPoint.X, endPoint.X);
			var y = Math.Min(startPoint.Y, endPoint.Y);
			var w = Math.Abs(startPoint.X - endPoint.X);
			var h = Math.Abs(startPoint.Y - endPoint.Y);

			// 保存选择的矩形区域，并关闭窗口
			SelectedRect = new Rect(x, y, w, h);
			DialogResult = true;
			Close();
		}

		/// <summary>
		/// 处理窗口的键盘事件，允许用户通过按下Esc键来取消选择
		/// </summary>
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				DialogResult = false;
				Close();
			}
		}
	}
}
