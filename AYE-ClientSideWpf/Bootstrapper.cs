using AYE_ClientSideWpf.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace AYE_ClientSideWpf
{
    /// <summary>
    /// 原本类似 痕迹 的操作 一样  直接去操作APP启动项文件就可以聊聊
    /// 但是这个包也提供了如下这种操作，个人也偏向项目模版自带的这种
    /// </summary>
    public class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            // 通过容器去 拿到这个 启动类
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //在这里 添加依赖注入 添加其他 用户控件
            //containerRegistry.RegisterForNavigation<UserControlDemoA>();
            //containerRegistry.RegisterForNavigation<FloatingBlockDemoCtl1>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // 这里是添加其他 类库的模块注册类中 注册行为
            //moduleCatalog.AddModule<ModuleAProfile>();
            //moduleCatalog.AddModule<ModuleBProfile>();
            base.ConfigureModuleCatalog(moduleCatalog);
        }

    }
}
