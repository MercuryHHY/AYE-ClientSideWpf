using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Entity.Dtos
{
    public class DictionaryDto : BindableBase
    {
        public string? Remark { get; set; }

        public string DictType { get; set; }

        public string DictLabel { get; set; }

        public string DictValue { get; set; }

    }
}
