using AYE.BaseFramework.SqlSusgarCore;
using AYE_ClientSideWpf.Service;
using AYE_ClientSideWpf.ViewModels;
using AYE_ClientSideWpf.Views;
using AYE_ModuleRegistration;
using DemoModuleA;
using Microsoft.Extensions.Configuration;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Quartz.Impl;
using Quartz;
using SqlSugar;
using System.Windows;

namespace AYE_ClientSideWpf
{
    /// <summary>
    /// 原本类似 痕迹 的操作 一样  直接去操作APP启动项文件就可以聊聊
    /// 但是这个包也提供了如下这种操作，个人也偏向项目模版自带的这种
    /// 很久很久以前，在嵌入式开发中有一种启动项设置 她也叫boot
    /// </summary>
    public class Bootstrapper : PrismBootstrapper
    {
       
        /// <summary>
        /// 1,先执行注册
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //在这里 添加依赖注入 添加其他 用户控件
            //containerRegistry.RegisterForNavigation<UserControlDemoA>();


            // 注册配置服务
            containerRegistry.RegisterSingleton<IConfigurationService, ConfigurationService>();

            // 读取连接字符串
            var configurationService = new ConfigurationService();
            var connectionString = configurationService.Configuration.GetConnectionString("DefaultConnection");
            var dbType = configurationService.Configuration["DbType"];//暂时放这

            // 注册 SqlSugar 服务
            containerRegistry.RegisterInstance<ISqlSugarClient>(new SqlSugarClient(new  ConnectionConfig()
            {
                ConnectionString = connectionString,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            }));

            #if false
            #region 还没有想好同时支持多库如何处理
            {
                var mySqlConnectionString = configurationService.Configuration.GetConnectionString("MySqlConnection");
                var sqliteConnectionString = configurationService.Configuration.GetConnectionString("SQLiteConnection");

                // 注册 MySQL 的 SqlSugar 服务
                containerRegistry.RegisterInstance<ISqlSugarClient>(new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = mySqlConnectionString,
                    DbType = DbType.MySql,
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute
                }), "MySqlClient");

                // 注册 SQLite 的 SqlSugar 服务
                containerRegistry.RegisterInstance<ISqlSugarClient>(new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = sqliteConnectionString,
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
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();




            containerRegistry.RegisterInstance<ISchedulerFactory>(new StdSchedulerFactory());



        }


        /// <summary>
        /// 2，预加载模块,但是模块中的注册没有执行
        /// </summary>
        /// <param name="moduleCatalog"></param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // 这里是添加其他 类库的模块注册类中 注册行为
            moduleCatalog.AddModule<ModuleFile>();
            moduleCatalog.AddModule<ModuleAProfile>();
            base.ConfigureModuleCatalog(moduleCatalog);
        }


        /// <summary>
        /// 3，创建启动类
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject CreateShell()
        {
            // 通过容器去 拿到这个 启动类
            return Container.Resolve<MainWindow>();
        }

    }
}
