using SqlSugar;
namespace AYE_Entity;


/// <summary>
/// MES接口详情表
/// </summary>
[SugarTable("MES_InterfaceParamSettingDetail")]
public class MesInterfaceParamSettingDetailEntity 
{
    [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
    public Guid Id { get; set; }

    public string? ParentId { get; set; }

    public string RelId { get; set; }

    public string ParamType { get; set; }

    public string ParamCode { get; set; }

    public string ParamName { get; set; }

    public string? ParamValue { get; set; }

    public string? DataType { get; set; }

    public string? Remark { get; set; }

    public Guid? CreatorId { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid? LastModifierId { get; set; }

    public DateTime? LastModificationTime { get; set; }

}
