using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BackManageModudle.ViewModels;
using BackManageModudle.Views;
using Prism.Ioc;
using Prism.Modularity;
namespace BackManageModudle;

public class BackManageModudle : IModule
{
   
    /// <summary>
    /// 注册
    /// </summary>
    /// <param name="containerRegistry"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {

        containerRegistry.RegisterForNavigation<BackManage, BackManageViewModel>();

    }




    /// <summary>
    /// 模块化注册结束，最后的在这里进行一系列的数据初始化操作
    /// </summary>
    /// <param name="containerProvider"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnInitialized(IContainerProvider containerProvider)
    {
        
    }
}

