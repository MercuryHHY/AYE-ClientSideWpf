using AYE_ClientSideWpf.Common.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_ClientSideWpf;

public interface ILoginService
{
    Task<(string Message, bool Status, object Result)> Login(UserDto user);

    Task<(string Message, bool Status, object Result)> Resgiter(UserDto user);
}
