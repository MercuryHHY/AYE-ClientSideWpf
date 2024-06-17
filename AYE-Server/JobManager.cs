using AYE.BaseFramework.QuartzCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Service
{
    public class JobManager
    {
        private readonly ITaskService taskService;


        public JobManager(ITaskService taskService)
        {
            this.taskService = taskService;
        }








    }
}
