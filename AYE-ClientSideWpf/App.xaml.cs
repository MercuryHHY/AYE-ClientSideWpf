using AYE_Commom.Helper;
using AYE_Service;
using HandyControl.Themes;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace AYE_ClientSideWpf
{
    public partial class App : Application
    {

        private static Mutex _mutex;

        /// <summary>
        /// 之前一直在思考 如果这个启动方法设置成 继承之后重写成 异步的方法 会不会有问题
        /// 从ABP生成的WPF模版来看 是没有问题的
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            const string mutexName = "AYE_ClientSideWpfMutex";
            bool isNewInstance;

            // 创建互斥体对象并判断是否当前为新实例
            _mutex = new Mutex(true, mutexName, out isNewInstance);

            if (!isNewInstance)
            {
                // 如果不是新实例，说明已有一个实例在运行
                MessageBox.Show("已经有一个相同程序在运行中");
                Environment.Exit(0); // 关闭当前实例
            }
            else
            {
                // 如果是新实例，则正常启动
                ConsoleHelper.AllocConsole(); // 打开控制台
                base.OnStartup(e);

                // 启动Bootstrapper
                var boot = new Bootstrapper();
                boot.Run();
            }
        }


        /// <summary>
        /// 这个方法应该会程序最后执行
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnExit(ExitEventArgs e)
        {
            // 在应用程序退出时释放互斥体
            if (_mutex != null)
            {
                _mutex.ReleaseMutex();
                _mutex = null;
            }
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
