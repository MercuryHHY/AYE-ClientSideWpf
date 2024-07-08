using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETX.Service.MES.LongJing;
public class AlarmListModel
{
    public string EquipmentCode { get; set; }

    public string ResourceCode { get; set; }

    public string Status { get; set; }
    public string AlarmMsg { get; set; }
    public string AlarmCode { get; set; }

    public string AlarmLevel { get; set; }
}
 