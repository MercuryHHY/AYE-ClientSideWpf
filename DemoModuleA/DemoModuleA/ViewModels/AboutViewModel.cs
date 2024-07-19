using DemoModuleA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoModuleA.ViewModels;

public class AboutViewModel
{
    public ObservableCollection<LinkInfo> links { set; get; }

    public AboutViewModel()
    {
        links = new ObservableCollection<LinkInfo>
        {
            new LinkInfo { Title = "作者QQ", Text = "961501261" },
            new LinkInfo { Title = "QQ群", Text = "746533921" },
            new LinkInfo { Title = "GitHub", Content = "在这里下载源码", Text = "https://github.com/Monika1313/Wu.CommTool", Url = "https://github.com/Monika1313/Wu.CommTool", HasUrl = true },
            new LinkInfo { Title = "Gitee", Content = "在这里下载源码", Text = "https://gitee.com/Monika551/Wu.CommTool", Url = "https://gitee.com/Monika551/Wu.CommTool", HasUrl = true }
        };

    }
}
