using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.NLogCore;

public interface ILogService
{

    ILoggerFactory _loggerFactory { get; set; }

}


public class AyeNLogService: ILogService
{
    public ILoggerFactory _loggerFactory { get; set; }
    public AyeNLogService()
    {
        // 配置日志工厂
        this._loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddNLog("NLog.config");
        });


    }










}
