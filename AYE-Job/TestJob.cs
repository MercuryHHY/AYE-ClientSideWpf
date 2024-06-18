using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Volo.Abp.BackgroundWorkers.Quartz;

namespace AYE_Job
{
    public class TestJob : QuartzBackgroundWorkerBase
    {
        //IScheduler scheduler = (IScheduler)StdSchedulerFactory.GetDefaultScheduler();
        //public TestJob()
        //{
        //    scheduler = (IScheduler)StdSchedulerFactory.GetDefaultScheduler();
        //    JobDetail = JobBuilder.Create<TestJob>().WithIdentity(nameof(TestJob)).Build();
        //    Trigger = TriggerBuilder.Create().WithIdentity(nameof(TestJob)).StartNow()
        //    .WithSimpleSchedule(x => x
        //        .WithIntervalInSeconds(1000 * 3)
        //        .RepeatForever())
        //    .Build();


        //    scheduler.ScheduleJob(JobDetail, Trigger);
        //    scheduler.Start();
        //}

        //思考
        //  Job的触发如何实现  ===>>  封装一层调度中心
        // job的启动 又该放在哪里，====>>>>>>   放在 service 层，并且单独独立出来
        // job的执行如何 管理=====>>>> 还是需要再 构造函数中管控吗


        //测试成功
        public TestJob()
        {
            ScheduleJob = async (scheduler) =>
            {
                if (DateTime.Now.Minute % 2 == 0)
                {
                    await scheduler.ScheduleJob(JobDetail, Trigger);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("条件不满足，任务未被调度");
                }
            };


        }



        public override async Task Execute(IJobExecutionContext context)
        {
           
            System.Diagnostics.Debug.WriteLine("你好 hhy");
           
        }
    }
}
