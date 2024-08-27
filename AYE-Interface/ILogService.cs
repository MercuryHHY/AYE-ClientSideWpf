using AYE.BaseFramework.SqlSusgarCore;
using AYE_Entity.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Interface;

public interface ILogService
{
    IRepository<CommunicationLogEntity> CommunicationLogRepository { get; }

}


