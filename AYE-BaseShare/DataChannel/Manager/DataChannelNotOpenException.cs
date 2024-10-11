

namespace Microsoft.Extensions.DataChannel.Manager;

public sealed class DataChannelNotOpenException(string channelCode) : Exception($"编码为{channelCode}的连接通道未打开。")
{
    public string ChannelCode { get; } = ThrowHelper.IfNullOrWhiteSpace(channelCode);
}