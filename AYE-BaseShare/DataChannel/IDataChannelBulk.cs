

using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel;

/// <summary>
/// 批量读写数据通道
/// </summary>
public interface IDataChannelBulk
{
    /// <summary>
    /// 批量读数据
    /// </summary>
    /// <param name="addresses">地址集合</param>
    /// <returns>数据集合</returns>
    object[] Read(IEnumerable<VarAddress> addresses);

    /// <summary>
    /// 批量写数据
    /// </summary>
    /// <param name="addresses">数据地址集合</param>
    void Write(IDictionary<VarAddress, object> addresses);
}