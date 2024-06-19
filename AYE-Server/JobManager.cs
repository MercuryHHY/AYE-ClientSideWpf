using AYE.BaseFramework.QuartzCore;
using Quartz.Impl;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYE_Job;
using AYE.BaseFramework.QuartzCore.Dtos;
using AYE.BaseFramework.QuartzCore.Enums;

namespace AYE_Service
{
    public class JobManager
    {
        private readonly ITaskService taskService;


        public JobManager(ITaskService taskService)
        {
            this.taskService = taskService;

            InitializeSchedulerAsyncTest();
            //InitializeSchedulerAsync();

        }

        /// <summary>
        /// 初始版本使用方法Demo
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

            //这个启动是调度中心中的所有
            await scheduler.Start(); // 启动调度器
        }


        /// <summary>
        /// 调度中心使用方法Demo
        /// </summary>
        /// <returns></returns>
        private async Task InitializeSchedulerAsyncTest()
        {
            TaskCreateInput taskCreateInput=new TaskCreateInput();
            //taskCreateInput.AssemblyName = "AYE-Job";
            taskCreateInput.AssemblyName = typeof(TestJob).Assembly.GetName().Name!;
            taskCreateInput.JobId = "TestJob";
            taskCreateInput.GroupName = "default";
            taskCreateInput.Concurrent = true;
            taskCreateInput.Type = JobTypeEnum.Millisecond;
            taskCreateInput.Millisecond = 2000;//毫秒
            taskCreateInput.JobType = nameof(TestJob);
            taskCreateInput.Description = "A test job";
            await taskService.CreateAsync(taskCreateInput);

            // 启动作业
            await taskService.StartAsync("TestJob");
            //await taskService.ResumeJobAsync("TestJob");

            taskCreateInput.JobId = "TestJob_2";
            await taskService.CreateAsync(taskCreateInput);

            // 启动作业
            await taskService.StartAsync("TestJob_2");








            TaskCreateInput demoJob = new TaskCreateInput();
            demoJob.AssemblyName = typeof(DemoJob).Assembly.GetName().Name!;
            demoJob.JobId = "DemoJob";
            demoJob.GroupName = "default";
            demoJob.Concurrent = true;
            demoJob.Type = JobTypeEnum.Millisecond;
            demoJob.Millisecond = 2000;//毫秒
            demoJob.JobType = nameof(DemoJob);
            demoJob.Description = "A demoJob job";
            await taskService.CreateAsync(demoJob);

            // 启动作业
            await taskService.StartAsync("DemoJob");
            await taskService.ResumeJobAsync("DemoJob");




        }


    }
}
