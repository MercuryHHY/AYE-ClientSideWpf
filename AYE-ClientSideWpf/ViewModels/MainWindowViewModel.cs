using AYE_ClientSideWpf.Extensions;
using MyToDo.Common.Models;
using MyToDo.Common;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;

using SqlSugar;
using System;
using System.Linq.Expressions;
using AYE.BaseFramework.SqlSusgarCore;
using AYE_BaseFramework.ConfigurationCore;
using System.Threading.Tasks;

namespace AYE_ClientSideWpf.ViewModels
{
    public class MainWindowViewModel : BindableBase, IConfigureService
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string userName="放开那妞 让我来";

        public string UserName
        {
            get { return userName; }
            set { userName = value; RaisePropertyChanged(); }
        }

        public DelegateCommand LoginOutCommand { get; private set; }


        //private readonly IConfigurationService _configurationService;

        public MainWindowViewModel(IContainerProvider containerProvider,
            IRegionManager regionManager)
        {
            MenuBars = new ObservableCollection<MenuBar>();
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);
            GoBackCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoBack)
                    journal.GoBack();
            });
            GoForwardCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoForward)
                    journal.GoForward();
            });
            LoginOutCommand = new DelegateCommand(() =>
            {
                //注销当前用户
                //App.LoginOut(containerProvider);
            });
            this.containerProvider = containerProvider;
            this.regionManager = regionManager;
            //_configurationService = configurationService;
        }

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

        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }
        public DelegateCommand GoBackCommand { get; private set; }
        public DelegateCommand GoForwardCommand { get; private set; }

        private ObservableCollection<MenuBar> menuBars;
        private readonly IContainerProvider containerProvider;
        private readonly IRegionManager regionManager;
        private IRegionNavigationJournal journal;

        public ObservableCollection<MenuBar> MenuBars
        {
            get { return menuBars; }
            set { menuBars = value; RaisePropertyChanged(); }
        }


        void CreateMenuBar()
        {
            MenuBars.Add(new MenuBar() { Icon = "Home", Title = "首页", NameSpace = "IndexView" });
            MenuBars.Add(new MenuBar() { Icon = "NotebookOutline", Title = "待办事项", NameSpace = "ToDoView" });
            MenuBars.Add(new MenuBar() { Icon = "NotebookPlus", Title = "备忘录", NameSpace = "MemoView" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "设置", NameSpace = "SettingsView" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "模块A", NameSpace = "UserControlA" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "模块B", NameSpace = "UserControlB" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "字典表", NameSpace = "DataGridDemo" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "关于", NameSpace = "About" });
        }

        /// <summary>
        /// 配置首页初始化参数
        /// </summary>
        public async Task Configure()
        {
            await Task.Delay(5);
            CreateMenuBar();

            //暂时注释
            //regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("IndexView");
        }


      


    }
}
