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
       
        /// <summary>
        /// 此模块预加载之后，当主页面成功得到之后 才会加载子模块内部的注册
        /// </summary>
        /// <param name="containerRegistry"></param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 注册配置服务
            containerRegistry.RegisterSingleton<IConfigurationService, ConfigurationService>();

            // 读取连接字符串
            var configurationService = new ConfigurationService();
            var connectionString = configurationService.Configuration.GetConnectionString("DBConnection");
            var dbType = configurationService.Configuration["DbType"];//暂时放这
            if (Enum.TryParse(dbType, out DbType dbTypeEnum) == false) throw new ArgumentException($"'{dbType}' is not a valid value for DbType enum.");
            
            // 注册 SqlSugar 服务
            containerRegistry.RegisterInstance<ISqlSugarClient>(new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connectionString,
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
                ConnectionString = "server=127.0.0.1;Database=aye-hhy;Uid=root;Pwd=root;sslMode=None",
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
            }), DbType.MySql.ToString());






#if false
            #region 还没有想好同时支持多库如何处理
            {
                //我想尽了一切可能，似乎没有没办法直接这么玩，可是如果分装一个工厂，又会违背我简化操作的初衷
                // 那还不如就按下面所示 直接按 Key 注册出SqlSugarClient使用

                // 注册 MySQL 的 SqlSugar 服务
                containerRegistry.RegisterInstance<ISqlSugarClient>(new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = "server=127.0.0.1;Database=aye-hhy;Uid=root;Pwd=root;sslMode=None",
                    DbType = DbType.MySql,
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute
                }), DbType.MySql.ToString());
                //containerRegistry.Register(typeof(ISuperRepository<>), c => new SuperRepository<object>(c.Resolve<IContainerProvider>(), "MySql"));
                //containerRegistry.Register(typeof(ISuperRepository<>), c =>
                //{
                //    var serviceType = c.GetType().GenericTypeArguments[0];
                //    var genericType = typeof(SuperRepository<>).MakeGenericType(serviceType);
                //    return Activator.CreateInstance(genericType, c.Resolve<IContainerProvider>(), "MySql");
                //});
                //containerRegistry.Register(typeof(ISuperRepository<>), typeof(SuperRepository<>));
                //containerRegistry.RegisterInstance(typeof(ISuperRepository<>),new SuperRepository<>("MySql"),)
            }
            #endregion
#endif

            //注册仓储  （确定要用单例吗，最好是用瞬态）
            //没关系 VM层在注册的时候也是瞬态的，所以这里可以用瞬态,VM层可以直接注入仓储
            containerRegistry.Register(typeof(IRepository<>), typeof(Repository<>));
            

            containerRegistry.RegisterInstance<ISchedulerFactory>(new StdSchedulerFactory());
            containerRegistry.Register<ITaskService, TaskService>();


        }


        /// <summary>
        /// 模块化注册结束，最后的在这里进行一系列的数据初始化操作
        /// </summary>
        /// <param name="containerProvider"></param>
        public void OnInitialized(IContainerProvider containerProvider)
        {

            //建库：如果不存在创建数据库存在不会重复创建 createdb
            containerProvider.Resolve<ISqlSugarClient>(DbType.MySql.ToString()).DbMaintenance.CreateDatabase();
            containerProvider.Resolve<ISqlSugarClient>().DbMaintenance.CreateDatabase();
            //在这里进行数据库的CodeFirst初始化
            Type[] types = typeof(UserInfo002Entity).Assembly.GetTypes()
                            .Where(it => it.FullName!=null&&it.FullName.Contains("AYE_Entity")&&it.Name.Contains("Entity"))//命名空间过滤，当然你也可以写其他条件过滤
                            .ToArray();

            containerProvider.Resolve<ISqlSugarClient>(DbType.MySql.ToString()).CodeFirst.SetStringDefaultLength(200).InitTables(types);//根据types创建表
            containerProvider.Resolve<ISqlSugarClient>().CodeFirst.SetStringDefaultLength(200).InitTables(types);//默认的sqllite
        }
        
    }
}
