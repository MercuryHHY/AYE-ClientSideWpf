using AYE.BaseFramework.SqlSusgarCore;
using AYE_Entity.Log;
using AYE_Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Service;

public class LogService : ILogService
{
    public IRepository<CommunicationLogEntity> CommunicationLogRepository { get; }

    public LogService(IRepository<CommunicationLogEntity> communicationLogRepository)
    {
        CommunicationLogRepository = communicationLogRepository;
    }


    #region 创建
    //public async Task<ConnectLogGetListOutputDto> CommunicationCreateAsync(ConnectLogCreateInput input)
    //{
    //    var entity = await CommunicationLogRepository.CopyNew().AsInsertable(input.Adapt<CommunicationLogEntity>()).SplitTable().ExecuteCommandAsync();
    //    return entity.Adapt<ConnectLogGetListOutputDto>();
    //}



    #endregion

    #region 查询

    #endregion

    #region 导出

    #endregion
}
