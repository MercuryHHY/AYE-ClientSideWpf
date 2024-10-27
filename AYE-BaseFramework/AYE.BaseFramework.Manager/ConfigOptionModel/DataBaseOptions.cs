using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace AYE.BaseFramework.Manager.ConfigOptionModel;

#if false
public class DataBaseOptions
{
    public bool UseCodeFirst { get; set; }
    public string DbType1 { get; set; }
    public ConnectionStrings ConnectionStringsDbType1 { get; set; }
    public string DbType2 { get; set; }
    public ConnectionStrings ConnectionStringsDbType2 { get; set; }
}
public class ConnectionStrings
{
    public string DBConnection { get; set; }
}
#endif

public class DataBaseOptions
{
    public bool UseCodeFirst { get; set; }
    public List<DatabaseConfig> Databases { get; set; } = new List<DatabaseConfig>();
}


public class DatabaseConfig
{
    public string? DbType { get; set; }
    public bool IsEnable { get; set; }
    public string? ConnectionString { get; set; }
}





public class AppSettings
{
    public string Setting1 { get; set; }
    public string Setting2 { get; set; }
}