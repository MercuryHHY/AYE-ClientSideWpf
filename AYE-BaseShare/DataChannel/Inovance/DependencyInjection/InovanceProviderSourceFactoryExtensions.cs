
using Microsoft.Extensions.DataChannel.Manager;

namespace Microsoft.Extensions.DataChannel.Inovance.DependencyInjection;

public static class InovanceProviderSourceFactoryExtensions
{
    public const string ProtocolType = nameof(InovanceTcp);

    public static IProviderSourceFactory UseInovanceTcpProtocol(this IProviderSourceFactory factory, IServiceProvider service)
    {
        factory.AddProtocolFunc(ProtocolType, () => new ProviderSource());
        return factory;
    }

    private class ProviderSource: DataChannelProviderSourceBase
    {
        public override IDataChannelProvider CreateProvider()
        {
            return new Provider(this);
        }
    }
    private class Provider(InovanceProviderSourceFactoryExtensions.ProviderSource source) : DataChannelProviderBase(source)
    {
        protected override IDataChannel CreateChannel()
        {
            return new InovanceTcp(this.Source.DataChannelInfo.Name, this.Source.DataChannelInfo.ConnectionString.ToString());
        }
    }
}
