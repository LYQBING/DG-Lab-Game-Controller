using System;
using Avalonia;

namespace DGLabGameController
{
    internal sealed class Program
    {
		// 初始化代码。不要使用任何Avalonia，第三方api或任何
		// 调用AppMain之前的synchronizationcontext依赖代码：东西没有初始化
		// yet and stuff might break.
		[STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
