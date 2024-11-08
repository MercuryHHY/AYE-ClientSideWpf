using AYE.BaseFramework.SqlSusgarCore;
using AYE_ClientSideWpf.ViewModels;
using AYE_ClientSideWpf.Views;
using AYE_ModuleRegistration;
using DemoModuleA;
using Microsoft.Extensions.Configuration;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Quartz.Impl;
using Quartz;
using SqlSugar;
using System.Windows;
using AYE.BaseFramework.QuartzCore;
using Volo.Abp.Timing;
using Microsoft.Extensions.Logging;
using DryIoc;
using DemoModuleB;
using AYE.BaseFramework.Manager;
using AYE.BaseFramework.Manager.Extensions;
using Prism.Services.Dialogs;
using MyToDo.Common;
using System;


namespace AYE_ClientSideWpf;

/// <summary>
/// 原本类似 痕迹 的操作 一样  直接去操作APP启动项文件就可以聊聊
/// 但是这个包也提供了如下这种操作，个人也偏向项目模版自带的这种
/// 很久很久以前，在嵌入式开发中有一种启动项设置 她也叫boot
/// </summary>
public class Bootstrapper : PrismBootstrapper
{
    //private readonly ILoggerFactory _loggerFactory;

    //public Bootstrapper(ILoggerFactory loggerFactory)
    //{
    //    _loggerFactory = loggerFactory;
    //}



    /// <summary>
    /// 1,先执行注册
    /// </summary>
    /// <param name="containerRegistry"></param>
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        /// 兜兜转转还是 需要将DB的注册放在 主注册界面之前
        /// 否则 主页面的权限管控以及登录的校验 都无法处理
        #region 日志以及配置系统的注册必须放在最开始的地方,外加DB的注册
        containerRegistry.RegisterLogging();
        containerRegistry.RegisterConfiguration();
        containerRegistry.RegisterDatabase();
        #endregion

        containerRegistry.Register<ILoginService, LoginService>();
        containerRegistry.RegisterDialog<LoginView, LoginViewModel>();
        containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();

        //测试
        containerRegistry.RegisterForNavigation<WindowTest1, WindowTest1ViewModel>();
        containerRegistry.RegisterForNavigation<WindowTest2, WindowTest2ViewModel>();

    }


    /// <summary>
    /// 2，预加载模块,但是模块中的注册没有执行
    /// </summary>
    /// <param name="moduleCatalog"></param>
    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        // 这里是添加其他 类库的模块注册类中 注册行为
        // 最先添加的最先注册

        //1,底层框架
        moduleCatalog.AddModule<BaseFrameworkManagerModule>();
        //2,上层PO后端服务
        moduleCatalog.AddModule<ModuleFile>();

        //3,各个子模块
        moduleCatalog.AddModule<ModuleAProfile>();
        moduleCatalog.AddModule<ModuleBProfile>();

        base.ConfigureModuleCatalog(moduleCatalog);
    }


    /// <summary>
    /// 3，创建启动类
    /// </summary>
    /// <returns></returns>
    protected override DependencyObject CreateShell()
    {
        // 通过容器去 拿到这个 启动类
        return Container.Resolve<MainWindow>();

        //测试
        //return Container.Resolve<WindowTest1>();
        //return Container.Resolve<WindowTest2>();
    }


    /// <summary>
    /// 4,加载主页面之后，会进入这里执行最后的逻辑
    /// 可以将 登录界面的加载激活放在这里，或者数据初始化操作也可以放这里
    /// </summary>
    protected override void OnInitialized()
    {
        //需要在这里将 登录界面显示
        var dialog = Container.Resolve<IDialogService>();
        dialog.ShowDialog("LoginView", callback =>
        {
            if (callback.Result != ButtonResult.OK)
            {
                Environment.Exit(0);
                return;
            }

            //在MainWindow 与  MainWindowViewModel 加载之后，
            //在这里强制性 拿到MainWindow的DataContext（也就是prism指定的MainWindowViewModel）
            //去执行service.Configure();
            // 如此方可确保 在登录成功之后才会显示 主页面
            var service = App.Current.MainWindow.DataContext as IConfigureService;
            if (service != null)
                service.Configure();
            base.OnInitialized();
        });

        base.OnInitialized();
    }





}
