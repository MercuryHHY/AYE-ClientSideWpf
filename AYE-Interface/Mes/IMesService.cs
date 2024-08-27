using AYE_Commom.Enum.Mes;
using AYE_Commom.Models.Mes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Interface.Mes;

public interface IMesService
{
    /// <summary>
    /// 获取MES的类型
    /// </summary>
    /// <returns></returns>
    MesTypeEnum MesType { get; set; }

    /// <summary>
    /// MES状态
    /// </summary>
    MesStatusEnum MesStatus { get; set; }

    /// <summary>
    /// MES通用设置
    /// </summary>
    MesCommonDataModel MesCommonData { get; set; }

    /// <summary>
    /// 接口配置，Key:接口名称
    /// </summary>
    Dictionary<string, MesInterfaceData> MesInterfaceDataList { get; set; }


    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    //Task<bool> Start();

    /// <summary>
    /// 断开连接并且清理相关数据
    /// </summary>
    /// <returns></returns>
    //Task<bool> Stop();

    //bool SetStatuts(MesStatusEnum status);

    //MesStatusEnum GetMesStatus();


    /// <summary>
    /// 通用接口名称
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    Task<MesResponseModel> InvokeMethod(string methodName, Dictionary<string, object> data);
    //Dictionary<string, object> GetInStationData(Dictionary<int, Dictionary<string, object>> data);
    //Dictionary<string, object> GetOutStationData(int triggerIndex, ProductInfoEntity dataInfo, List<ProductInfoDetailEntity> detailList);
    //Dictionary<string, object> GetHeartBeatData(int status);
    //Dictionary<string, object> GetDeivceStatusData(Dictionary<string, string> data, out bool isChange);
    //Dictionary<string, object> GetAlarmData(List<ProductDataSettingEntity> productDataList, bool isCancel);
    //Dictionary<string, object> GetStopReasonData(StopReasontDto data);



}
