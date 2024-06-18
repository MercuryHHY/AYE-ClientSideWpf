using AYE.BaseFramework.QuartzCore;
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
        public UserControlAViewModel(ITaskService taskService)
        {
            _taskService = taskService;
            //InitializeSchedulerAsync();

            //这里直接手动new 先不要DI注入，先手动测试
            JobManager jobManager = new JobManager(_taskService);

        }


        /// <summary>
        /// TestJob 启动测试
        /// </summary>
        /// <returns></returns>
        private async Task InitializeSchedulerAsync()
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler(); // 等待异步任务完成获取调度器


            IJobDetail jobDetail = JobBuilder.Create<TestJob>().WithIdentity(nameof(TestJob)).Build();
            ITrigger trigger = TriggerBuilder.Create().WithIdentity(nameof(TestJob)).StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(3)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(jobDetail, trigger); // 等待调度任务完成

            //这个启动是只启动一个吗，还是调度中心中的所有
            await scheduler.Start(); // 启动调度器
        }



    }
}
