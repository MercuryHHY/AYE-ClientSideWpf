using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace AYE_Entity;


[SugarTable("aye_dictionary","字典表")]
public class DictionaryEntity 
{

    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; }

    public bool IsDeleted { get; set; }=false;

    public int? OrderNum { get; set; }

    public bool? State {  get; set; }=true;

    public string? Remark {  get; set; }

    public string DictType {  get; set; }

    public string DictLabel {  get; set; }

    public string DictValue {  get; set; }


}


