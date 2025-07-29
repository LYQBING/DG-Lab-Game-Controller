using DGLabGameController.Core.Config;
using DGLabGameController.Core.Module;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DGLabGameController
{
    public partial class MainWindow : Window
    {
        // 缓存的标准页面
        private FuncSelectPage? _funcPage;
        private LogPage? _logPage;

        private SettingPage? _settingPage;

        // 缓存的模块页面
        private ModuleBase? _activeModule;
        private string? _nowPage;

        #region 启动及关闭事件

        public MainWindow()
        {
            InitializeComponent();
			NavFunc_Click();

			ConfigUpdate.Update();
		}

        protected override void OnClosing(System.ComponentModel.CancelEventArgs eventArgs)
        {
            if (SettingsRepository.Current.ExitMenu)
            {
                eventArgs.Cancel = true;
                this.Hide();
                _ = Application.Current.Resources["MyNotifyIcon"];
                return;
            }

            // 移除模块页面
            _activeModule?.OnModulePageClosed();
        }

        #endregion

        #region Dock相关

        public void NavFunc_Click(object? sender = null, RoutedEventArgs? e = null)
        {
            SetNavImages("Func");
            MainContent.Content = _activeModule == null
                ? _funcPage ??= new FuncSelectPage()
                : _activeModule.Page;
        }

        public void NavLog_Click(object? sender = null, RoutedEventArgs? e = null)
        {
            SetNavImages("Log");
            MainContent.Content = _logPage ??= new LogPage();
        }

        public void NavSetting_Click(object? sender = null, RoutedEventArgs? e = null)
        {
            SetNavImages("Setting");
            MainContent.Content = _settingPage ??= new SettingPage();
        }

        private void SetNavImages(string page)
        {
            if (_nowPage == page)
                return;

            _nowPage = page;
            SetImageSource(FuncImage, "Func", "func");
            SetImageSource(LogImage, "Log", "log");
            SetImageSource(SettingImage, "Setting", "setting");
        }

        private void SetImageSource(Image image, string pageName, string iconName)
        {
            string state = _nowPage == pageName ? "_selected" : string.Empty;
            string path = $"pack://application:,,,/Assets/icon/{iconName}{state}.png";
            image.Source = new BitmapImage(new Uri(path));
        }

        #endregion

        #region 模块页面相关

        public void ShowModulePage(ModuleBase module)
        {
            _activeModule = module;
            NavFunc_Click();
        }

        public void CloseActiveModule()
        {
            _activeModule?.OnModulePageClosed();
            _activeModule = null;

            NavFunc_Click();
        }

        #endregion
    }
}