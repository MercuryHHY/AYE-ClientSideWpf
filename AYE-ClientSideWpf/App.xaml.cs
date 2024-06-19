using AYE_Commom.Helper;
using AYE_Service;
using HandyControl.Themes;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Windows;
using System.Windows.Media;

namespace AYE_ClientSideWpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ConsoleHelper.AllocConsole();//打开控制台
            base.OnStartup(e);

            // 配置日志工厂
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog("NLog.config");
            });

            // 创建日志记录器
            var logger = loggerFactory.CreateLogger<App>();
            logger.LogInformation("Application started.");


            // 启动Bootstrapper
            //var boot = new Bootstrapper();
            var boot = new Bootstrapper(loggerFactory);       
            boot.Run();
        }

        internal void UpdateTheme(ApplicationTheme theme)
        {
            if (ThemeManager.Current.ApplicationTheme != theme)
            {
                ThemeManager.Current.ApplicationTheme = theme;
            }
        }

        internal void UpdateAccent(Brush accent)
        {
            if (ThemeManager.Current.AccentColor != accent)
            {
                ThemeManager.Current.AccentColor = accent;
            }
        }
    }
}
