

using Microsoft.Extensions.DataChannel.VarReaders;
using Microsoft.Extensions.DataChannel.VarWriters;

namespace Microsoft.Extensions.DataChannel.Inovance.DependencyInjection;

public static class DataChannelDependencyInjection
{
    public static IServiceCollection AddInovanceTcp(this IServiceCollection services, IConfiguration configuration, string optionName = nameof(DataChannelOptions))
    {
        DataChannelOptions options = configuration.TryGetValue<DataChannelOptions>(optionName);

        return services.AddInovanceTcp(options);
    }

    public static IServiceCollection AddInovanceTcp(this IServiceCollection services, DataChannelOptions options)
    {
        IDataChannel dataChannel = new InovanceTcp(options.Name, options.ConnectionString.ToString());
        IVarReader varReader = new VarReaderDirectly(dataChannel);
        IVarWriter varWriter = new VarWriterDirectly(options.IsRWSplitting ? new InovanceTcp(options.Name, options.ConnectionString.ToString()) : dataChannel);

        services.AddSingleton(x =>
        {
            if (options.AutoOpen)
            {
                varReader.Open();
            }
            return varReader;
        });
        services.AddSingleton(x =>
        {
            if (options.AutoOpen)
            {
                varReader.Open();
            }
            return varWriter;
        });
        return services;
    }
}
