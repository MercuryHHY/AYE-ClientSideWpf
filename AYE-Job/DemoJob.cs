using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers.Quartz;

namespace AYE_Job;

public class DemoJob : QuartzBackgroundWorkerBase
{
    public override async Task Execute(IJobExecutionContext context)
    {
       
        System.Diagnostics.Debug.WriteLine("我是Demo");
       
    }
}
