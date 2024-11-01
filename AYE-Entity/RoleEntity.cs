using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Entity;

[SugarTable("aye_role", "角色表")]
public class RoleEntity
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; }

    public bool IsDeleted { get; set; } = false;

    public int OrderNum { get; set; }
    public string RoleName { get; set; }
    public string RoleCode { get; set; }
    public string Remark { get; set; }
    
}
