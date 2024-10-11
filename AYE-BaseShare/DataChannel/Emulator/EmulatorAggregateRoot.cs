

using System.Data.Domains.Entities;

using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel.Emulator;

public sealed class EmulatorAggregateRoot : AggregateRoot<Guid>/*, IHasModificationTime*/
{
    /// <summary>
    /// 数据通道名称
    /// </summary>
    public string ChannelName { get; set; }

    /// <summary>
    /// 地址名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 地址值
    /// </summary>
    public string Address { get; set; }

    public uint Count { get; set; }

    public VarType Type { get; set; }

    /// <summary>
    /// 地址数据
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 模拟器名称
    /// </summary>
    public string? DataGeneratorName { get; set; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime? LastModificationTime { get; set; }

    internal void UpdateValue(string value)
    {
        this.Value = value;
        this.LastModificationTime = DateTime.Now;
    }

    internal void UpdateDataGeneratorName(string value)
    {
        this.DataGeneratorName = value;
        this.LastModificationTime = DateTime.Now;
    }
}
