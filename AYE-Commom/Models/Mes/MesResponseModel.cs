using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Commom.Models.Mes;

public class MesResponseModel
{
    public bool Success { get; set; }
    public string Code { get; set; }
    public string? Message { get; set; }
    public object Data { get; set; }

    public static MesResponseModel OK(object data, string code = "200", string message = "成功")
    {

        return new MesResponseModel
        {
            Code = code,
            Message = message,
            Data = data,
            Success = true,
        };
    }

    public static MesResponseModel Fail(object data, string code = "500", string message = "")
    {

        return new MesResponseModel
        {
            Code = code,
            Message = message,
            Data = data,
            Success = false,
        };
    }

}
