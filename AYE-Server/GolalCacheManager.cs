using AYE.BaseFramework.SqlSusgarCore;
using AYE_Entity;
using AYE_Interface;
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
    private readonly IRepository<UserInfo001> _Userrepository;
    public GolalCacheManager(IRepository<UserInfo001> userrepository)
    {
        _Userrepository = userrepository;

        _Userrepository._Db.DbMaintenance.CreateDatabase();

    }


}
