using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.GolalUiCore.ViewModels;


/// <summary>
/// 以下代码中 与弹框激活者的参数相互传递 只是一个演示，目前没有这个应用场景，只是确定可以这么玩
/// </summary>
public class MsgViewModel : IDialogAware
{

    public DelegateCommand<string> SaveCommand { get; set; }
    public DelegateCommand<string> CancelCommand { get; set; }

    public MsgViewModel()
    {
        SaveCommand = new DelegateCommand<string>(Save);
        CancelCommand = new DelegateCommand<string>(Cancel);
    }

    private void Cancel(string obj)
    {
        ///这里只是发布事件，至于谁去处理，怎么处理，这里不管
        //_eventAggregator.GetEvent<MessageEvent>().Publish("hello,publish");

        ///这里是将 弹框提供的参数，返回给 弹框的激活者
        RequestClose?.Invoke(new DialogResult(ButtonResult.No));
    }

    private void Save(string obj)
    {
        ///这里是将 弹框提供的参数，返回给 弹框的激活者
        DialogParameters keyValuePairs = new DialogParameters();
        keyValuePairs.Add("Value", "Hello LeiYun");
        RequestClose?.Invoke(new DialogResult(ButtonResult.Yes, keyValuePairs));

    }




    public string? Title { set; get; }
    public string? Content { set; get; }

    public event Action<IDialogResult>? RequestClose;

    public bool CanCloseDialog()
    {
        return true;
    }


    /// <summary>
    /// 在 Dialog 关闭时，最后的自定义动作放在这里
    /// 发布特定的事件之类的操作
    /// </summary>
    public void OnDialogClosed()
    {
        
    }


    /// <summary>
    /// 在 Dialog 打开时与其通信，传递参数
    /// </summary>
    /// <param name="parameters"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnDialogOpened(IDialogParameters parameters)
    {
        ///拿到弹框激活者 给的参数
        Title = parameters.GetValue<string>("Title");
        Content = parameters.GetValue<string>("Content");
        
    }
}
