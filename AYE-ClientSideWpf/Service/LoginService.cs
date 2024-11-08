using AYE_ClientSideWpf.Common.Models;
using AYE_Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_ClientSideWpf;

public class LoginService : ILoginService
{
    private readonly string serviceName = "Login";

   

    async Task<(string Message, bool Status, object Result)> ILoginService.Login(UserDto user)
    {
        string Message = "OK";
        bool Status=true;
        object Result = "ok";
        await Task.Delay(10);
        return (Message, Status, Result);
    }

    async Task<(string Message, bool Status, object Result)> ILoginService.Resgiter(UserDto user)
    {
        string Message = "OK";
        bool Status = true;
        object Result = "ok";
        await Task.Delay(10);
        return (Message, Status, Result);
    }
}
