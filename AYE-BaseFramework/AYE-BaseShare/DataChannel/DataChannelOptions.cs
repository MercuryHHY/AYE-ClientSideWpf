


namespace Microsoft.Extensions.DataChannel;

public class DataChannelOptions : IOptions<DataChannelOptions>
{
    public bool IsRWSplitting { get; set; } = false;

    public bool AutoOpen { get; set; } = false;

    public string Name { get; set; }

    public ConnectionStrings ConnectionString { get; set; }

    DataChannelOptions IOptions<DataChannelOptions>.Value => this;
}
