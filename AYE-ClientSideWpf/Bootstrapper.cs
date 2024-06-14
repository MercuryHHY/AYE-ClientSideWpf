using AYE_BaseShare;
using AYE_ClientSideWpf.Service;
using AYE_ClientSideWpf.ViewModels;
using AYE_ClientSideWpf.Views;
using AYE_ModuleRegistration;
using AYE_Service;
using MyToDo.Common;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using SqlSugar;
using System.Windows;
using static AYE_BaseShare._1_CodeFirst;

namespace AYE_ClientSideWpf
{
    /// <summary>
    /// 原本类似 痕迹 的操作 一样  直接去操作APP启动项文件就可以聊聊
    /// 但是这个包也提供了如下这种操作，个人也偏向项目模版自带的这种
    /// </summary>
    public class Bootstrapper : PrismBootstrapper
    {
       
        /// <summary>
        /// 1,先执行注册
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //在这里 添加依赖注入 添加其他 用户控件
            //containerRegistry.RegisterForNavigation<UserControlDemoA>();

            var connectionString= "server=127.0.0.1;Database=aye-hhy;Uid=root;Pwd=root;sslMode=None";

            // 注册 SqlSugar 服务
            containerRegistry.RegisterInstance<ISqlSugarService>(new SqlSugarService(connectionString));
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
            containerRegistry.RegisterSingleton(typeof(ISimpleClient<>), typeof(SimpleClient<>));
            //containerRegistry.RegisterScoped<IDemoInterface12, DemoService1>();


        }


        /// <summary>
        /// 2，预加载模块,但是模块中的注册没有执行
        /// </summary>
        /// <param name="moduleCatalog"></param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // 这里是添加其他 类库的模块注册类中 注册行为
            //moduleCatalog.AddModule<ModuleAProfile>();
            moduleCatalog.AddModule<ModuleFile>();
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
        }

    }
}
