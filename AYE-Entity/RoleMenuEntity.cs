using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Entity;

[SugarTable("aye_RoleMenu", "角色关联菜单")]
public class RoleMenuEntity
{

    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; }
    public string RoleId { get; set; }
    public string MenuId { get; set; }

}
