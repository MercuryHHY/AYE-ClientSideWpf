using AYE.BaseFramework.QuartzCore;
using AYE.BaseFramework.SqlSusgarCore;
using AYE_BaseFramework.ConfigurationCore;
using Microsoft.Extensions.Logging;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Quartz.Impl;
using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AYE.BaseFramework.Manager;

public class BaseFrameworkManagerModule : IModule
{
   

    public void OnInitialized(IContainerProvider containerProvider)
    {
        throw new NotImplementedException();
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        throw new NotImplementedException();
    }
}
