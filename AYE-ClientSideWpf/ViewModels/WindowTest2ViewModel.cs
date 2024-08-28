using AYE_ClientSideWpf.Extensions;
using MyToDo.Common;
using MyToDo.Common.Models;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_ClientSideWpf.ViewModels;

public class WindowTest2ViewModel : BindableBase, IConfigureService
{

    #region 参考
    /// <summary>
    /// Prism.Mvvm框架中 提供了这种数据通知的函数使用，其本质其实与下面的一致
    /// 这里没有删，只是提供参考
    /// </summary>
    private string _title = "Prism Application";
    public string Title
    {
        get { return _title; }
        set { SetProperty(ref _title, value); }
    }
    #endregion

    private string userName = "啊烨";

    public string UserName
    {
        get { return userName; }
        set { userName = value; RaisePropertyChanged(); }
    }

    public DelegateCommand LoginOutCommand { get; private set; }
    public DelegateCommand<MenuBar> NavigateCommand { get; private set; }
    public DelegateCommand GoBackCommand { get; private set; }
    public DelegateCommand GoForwardCommand { get; private set; }

    public DelegateCommand TestCommand { get; private set; }


    private readonly IContainerProvider containerProvider;
    private readonly IRegionManager regionManager;
    private IRegionNavigationJournal journal;

    //private readonly IConfigurationService _configurationService;

    public WindowTest2ViewModel(IContainerProvider containerProvider,
        IRegionManager regionManager)
    {
        MenuBars = new ObservableCollection<MenuBar>();

        //注入用户窗口
        NavigateCommand = new DelegateCommand<MenuBar>(Navigate);

        //向后回退
        GoBackCommand = new DelegateCommand(() =>
        {
            if (journal != null && journal.CanGoBack)
                journal.GoBack();
        });

        //向前回退
        GoForwardCommand = new DelegateCommand(() =>
        {
            if (journal != null && journal.CanGoForward)
                journal.GoForward();
        });

        //登出
        LoginOutCommand = new DelegateCommand(() =>
        {
            //注销当前用户
            //App.LoginOut(containerProvider);
        });

        //TestCommand
        //TestCommand = new DelegateCommand(() =>
        //{
        //    //测试点击卡顿问题
        //    Console.WriteLine("点击了");
        //    //MenuToggleButton

        //});

        this.containerProvider = containerProvider;
        this.regionManager = regionManager;
        //_configurationService = configurationService;
    }


    //注入用户窗口的具体执行
    private void Navigate(MenuBar obj)
    {
        if (obj == null || string.IsNullOrWhiteSpace(obj.NameSpace))
            return;

        regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj.NameSpace, back =>
        {
            //记录导航日志
            journal = back.Context.NavigationService.Journal;
        });
    }




    #region 菜单数据初始化
    private ObservableCollection<MenuBar> menuBars;
    public ObservableCollection<MenuBar> MenuBars
    {
        get { return menuBars; }
        set { menuBars = value; RaisePropertyChanged(); }
    }


    private void CreateMenuBar()
    {
        MenuBars.Add(new MenuBar() { Icon = "Home", Title = "首页", NameSpace = "IndexView" });
        //MenuBars.Add(new MenuBar() { Icon = "NotebookOutline", Title = "待办事项", NameSpace = "ToDoView" });
        //MenuBars.Add(new MenuBar() { Icon = "NotebookPlus", Title = "备忘录", NameSpace = "MemoView" });
        //MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "设置", NameSpace = "SettingsView" });
        MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "模块A", NameSpace = "UserControlA" });
        MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "模块B", NameSpace = "UserControlB" });
        MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "字典表", NameSpace = "DataGridDemo" });
        MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "关于", NameSpace = "About" });
    }
    #endregion



    /// <summary>
    /// 配置首页初始化参数
    /// </summary>
    public async Task Configure()
    {
        await Task.Delay(5);
        CreateMenuBar();

        //暂时注释   主页面
        regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("IndexView");
    }



}
