using AYE_ClientSideWpf.Extensions;
using MyToDo.Common.Models;
using MyToDo.Common;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;

using AYE_ClientSideWpf.Service;
using AYE_BaseShare;
using static AYE_BaseShare._1_CodeFirst;
using SqlSugar;
using System;
using System.Linq.Expressions;

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
        //public IDemoInterface12 _demoInterface1;
        //private readonly ISimpleClient<UserInfo001> _repository;
        private readonly ISqlSugarService _sqlSugarService;


        public MainWindowViewModel(IContainerProvider containerProvider,
            IRegionManager regionManager,
            ISqlSugarService sqlSugarService)
        {
            MenuBars = new ObservableCollection<MenuBar>();
            //CreateMenuBar();//放在这里也可以
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
            _sqlSugarService = sqlSugarService;
            //_repository = repository ?? throw new ArgumentNullException(nameof(repository));
            //_demoInterface1 = demoInterface1;
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
        }

        /// <summary>
        /// 配置首页初始化参数
        /// </summary>
        public  void Configure()
        {
            //var v1 = _repository.GetFirst(it => it.UserName == "admin");
            //UserName = v1.UserName;

            ConfigureSqlSugar();

            CreateMenuBar();

            //暂时注释
            //regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("IndexView");
        }





        private void ConfigureSqlSugar()
        {
            var db = _sqlSugarService.GetClient();
            // 进行数据库操作
            var result = db.Queryable<UserInfo001>().Where(x => x.UserName == "admin").First();
            // 其他配置和操作
        }
    }
}
