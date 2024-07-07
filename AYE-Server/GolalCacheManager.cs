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
    private readonly ISuperRepository<DictionaryEntity> _DictionaryRepository;//仓储测试
    private readonly ISqlSugarClient _MySqlDb;
    private readonly IContainerProvider _containerProvider;
    private readonly ILogger<GolalCacheManager> _logger;
    public GolalCacheManager(IContainerProvider containerProvider, ILogger<GolalCacheManager> logger)
    {
        _containerProvider = containerProvider;
        _MySqlDb = containerProvider.Resolve<ISqlSugarClient>(DbType.MySql.ToString());
        _DictionaryRepository = new SuperRepository<DictionaryEntity>(containerProvider, DbType.MySql.ToString());
        _logger = logger;
    }


    public List<DictionaryEntity> DictionaryEntities { get; set; } = new List<DictionaryEntity>();


    public FtpSetting GlobalFtpSetting { get; set; } = new FtpSetting();

    public async Task LoadAllAsync()
    {
        DictionaryEntities = await _DictionaryRepository.GetListAsync();
        _logger.LogDebug($"数据初始化成功: {JsonConvert.SerializeObject(DictionaryEntities)}");
    }
}
