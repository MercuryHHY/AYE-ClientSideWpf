using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_ClientSideWpf.Common.Models;

public class LoginResponse
{
    public string Message { get; set; }

    public bool Status { get; set; }

    public object Result { get; set; }
}

public class LoginResponse<T>
{
    public string Message { get; set; }

    public bool Status { get; set; }

    public T Result { get; set; }
}