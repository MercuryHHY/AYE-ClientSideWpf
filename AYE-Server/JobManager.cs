﻿using AYE.BaseFramework.QuartzCore;
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
using AYE_Interface;

namespace AYE_Service
{
    public class JobManager: IJobManager
    {
        private readonly ITaskService taskService;
        private readonly IGolalCacheManager _golalCacheManager;

        public JobManager(ITaskService taskService, IGolalCacheManager golalCacheManager)
        {
            this.taskService = taskService;
            _golalCacheManager = golalCacheManager;

            //测试 先将这函数放在这里，一般来说会放在异步命令的执行中，服务中只提供接口的实现即可，工给异步命令调用
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
        public async Task InitializeSchedulerAsyncTest()
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
            await taskService.StartAsync("TestJob");
            //await taskService.ResumeJobAsync("TestJob");

            //创建一个同样的JOB，只改ID，先创建再启动
            taskCreateInput.JobId = "TestJob_2";
            await taskService.CreateAsync(taskCreateInput);
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
            await taskService.StartAsync("DemoJob");
            //await taskService.ResumeJobAsync("DemoJob");//还原




        }

        
    }
}
