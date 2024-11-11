using AYE_ClientSideWpf.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AYE_ClientSideWpf.Common.Models;

public class ResgiterUserDto : BaseDto, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string userName;

    public string UserName
    {
        get { return userName; }
        set { userName = value; OnPropertyChanged(); }
    }

    private string account;

    public string Account
    {
        get { return account; }
        set { account = value; OnPropertyChanged(); }
    }

    private string passWord;

    public string PassWord
    {
        get { return passWord; }
        set { passWord = value; OnPropertyChanged(); }
    }

    private string newpassWord;

    public string NewPassWord
    {
        get { return newpassWord; }
        set { newpassWord = value; OnPropertyChanged(); }
    }
}