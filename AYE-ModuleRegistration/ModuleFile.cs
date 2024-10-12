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
using AYE_Commom.ConfigOptionModel;
using System.Configuration;
using StackExchange.Redis;
using AYE.BaseFramework.RedisCore;
using System.Security.Cryptography;
using AYE.BaseFramework.QuartzCore.Enums;


namespace AYE_ModuleRegistration
{

    /// <summary>
    /// 这个类库最开始是模块化注册的管理者，依赖于 AYE_Service  ，被高层模块引用 
    /// 为什么要这么做呢？ 
    /// 因为我不希望底层模块与 prism框架强耦合
    /// 但是到了现在，我发觉还不止如此，它应该还可以用于 减少高层模块与子模块的依赖包引用
    /// 于是乎，重心转移之后，这个类库就变的非常重要
    /// 由于框架所需，重心再次转移 转移到Commom层
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
            // 注册配置服务
            containerRegistry.RegisterSingleton<IConfigurationService, ConfigurationService>();

            // 读取连接字符串
            var configurationService = containerRegistry.GetContainer().Resolve<IConfigurationService>();

            #region 数据库注册
            {
                //var connectionString2 = configurationService.Configuration.GetConnectionString("DBConnection");
                //var connectionString1 = configurationService.Configuration["ConnectionStrings:DBConnection"];
                //var connectionString = configurationService.Configuration.GetSection("ConnectionStrings")["DBConnection"];
                //var dbType = configurationService.Configuration["DbType"];//暂时放这

                var dataBaseOptions = new DataBaseOptions();
                configurationService.Configuration.GetSection("DataBaseOptions").Bind(dataBaseOptions);
                containerRegistry.RegisterInstance(dataBaseOptions);

                var appSettings = new AppSettings();
                configurationService.Configuration.GetSection("AppSettings").Bind(appSettings);
                containerRegistry.RegisterInstance(appSettings);


#if false
                foreach (var item in dataBaseOptions.Databases)
                {
                    if (!item.IsEnable) { continue; }

                    if (Enum.TryParse(item.DbType, out DbType dbTypeEnum) == false) throw new ArgumentException($"'{item.DbType}' is not a valid value for DbType enum.");

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
                            DbType = DbType.MySql,
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

                }
#endif

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
                            DbType = DbType.MySql,
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




#if false

                if (Enum.TryParse(dataBaseOptions.Databases.DbType2, out DbType dbTypeEnum) == false) throw new ArgumentException($"'{dataBaseOptions.DbType2}' is not a valid value for DbType enum.");

                // 注册 默认的 SqlSugar 服务
                containerRegistry.RegisterInstance<ISqlSugarClient>(new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = dataBaseOptions.ConnectionStringsDbType2.DBConnection,
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

                // 注册 MySQL 的 SqlSugar 服务
                containerRegistry.RegisterInstance<ISqlSugarClient>(new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = dataBaseOptions.ConnectionStringsDbType1.DBConnection,
                    DbType = DbType.MySql,
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
                }), DbType.MySql.ToString());//注意，这里我多加了一个参数用作KEY

#endif


                //注册仓储  （确定要用单例吗，最好是用瞬态）
                //没关系 VM层在注册的时候也是瞬态的，所以这里可以用瞬态,VM层可以直接注入仓储
                containerRegistry.Register(typeof(IRepository<>), typeof(Repository<>));
            }
#endregion

            #region Redis注册
            {
                // 读取和绑定配置
                var redisOptions = new RedisOptions();
                configurationService.Configuration.GetSection("RedisOptions").Bind(redisOptions);
                containerRegistry.RegisterInstance(redisOptions);

                // 配置Redis连接
                //var redisConnection = ConnectionMultiplexer.Connect(redisOptions.Configuration);
                //containerRegistry.RegisterInstance(redisConnection);

                // 注册Redis服务
                //containerRegistry.Register<IRedisService, RedisService>();
                //或者把DB注册出来用

            }
            #endregion

            #region Quartz定时任务注册
            //这里给参数配置Quartz的生成调度中心的工厂，那么此时工厂生产的调度器默认线程池会是20
            containerRegistry.RegisterInstance<ISchedulerFactory>(new StdSchedulerFactory(new System.Collections.Specialized.NameValueCollection
            {
                { "quartz.threadPool.threadCount", "20" }, // 设置线程池大小为20
            }));

            //如果每次使用调度器都要从 工厂去拿，对一个单机WPF程序而言，根本没必要，所以我决定直接将调度器注入IOC
            containerRegistry.RegisterInstance<IScheduler>(containerRegistry.GetContainer().Resolve<ISchedulerFactory>().GetScheduler().Result);//我有点长，你需要忍耐
            containerRegistry.Register<ITaskService, TaskService>();
            #endregion

            ServiceRegisterTypes(containerRegistry);
        }


        /// <summary>
        /// 业务的服务注册
        /// </summary>
        /// <param name="containerRegistry"></param>
        public void ServiceRegisterTypes(IContainerRegistry containerRegistry)
        {
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
            //根据配置文件决定 是否开启Codefirst
            var dataBaseOptions = containerProvider.Resolve<DataBaseOptions>();
            if (dataBaseOptions.UseCodeFirst)
            {
                _logger.LogDebug("CodeFirst 正在执行");
                Type[] types = typeof(DictionaryEntity).Assembly.GetTypes()
                                .Where(it => it.FullName != null && it.FullName.Contains("AYE_Entity") && it.Name.Contains("Entity"))//命名空间过滤，当然也可以写其他条件过滤
                                .ToArray();

#if false
                //建库：如果不存在创建数据库存在不会重复创建 createdb
                containerProvider.Resolve<ISqlSugarClient>(DbType.MySql.ToString()).DbMaintenance.CreateDatabase();
                containerProvider.Resolve<ISqlSugarClient>().DbMaintenance.CreateDatabase();
                //两种数据库的Codefirst 执行
                containerProvider.Resolve<ISqlSugarClient>(DbType.MySql.ToString()).CodeFirst.SetStringDefaultLength(200).InitTables(types);//根据types创建表
                containerProvider.Resolve<ISqlSugarClient>().CodeFirst.SetStringDefaultLength(200).InitTables(types);//默认的sqllite
#endif

#if false
                foreach (var item in dataBaseOptions.Databases)
                {
                    if (!item.IsEnable) { continue; }

                    if (Enum.TryParse(item.DbType, out DbType dbTypeEnum) == false) throw new ArgumentException($"'{item.DbType}' is not a valid value for DbType enum.");

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
                }
#endif


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

            //缓存数据初始化
            await containerProvider.Resolve<GolalCacheManager>().LoadAllAsync();





        }


        /// <summary>
        /// 批量处理DB配置项对应的操作
        /// </summary>
        /// <param name="dataBaseOptions"></param>
        /// <param name="action"></param>
        /// <exception cref="ArgumentException"></exception>
        private void BatchProcessing(DataBaseOptions dataBaseOptions ,Action<DbType, DatabaseConfig> action)
        {
            foreach (var item in dataBaseOptions.Databases)
            {
                if (!item.IsEnable) { continue; }

                if (Enum.TryParse(item.DbType, out DbType dbTypeEnum) == false) throw new ArgumentException($"'{item.DbType}' is not a valid value for DbType enum.");

                action(dbTypeEnum, item);
            }
        }




    }
}
