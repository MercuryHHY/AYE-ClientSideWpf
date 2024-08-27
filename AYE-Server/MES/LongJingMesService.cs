using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using AYE.BaseFramework.SqlSusgarCore;
using AYE_Commom.Enum.Log.Connect;
using AYE_Commom.Enum.Mes;
using AYE_Commom.Models.Mes;
using AYE_Entity;
using AYE_Interface.Mes;
using AYE_Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AYE_Service.MES;

/// <summary>
/// 福建龙净
/// </summary>
public class LongJingMesService : IMesService
{
    private IConfiguration _configuration;
    private ILogService _iLogService;
    private IGolalCacheManager _golalCacheManager;
    //private INoticeService<NoticeHub> _noticeService;
    private readonly IRepository<DictionaryEntity> _dictionaryRepository;
    private readonly IRepository<MesInterfaceParamSettingEntity> _mesInterfaceParamSetting;
    private readonly IRepository<MesInterfaceParamSettingDetailEntity> _mesInterfaceParamSettingDetail;

    //public MesTypeEnum MesType { get; set; } = MesTypeEnum.LongJing;
    public MesTypeEnum MesType { get; set; }
    public MesStatusEnum MesStatus { get; set; }
    public MesCommonDataModel MesCommonData { get; set; }
    public Dictionary<string, MesInterfaceData> MesInterfaceDataList { get; set; } = new Dictionary<string, MesInterfaceData>();

    private List<string> commInterfacePara = new List<string>() { "Url", "WsPwd", "WsUser", "TimeOut", "RetryCount" };

    private string CurrentDeivceStatus = "";
    private DateTime stopTime = DateTime.Now;//停机开始时间


    //设备状态（1.运行 2.待机 3.报警 4.停机）
    // 0报警清除过程 1停 止	2启动过程 3空闲 4无	5执行中	6停止过程 7报警过程	8报警 9无 10无 11无	12无 13无 14复位过程 15无 16无 17无	18等待初始化

    private Dictionary<string, string> deviceStatus = new Dictionary<string, string>()
    {
        { "0","3" }, {"1","4"} , {"2","1"}, {"3","2"}, {"4","2"}, {"5","1"}, {"6","4"}, {"7","3"}, {"8","3"}, {"9","2"},
        {"10","2"}, {"11","2"}, {"12","2"}, {"13","2"}, {"14","1"}, {"15","2"}, {"16","2"}, {"17","2"}, {"18","1"}, {"100","2"}
    };

    /// <summary>
    /// 最新约定的设备状态编码
    /// 0.宕机    1.待料    5.堵料    10.生产
    /// </summary>
    private Dictionary<string, string> toMesDeviceStatus = new Dictionary<string, string>()
    {
        { "0","4" }, {"1","5"} , {"5","6"}, {"10","1"}
    };


    /// <summary>
    /// CSV文件头
    /// </summary>
    private string strFileKey = "产品条码,调用开始时间,调用返回时间,耗时(ms),发送信息,返回代码,返回消息,设备编号,资源编号,生产类型";

    public LongJingMesService(IConfiguration configuration,
        ILogService iLogService,
     IRepository<DictionaryEntity> dictionaryRepository,
    IRepository<MesInterfaceParamSettingEntity> mesInterfaceParamSetting,
        IRepository<MesInterfaceParamSettingDetailEntity> mesInterfaceParamSettingDetail,
        IGolalCacheManager golalCacheManager)
    {
        _configuration = configuration;
        _iLogService = iLogService;
        _dictionaryRepository = dictionaryRepository;
        _mesInterfaceParamSetting = mesInterfaceParamSetting;
        _mesInterfaceParamSettingDetail = mesInterfaceParamSettingDetail;
        _golalCacheManager = golalCacheManager;
        //InitData();
    }

    public Task<MesResponseModel> InvokeMethod(string methodName, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }


#if false

