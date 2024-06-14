using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_ClientSideWpf.Service
{

    public interface ISqlSugarService
    {
        SqlSugarClient GetClient();
    }

    public class SqlSugarService : ISqlSugarService
    {
        private readonly string _connectionString;

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




}
