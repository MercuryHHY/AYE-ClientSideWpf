using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace AYE_Commom.ConfigOptionModel;

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

public class AppSettings
{
    public string Setting1 { get; set; }
    public string Setting2 { get; set; }
}