    /// <summary>
    /// 初始化获取数据
    /// </summary>
    private void InitData()
    {
        MesType = (MesTypeEnum)Enum.Parse(typeof(MesTypeEnum), _golalCacheManager.GlobalBasicSetting.CurrentMesType);
        if (_golalCacheManager.GlobalBasicSetting.CurrentMesType != MesType.ToString()) { return; }
        //获取MES基本参数
        var dictionaryList = _golalCacheManager.DictionaryEntities.Where(x => x.DictType == nameof(BasicSetting));



        MesCommonData = new MesCommonDataModel();
        MesCommonData.EquipNum = dictionaryList.FirstOrDefault(x => x.DictLabel == nameof(BasicSetting.EquipNum))?.DictValue ?? "";
        MesCommonData.EquipName = dictionaryList.FirstOrDefault(x => x.DictLabel == nameof(BasicSetting.EquipName))?.DictValue ?? "";
        string equipType = dictionaryList.FirstOrDefault(x => x.DictLabel == nameof(BasicSetting.EquipType))?.DictValue ?? "0";
        MesCommonData.EquipType = (DeviceTypeEnum)Convert.ToInt32(equipType);
        MesCommonData.ProductionType = dictionaryList.FirstOrDefault(x => x.DictLabel == nameof(BasicSetting.ProductionType))?.DictValue ?? "";

        //MES相关配置
        var dictionaryMes = _golalCacheManager.DictionaryEntities.Where(x => x.DictType == "MesSetting");
        MesCommonData.OtherParams.Add("OrderNum", dictionaryMes.FirstOrDefault(x => x.DictLabel == "OrderNum")?.DictValue ?? "");
        MesCommonData.OtherParams.Add("MesSavePath", dictionaryMes.FirstOrDefault(x => x.DictLabel == "MesSavePath")?.DictValue ?? "");

        //初始化MES状态
        string MesCurrentStatus = dictionaryList.FirstOrDefault(x => x.DictLabel == "MesCurrentStatus")?.DictValue ?? "0";
        MesStatus = (MesStatusEnum)Convert.ToInt32(MesCurrentStatus);
        //初始接口参数数据
        var mesEntityList = _mesInterfaceParamSetting.GetList(x => x.MesType == MesType.ToString() && (x.EquipType == equipType || x.EquipType == "ALL"));
        var ids = mesEntityList.Select(x => x.Id.ToString()).Distinct().ToList();
        var details = _mesInterfaceParamSettingDetail.GetList(x => ids.Contains(x.RelId));
        var codes = mesEntityList.Select(x => x.InterfaceType).Distinct().ToList();
        foreach (var item in mesEntityList)
        {
            var detail = details.Where(x => x.RelId == item.Id.ToString()).ToList();

            //获取通用配置参数
            MesInterfaceData mesInterfaceData = new MesInterfaceData
            {
                InferfaceName = item.InterfaceName,
                Url = detail.FirstOrDefault(x => x.ParamCode == "Url")?.ParamValue!,
                WsPwd = detail.FirstOrDefault(x => x.ParamCode == "WsPwd")?.ParamValue!,
                WsUser = detail.FirstOrDefault(x => x.ParamCode == "WsUser")?.ParamValue!
            };
            string timeOut = detail.FirstOrDefault(x => x.ParamCode == "TimeOut")?.ParamValue!;
            mesInterfaceData.TimeOut = string.IsNullOrEmpty(timeOut) ? 5 : Convert.ToInt32(timeOut);
            string retryCount = detail.FirstOrDefault(x => x.ParamCode == "RetryCount")?.ParamValue!;
            mesInterfaceData.RetryCount = string.IsNullOrEmpty(retryCount) ? 5 : Convert.ToInt32(retryCount);
            string isEnable = detail.FirstOrDefault(x => x.ParamCode == "IsEnable")?.ParamValue!;
            isEnable = isEnable == null ? mesInterfaceData.IsEnable.ToString() : isEnable;
            mesInterfaceData.IsEnable = !bool.TryParse(isEnable, out var result) || result;
            //获取其它配置参数
            var listOther = detail.Where(x => !commInterfacePara.Contains(x.ParamCode)).ToList();
            foreach (var other in listOther)
            {
                if (other.ParamType != "2") continue;
                mesInterfaceData.Params.Add(other.ParamCode, other.ParamValue!);
            }
            MesInterfaceDataList.Add(item.InterfaceType, mesInterfaceData);
        }

    }

    #region 通用抽象方法

    public Task<bool> Start()
    {
        throw new NotImplementedException();
    }

    public Task<bool> Stop()
    {
        throw new NotImplementedException();
    }



    public MesStatusEnum GetMesStatus()
    {
        var MesCurrentStatus = _golalCacheManager.DictionaryEntities
            .Where(x => x.DictType == nameof(BasicSetting) && x.DictLabel == "MesCurrentStatus")
            .FirstOrDefault()?.DictValue ?? "0";
        //获取MES状态
        MesStatus = (MesStatusEnum)Convert.ToInt32(MesCurrentStatus);
        return MesStatus;
    }

    public bool SetStatuts(MesStatusEnum status)
    {
        this.MesStatus = status;
        return true;
    }

