using AYE.BaseFramework.MqttClientCore;
using AYE.BaseFramework.SqlSusgarCore;
using AYE_Commom.Helper;
using AYE_Commom.Models;
using AYE_Entity;
using AYE_Interface;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
    //应对多种类型DB同时使用的场景，提供以下两种方式
    //private readonly ISuperRepository<DictionaryEntity> _DictionaryRepository;//仓储测试
    //private readonly ISqlSugarClient _MySqlDb;

    private readonly IContainerProvider _containerProvider;
    private readonly ILogger<GolalCacheManager> _logger;
    private readonly IRepository<DictionaryEntity> _DictionaryRepository;
    private readonly IMqtt5ClientService _mqtt5ClientService;

    public GolalCacheManager(IContainerProvider containerProvider, ILogger<GolalCacheManager> logger, IRepository<DictionaryEntity> dictionaryRepository, IMqtt5ClientService mqtt5ClientService)
    {
        _containerProvider = containerProvider;
        //_MySqlDb = containerProvider.Resolve<ISqlSugarClient>(DbType.MySql.ToString());
        //_DictionaryRepository = new SuperRepository<DictionaryEntity>(containerProvider, DbType.MySql.ToString());

        //以下写法很麻烦，我的评价是不如直接像上面那样new 最简单省事，有些特定场景，无法使用ioc
        //_DictionaryRepository = containerProvider.Resolve<ISuperRepository<DictionaryEntity>>(DbType.MySql.ToString(),(typeof(SuperRepository<>),));

        _logger = logger;
        _DictionaryRepository = dictionaryRepository;
        _mqtt5ClientService = mqtt5ClientService;
    }


    public List<DictionaryEntity> DictionaryEntities { get; set; } = new List<DictionaryEntity>();


    public FtpSetting GlobalFtpSetting { get; set; } = new FtpSetting();

    public async Task LoadAllAsync()
    {
        DictionaryEntities = await _DictionaryRepository.GetListAsync();
        _logger.LogDebug($"数据初始化成功: {JsonConvert.SerializeObject(DictionaryEntities)}");
    }
}
