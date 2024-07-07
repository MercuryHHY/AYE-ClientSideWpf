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

        /// <summary>
        /// 之前一直在思考 如果这个启动方法设置成 继承之后重写成 异步的方法 会不会有问题
        /// 从ABP生成的WPF模版来看 是没有问题的
        /// </summary>
        /// <param name="e"></param>
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


        /// <summary>
        /// 这个方法应该会程序最后执行
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnExit(ExitEventArgs e)
        { 
           await System.Threading.Tasks.Task.CompletedTask;    
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
