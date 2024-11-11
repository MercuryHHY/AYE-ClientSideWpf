using AYE_ClientSideWpf.Extensions;
using AYE_ClientSideWpf.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AYE_ClientSideWpf.Views;

/// <summary>
/// WindowTest2.xaml 的交互逻辑
/// </summary>
public partial class WindowTest2 : Window
{
    private readonly IDialogHostService dialogHostService;
    public WindowTest2()
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

        

        menuBar.SelectionChanged += (s, e) =>
        {
            // 选择项 发生Changed 时
            //左边部分是否默认打开 设置为false  不打开  也就是收缩效果
            drawerHost.IsLeftDrawerOpen = false;
        };

       


        this.dialogHostService = dialogHostService;


        var service = App.Current.MainWindow.DataContext as IConfigureService;
        if (service != null)
            service.Configure();

    }

    

}
