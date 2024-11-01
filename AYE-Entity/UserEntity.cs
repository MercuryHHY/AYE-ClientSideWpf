using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Entity;



[SugarTable("aye_user", "用户表")]
public class UserEntity
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; }

    public bool IsDeleted { get; set; } = false;

    public int OrderNum { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string? Email { get; set; }
    public string? CardId { get; set; }
    public string? Phone {  get; set; }
    public string? Introduction {  get; set; }

}


