using AYE.BaseFramework.MqttClientCore;
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

public class DemoService
{
    private readonly ISuperRepository<UserInfo001Entity> _Userrepository;//仓储测试
    private readonly ISqlSugarClient _MySqlDb;
    private readonly IContainerProvider _containerProvider;
    
    public DemoService(IContainerProvider containerProvider)
    {
        _containerProvider = containerProvider;
        _MySqlDb = containerProvider.Resolve<ISqlSugarClient>(DbType.MySql.ToString());
        _Userrepository = new SuperRepository<UserInfo001Entity>(containerProvider, DbType.MySql.ToString());
    }

    public void DemoTest()
    {
        //var v1 = db.GetSimpleClient<UserInfo001Entity>().GetFirst(x => true);
        //var v2 = _Userrepository.GetFirst(x => true);
    }

}
