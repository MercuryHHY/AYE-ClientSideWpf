using EasyTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Commom.Enum.Log.Connect;

/// <summary>
/// [DictionaryType("通信方式类型")]
/// </summary>
public enum ProtocolEnum
{
    [Description("WebService")]
    WebService,
    [Description("Modbus")]
    Modbus,
    [Description("USB")]
    USB,
    [Description("TCP_IP")]
    TCP_IP,
    [Description("HTTP")]
    HTTP,
    [Description("UDP")]
    UDP,
    [Description("OpcUa")]
    OpcUa,
    [Description("S7")]
    S7,
    [Description("Sim")]
    Sim
}
