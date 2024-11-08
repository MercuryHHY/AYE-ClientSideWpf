using AYE_ClientSideWpf.Common.Models;
using AYE_ClientSideWpf.Extensions;
using AYE_Interface;
using AYE_Service;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace AYE_ClientSideWpf.ViewModels;

public class LoginViewModel : BindableBase, IDialogAware
{
    private readonly ILoginService loginService;
    private readonly ILogger<LoginViewModel> _logger;
    private readonly IEventAggregator aggregator;
    public LoginViewModel(IEventAggregator aggregator, ILogger<LoginViewModel> logger, ILoginService loginService)
    {
        UserDto = new ResgiterUserDto();
        ExecuteCommand = new DelegateCommand<string>(Execute);
        this.aggregator = aggregator;
        _logger = logger;
        this.loginService = loginService;
    }

    public string Title { get; set; } = "ToDo";

    public event Action<IDialogResult> RequestClose;

    public bool CanCloseDialog()
    {
        return true;
    }

    public void OnDialogClosed()
    {
        LoginOut();
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
    }

    #region Login

    private int selectIndex;
    public int SelectIndex
    {
        get { return selectIndex; }
        set { selectIndex = value; RaisePropertyChanged(); }
    }


    public DelegateCommand<string> ExecuteCommand { get; private set; }


    private string userName;
    public string UserName
    {
        get { return userName; }
        set { userName = value; RaisePropertyChanged(); }
    }

    private string passWord;
    public string PassWord
    {
        get { return passWord; }
        set { passWord = value; RaisePropertyChanged(); }
    }

    private void Execute(string obj)
    {
        switch (obj)
        {
            case "Login": Login(); break;
            case "LoginOut": LoginOut(); break;
            case "Resgiter": Resgiter(); break;
            case "ResgiterPage": SelectIndex = 1; break;
            case "Return": SelectIndex = 0; break;
        }
    }

    private ResgiterUserDto userDto;
    public ResgiterUserDto UserDto
    {
        get { return userDto; }
        set { userDto = value; RaisePropertyChanged(); }
    }

    async void Login()
    {
        if (string.IsNullOrWhiteSpace(UserName) ||
            string.IsNullOrWhiteSpace(PassWord))
        {
            return;
        }

        var loginResult = await loginService.Login(new UserDto()
        {
            Account = UserName,
            PassWord = PassWord
        });

        if (loginResult.Status)
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }
        else
        {
            //登录失败提示...
            aggregator.SendMessage(loginResult.Message, "Login");
        }
    }

    private async void Resgiter()
    {
        if (string.IsNullOrWhiteSpace(UserDto.Account) ||
            string.IsNullOrWhiteSpace(UserDto.UserName) ||
            string.IsNullOrWhiteSpace(UserDto.PassWord) ||
            string.IsNullOrWhiteSpace(UserDto.NewPassWord))
        {
            aggregator.SendMessage("请输入完整的注册信息！", "Login");
            return;
        }

        if (UserDto.PassWord != UserDto.NewPassWord)
        {
            aggregator.SendMessage("密码不一致,请重新输入！", "Login");
            return;
        }

        var resgiterResult = await loginService.Resgiter(new UserDto()
        {
            Account = UserDto.Account,
            UserName = UserDto.UserName,
            PassWord = UserDto.PassWord
        });

        if (resgiterResult.Status)
        {
            aggregator.SendMessage("注册成功", "Login");
            //注册成功,返回登录页页面
            SelectIndex = 0;
        }
        else
            aggregator.SendMessage(resgiterResult.Message, "Login");
    }

    void LoginOut()
    {
        RequestClose?.Invoke(new DialogResult(ButtonResult.No));
    }

    #endregion
}
