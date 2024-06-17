using AYE.BaseFramework.QuartzCore.Dtos;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.QuartzCore
{
    public interface ITaskService
    {
        Task<TaskGetOutput> GetAsync(string jobId);

        Task CreateAsync(TaskCreateInput input);

        Task DeleteAsync(IEnumerable<string> id);

        Task PauseAsync(string jobId);

        Task StartAsync(string jobId);

        Task UpdateAsync(string id, TaskUpdateInput input);

        Task RunOnceAsync(string id);

    }
}
