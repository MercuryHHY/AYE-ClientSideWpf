using AYE.BaseFramework.QuartzCore;
using AYE.BaseFramework.SqlSusgarCore;
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
using NLog.Extensions.Logging;
using DryIoc;
using Microsoft.Extensions.Configuration;
using AYE.BaseFramework.RedisCore;
using AYE.BaseFramework.Manager.ConfigOptionModel;
using System.Collections;
using AYE.BaseFramework.Manager.Extensions;

namespace AYE.BaseFramework.Manager;

public class BaseFrameworkManagerModule : IModule
{

    


    /// <summary>
    /// 此模块预加载之后，当主页面成功得到之后 才会加载各个部分内部的注册
    /// </summary>
    /// <param name="containerRegistry"></param>
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        #region 日志的注册必须放在最开始的地方
        containerRegistry.RegisterLogging();
        #endregion

        containerRegistry.RegisterConfiguration();

        containerRegistry.RegisterDatabase();
#if false
        // 读取连接字符串
        var configuration = containerRegistry.GetContainer().Resolve<IConfiguration>();

        #region 数据库注册
        {

            var dataBaseOptions = new DataBaseOptions();
            configuration.GetSection("DataBaseOptions").Bind(dataBaseOptions);
            containerRegistry.RegisterInstance(dataBaseOptions);

            var appSettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(appSettings);
            containerRegistry.RegisterInstance(appSettings);

            BatchProcessing(dataBaseOptions, (dbTypeEnum, item) =>
            {
                if (dbTypeEnum == DbType.Sqlite)
                {
                    // 注册 默认的 SqlSugar 服务
                    containerRegistry.RegisterInstance<ISqlSugarClient>(new SqlSugarClient(new ConnectionConfig()
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
                                /***高版C#写法***/
                                //支持string?和string  
                                if (p.IsPrimarykey == false && new NullabilityInfoContext()
                                 .Create(c).WriteState is NullabilityState.Nullable)
                                {
                                    p.IsNullable = true;
                                }
                            }
                        }
                    }));

                }
                else
                {
                    // 注册 MySQL 的 SqlSugar 服务
                    containerRegistry.RegisterInstance<ISqlSugarClient>(new SqlSugarClient(new ConnectionConfig()
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
                                /***高版C#写法***/
                                //支持string?和string  
                                if (p.IsPrimarykey == false && new NullabilityInfoContext()
                                 .Create(c).WriteState is NullabilityState.Nullable)
                                {
                                    p.IsNullable = true;
                                }
                            }
                        }
                    }), dbTypeEnum.ToString());//注意，这里我多加了一个参数用作KEY

                }

            });

            //注册仓储  （确定要用单例吗，最好是用瞬态）
            //没关系 VM层在注册的时候也是瞬态的，所以这里可以用瞬态,VM层可以直接注入仓储
            containerRegistry.Register(typeof(IRepository<>), typeof(Repository<>));
        }
        #endregion


#endif


        containerRegistry.RegisterRedis();

#if false
        #region Redis注册
        {
            var configuration = containerRegistry.GetContainer().Resolve<IConfiguration>();
            // 读取和绑定配置
            var redisOptions = new RedisOptions();
            configuration.GetSection("RedisOptions").Bind(redisOptions);
            containerRegistry.RegisterInstance(redisOptions);

            // 配置Redis连接
            var redisConnection = ConnectionMultiplexer.Connect(redisOptions.Configuration);
            containerRegistry.RegisterInstance(redisConnection);

            // 注册Redis服务
            containerRegistry.Register<IRedisService, RedisService>();

        }
        #endregion
#endif

        containerRegistry.RegisterQuartzSchedulerAsync();

        //自定义 扩展方法，用于注册MQTT客户端
        containerRegistry.RegisterMqtt5ClientService(options =>
        {
            options.BrokerAddress = "your-broker-address";
            options.BrokerPort = 1883;
            options.ClientId = "your-client-id";
            options.Username = "your-username";
            options.Password = "your-password";
            options.UseTls = false;
            options.CleanSession = true;
        });



    }



    /// <summary>
    /// 模块化注册结束，最后的在这里进行一系列的数据初始化操作
    /// </summary>
    /// <param name="containerProvider"></param>
    public void OnInitialized(IContainerProvider containerProvider)
    {
        var _logger = containerProvider.Resolve<ILogger<BaseFrameworkManagerModule>>();

        //根据配置文件决定 是否开启Codefirst
        var dataBaseOptions = containerProvider.Resolve<DataBaseOptions>();
        if (dataBaseOptions.UseCodeFirst)
        {
            _logger.LogDebug("CodeFirst 正在执行");
            //Type[] types = typeof(DictionaryEntity).Assembly.GetTypes()
            //                .Where(it => it.FullName != null && it.FullName.Contains("AYE_Entity") && it.Name.Contains("Entity"))//命名空间过滤，当然也可以写其他条件过滤
            //                .ToArray();

            Type[] types = Assembly
                    .LoadFrom("AYE-Entity.dll")//如果 .dll报错，可以换成 xxx.exe 有些生成的是exe 
                    .GetTypes()
                    .Where(it => it.FullName != null && it.FullName.Contains("AYE_Entity") && it.Name.Contains("Entity"))//命名空间过滤，当然也可以写其他条件过滤
                    .ToArray();



            BatchProcessing(dataBaseOptions, (dbTypeEnum, item) =>
            {
                if (dbTypeEnum == DbType.Sqlite)
                {
                    containerProvider.Resolve<ISqlSugarClient>().DbMaintenance.CreateDatabase();
                    containerProvider.Resolve<ISqlSugarClient>().CodeFirst.SetStringDefaultLength(200).InitTables(types);//默认的sqllite
                }
                else
                {
                    containerProvider.Resolve<ISqlSugarClient>(dbTypeEnum.ToString()).DbMaintenance.CreateDatabase();
                    containerProvider.Resolve<ISqlSugarClient>(dbTypeEnum.ToString()).CodeFirst.SetStringDefaultLength(200).InitTables(types);//根据types创建表
                }

            });

            _logger.LogDebug("CodeFirst 执行完成！！！！！！");

        }

    }

    /// <summary>
    /// 批量处理DB配置项对应的操作
    /// </summary>
    /// <param name="dataBaseOptions"></param>
    /// <param name="action"></param>
    /// <exception cref="ArgumentException"></exception>
    private void BatchProcessing(DataBaseOptions dataBaseOptions, Action<DbType, DatabaseConfig> action)
    {
        foreach (var item in dataBaseOptions.Databases)
        {
            if (!item.IsEnable) { continue; }

            if (Enum.TryParse(item.DbType, out DbType dbTypeEnum) == false) throw new ArgumentException($"'{item.DbType}' is not a valid value for DbType enum.");

            action(dbTypeEnum, item);
        }
    }





}
