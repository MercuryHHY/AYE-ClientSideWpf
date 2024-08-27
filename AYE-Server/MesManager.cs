using AYE.BaseFramework.SqlSusgarCore;
using AYE_Commom.Enum.Mes;
using AYE_Commom.Models.Mes;
using AYE_Entity;
using AYE_Interface;
using AYE_Interface.Mes;
using AYE_Service.MES;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Service;

public class MesManager : IMesManager
{

    //外加发布一个事件 给前端弹框，待完成！！！

    private IConfiguration _configuration;
    private ILogService _iLogService;
    private IGolalCacheManager _golalCacheManager;
    private readonly IRepository<MesInterfaceParamSettingEntity> _mesInterfaceParamSetting;
    private readonly IRepository<MesInterfaceParamSettingDetailEntity> _mesInterfaceParamSettingDetail;
    private readonly IRepository<DictionaryEntity> _dictionaryRepository;

    private string mesType;

    private IMesService CurrentMes { get; set; }

    public MesManager(IConfiguration configuration, ILogService iLogService, IGolalCacheManager golalCacheManager, IRepository<MesInterfaceParamSettingEntity> mesInterfaceParamSetting, IRepository<MesInterfaceParamSettingDetailEntity> mesInterfaceParamSettingDetail, IRepository<DictionaryEntity> dictionaryRepository)
    {
        _configuration = configuration;
        _iLogService = iLogService;
        _golalCacheManager = golalCacheManager;
        _mesInterfaceParamSetting = mesInterfaceParamSetting;
        _mesInterfaceParamSettingDetail = mesInterfaceParamSettingDetail;
        _dictionaryRepository = dictionaryRepository;
    }

    public IMesService GetCurrentMes()
    {
        return CurrentMes;
    }

    public void InitData()
    {
        //mesType = _golalCacheManager.GlobalBasicSetting.CurrentMesType;
        mesType = "LongJing";
        if (mesType == MesTypeEnum.LongJing.ToString())
        {
            CurrentMes = new LongJingMesService(_configuration, _iLogService, _dictionaryRepository, _mesInterfaceParamSetting, _mesInterfaceParamSettingDetail, _golalCacheManager);
            return;
        }
        
    }

    public async Task<MesResponseModel> InvokeMethod(string methodName, Dictionary<string, object> data)
    {
        if (CurrentMes == null)
        {
            return MesResponseModel.Fail("失败", "500", "未找到对应MES实例");
        }
        else
        {
            if (string.IsNullOrEmpty(methodName))
            {
                return MesResponseModel.Fail("失败", "500", "methodName必传");
            }

            return await CurrentMes.InvokeMethod(methodName, data);
        }
    }
}
