﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Entity;


[SugarTable("aye_UserRole", "用户关联角色")]
public class UserRoleEntity
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; }
    
    public string UserId {  get; set; }

    public string RoleId { get; set; }

}