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

    public string Title { get; set; } = "AYE";

    public event Action<IDialogResult> RequestClose;


    /// <summary>
    /// 这里的返回值决定 Dialog 是否能被关闭
    /// </summary>
    /// <returns></returns>
    public bool CanCloseDialog()
    {
        return true;
    }


    /// <summary>
    /// 在 Dialog 关闭时，最后的自定义动作放在这里
    /// </summary>
    public void OnDialogClosed()
    {
        //登出
        LoginOut();
    }


    /// <summary>
    /// 在 Dialog 打开时与其通信，传递参数
    /// </summary>
    /// <param name="parameters"></param>
    public void OnDialogOpened(IDialogParameters parameters)
    {
        ///拿到弹框激活者（MainWindowViewModel） 给的参数
        Title = parameters.GetValue<string>("Title");

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


    /// <summary>
    /// 用户登录
    /// </summary>
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
            // 这里只是发布，至于谁去处理，怎么处理，这里不管
            aggregator.SendMessage(loginResult.Message, "Login");
        }
    }


    /// <summary>
    /// 用户注册
    /// </summary>
    private async void Resgiter()
    {
        //注册时，数据必须齐全
        if (string.IsNullOrWhiteSpace(UserDto.Account) ||
            string.IsNullOrWhiteSpace(UserDto.UserName) ||
            string.IsNullOrWhiteSpace(UserDto.PassWord) ||
            string.IsNullOrWhiteSpace(UserDto.NewPassWord))
        {
            aggregator.SendMessage("请输入完整的注册信息！", "Login");
            return;
        }


        // 注册时，两次密码不一致
        if (UserDto.PassWord != UserDto.NewPassWord)
        {
            aggregator.SendMessage("密码不一致,请重新输入！", "Login");
            return;
        }


        // 数据注册
        var resgiterResult = await loginService.Resgiter(new UserDto()
        {
            Account = UserDto.Account,
            UserName = UserDto.UserName,
            PassWord = UserDto.PassWord
        });

        //根据校验结果，执行不同的发布
        if (resgiterResult.Status)
        {
            aggregator.SendMessage("注册成功", "Login");
            //注册成功,返回登录页页面
            SelectIndex = 0;
        }
        else
            aggregator.SendMessage(resgiterResult.Message, "Login");
    }


    /// <summary>
    /// 登出，返回给激活者 一个ButtonResult.No
    /// </summary>
    void LoginOut()
    {
        RequestClose?.Invoke(new DialogResult(ButtonResult.No));
    }

    #endregion
}
