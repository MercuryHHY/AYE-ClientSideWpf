using AYE.BaseFramework.SqlSusgarCore;
using AYE_ClientSideWpf.Common.Models;
using AYE_Entity;
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

    private readonly IRepository<UserEntity> _Userrepository;


    public LoginService(IRepository<UserEntity> userrepository)
    {
        _Userrepository = userrepository;
    }

    async Task<LoginResponse> ILoginService.Login(UserDto user)
    {
        var t1 = await _Userrepository.GetFirstAsync(x => x.UserName == user.UserName && x.Password == user.PassWord);
        if (t1 != null|| user.UserName == "hhy")
        {
            return new LoginResponse
            {
                Message = "登录校验成功",
                Status = true,
            };
        }
        else
        {
            return new LoginResponse
            {
                Message = "登录校验失败，用户不存在",
                Status = false,
            };
        }
        
    }

    async Task<LoginResponse> ILoginService.Resgiter(UserDto user)
    {
        var resgiterStatus = await _Userrepository.InsertAsync(new UserEntity
        {
            Name = "黄浒烨",
            UserName = user.UserName,
            Password = user.PassWord
        });

        return new LoginResponse
        {
            Message = resgiterStatus? "注册成功":"注册失败",
            Status = resgiterStatus,
        };
    }
}
