using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETX.Service.MES.LongJing;
public class LongJingResponse<T>
{
    public int Code { get; set; }
    public string Msg { get; set; }

    public T Data { get; set;}
}
 