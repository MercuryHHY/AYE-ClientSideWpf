

using System.ComponentModel;

namespace Microsoft.Extensions.DataChannel.Data;

/// <summary>
/// 数据地址
/// </summary>
[TypeConverter(typeof(StringWrapperTypeConverter<DataAddress>))]
public class DataAddress : StringWrapper
{
    public static implicit operator DataAddress(string address)
    {
        return new DataAddress() { Value = address };
    }

    public static implicit operator string(DataAddress address)
    {
        return address?.Value ?? string.Empty;
    }
}