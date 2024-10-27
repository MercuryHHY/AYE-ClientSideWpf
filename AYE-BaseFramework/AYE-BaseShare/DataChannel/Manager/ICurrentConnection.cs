

namespace Microsoft.Extensions.DataChannel.Manager;

/// <summary>
/// 当前连接配置
/// </summary>
public interface ICurrentConnection
{
    /// <summary>
    /// 获取当前连接配置
    /// </summary>
    /// <returns></returns>
    string GetCurrentCode();
}
