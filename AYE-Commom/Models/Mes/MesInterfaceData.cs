using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Commom.Models.Mes;
public class MesInterfaceData
{
    /// <summary>
    /// 接口名称
    /// </summary>
    public string InferfaceName { get; set; }
    /// <summary>
    /// 接口Url路径
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// 用户名
    /// </summary>
    public string WsUser { get; set; }
    /// <summary>
    /// 用户密码
    /// </summary>
    public string WsPwd { get; set; }
    /// <summary>
    /// 超时时间，单位秒，默认5秒
    /// </summary>
    public int TimeOut { get; set; } = 5;
    /// <summary>
    /// 接口重试次数，默认0
    /// </summary>
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// 此接口是否启动，默认true启用
    /// </summary>
    public bool IsEnable { get; set; } = true;


    /// <summary>
    /// 其它参数
    /// </summary>
    public Dictionary<string, string> Params=new Dictionary<string, string>();
}
