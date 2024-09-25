
namespace Microsoft.Extensions.DataChannel.ComponentModel;

/// <summary>
/// 连接状态改变事件参数
/// </summary>
public class ConnectionStatusChangedEventArgs : EventArgs
{
    public ConnectionStatusChangedEventArgs(bool isConnected)
    {
        this.IsConnected = isConnected;
    }

    /// <summary>
    /// 是否已连接
    /// </summary>
    public bool IsConnected { get; }
}
