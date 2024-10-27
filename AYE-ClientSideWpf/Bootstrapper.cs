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

namespace AYE_ClientSideWpf
{
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

#if false
            // 注册ILoggerFactory和ILogger
            containerRegistry.RegisterInstance(_loggerFactory);
            // 方法1 ：正常注册
            containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));
            // 方法2：使用DryIoc的扩展方法注册开放泛型类型ILogger<T>
            //containerRegistry.GetContainer().Register(typeof(ILogger<>), typeof(Logger<>));
#endif
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

            //底层框架
            moduleCatalog.AddModule<BaseFrameworkManagerModule>();
            //上层PO后端服务
            moduleCatalog.AddModule<ModuleFile>();

            //各个子模块
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

    }
}
