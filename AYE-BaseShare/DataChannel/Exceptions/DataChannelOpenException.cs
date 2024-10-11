

namespace Microsoft.Extensions.DataChannel.Exceptions;

public class DataChannelOpenException : Exception
{
    public DataChannelOpenException(IDataChannel dataChannel, Exception? innerException) : base($"Open异常。异常：{innerException}", innerException)
    {
        this.DataChannel = dataChannel;
    }

    public IDataChannel DataChannel { get; }
}
