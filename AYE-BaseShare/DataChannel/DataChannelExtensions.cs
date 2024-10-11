
using Microsoft.Extensions.DataChannel;
using System.Reflection;

namespace Microsoft.Extensions.DataChannel;



/// <summary>
/// 数据信道扩展方法
/// </summary>
public static class DataChannelExtensions
{
    public static MethodInfo DataChannelReadMethodInfo { get; } = typeof(IDataChannel).GetMethod(nameof(IDataChannel.Read))!;
    public static MethodInfo DataChannelWriteMethodInfo { get; } = typeof(IDataChannel).GetMethod(nameof(IDataChannel.Write))!;
    public static TDataChannel Open<TDataChannel>(this TDataChannel dataChannel) where TDataChannel : IDataChannel
    {
        return (TDataChannel)dataChannel.Open();
    }

    public static TDataChannel Close<TDataChannel>(this TDataChannel dataChannel) where TDataChannel : IDataChannel
    {
        return (TDataChannel)dataChannel.Open();
    }
}