using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AYE_Commom.Models.Mes;
public class MesCommonDataModel
{
    /// <summary>
    /// 设备类型
    /// </summary>
    //public DeviceTypeEnum EquipType { get; set; }


    /// <summary>
    /// 设备型号
    /// </summary>
    public string EquipModel { get; set; }
    /// <summary>
    /// 设备编号
    /// </summary>
    public string EquipNum { get; set; }

    /// <summary>
    /// 设备名称
    /// </summary>
    public string EquipName { get; set; }
 
    /// <summary>
    /// 授权账号
    /// </summary>
    public string VertifyName { get; set; }

    /// <summary>
    /// 授权秘钥
    /// </summary>
    public string VertifyPwd { get; set; }

    /// <summary>
    /// 生产类型
    /// </summary>
    public string ProductionType { get;set; }

    /// <summary>
    /// 其它参数
    /// </summary>

    public Dictionary<string,string> OtherParams { get; set; } = new Dictionary<string,string>();
 

}
