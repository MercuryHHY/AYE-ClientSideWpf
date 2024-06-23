using AYE_Entity;
using AYE_Entity.Dtos;
using AYE_Interface;
using AYE_Service;
using HandyControl.Tools.Command;
using ImTools;
using Prism.Ioc;
using Prism.Mvvm;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace DemoModuleA.ViewModels
{
    public class DataGridDemoViewModel : BindableBase
    {
        private readonly ISuperRepository<DictionaryEntity> _DictionaryRepository;
        private List<DictionaryEntity> dataList;
        public AsyncCommand<string, bool> AsyncUpCommand { get; set; }
        public List<DictionaryEntity> DataList
        {
            get { return dataList; }
            set { dataList = value; RaisePropertyChanged(); }
        }
        public DataGridDemoViewModel(IContainerProvider containerProvider)
        {
            _DictionaryRepository = new SuperRepository<DictionaryEntity>(containerProvider, DbType.MySql.ToString());
            DataList = new List<DictionaryEntity>();
            AsyncUpCommand = new AsyncCommand<string, bool>(ExecuteAsync);
           
        }

        private async Task<bool> ExecuteAsync(string arg)
        {
            DataList = await _DictionaryRepository.GetListAsync();
            return true;
        }
    }
}
