using AYE_Entity;
using AYE_Interface;
using AYE_Service;
using HandyControl.Tools.Command;
using Prism.Commands;
using Prism.Ioc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace DemoModuleB.ViewModels;

public class UserControlBViewModel
{
    private readonly ISuperRepository<DictionaryEntity> _DictionaryRepository;
    public AsyncCommand<string,bool> AsyncDBCommand { get; set; }

    public UserControlBViewModel(IContainerProvider containerProvider)
    {
        _DictionaryRepository = new SuperRepository<DictionaryEntity>(containerProvider, DbType.MySql.ToString());

        //注意 这里 HandyControl.Tools.Command中的 异步命令AsyncCommand 的使用与 prism 中的命令不同，要注意看源码中的定义
        AsyncDBCommand = new AsyncCommand<string, bool>(ExecuteAsync);

        


    }


    private async Task<bool> ExecuteAsync(string op)
    {
        try
        {
            // 模拟一个异步操作
            //await Task.Delay(20);
            await _DictionaryRepository.InsertAsync(new DictionaryEntity() { DictType = "myDictType", DictLabel = "myDictLabel", DictValue = "myDictValue", Remark = "myRemark" });
            await _DictionaryRepository.InsertAsync(new DictionaryEntity() { DictType = "myDictType", DictLabel = "myDictLabel", DictValue = "myDictValue", Remark = "myRemark" });
            await _DictionaryRepository.InsertAsync(new DictionaryEntity() { DictType = "myDictType", DictLabel = "myDictLabel", DictValue = "myDictValue", Remark = "myRemark" });
            await _DictionaryRepository.InsertAsync(new DictionaryEntity() { DictType = "myDictType", DictLabel = "myDictLabel", DictValue = "myDictValue", Remark = "myRemark" });
            await _DictionaryRepository.InsertAsync(new DictionaryEntity() { DictType = "myDictType", DictLabel = "myDictLabel", DictValue = "myDictValue", Remark = "myRemark" });
            Console.WriteLine($"异步操作完成。所得参数是{op}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        return true;
    }



}
