#region << License >>
// MIT License
// 
// 2024 - 上位机软件
//
// Copyright (c) @ Daniel大妞（guanhu）. All rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

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