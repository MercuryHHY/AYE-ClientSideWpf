

namespace Microsoft.Extensions.DataChannel.Manager;

public interface IProviderSourceFactory
{
    void AddWriterFuc(string writerStrategy, Func<IVarWriter> func);

    void AddReaderFunc(string readStrategy, Func<IVarReader> func);

    void AddProtocolFunc(string protocol, Func<DataChannelProviderSourceBase> func);

    DataChannelProviderSourceBase CreateProviderSource(string protocol);

    IVarReader CreateReader(string readStrategy);

    IVarWriter CreateWriter(string writerStrategy);

    IList<string> GetSupportprotocol();

    IList<string> GetSupportReadStrategy();

    IList<string> GetSupportWriterStrategy();
}
