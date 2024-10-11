

using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel.Manager;

public interface IDataChannelManager
{
    IDataChannelProvider? FindChannelProvider(string channelCode);

    IReadOnlyCollection<IDataChannelProvider> GetChannelProviders();

    bool IsOpen(string channelCode);

    bool Open(DataChannelInfo info);

    bool Close(string channelCode);

    VarValue Read(string channelCode, VarAddress varAddress);

    VarValue<TValue> Read<TValue>(string channelCode, VarAddress varAddress);

    VarValue[] Read(string channelCode, IEnumerable<VarAddress> addresss);

    void Write(string channelCode, VarAddress varAddress, object value);

    void Write(string channelCode, IDictionary<VarAddress, object> addresses);

    bool IsConnected(string channelCode);
}
