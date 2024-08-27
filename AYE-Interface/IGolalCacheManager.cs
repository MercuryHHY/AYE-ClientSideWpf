using AYE_Commom.Models;
using AYE_Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Interface
{
    public interface IGolalCacheManager
    {
        List<DictionaryEntity> DictionaryEntities { get; set; }
        FtpSetting GlobalFtpSetting { get; set; }



        Task LoadAllAsync();


    }
}
