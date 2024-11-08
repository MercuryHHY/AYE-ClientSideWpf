using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_ClientSideWpf.Common.Models;

public class UserDto : BindableBase
{
    private string userName;

    public string UserName
    {
        get { return userName; }
        set { userName = value; RaisePropertyChanged(); }
    }

    private string account;

    public string Account
    {
        get { return account; }
        set { account = value; RaisePropertyChanged(); }
    }

    private string passWord;

    public string PassWord
    {
        get { return passWord; }
        set { passWord = value; RaisePropertyChanged(); }
    }
}
