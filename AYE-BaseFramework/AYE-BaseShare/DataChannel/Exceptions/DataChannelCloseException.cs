
namespace Microsoft.Extensions.DataChannel.Exceptions;

public class DataChannelCloseException : Exception
{
    public DataChannelCloseException(IDataChannel dataChannel, Exception? innerException) : base($"Close异常。异常：{innerException}", innerException)
    {
        this.DataChannel = dataChannel;
    }

    public IDataChannel DataChannel { get; }
}
