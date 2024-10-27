using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_BaseShare;

/// <summary>
/// 此文件作废，仅供参考
/// </summary>


public interface ISqlSugarService
{
    

    //ISqlSugarClient GetSqlSugarClient();
    SqlSugarClient GetClient();

    //SimpleClient<T> GetSimpleClient();

    //Task<ISqlSugarClient> GetSimpleClient();

    
}

public class SqlSugarService : ISqlSugarService
{
    private readonly string _connectionString;
    public ISqlSugarClient sqlSugarClient;

    public SqlSugarService(ISqlSugarClient sqlSugarClient)
    {
        this.sqlSugarClient = sqlSugarClient;
    }

    public SqlSugarService(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

   
    public SqlSugarClient GetClient()
    {

        return new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = _connectionString,
            DbType = DbType.MySql,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });
    }

    
}
