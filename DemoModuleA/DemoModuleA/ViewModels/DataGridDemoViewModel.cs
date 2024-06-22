using AYE_Entity.Dtos;
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

        private ObservableCollection<DictionaryDto> dataList;

        public ObservableCollection<DictionaryDto> DataList
        {
            get { return dataList; }
            set { dataList = value; RaisePropertyChanged(); }
        }
        public DataGridDemoViewModel()
        {
            DataList=new ObservableCollection<DictionaryDto>();
            DataList.Add(new DictionaryDto() { DictType = "myDictType" , DictLabel = "myDictLabel" , DictValue = "myDictValue", Remark = "myRemark" });

        }
    }
}
