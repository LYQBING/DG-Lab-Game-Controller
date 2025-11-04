using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace DGLabGameController.Views
{
	public partial class MainWindow : Window
	{
		private readonly MainWindowViewModel _viewModel;

		public MainWindow()
		{
			InitializeComponent();

			_viewModel = new MainWindowViewModel();

			DataContext = _viewModel;
			_viewModel.DockButtons.Add(new DockButton
			{
				IconNormal = Application.Current?.FindResource("Home_Active"),
				IconActive = Application.Current?.FindResource("Home_Normal"),
				Title = "控制台",
				PageFactory = () => new()
			});
			_viewModel.DockButtons.Add(new DockButton
			{
				IconNormal = Application.Current?.FindResource("Model_Active"),
				IconActive = Application.Current?.FindResource("Model_Normal"),
				Title = "模块",
				PageFactory = () => "这是AAAAAAAAAAAAAAAAAAAA"
			});
			_viewModel.DockButtons.Add(new DockButton
			{
				IconNormal = Application.Current?.FindResource("Setting_Active"),
				IconActive = Application.Current?.FindResource("Setting_Normal"),
				Title = "设置",
				PageFactory = () => "这是AAAAAAAAAAAAAAAAAAAA"
			});

			// 打开 第一个 Dock 按钮
			_viewModel.SelectedDockButton = _viewModel.DockButtons[0];
		}
	}
}