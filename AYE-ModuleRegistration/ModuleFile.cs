using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYE_Server;
using AYE_Interface;

namespace AYE_ModuleRegistration
{
    public class ModuleFile : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterScoped<IDemoInterface1, DemoService>();
        }
    }
}
