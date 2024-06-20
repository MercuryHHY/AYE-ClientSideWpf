using AYE.BaseFramework.SqlSusgarCore;
using AYE_Commom.Helper;
using AYE_Entity;
using AYE_Interface;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Service;

/// <summary>
/// 全局缓存管理者
/// </summary>
public class GolalCacheManager : IGolalCacheManager
{
    private readonly ISuperRepository<UserInfo001Entity> _Userrepository;//仓储测试
    public GolalCacheManager(IContainerProvider containerProvider)
    {

        //方法1 
        var db = containerProvider.Resolve<ISqlSugarClient>(DbType.MySql.ToString());
        var v1 = db.GetSimpleClient<UserInfo001Entity>().GetFirst(x => true);


        //方法2
        _Userrepository = new SuperRepository<UserInfo001Entity>(containerProvider, DbType.MySql.ToString());
        var v2 = _Userrepository.GetFirst(x => true);

    }


}
