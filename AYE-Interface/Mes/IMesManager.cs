using AYE_Commom.Models.Mes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Interface.Mes;

public interface IMesManager
{
    IMesService GetCurrentMes();
    void InitData();
    Task<MesResponseModel> InvokeMethod(string methodName, Dictionary<string, object> data);

}
