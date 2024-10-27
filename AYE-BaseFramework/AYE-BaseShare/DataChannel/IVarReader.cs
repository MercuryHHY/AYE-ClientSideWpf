

using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel;

public interface IVarReader
{
    /// <summary>
    /// 打开
    /// </summary>
    void Open();

    /// <summary>
    /// 关闭
    /// </summary>
    void Close();

    IDataChannel Channel { get; set; }

    VarValue Read(VarAddress address);

    VarValue[] Read(IEnumerable<VarAddress> addresses);
}

public static class VarReaderExtensions
{
    public static VarValue<TValue> Read<TValue>(this IVarReader reader, VarAddress address)
    {
        VarValue result = reader.Read(address);
        return result.Cast<TValue>();
    }
}