using AYE.BaseFramework.QuartzCore;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace AYE_ModuleRegistration
{

    /// <summary>
    /// 这个类库最开始是模块化注册的管理者，依赖于 AYE_Service  ，被高层模块引用
    /// 为什么要这么做呢？ 
    /// 因为我不希望底层模块与 prism框架强耦合
    /// 但是到了现在，我发觉还不止如此，它应该还可以用于 减少高层模块与子模块的依赖包引用
    /// 于是乎，重心转移之后，这个类库就变的非常重要
    /// </summary>
    public class ModuleFile : IModule
    {
       
        /// <summary>
        /// 此模块预加载之后，当主页面成功得到之后 才会加载子模块内部的注册
        /// </summary>
        /// <param name="containerRegistry"></param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.Register<ITaskService, TaskService>();


        }



        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

    }
}
