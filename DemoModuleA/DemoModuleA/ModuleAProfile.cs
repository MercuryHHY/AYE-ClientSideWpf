using AYE.BaseFramework.SqlSusgarCore;
using DemoModuleA.ViewModels;
using DemoModuleA.Views;
using Microsoft.Win32;
using Prism.Ioc;
using Prism.Modularity;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DemoModuleA
{
    public class ModuleAProfile : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<UserControlA, UserControlAViewModel>();
            containerRegistry.RegisterForNavigation<DataGridDemo, DataGridDemoViewModel>();
        }

        
    }
}
