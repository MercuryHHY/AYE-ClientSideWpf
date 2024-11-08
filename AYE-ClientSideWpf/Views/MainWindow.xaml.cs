using AYE_ClientSideWpf.Extensions;
using HandyControl.Themes;
using HandyControl.Tools;
using MyToDo.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AYE_ClientSideWpf.Views
{
    /// <summary>
    /// 这里的代码爆红 是因为 我的XAML代码与内部CS代码在编译执行时才会合并在一起，
    /// 我这里是通过编译执行时利用prism框架来合并这个类，并没有显示的给出数据上下文，
    /// 所以VS时不时会显示找不到定义，但是运行起来是没有问题的
    /// VS重新打开此项目就好了
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDialogHostService dialogHostService;
        public MainWindow()
        {
            InitializeComponent();

            btnMin.Click += (s, e) => { this.WindowState = WindowState.Minimized; };
            btnMax.Click += (s, e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Maximized;
            };
            btnClose.Click += async (s, e) =>
            {
                var dialogResult = await dialogHostService.Question("温馨提示", "确认退出系统?");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                this.Close();
            };


            ///通过在 ColorZone 控件上按住鼠标左键并移动鼠标来拖动整个窗口
            ColorZone.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };

            //ColorZone.MouseDoubleClick += (s, e) =>
            //{
            //    if (this.WindowState == WindowState.Normal)
            //        this.WindowState = WindowState.Maximized;
            //    else
            //        this.WindowState = WindowState.Normal;
            //};

            menuBar.SelectionChanged += (s, e) =>
            {
                // 选择项 发生Changed 时
                //左边部分是否默认打开 设置为false  不打开  也就是收缩效果
                drawerHost.IsLeftDrawerOpen = false;
            };

            this.dialogHostService = dialogHostService;

            ///App.Current.MainWindow.DataContext可以拿到 数据上下文（也就是对应的VM层）
            //var service = App.Current.MainWindow.DataContext as IConfigureService;
            //if (service != null)
            //    service.Configure();

        }

       
    }
}
