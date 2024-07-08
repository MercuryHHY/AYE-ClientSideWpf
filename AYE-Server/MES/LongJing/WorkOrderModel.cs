using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETX.Service.MES.LongJing;
public class WorkOrderModel
{ 
    public string WorkOrderCode { get; set; }

    public string ProductCode { get; set; }
    public string ProductName { get; set; }

    public string Quantity { get; set;}

    public string StartTime { get; set; }
    public string EndTime { get; set; }

}
 