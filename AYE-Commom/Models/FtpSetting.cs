using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Commom.Models;
public class FtpSetting
{
    public string FtpServerAddress { get; set; }
    public string FtpPort { get; set; }
    public string FtpUser { get; set; }
    public string FtpPassword { get; set; }
    public bool FtpEnable { get; set; }
    public string FtpServerProductDataSavePath { get; set; }
    public string FtpServerEquipmentLogSavePath { get; set; }

    public DateTime FtpUploadTimeFlag { get; set; }
}