using AYE.BaseFramework.QuartzCore;
using AYE.BaseFramework.SqlSusgarCore;
using AYE_Entity;
using AYE_Job;
using AYE_Service;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoModuleA.ViewModels
{
    public class UserControlAViewModel
    {
        private  ITaskService _taskService;
        private readonly IRepository<UserInfo001> _Userrepository;
        public UserControlAViewModel(ITaskService taskService, IRepository<UserInfo001> userrepository)
        {
            _taskService = taskService;
            _Userrepository = userrepository;
            //InitializeSchedulerAsync();

            //这里直接手动new 先不要DI注入，先手动测试
            JobManager jobManager = new JobManager(_taskService);
            GolalCacheManager golalCache = new GolalCacheManager(_Userrepository);

        }






    }
}
