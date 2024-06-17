using AYE.BaseFramework.QuartzCore.Dtos;
using AYE.BaseFramework.QuartzCore.Enums;
using Quartz.Impl.Matchers;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Timing;
using Newtonsoft.Json;
using Mapster;

namespace AYE.BaseFramework.QuartzCore;

public class TaskService : ITaskService
{
    private readonly ISchedulerFactory _schedulerFactory;
    ///private readonly IClock _clock;
    public TaskService(ISchedulerFactory schedulerFactory)
    {
        //_clock = clock;
        _schedulerFactory = schedulerFactory;
    }


    /// <summary>
    /// 单查job
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>

    public async Task<TaskGetOutput> GetAsync(string jobId)
    {
        var scheduler = await _schedulerFactory.GetScheduler();

        var jobDetail = await scheduler.GetJobDetail(new JobKey(jobId));
        var trigger = (await scheduler.GetTriggersOfJob(new JobKey(jobId))).First();
        //状态
        var state = await scheduler.GetTriggerState(trigger.Key);


        var output = new TaskGetOutput
        {
            JobId = jobDetail.Key.Name,
            GroupName = jobDetail.Key.Group,
            JobType = jobDetail.JobType.Name,
            Properties = JsonConvert.SerializeObject(jobDetail.JobDataMap),
            Concurrent = !jobDetail.ConcurrentExecutionDisallowed,
            Description = jobDetail.Description,
            //LastRunTime = _clock.Normalize(trigger.GetPreviousFireTimeUtc()?.DateTime ?? DateTime.MinValue),
            LastRunTime = (trigger.GetPreviousFireTimeUtc()?.DateTime ?? DateTime.MinValue),
            //NextRunTime = _clock.Normalize(trigger.GetNextFireTimeUtc()?.DateTime ?? DateTime.MinValue),
            NextRunTime = (trigger.GetNextFireTimeUtc()?.DateTime ?? DateTime.MinValue),
            AssemblyName = jobDetail.JobType.Assembly.GetName().Name,
            Status = state.ToString()
        };

        if (trigger is ISimpleTrigger simple)
        {
            output.TriggerArgs = Math.Round(simple.RepeatInterval.TotalMinutes, 2).ToString() + "分钟";
            output.Type = JobTypeEnum.Millisecond;
            output.Millisecond = simple.RepeatInterval.TotalMilliseconds;
        }
        else if (trigger is ICronTrigger cron)
        {
            output.TriggerArgs = cron.CronExpressionString!;
            output.Type = JobTypeEnum.Cron;
            output.Cron = cron.CronExpressionString;
        }
        return output;
    }

    

    /// <summary>
    /// 创建job，只是在调度中心创建一个实例，但是没有启动
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task CreateAsync(TaskCreateInput input)
    {
        var scheduler = await _schedulerFactory.GetScheduler();

        //设置启动时执行一次，然后最大只执行一次


        //jobBuilder
        var jobClassType = Assembly.Load(input.AssemblyName).GetTypes().Where(x => x.Name == input.JobType).FirstOrDefault();

        if (jobClassType is null)
        {
            throw new UserFriendlyException($"程序集：{input.AssemblyName}，{input.JobType} 不存在");
        }

        var jobBuilder = JobBuilder.Create(jobClassType).WithIdentity(new JobKey(input.JobId, input.GroupName))
            .WithDescription(input.Description);
        if (!input.Concurrent)
        {
            jobBuilder.DisallowConcurrentExecution();
        }

        //triggerBuilder
        TriggerBuilder triggerBuilder = null;
        switch (input.Type)
        {
            case JobTypeEnum.Cron:
                triggerBuilder =
                   TriggerBuilder.Create()
                   .WithCronSchedule(input.Cron);




                break;
            case JobTypeEnum.Millisecond:
                triggerBuilder =
                 TriggerBuilder.Create().StartNow()
                                        .WithSimpleSchedule(x => x
                                        .WithInterval(TimeSpan.FromMilliseconds(input.Millisecond ?? 10000))
                                        .RepeatForever()
                                        );
                break;
        }

        //作业计划,单个jobBuilder与多个triggerBuilder组合
        await scheduler.ScheduleJob(jobBuilder.Build(), triggerBuilder.Build());
    }

    /// <summary>
    /// 移除job
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteAsync(IEnumerable<string> id)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.DeleteJobs(id.Select(x => new JobKey(x)).ToList());
    }

    /// <summary>
    /// 暂停job
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public async Task PauseAsync(string jobId)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.PauseJob(new JobKey(jobId));
    }

    /// <summary>
    /// 开始job
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public async Task StartAsync(string jobId)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.ResumeJob(new JobKey(jobId));
    }

    /// <summary>
    /// 更新job
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task UpdateAsync(string id, TaskUpdateInput input)
    {
        await DeleteAsync(new List<string>() { id });
        await CreateAsync(input.Adapt<TaskCreateInput>());
    }

    public async Task RunOnceAsync(string id)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobDetail = await scheduler.GetJobDetail(new JobKey(id));

        var jobBuilder = JobBuilder.Create(jobDetail.JobType).WithIdentity(new JobKey(Guid.NewGuid().ToString()));
        //设置启动时执行一次，然后最大只执行一次
        var trigger = TriggerBuilder.Create().WithIdentity(Guid.NewGuid().ToString()).StartNow()
            .WithSimpleSchedule(x => x
           .WithIntervalInHours(1)
            .WithRepeatCount(1))
          .Build();

        await scheduler.ScheduleJob(jobBuilder.Build(), trigger);
    }
}
