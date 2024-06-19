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
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            }));

#if false
            #region 还没有想好同时支持多库如何处理
            {
                //var mySqlConnectionString = configurationService.Configuration.GetConnectionString("MySqlConnection");
                var sqliteConnectionString1 = configurationService.Configuration.GetConnectionString("SQLiteConnection");
                var sqliteConnectionString2 = configurationService.Configuration["SqlLiteConnectionStrings:SqlLiteConnection"];
               
                // 注册 MySQL 的 SqlSugar 服务
                //containerRegistry.RegisterInstance<ISqlSugarClient>(new SqlSugarClient(new ConnectionConfig()
                //{
                //    ConnectionString = mySqlConnectionString,
                //    DbType = DbType.MySql,
                //    IsAutoCloseConnection = true,
                //    InitKeyType = InitKeyType.Attribute
                //}), "MySqlClient");

                // 注册 SQLite 的 SqlSugar 服务
                containerRegistry.RegisterInstance<ISqlSugarClient>(new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = sqliteConnectionString2,
                    DbType = DbType.Sqlite,
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute
                }), "SQLiteClient");

            }
            #endregion
#endif

            //注册仓储  （确定要用单例吗，最好是用瞬态）没关系 VM层在注册的时候也是瞬态的，所以这里可以用瞬态
            //containerRegistry.RegisterSingleton(typeof(IRepository<>), typeof(Repository<>));
            containerRegistry.Register(typeof(IRepository<>), typeof(Repository<>));
            
            containerRegistry.RegisterInstance<ISchedulerFactory>(new StdSchedulerFactory());
            containerRegistry.Register<ITaskService, TaskService>();


        }



        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

    }
}
