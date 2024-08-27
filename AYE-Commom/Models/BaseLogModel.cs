
using SqlSugar;

namespace AYE_Commom.Models;


public class BaseLogModel 
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; protected set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
    /// <summary>
    /// 消息，必填，什么都能接受
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 用户名，进行筛选
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 服务名，必填，没有服务名直接报错
    /// </summary>
    public string ServerName { get; set; }

    /// <summary>
    /// 班次
    /// </summary>
    public string? TeamTime { get; set; }

    /// <summary>
    /// 日志创建时间
    /// </summary>
    [SplitField]
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 多租户
    /// </summary>
    public Guid TenantId { get; set; }
}