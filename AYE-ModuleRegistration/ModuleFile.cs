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
    public class ModuleFile : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        /// <summary>
        /// 此模块预加载之后，当主页面成功得到之后 才会加载子模块内部的注册
        /// </summary>
        /// <param name="containerRegistry"></param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterScoped<IDemoInterface1, DemoService>();
            //var connectionString= "server=127.0.0.1;Database=aye-hhy;Uid=root;Pwd=root;sslMode=None";

            //// 注册 SqlSugar 服务
            //containerRegistry.RegisterInstance<ISqlSugarService>(new SqlSugarService(connectionString));

            containerRegistry.Register<ITaskService, TaskService>();



        }
    }
}
