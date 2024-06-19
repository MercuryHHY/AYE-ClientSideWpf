using AYE.BaseFramework.SqlSusgarCore;
using AYE_Entity;
using AYE_Interface;
using Prism.Ioc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Service;

/// <summary>
/// 全局缓存管理者
/// </summary>
public class GolalCacheManager : IGolalCacheManager
{
    //private readonly ISuperRepository<UserInfo001> _Userrepository;
    private readonly IContainerProvider _containerProvider;
    public GolalCacheManager(IContainerProvider containerProvider)
    {
        _containerProvider = containerProvider;
        //bool isRegistered = _containerProvider.IsRegistered<ISuperRepository<UserInfo001>>("MySql");
        //_Userrepository = _containerProvider.Resolve<ISuperRepository<UserInfo001>>("MySql");
        var db=containerProvider.Resolve<ISqlSugarClient>("MySql");
        var v1=db.GetSimpleClient<UserInfo001>().GetFirst(x=>true);

        //_Userrepository._Db.DbMaintenance.CreateDatabase();

    }


}
