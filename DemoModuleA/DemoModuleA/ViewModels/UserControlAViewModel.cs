using AYE_Job;
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
        //IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
        
        public UserControlAViewModel()
        {
            InitializeSchedulerAsync();



        }

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
            await scheduler.Start(); // 启动调度器
        }



    }
}
