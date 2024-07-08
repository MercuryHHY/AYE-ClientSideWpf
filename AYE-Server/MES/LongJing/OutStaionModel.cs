using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETX.Service.MES.LongJing;
public class OutStaionModel
{
    public string SFC { get; set; }

    public string Passed { get; set; }

    public List<OutStationParmModel> ParamList { get; set; } = new List<OutStationParmModel>();
    public List<string> BindFeedingCodes { get; set; } = new List<string>();
    public List<string> BindProductCodes { get; set; } = new List<string>();
    public List<OutStaionNgModel> NG { get; set; } = new List<OutStaionNgModel>();

}
