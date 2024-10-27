

namespace Microsoft.Extensions.DataChannel.VarWriters;

/// <summary>
/// 默认变量写入器，直接写入到硬件
/// </summary>
public class VarWriterDirectly : VarWriterBase, IVarWriter
{
    public VarWriterDirectly()
    {
    }

    public VarWriterDirectly(IDataChannel dataChannel) : base(dataChannel)
    {
    }
}
