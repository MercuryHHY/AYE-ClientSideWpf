using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Entity;

[SugarTable("aye_menu", "菜单表")]
public class MenuEntity
{

    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int OrderNum { get; set; }
    public string? MenuName { get; set; }
    public string? MenuType { get; set; }
    public string? PermissionCode { get; set; }
    public string? ParentId { get; set; }
    public string? MenuIcon { get; set; }
    public string? Router { get; set; }
    public string? Remark { get; set; }

}
