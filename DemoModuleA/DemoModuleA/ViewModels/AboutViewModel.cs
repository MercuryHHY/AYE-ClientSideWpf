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
            new LinkInfo { Title = "作者", Text = "阿烨" },
            new LinkInfo { Title = "QQ", Text = "329-4084-766" },
            new LinkInfo { Title = "微信", Text = "182-1636-0221" },
            new LinkInfo { Title = "GitHub", Content = "在这里下载源码", Text = "https://github.com/MercuryHHY/AYE-ClientSideWpf", Url = "https://github.com/MercuryHHY/AYE-ClientSideWpf", HasUrl = true },
            new LinkInfo { Title = "寄语",  Text = "保持对这个世界的热爱" }
        };

    }
}
