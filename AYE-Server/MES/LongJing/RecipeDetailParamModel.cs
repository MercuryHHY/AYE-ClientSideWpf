﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETX.Service.MES.LongJing;
public class RecipeDetailParamModel
{   
    public string ParamCode { get; set; }
    public string ParamUpper { get; set; }
    public string ParamLower { get; set; }

    public string ParamValue { get; set;}
    public string Timestamp { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}
 