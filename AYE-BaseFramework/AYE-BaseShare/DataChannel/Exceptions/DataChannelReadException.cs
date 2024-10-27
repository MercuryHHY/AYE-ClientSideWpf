

using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel.Exceptions;

public class DataChannelReadException : Exception
{
    public DataChannelReadException(IDataChannel dataChannel, VarAddress varAddress, Exception? innerException) : base($"读取异常，地址：{varAddress},异常：{innerException}", innerException)
    {
        this.DataChannel = dataChannel;
        this.VarAddress = varAddress;
    }

    public IDataChannel DataChannel { get; }

    public VarAddress VarAddress { get; }
}
