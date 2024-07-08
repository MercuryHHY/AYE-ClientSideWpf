using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETX.Service.MES.LongJing;
public class RecipeDetailModel
{   
      
    public string Version { get; set; }

    public string LastUpdateOnTime { get; set; }
    public List<RecipeDetailParamModel> ParamList { get; set; }



}
 