using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETX.Service.MES.LongJing;
public class ProductProcessParamModel
{
    public string SFC { get; set; }


    public List<OutStationParmModel> ParamList { get; set; } = new List<OutStationParmModel>();

}
 