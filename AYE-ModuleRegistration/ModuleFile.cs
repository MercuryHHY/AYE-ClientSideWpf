using AYE.BaseFramework.QuartzCore;
using AYE.BaseFramework.SqlSusgarCore;
using AYE_BaseFramework.ConfigurationCore;
using Microsoft.Extensions.Configuration;
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
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using AYE_Service;
using AYE_Interface;
using Microsoft.Extensions.DependencyInjection;
using DryIoc;
using NLog;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Reflection;
using AYE_Entity;
using Prism.DryIoc;
using System.Xml.Linq;
using System.Configuration;
using StackExchange.Redis;
using AYE.BaseFramework.RedisCore;
using System.Security.Cryptography;
using AYE.BaseFramework.QuartzCore.Enums;
using AYE.BaseFramework.Manager.ConfigOptionModel;


namespace AYE_ModuleRegistration;


/// <summary>
/// 到了这一步
/// 我希望这个类，仅仅只是作为 上层PO应用的服务 注册
/// 至于底层框架在构建时，应由专门的管理者 实行注册
/// 至于各个子模块的 
/// 
/// Codefirst 只能先放这里
/// </summary>
public class ModuleFile : IModule
{
    private readonly ILogger<ModuleFile> _logger;
    public ModuleFile(ILogger<ModuleFile> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 此模块预加载之后，当主页面成功得到之后 才会加载子模块内部的注册
    /// </summary>
    /// <param name="containerRegistry"></param>
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        //业务的服务注册
        containerRegistry.RegisterSingleton<IGolalCacheManager, GolalCacheManager>();
        containerRegistry.RegisterSingleton<IJobManager, JobManager>();

    }


    /// <summary>
    /// 模块化注册结束，最后的在这里进行一系列的数据初始化操作
    /// 全局缓存数据的初始化 也在这里
    /// 
    /// </summary>
    /// <param name="containerProvider"></param>
    public async void OnInitialized(IContainerProvider containerProvider)
    {
        //缓存数据初始化
        await containerProvider.Resolve<GolalCacheManager>().LoadAllAsync();

    }


}
