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
        public override async Task Execute(IJobExecutionContext context)
        {
            //定时任务，非常简单
            //Console.WriteLine("你好，世界");
            System.Diagnostics.Debug.WriteLine("你好 hhy");
            // var eneities= await _repository.GetListAsync();
            //var entities=   await _sqlSugarClient.Queryable<UserEntity>().ToListAsync();
            //await Console.Out.WriteLineAsync(entities.Count().ToString());
        }
    }
}
