using AYE.BaseFramework.MqttClientCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Prism.Ioc;
using Quartz.Impl;
using Quartz;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYE.BaseFramework.QuartzCore;
using AYE.BaseFramework.Manager.ConfigOptionModel;
using AYE.BaseFramework.SqlSusgarCore;
using Prism.DryIoc;
using SqlSugar;
using System.Reflection;
using DryIoc;
using AYE.BaseFramework.RedisCore;
using StackExchange.Redis;

namespace AYE.BaseFramework.Manager.Extensions;

public static class ContainerRegistryExtensions
{
    public static IContainerRegistry RegisterMqtt5ClientService(this IContainerRegistry containerRegistry, Action<MqttClientBuilderSettings> configureSettings)
    {
        var settings = new MqttClientBuilderSettings();
        configureSettings(settings);

        containerRegistry.RegisterInstance(settings);
        containerRegistry.Register<IMqtt5ClientService, Mqtt5ClientService>();

        return containerRegistry;
    }

    public static IContainerRegistry RegisterLogging(this IContainerRegistry containerRegistry, string nlogConfigFile = "NLog.config", LogLevel minimumLevel = LogLevel.Trace)
    {
        // 配置日志工厂
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(minimumLevel);
            builder.AddNLog(nlogConfigFile);
        });

        // 注册 ILoggerFactory 和 ILogger
        containerRegistry.RegisterInstance(loggerFactory);
        containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));

        return containerRegistry;
    }


    public static IContainerRegistry RegisterConfiguration(this IContainerRegistry containerRegistry, string settingsFileName = "appsettings.json")
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(settingsFileName, optional: false, reloadOnChange: true);

        var configuration = builder.Build();

        // 将 IConfiguration 实例注入到容器中
        containerRegistry.RegisterInstance<IConfiguration>(configuration);

        return containerRegistry;
    }



    public static IContainerRegistry RegisterQuartzSchedulerAsync(this IContainerRegistry containerRegistry, int threadCount = 20)
    {
        // 配置Quartz调度器工厂的参数
        var properties = new NameValueCollection
        {
            { "quartz.threadPool.threadCount", threadCount.ToString() }
        };

        // 创建调度器工厂并注册
        var schedulerFactory = new StdSchedulerFactory(properties);
        containerRegistry.RegisterInstance<ISchedulerFactory>(schedulerFactory);

        // 从工厂创建调度器并注册
        var scheduler = schedulerFactory.GetScheduler().Result;
        containerRegistry.RegisterInstance<IScheduler>(scheduler);

        // 自定义封装也在这里注册
        containerRegistry.Register<ITaskService, TaskService>();

        return containerRegistry;
    }


    public static IContainerRegistry RegisterDatabase(this IContainerRegistry containerRegistry)
    {
        // 读取连接字符串
        var configuration = containerRegistry.GetContainer().Resolve<IConfiguration>();

        // 获取并绑定 DataBaseOptions
        var dataBaseOptions = new DataBaseOptions();
        configuration.GetSection("DataBaseOptions").Bind(dataBaseOptions);
        containerRegistry.RegisterInstance(dataBaseOptions);

        //// 获取并绑定 AppSettings
        //var appSettings = new AppSettings();
        //configuration.GetSection("AppSettings").Bind(appSettings);
        //containerRegistry.RegisterInstance(appSettings);

        // 批量注册SQL Sugar客户端
        BatchProcessing(dataBaseOptions, (dbTypeEnum, item) =>
        {
            var sqlSugarClient = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = item.ConnectionString,
                DbType = dbTypeEnum,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    //注意:  这儿AOP设置不能少
                    EntityService = (c, p) =>
                    {
                        //支持string?和string  
                        if (p.IsPrimarykey == false && new NullabilityInfoContext()
                         .Create(c).WriteState is NullabilityState.Nullable)
                        {
                            p.IsNullable = true;
                        }
                    }
                }
            });

            if (dbTypeEnum == DbType.Sqlite)
            {
                containerRegistry.RegisterInstance<ISqlSugarClient>(sqlSugarClient);
            }
            else
            {
                containerRegistry.RegisterInstance<ISqlSugarClient>(sqlSugarClient, dbTypeEnum.ToString());
            }
        });

        // 注册仓储，使用瞬态模式
        containerRegistry.Register(typeof(IRepository<>), typeof(Repository<>));

        return containerRegistry;
    }

    /// <summary>
    /// 批量处理DB配置项对应的操作
    /// </summary>
    /// <param name="dataBaseOptions"></param>
    /// <param name="action"></param>
    /// <exception cref="ArgumentException"></exception>
    private static void BatchProcessing(DataBaseOptions dataBaseOptions, Action<DbType, DatabaseConfig> action)
    {
        foreach (var item in dataBaseOptions.Databases)
        {
            if (!item.IsEnable) { continue; }

            if (Enum.TryParse(item.DbType, out DbType dbTypeEnum) == false)
                throw new ArgumentException($"'{item.DbType}' is not a valid value for DbType enum.");

            action(dbTypeEnum, item);
        }
    }



    public static IContainerRegistry RegisterRedis(this IContainerRegistry containerRegistry)
    {
        // 读取连接字符串
        var configuration = containerRegistry.GetContainer().Resolve<IConfiguration>();
        // 读取和绑定 Redis 配置
        var redisOptions = new RedisOptions();
        configuration.GetSection("RedisOptions").Bind(redisOptions);
        containerRegistry.RegisterInstance(redisOptions);
        if(!redisOptions.IsEnable)
            return containerRegistry;

        // 配置 Redis 连接
        var redisConnection = ConnectionMultiplexer.Connect(redisOptions.Configuration);
        containerRegistry.RegisterInstance<IConnectionMultiplexer>(redisConnection);

        // 注册 Redis 服务
        containerRegistry.Register<IRedisService, RedisService>();

        return containerRegistry;
    }





}