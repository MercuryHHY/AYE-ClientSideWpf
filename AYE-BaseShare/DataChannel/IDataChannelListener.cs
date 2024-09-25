
using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel;

public interface IDataChannelListener
{
    Task OpenningAsync(IDataChannel dataChannel, CancellationToken cancellationToken = default);
    Task OpenedAsync(IDataChannel dataChannel, CancellationToken cancellationToken = default);
    Task OpenExceptionAsync(IDataChannel dataChannel, Exception exception, CancellationToken cancellationToken = default);

    Task ClosingAsync(IDataChannel dataChannel, CancellationToken cancellationToken = default);
    Task ClosedAsync(IDataChannel dataChannel, CancellationToken cancellationToken = default);
    Task ClosedExceptionAsync(IDataChannel dataChannel, Exception exception, CancellationToken cancellationToken = default);

    Task ReadingAsync(IDataChannel dataChannel, VarAddress varAddress, CancellationToken cancellationToken = default);
    Task ReadedAsync(IDataChannel dataChannel, VarAddress varAddress, object[] values, CancellationToken cancellationToken = default);
    Task ReadExceptionAsync(IDataChannel dataChannel, VarAddress varAddress, Exception exception, CancellationToken cancellationToken = default);

    Task WritingAsync(IDataChannel dataChannel, VarAddress varAddress, object[] values, CancellationToken cancellationToken = default);
    Task WritedAsync(IDataChannel dataChannel, VarAddress varAddress, CancellationToken cancellationToken = default);
    Task WriteExceptionAsync(IDataChannel dataChannel, VarAddress varAddress, object[] values, Exception exception, CancellationToken cancellationToken = default);


    Task OnConnectionStatusChangedAsync(IDataChannel dataChannel, bool connectionStatus, CancellationToken cancellationToken = default);
}
