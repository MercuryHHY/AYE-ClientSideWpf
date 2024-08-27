using SqlSugar;
namespace AYE_Entity;

/// <summary>
/// MES接口主表
/// </summary>
[SugarTable("MES_InterfaceParamSetting")]
public class MesInterfaceParamSettingEntity 
{
    [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public string EquipType { get; set; }

    public string MesType { get; set; }

    public string InterfaceType { get; set; }

    public string InterfaceName { get; set; }

    public string? Version { get; set; }

    public string? Remark { get; set; }

    public Guid? CreatorId { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid? LastModifierId { get; set; }

    public DateTime? LastModificationTime { get; set; }
}
