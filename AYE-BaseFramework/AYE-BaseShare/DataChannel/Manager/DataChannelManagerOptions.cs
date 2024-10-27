
namespace Microsoft.Extensions.DataChannel.Manager;

public class DataChannelManagerOptions : IOptions<DataChannelManagerOptions>
{
    /// <summary>
    /// 数据通道连接配置
    /// </summary>

    /// <summary>
    /// 支持扩展自定义协议方法
    /// <![CDATA[
    /// ProviderSourceFactoryAction = (IServiceProvider x,IProviderSourceFactory y) =>
    /// {
    ///     y.AddProtocolFunc(nameof(TmSiemensS7), () => new TmSiemensS7ProviderSource();
    /// };
    /// ]]>
    /// </summary>
    [JsonIgnore]
    public Action<IServiceProvider, IProviderSourceFactory> ProviderSourceFactoryAction { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonIgnore]
    public Action<IServiceProvider, IConnectionMonitor> ConnectionMonitorAction { get; set; }

    DataChannelManagerOptions IOptions<DataChannelManagerOptions>.Value => this;
}
