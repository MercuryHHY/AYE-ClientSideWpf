

using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel.Exceptions;

public class DataChannelWriteException : Exception
{
    public DataChannelWriteException(IDataChannel dataChannel, VarAddress varAddress, Exception? innerException) : base($"写入异常，地址：{varAddress},异常：{innerException}", innerException)
    {
        this.DataChannel = dataChannel;
        this.VarAddress = varAddress;
    }

    public IDataChannel DataChannel { get; }

    public VarAddress VarAddress { get; }
}
