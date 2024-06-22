using AYE.BaseFramework.QuartzCore;
using AYE.BaseFramework.SqlSusgarCore;
using AYE_Entity;
using AYE_Interface;
using AYE_Job;
using AYE_Service;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoModuleA.ViewModels
{
    /// <summary>
    /// 模块测试
    /// </summary>
    public class UserControlAViewModel
    {
        private ITaskService _taskService;
        private readonly IRepository<UserInfo001Entity> _Userrepository;
        private readonly ILogger<UserControlAViewModel> _logger;
        private readonly IJobManager _jobManager;
        public UserControlAViewModel(ITaskService taskService, IContainerProvider containerProvider, IRepository<UserInfo001Entity> userrepository, ILogger<UserControlAViewModel> logger, IJobManager jobManager)
        {
            _taskService = taskService;
            _jobManager = jobManager;
            _Userrepository = userrepository;
            _logger = logger;

            //这里直接手动new 先不要DI注入，先手动测试
            //JobManager jobManager = new JobManager(_taskService);
            //GolalCacheManager golalCache = new GolalCacheManager(containerProvider);

            //_Userrepository._Db.DbMaintenance.CreateDatabase();//存在则不创建


            _logger.LogDebug("我是Debug");
            
        }






    }
}
