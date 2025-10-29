using Avalonia.Controls;

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
				IconNormal = "avares://DGLabGameController/Assets/icons/dock/setting_active.png",
				IconActive = "avares://DGLabGameController/Assets/icons/dock/setting_normal.png",
				Title = "设置中心",
				PageFactory = () => new()
			});
			_viewModel.DockButtons.Add(new DockButton
			{
				IconNormal = "avares://DGLabGameController/Assets/icons/dock/log_active.png",
				IconActive = "avares://DGLabGameController/Assets/icons/dock/log_normal.png",
				Title = "首页",
				PageFactory = () => "这是AAAAAAAAAAAAAAAAAAAA"
			});
			_viewModel.DockButtons.Add(new DockButton
			{
				IconNormal = "avares://DGLabGameController/Assets/icons/dock/log_active.png",
				IconActive = "avares://DGLabGameController/Assets/icons/dock/log_normal.png",
				Title = "首页",
				PageFactory = () => "这是AAAAAAAAAAAAAAAAAAAA"
			});
			// 打开 第一个 Dock 按钮
			_viewModel.SelectedDockButton = _viewModel.DockButtons[0];
		}
	}
}