    public async Task<MesResponseModel> InvokeMethod(string methodName, Dictionary<string, object> data)
    {
        var res = MesResponseModel.Fail("失败", "500", "MES接口访问失败");
        if (string.IsNullOrEmpty(methodName))
        {
            return MesResponseModel.Fail("失败", "500", "methodName必传");
        }
        else
        {

            var isSetting = MesInterfaceDataList.TryGetValue(methodName, out var basic);
            if (basic == null)//如果接口参数没有设置
            {
                return MesResponseModel.Fail("失败", "500", methodName + "接口参数没有配置");
            }
            if (!basic.IsEnable)
            {
                //return MesResponseModel.Fail("失败", "500", "MES接口未启用");
                return MesResponseModel.OK("", "200", "MES接口未启用");
            }
            int count = basic.RetryCount;
            int reExcute = 0;
        ReExcute://发生错误时重新执行
            try
            {
                //调用实例
                //  res = await InvokeMethod(methodName,data, basic);
                //反射调用实际方法
                Type t = this.GetType();
                var method = t.GetMethod(methodName);
                if (method == null)
                {
                    return MesResponseModel.Fail("未找到" + methodName + "方法", "501");
                }

                var task = method.Invoke(this, new object[] { data, basic }) as Task;
                await task;
                res = task.GetType().GetProperty("Result").GetValue(task, null) as MesResponseModel;
                if (count > 0 && res != null && res.Code == "503")
                {
                    //reExcute++;
                    //++reExcute;
                    if (reExcute < count)
                    {
                        reExcute++;
                        goto ReExcute;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = "MES接口访问失败," + ex.Message;

            }
            return res;
        }
    }

    /// <summary>
    /// 获取心跳参数
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public Dictionary<string, object> GetHeartBeatData(int status)
    {
        var res = new Dictionary<string, object>
        {
            { "IsOnline", status == 0 ? "false" : "true" }
        };
        return res;
    }

    /// <summary>
    /// 获取设备状态
    /// </summary>
    /// <param name="data"></param>
    /// <param name="isChange"></param>
    /// <returns></returns>
    public Dictionary<string, object> GetDeivceStatusData(Dictionary<string, string> data, out bool isChange)
    {
        var res = new Dictionary<string, object>();
        //var temp = deviceStatus[data["status"]];
        var temp = toMesDeviceStatus[data["status"]];
        if (CurrentDeivceStatus != temp)
        {
            isChange = true;
            CurrentDeivceStatus = temp;
            if (CurrentDeivceStatus == "4")//停机
            {
                stopTime = DateTime.Now;
            }
        }
        else
        {
            isChange = false;
        }
        res.Add("StateCode", temp);
        return res;
    }

    /// <summary>
    /// 获取报警信息
    /// </summary>
    /// <param name="productDataList"></param>
    /// <param name="isCancel">是否取消</param>
    /// <returns></returns>
    public Dictionary<string, object> GetAlarmData(List<ProductDataSettingEntity> productDataList, bool isCancel)
    {
        var res = new Dictionary<string, object>();
        List<AlarmListModel> datas = new List<AlarmListModel>();
        foreach (var item in productDataList)
        {
            string code = !string.IsNullOrEmpty(item.MesCode) ? item.MesCode : item.DataCode;
            datas.Add(new AlarmListModel()
            {
                Status = isCancel ? "0" : "1",
                AlarmCode = code,
                AlarmLevel = "H",
                AlarmMsg = item.DataName
            });
        }
        res.Add("AlarmList", JsonConvert.SerializeObject(datas));
        return res;
    }


    /// <summary>
    /// 获取MES进站参数
    /// </summary>
    /// <param name="data">key：PLC触发下标  value:采集参数</param>
    /// <returns></returns>
    public Dictionary<string, object> GetInStationData(Dictionary<int, Dictionary<string, object>> data)
    {
        var res = new Dictionary<string, object>();
        var datas = data.Values.GetItem(0);
        List<string> list = new List<string>();
        foreach (string item in datas.Keys)
        {
            if (_golalCacheManager.ProductDataSettingInStaionEntities.ContainsKey(item))
            {
                var temp = _golalCacheManager.ProductDataSettingInStaionEntities[item];
                if (!string.IsNullOrEmpty(temp.MesCode))//如果MES编码不为空，说明需要上传MES校验
                {
                    list.Add(datas[item].ToString()!);
                }
            }
        }
        res.Add("SFCs", JsonConvert.SerializeObject(list));
        res.Add("ProductionMode", "0");
        return res;
    }


    /// <summary>
    /// 获取MES出站参数
    /// </summary>
    /// <param name="triggerIndex">PLC触发下标</param>
    /// <param name="dataInfo">主表信息</param>
    /// <param name="detailList">子表信息</param>
    /// <returns></returns>
    public Dictionary<string, object> GetOutStationData(int triggerIndex, ProductInfoEntity dataInfo, List<ProductInfoDetailEntity> detailList)
    {
        var res = new Dictionary<string, object>();
        var list = new List<OutStaionModel>();
        var settings = _golalCacheManager.ProductDataSettingOutStationEntities.Values.ToList();

        //超时波终焊机
        if (MesCommonData.EquipType == DeviceTypeEnum.PairWeld)
        {
            var commonSetting = settings.Where(x => !x.Category.Contains(DataCategoryEnum.DataA.GetHashCode())
            && !x.Category.Contains(DataCategoryEnum.DataB.GetHashCode())).ToList();
            var settingLs = settings.Where(x => x.Category.Contains(DataCategoryEnum.DataA.GetHashCode())).ToList();
            //A电芯
            list.Add(GetOutStationDataOne(triggerIndex, dataInfo, detailList, settingLs, "", commonSetting));
            //B电芯
            settingLs = settings.Where(x => x.Category.Contains(DataCategoryEnum.DataB.GetHashCode())).ToList();
            string barCode = detailList.FirstOrDefault(x => x.DataCode == _golalCacheManager.MesBarCodeIds.BatteryBSettingCodeOut)?.DataValue ?? "";
            list.Add(GetOutStationDataOne(triggerIndex, dataInfo, detailList, settingLs, barCode, commonSetting));
        }
        else
        {
            list.Add(GetOutStationDataOne(triggerIndex, dataInfo, detailList, settings));
        }

        res.Add("SFCs", JsonConvert.SerializeObject(list));
        res.Add("ProductionMode", dataInfo.ProductType);
        res.Add("WorkOrderCode", dataInfo.WorkNumber);
        return res;
    }



    /// <summary>
    /// 获取停机原因
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public Dictionary<string, object> GetStopReasonData(StopReasontDto data)
    {
        var res = new Dictionary<string, object>
        {
            { "DownReasonCode", data.StopCode },
            { "BeginTime",data.StartTime+"" },
            { "EndTime", data.EndTime+"" }
        };
        return res;
    }


    private OutStaionModel GetOutStationDataOne(int triggerIndex, ProductInfoEntity dataInfo, List<ProductInfoDetailEntity> detailList,
        List<ProductDataSettingEntity> setting, string barCode = "", List<ProductDataSettingEntity> common = null)
    {
        OutStaionModel model = new();
        model.SFC = string.IsNullOrEmpty(barCode) ? dataInfo.BarCode! : barCode;
        model.Passed = dataInfo.Result == "OK" ? "1" : "0";
        //model.BindFeedingCodes = _golalCacheManager.DictionaryMeaterailEntities.Select(x => x.DictValue).ToList();
        if (!string.IsNullOrEmpty(_golalCacheManager.MesBarCodeIds.TotalResultSettingCode))
        {
            model.ParamList.Add(new OutStationParmModel()
            {
                ParamCode = _golalCacheManager.MesBarCodeIds.TotalResultSettingCode,
                ParamValue = dataInfo.Result == "OK" ? "true" : "false"
            });
        }

        //转接片的电芯极芯进站时间，代码写了一半，也不知道客户要不要这个极芯码的进站时间
        //if (!string.IsNullOrEmpty(_golalCacheManager.MesBarCodeIds.CapCodeInStationTimeMesCode))
        //{
        //    //机芯码
        //    //string str= detailList.FirstOrDefault(x => x.DataCode == _golalCacheManager.MesBarCodeIds.BatteryASettingCodeOut)?.DataValue!;
        //    //DateTime aInStationTime =await _instationCodeRepository.AsQueryable().WhereIF(str is not null, x => x.StationType==0&& x.BarCode == str).Select(x => x.CreationTime).FirstAsync();
        //    model.ParamList.Add(new OutStationParmModel()
        //    {
        //        ParamCode = _golalCacheManager.MesBarCodeIds.CapCodeInStationTimeMesCode,
        //        //ParamValue = dataInfo.InStationTime == null ? "" : dataInfo.InStationTime?.ToString("yyyy-MM-dd HH:mm:ss")!
        //        ParamValue = dataInfo.InStationTime == null ? "" : dataInfo.InStationTime?.ToString("yyyy-MM-dd HH:mm:ss")!
        //    });
        //}

        //电芯进站时间（对于转接片，这里是顶盖进站时间）
        if (!string.IsNullOrEmpty(_golalCacheManager.MesBarCodeIds.InStationTimeMesCode))
        {
            model.ParamList.Add(new OutStationParmModel()
            {
                ParamCode = _golalCacheManager.MesBarCodeIds.InStationTimeMesCode,
                ParamValue = dataInfo.InStationTime == null ? "" : dataInfo.InStationTime?.ToString("yyyy-MM-dd HH:mm:ss")!
            });
        }

        //出站时间的mes编码
        if (!string.IsNullOrEmpty(_golalCacheManager.MesBarCodeIds.OutStationTimeMesCode))
        {
            model.ParamList.Add(new OutStationParmModel()
            {
                ParamCode = _golalCacheManager.MesBarCodeIds.OutStationTimeMesCode,
                ParamValue = dataInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        //生产时间的mes编码   生产所花费的时间，和客户沟通之后，删除此采集项
        //if (!string.IsNullOrEmpty(_golalCacheManager.MesBarCodeIds.ProductTimeSettingCode))
        //{
        //    double totalSec = 999999;
        //    if (dataInfo.InStationTime != null)
        //    {
        //        //如果二者相减得到的是负数，那么数据转换之后就会出现异常（创建时间-入站时间）
        //        TimeSpan dt = (TimeSpan)(dataInfo.CreationTime - dataInfo.InStationTime);
        //        totalSec = dt.TotalSeconds >= 0 ? dt.TotalSeconds : 555555;
        //    }
        //    model.ParamList.Add(new OutStationParmModel()
        //    {
        //        ParamCode = _golalCacheManager.MesBarCodeIds.ProductTimeSettingCode,
        //        ParamValue = totalSec.ToString()
        //    });
        //}

        if (dataInfo.Result != "OK")
        {
            var ngCodeDic = _golalCacheManager.DictionaryNgCodeEntities.FirstOrDefault(x => x.DictLabel == dataInfo.NGCode);
            //string ngString = ngCodeDic != null ? ngCodeDic.Remark + "" : "NG";//暂用备注字段 需要转换客户的NGCode
            string ngString = ngCodeDic != null ? ngCodeDic.Remark + "" : "PLC写入了一个未定义的NGcode值：" + dataInfo.NGCode;//暂用备注字段 需要转换客户的NGCode
            model.NG.Add(new OutStaionNgModel()
            {
                NGCode = ngString
            });
            if (!string.IsNullOrEmpty(_golalCacheManager.MesBarCodeIds.NgSettingCode))
            {
                model.ParamList.Add(new OutStationParmModel()
                {
                    ParamCode = _golalCacheManager.MesBarCodeIds.NgSettingCode,
                    ParamValue = ngString
                });
            }
        }

        //遍历子表
        foreach (var item in detailList)
        {
            //比对交互表   
            var temp = setting.FirstOrDefault(x => x.DataCode == item.DataCode);
            if (temp != null)
            {
                //如果数据编码不为空，那么比对MES编码是否为空
                if (!string.IsNullOrEmpty(temp.MesCode))//如果MES编码不为空，说明需要上传MES校验
                {
                    string value = item.DataValue;
                    if (temp.InitDataDic?.Count > 0 && temp.InitDataDic.ContainsKey(value)) //存在预设值
                    {
                        value = SR.GetStringNative(temp.InitDataDic[value]).Value;
                    }
                    model.ParamList.Add(new OutStationParmModel()
                    {
                        ParamCode = temp.MesCode,
                        ParamValue = value
                    });
                }
                continue;
            }
            if (common == null) continue;
            temp = common.FirstOrDefault(x => x.DataCode == item.DataCode);
            if (temp != null)
            {
                if (!string.IsNullOrEmpty(temp.MesCode))//如果MES编码不为空，说明需要上传MES校验
                {
                    string value = item.DataValue;
                    if (temp.InitDataDic?.Count > 0 && temp.InitDataDic.ContainsKey(value)) //存在预设值
                    {
                        value = SR.GetStringNative(temp.InitDataDic[value]).Value;
                    }
                    model.ParamList.Add(new OutStationParmModel()
                    {
                        ParamCode = temp.MesCode,
                        ParamValue = value
                    });
                }
                continue;
            }
        }
        //foreach (var item in _golalCacheManager.DictionaryMesOhterDataEntities)
        //{
        //    if (!model.ParamList.Any(x => x.ParamCode == item.DictLabel))
        //    {
        //        model.ParamList.Add(new OutStationParmModel()
        //        {
        //            ParamCode = item.DictLabel,
        //            ParamValue = item.DictValue
        //        });
        //    }
        //}
        return model;
    }

    #endregion

    #region 客户MES方法

    /// <summary>
    /// 操作员登录接口
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> UserLogin(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;

            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = data.ContainsKey("OperatorUserID") ? data["OperatorUserID"].ToString() : "",
                OperatorPassword = data.ContainsKey("OperatorPassword") ? data["OperatorPassword"].ToString() : "",
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss")
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");

            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();


                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<object>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                        GolalCacheManager.UserCode = data.ContainsKey("OperatorUserID") ? data["OperatorUserID"].ToString() : "";
                        GolalCacheManager.UserPwd = data.ContainsKey("OperatorPassword") ? data["OperatorPassword"].ToString() : "";
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {
                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice("", starttime, endtime, usingtime, sendStr, res, "操作员登录接口", interfaceSetting);
            httpClient.Dispose();


        });
        return res;
    }

    /// <summary>
    /// 设备在线检测(PLC的状态)
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> HeartBeat(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;

            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                IsOnline = data.ContainsKey("IsOnline") ? data["IsOnline"].ToString() : "",
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");
            try
            {

                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();


                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<object>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);

                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice("", starttime, endtime, usingtime, sendStr, res, "设备在线检测", interfaceSetting);
            httpClient.Dispose();


        });
        return res;
    }

    /// <summary>
    /// 设备状态上传
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> EquipStatePost(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");

        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;

            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                StateCode = data.ContainsKey("StateCode") ? data["StateCode"].ToString() : "1",
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");

            try
            {

                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<object>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice("", starttime, endtime, usingtime, sendStr, res, "设备状态上传", interfaceSetting);
            httpClient.Dispose();


        });
        return res;

    }

    /// <summary>
    /// 报警信息上传
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> AlarmDataPost(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;
            List<AlarmListModel> datas = new List<AlarmListModel>();
            if (data.ContainsKey("AlarmList") && !string.IsNullOrEmpty(data["AlarmList"].ToString()))
            {
                datas = JsonConvert.DeserializeObject<List<AlarmListModel>>(data["AlarmList"].ToString());
                foreach (var item in datas)
                {
                    item.EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString();
                    item.ResourceCode = interfaceSetting.Params["ResourceCode"].ToString();
                }
            }
            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                AlarmList = datas,
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");


            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<object>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice("", starttime, endtime, usingtime, sendStr, res, "设备报警采集", interfaceSetting);
            httpClient.Dispose();


        });
        return res;

    }

    /// <summary>
    /// 停机原因采集
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> DownReason(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {

        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");

        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;
            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                DownReasonCode = data.ContainsKey("DownReasonCode") ? data["DownReasonCode"].ToString() : "",
                BeginTime = data.ContainsKey("BeginTime") ? data["BeginTime"].ToString() : "",
                EndTime = data.ContainsKey("EndTime") ? data["EndTime"].ToString() : "",
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");

            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<object>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice("", starttime, endtime, usingtime, sendStr, res, "设备停机采集", interfaceSetting);
            httpClient.Dispose();


        });
        return res;

    }



    /// <summary>
    /// 过程参数采集
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> ProcessParam(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;

            List<ProductProcessParamModel> sn = new List<ProductProcessParamModel>();
            if (data.ContainsKey("SFCParams") && !string.IsNullOrEmpty(data["SFCParams"].ToString()))
            {
                sn = JsonConvert.DeserializeObject<List<ProductProcessParamModel>>(data["SFCParams"].ToString());
            }
            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                SFCParams = sn,
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");


            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<List<InOutStaionResponseModel>>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice(string.Join(';', sn.Select(x => x.SFC)), starttime, endtime, usingtime, sendStr, res, "过程参数采集", interfaceSetting);
            httpClient.Dispose();


        });
        return res;
    }



    /// <summary>
    /// 进站
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> InboundMore(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;

            List<string> sn = new List<string>();
            if (data.ContainsKey("SFCs") && !string.IsNullOrEmpty(data["SFCs"].ToString()))
            {
                sn = JsonConvert.DeserializeObject<List<string>>(data["SFCs"].ToString());
            }
            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                ProductionMode = data.ContainsKey("ProductionMode") ? data["ProductionMode"].ToString() : "0",
                SFCs = sn,
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");

            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<List<InOutStaionResponseModel>>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice(string.Join(';', sn), starttime, endtime, usingtime, sendStr, res, "进站", interfaceSetting);
            httpClient.Dispose();


        });
        return res;
    }


    /// <summary>
    /// 出站
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> OutboundMore(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;

            List<OutStaionModel> sn = new List<OutStaionModel>();
            if (data.ContainsKey("SFCs") && !string.IsNullOrEmpty(data["SFCs"].ToString()))
            {
                sn = JsonConvert.DeserializeObject<List<OutStaionModel>>(data["SFCs"].ToString());
            }
            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                WorkOrderCode = data.ContainsKey("WorkOrderCode") ? data["WorkOrderCode"].ToString() : "",
                ProductionMode = data.ContainsKey("ProductionMode") ? data["ProductionMode"].ToString() : "0",
                SFCs = sn,
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");

            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<List<InOutStaionResponseModel>>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice(string.Join(';', sn.Select(x => x.SFC)), starttime, endtime, usingtime, sendStr, res, "出站", interfaceSetting);
            httpClient.Dispose();


        });
        return res;
    }


    /// <summary>
    /// 获取工单列表
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> GetWorkOrderList(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;

            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");


            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<List<WorkOrderModel>>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice("", starttime, endtime, usingtime, sendStr, res, "获取工单列表", interfaceSetting);
            httpClient.Dispose();


        });
        return res;
    }


    /// <summary>
    /// 工单激活
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> OrderActivation(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {

        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");

        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;
            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                WorkOrderCode = data.ContainsKey("WorkOrderCode") ? data["WorkOrderCode"].ToString() : "",
                Activate = data.ContainsKey("Activate") ? data["Activate"].ToString() : "",
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");


            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<object>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice("", starttime, endtime, usingtime, sendStr, res, "工单激活", interfaceSetting);
            httpClient.Dispose();


        });
        return res;

    }

    /// <summary>
    /// 获取开机参数列表
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> GetRecipeList(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;

            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                ProductCode = data.ContainsKey("ProductCode") ? data["ProductCode"].ToString() : "",
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");


            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<List<RecipeListModel>>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice("", starttime, endtime, usingtime, sendStr, res, "获取开机参数列表", interfaceSetting);
            httpClient.Dispose();


        });
        return res;
    }

    /// <summary>
    /// 获取开机参数明细
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> GetRecipe(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;

            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                RecipeCode = data.ContainsKey("RecipeCode") ? data["RecipeCode"].ToString() : "",
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");


            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<List<RecipeDetailModel>>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice("", starttime, endtime, usingtime, sendStr, res, "获取开机参数明细", interfaceSetting);
            httpClient.Dispose();


        });
        return res;
    }

    /// <summary>
    /// 开机参数版本校验
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> RecipeVersionExamine(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {

        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");

        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;
            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                RecipeCode = data.ContainsKey("RecipeCode") ? data["RecipeCode"].ToString() : "",
                Version = data.ContainsKey("Version") ? data["Version"].ToString() : "",
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");


            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<object>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice("", starttime, endtime, usingtime, sendStr, res, "开机参数版本校验", interfaceSetting);
            httpClient.Dispose();


        });
        return res;

    }

    /// <summary>
    /// 开机参数采集
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> RecipePost(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;

            List<RecipeDetailParamModel> sn = new List<RecipeDetailParamModel>();
            if (data.ContainsKey("ParamList") && !string.IsNullOrEmpty(data["ParamList"].ToString()))
            {
                sn = JsonConvert.DeserializeObject<List<RecipeDetailParamModel>>(data["ParamList"].ToString());
            }

            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                RecipeCode = data.ContainsKey("RecipeCode") ? data["RecipeCode"].ToString() : "",
                Version = data.ContainsKey("Version") ? data["Version"].ToString() : "",
                ProductCode = data.ContainsKey("ProductCode") ? data["ProductCode"].ToString() : "",
                ParamList = sn
            };


            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");


            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();


                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<object>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice("", starttime, endtime, usingtime, sendStr, res, "开机参数采集", interfaceSetting);
            httpClient.Dispose();


        });
        return res;
    }



    /// <summary>
    /// 原材料上料
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> Feeding(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;
            string sn = data.ContainsKey("SFC") ? data["SFC"].ToString() : "";
            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                SFC = sn,
                Qty = data.ContainsKey("Qty") ? data["Qty"].ToString() : "",
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");


            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<object>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice(sn, starttime, endtime, usingtime, sendStr, res, "原材料上料", interfaceSetting);
            httpClient.Dispose();


        });
        return res;

    }

    /// <summary>
    /// 卸料
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> Unloading(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;
            string sn = data.ContainsKey("SFC") ? data["SFC"].ToString() : "";
            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                SFC = sn,
                // Qty = data.ContainsKey("Qty") ? data["Qty"].ToString() : "",
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");


            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<object>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice(sn, starttime, endtime, usingtime, sendStr, res, "卸料", interfaceSetting);
            httpClient.Dispose();


        });
        return res;

    }


    /// <summary>
    /// 条码绑定
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> BindSFC(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;
            string sn = data.ContainsKey("SFC") ? data["SFC"].ToString() : "";
            List<string> BindSFCs = new List<string>();
            if (data.ContainsKey("BindSFCs") && !string.IsNullOrEmpty(data["BindSFCs"].ToString()))
            {
                BindSFCs = JsonConvert.DeserializeObject<List<string>>(data["BindSFCs"].ToString());
            }
            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                SFC = sn,
                BindSFCs,
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");

            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<object>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice(sn, starttime, endtime, usingtime, sendStr, res, "条码绑定", interfaceSetting);
            httpClient.Dispose();


        });
        return res;

    }

    /// <summary>
    /// 条码解绑
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> UnBindSFC(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;
            string sn = data.ContainsKey("SFC") ? data["SFC"].ToString() : "";
            List<string> BindSFCs = new List<string>();
            if (data.ContainsKey("BindSFCs") && !string.IsNullOrEmpty(data["BindSFCs"].ToString()))
            {
                BindSFCs = JsonConvert.DeserializeObject<List<string>>(data["BindSFCs"].ToString());
            }
            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                SFC = sn,
                BindSFCs,
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");

            try
            {

                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<object>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice(sn, starttime, endtime, usingtime, sendStr, res, "条码解绑", interfaceSetting);
            httpClient.Dispose();


        });
        return res;

    }

    /// <summary>
    /// 剩余物料查询
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> RemainMaterialQuery(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;

            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");

            try
            {

                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    // 将返回的报文 反序列化为 LongJingResponse<T>类型的resObj， 且这个T 指定为 List<RemainMaterialModel>
                    // 注意，这里是将 resObj 作为 res的 data类型（object）传入
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<List<RemainMaterialModel>>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice("", starttime, endtime, usingtime, sendStr, res, "剩余物料查询", interfaceSetting);
            httpClient.Dispose();


        });
        return res;

    }

    /// <summary>
    /// 进站前获取这个条码的类型（A,B码）
    /// </summary>
    /// <param name="data"></param>
    /// <param name="interfaceSetting"></param>
    /// <returns></returns>
    public async Task<MesResponseModel> InboundInGetData(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;

            List<string> sn = new List<string>();

            if (data.ContainsKey("SFCs") && !string.IsNullOrEmpty(data["SFCs"].ToString()))
            {
                //sn.Add(JsonConvert.DeserializeObject<string>(data["SFCs"].ToString()));
                sn = JsonConvert.DeserializeObject<List<string>>(data["SFCs"].ToString());
            }
            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                //ProductionMode = data.ContainsKey("ProductionMode") ? data["ProductionMode"].ToString() : "0",
                SFC = sn[0],
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");

            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();
                //string responseBody = "{\"Code\":1,\"Msg\":\"获取数据成功\",\"Data\":{\"ParamList\":[{\"ParamCode\":\"attribute\",\"ParamValue\":\"B\"}],\"Code\":null,\"Msg\":null}}";

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<InboundInGetDataModel>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj.Data, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {

                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice(string.Join(';', sn), starttime, endtime, usingtime, sendStr, res, "进站判定条码类型", interfaceSetting);
            httpClient.Dispose();


        });
        return res;

    }


    public async Task<MesResponseModel> UploadPPM(Dictionary<string, object> data, MesInterfaceData interfaceSetting)
    {
        MesResponseModel res = MesResponseModel.Fail("错误", "501", "内部错误");
        await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(interfaceSetting.Url))
            {
                res = MesResponseModel.Fail("错误", "503", "Url不能为空");
            }
            DateTime starttime = DateTime.Now;
            var postData = new
            {
                EquUserID = interfaceSetting.WsUser,
                EquPassword = interfaceSetting.WsPwd,
                EquipmentCode = interfaceSetting.Params["EquipmentCode"].ToString(),
                ResourceCode = interfaceSetting.Params["ResourceCode"].ToString(),
                OperatorUserID = GolalCacheManager.UserCode,
                OperatorPassword = GolalCacheManager.UserPwd,
                LocalTime = starttime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                PPM = data.ContainsKey("PPM") ? data["PPM"].ToString() : "0"
            };

            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(interfaceSetting.TimeOut)
            };
            string sendStr = JsonConvert.SerializeObject(postData);
            var content = new StringContent(sendStr, Encoding.UTF8, "application/json");

            try
            {
                //// 发送Post请求
                HttpResponseMessage response = await httpClient.PostAsync(interfaceSetting.Url, content);

                ////获取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();
                //string responseBody = "{\"Code\":1,\"Msg\":\"成功\"}";

                try
                {
                    var resObj = JsonConvert.DeserializeObject<LongJingResponse<object>>(responseBody);
                    if (resObj?.Code == 1)
                    {
                        res = MesResponseModel.OK(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                    else
                    {
                        res = MesResponseModel.Fail(resObj, resObj?.Code.ToString(), resObj.Msg);
                    }
                }
                catch (Exception ex)
                {
                    res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "反序列化解析报错，mes的回复报文是：" + responseBody);
                }
            }
            catch (Exception ex)
            {
                res = MesResponseModel.Fail(ex, "503", ex.Message + ";" + "注意mes回复是否超时");
            }
            DateTime endtime = DateTime.Now;
            int usingtime = (int)(endtime - starttime).TotalMilliseconds;
            SendLogAndNotice(null, starttime, endtime, usingtime, sendStr, res, "上传PPM", interfaceSetting);
            httpClient.Dispose();


        });
        return res;


    }


    #endregion


    /// <summary>
    /// 保存MES日志到数据库以及本地MES接口文档 ，本地文档导出失败则消息推送
    /// </summary>
    /// <param name="identification">产品条码</param>
    /// <param name="startTime">接口调用开始时间</param>
    /// <param name="returnTime">接口返回时间</param>
    /// <param name="usedTime">接口调用用时</param>
    /// <param name="sendStr">发送数据</param>
    /// <param name="res">返回数据</param>
    /// <param name="interfaceName">接口名称</param>
    private void SendLogAndNotice(string identification,
        DateTime startTime, DateTime returnTime,
        int usedTime, string sendStr, MesResponseModel res,
        string interfaceName, MesInterfaceData interfaceSetting)
    {
        Task.Run(async () =>
        {
            //失败和成功都写文件，注意为了写入CSV文件，所以在每一次Append文本末追加内容时，都会添加英文逗号
            StringBuilder sb = new StringBuilder();

            identification = "\"" + identification + "\"";
            //不固定关键信息
            sb.Append($"{identification},");

            //固定时间项
            sb.Append($"{startTime.ToString("HH:mm:ss:fff")},");
            sb.Append($"{returnTime.ToString("HH:mm:ss:fff")},");
            sb.Append($"{usedTime},");
            //发送信息
            sb.Append($"{sendStr.Replace(',', ';')},");


            //返回信息
            sb.Append($"{res.Code},");

            //在这里要做出判断，如果res.code=="503" 那么在写入文件时要将中文提示写入
            //string msg=JsonConvert.SerializeObject (res.Data);
            string msg = res.Code == "503" ? JsonConvert.SerializeObject(res.Message) : JsonConvert.SerializeObject(res.Data);

            if (!string.IsNullOrEmpty(msg))
            {
                sb.Append($"{msg.Replace(',', ';')},");
            }
            else
            {
                sb.Append($"{msg},");
            }
            //sb.Append($"{res.Message.Replace(',', ';')},");

            //资源编号,服务器名称,生产类型
            sb.Append($"{interfaceSetting.Params["EquipmentCode"].ToString()},");
            sb.Append($"{interfaceSetting.Params["ResourceCode"].ToString()},");
            sb.Append($"{MesCommonData.ProductionType}");
            try
            {
                ETX.Common.Helper.CsvHelper.CsvWriteFile(strFileKey, sb.ToString(), $"{MesCommonData.OtherParams["MesSavePath"]}\\{interfaceName}",
                DateTime.Now.ToString("yyyy-MM-dd") + "-" + interfaceName + ".csv", true);
            }
            catch (Exception ex)
            {
                //文本写入失败，推送到前端提示
                var errMsg = new MESLogModel()
                {
                    CreateTime = DateTime.Now,
                    Message = ex.Message
                };
                _noticeService.OnAsync(new HubCallerModel<MESLogModel>().WithCategory(ComponentModelEnum.MESLog.ToString()).WithMessage(errMsg));
            }

            //接口调用结果推送到前端提示，只是这段代码没啥用，在前端似乎有BUG
            var mesLogModel = new MESLogModel()
            {
                CreateTime = DateTime.Now,
                Message = "接口调用：" + interfaceName + ",调用结果：" + (res.Success ? "成功" : "失败")
            };
            _noticeService.OnAsync(new HubCallerModel<MESLogModel>().WithCategory(ComponentModelEnum.MESLog.ToString()).WithMessage(mesLogModel));



            //通过仓储保存MES日志到数据库
            ConnectLogCreateInput connectLogCreateInput = new ConnectLogCreateInput()
            {
                ApiName = interfaceName,
                Request = sendStr,
                Return = "",
                Result = JsonConvert.SerializeObject(res),
                EquipmentNum = MesCommonData.EquipName,
                ConnectionStatus = ConnectionStatusEnum.Connected,
                //Remark ="",
                Message = msg,
                ServerName = _golalCacheManager.ServerName
            };
            _iLogService.ConnectCreateAsync(connectLogCreateInput);

        });

    }
#endif

}




