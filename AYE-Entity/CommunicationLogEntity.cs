using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYE_Commom.Enum.Log.Connect;
using AYE_Commom.Models;
using SqlSugar;

namespace AYE_Entity;

[SplitTable(SplitType.Day)]
//按照每天进行分表
[SugarTable("AYE_CommunicationLog_{year}{month}{day}")]
public class CommunicationLogEntity : BaseLogModel
{
    /// <summary>
    /// 连接接口名称
    /// </summary>
    public string? ApiName { get; set; }
    /// <summary>
    /// 请求参数
    /// </summary>
    [SugarColumn(Length = 8000)]
    public string? Request { get; set; }
    /// <summary>
    /// 返回结果
    /// </summary>
    [SugarColumn(Length = 4000)]
    public string? Return { get; set; }

    /// <summary>
    ///  记录原因
    /// </summary>
    [SugarColumn(Length = 4000)]
    public string? Result { get; set; }

    /// <summary>
    /// 设备编号
    /// </summary>
    public string? EquipmentNum { get; set; }

    /// <summary>
    /// 模组名称
    /// </summary>
    public string? ModuleName { get; set; }

    /// <summary>
    /// 连接状态枚举
    /// </summary>
    public ConnectionStatusEnum ConnectionStatus { get; set; }

    /// <summary>
    /// 协议状态枚举
    /// </summary>
    public ProtocolEnum ProtocolEnum { get; set; }

}
