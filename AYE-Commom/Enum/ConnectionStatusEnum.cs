using EasyTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Commom.Enum.Log.Connect;

/// <summary>
/// [DictionaryType("连接类型类型")]
/// </summary>
public enum ConnectionStatusEnum
{
    [Description("未连接")]
    NotConnected,
    [Description("已连接")]
    Connected,
    [Description("连接中断")]
    Disconnected,
    [Description("连接失败")]
    Failed
}
