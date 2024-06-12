using AYE_ClientSideWpf.Extensions;
using MyToDo.Common.Models;
using MyToDo.Common;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;

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

        public  MainWindowViewModel(IContainerProvider containerProvider,
            IRegionManager regionManager)
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
        public void Configure()
        {
            //这行代码似乎没有起作用
            //UserName = AppSession.UserName;

            CreateMenuBar();

            //暂时注释
            //regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("IndexView");
        }
    }
}
