
using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel.VarWriters;

/// <summary>
/// 变量写入器基类
/// </summary>
public abstract class VarWriterBase
{
    protected VarWriterBase()
    {
    }

    protected VarWriterBase(IDataChannel dataChannel)
    {
        this.Channel = ThrowHelper.IfNull(dataChannel);
    }

    /// <summary>
    /// 数据通道
    /// </summary>
    public IDataChannel Channel { get; set; }

    /// <summary>
    /// 打开
    /// </summary>
    public void Open()
    {
        if (!this.Channel.IsConnected)
        {
            this.Channel.Open();
        }
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        if (this.Channel.IsConnected)
        {
            this.Channel.Close();
        }
    }

    public virtual void Write(VarAddress address, object value)
    {
        object values = value;
        VarTypeInfo varTypeInfo = address.VarTypeInfo;
        if (!varTypeInfo.IsArray && value is not Array)
        {
            Array array = Array.CreateInstance(varTypeInfo.CSharpElementType, 1);
            array.SetValue(value, 0);
            values = array;
        }

        MethodInfo writeAsyncMethodInfo = DataChannelExtensions.DataChannelWriteMethodInfo.MakeGenericMethod(varTypeInfo.CSharpElementType);
        writeAsyncMethodInfo.Invoke(this.Channel, new object[] { address, values });
    }

    public virtual void Write(IDictionary<VarAddress, object> addresses)
    {
        if (this.Channel is IDataChannelBulk dataChannelBatchReadAndWrite)
        {
            dataChannelBatchReadAndWrite.Write(addresses);
        }
        else
        {
            addresses.ForEach(x => this.Write(x.Key, x.Value));
        }
    }
}