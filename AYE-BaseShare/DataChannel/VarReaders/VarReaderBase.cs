
using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel.VarReaders;

/// <summary>
/// 变量读取器基类
/// </summary>
public abstract class VarReaderBase
{
    protected VarReaderBase(IDataChannel dataChannel)
    {
        this.Channel = ThrowHelper.IfNull(dataChannel);
    }

    protected VarReaderBase()
    {
    }

    /// <summary>
    /// 数据通道
    /// </summary>
    public IDataChannel Channel { get; set; }

    public void Open()
    {
        if (!this.Channel.IsConnected)
        {
            this.Channel.Open();
        }
    }

    public void Close()
    {
        if (this.Channel.IsConnected)
        {
            this.Channel.Close();
        }
    }

    public virtual VarValue Read(VarAddress address)
    {
        Stopwatch sw = Stopwatch.StartNew();
        VarTypeInfo varTypeInfo = address.VarTypeInfo;
        MethodInfo readMethodInfo = DataChannelExtensions.DataChannelReadMethodInfo.MakeGenericMethod(varTypeInfo.CSharpElementType);
        object value = readMethodInfo.Invoke(this.Channel, new object[] { address })!;
        value = this.GetElementOrArray(value, varTypeInfo);
        sw.Stop();
        return new VarValue(address, value).WithExecuteTime(sw.Elapsed);
    }

    /// <summary>
    /// 批量读取
    /// </summary>
    /// <param name="addresses">地址集合</param>
    /// <returns>值数组</returns>
    public virtual VarValue[] Read(IEnumerable<VarAddress> addresses)
    {
        if (addresses.IsNullOrEmpty())
        {
            return Array.Empty<VarValue>();
        }

        // 判断Channel是否实现批量读写
        if (this.Channel is IDataChannelBulk dataChannelBulk)
        {
            VarAddress[] varAddresses = addresses.ToArray();
            Stopwatch sw = Stopwatch.StartNew();
            object[] values = dataChannelBulk.Read(varAddresses);
            sw.Stop();
            return values.Select((x, index) => new VarValue(varAddresses[index], x).WithExecuteTime(sw.Elapsed)).ToArray();
        }
        else
        {
            return addresses.Select(e => this.Read(e)).ToArray();
        }
    }

    private object GetElementOrArray(object value, VarTypeInfo varTypeInfo)
    {
        object result = value;

        if (!varTypeInfo.IsArray && result is Array array)
        {
            result = array.GetValue(0)!;
        }

        return result;
    }
}