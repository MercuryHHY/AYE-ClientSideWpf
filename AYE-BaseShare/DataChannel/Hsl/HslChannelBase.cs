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

using HslCommunication;
using HslCommunication.Core;

using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel.Hsl;

public abstract class HslChannelBase(string name, ConnectionStrings connectionString) : DataChannelBase(name, connectionString)
{
    static HslChannelBase()
    {
        // Hsl授权验证
        HslCommunication.Authorization.SetAuthorizationCode("5b970d33-6559-43db-9943-c22d60525182");
    }

    /// <summary>
    /// 数据读写器
    /// </summary>
    protected abstract IReadWriteNet HslPlc { get; }

    /// <summary>
    /// 抛出异常信息
    /// </summary>
    /// <param name="result">异常信息</param>
    /// <exception cref="ApplicationException">异常信息</exception>
    protected virtual void ThrowResultException(OperateResult result)
    {
        if (!result.IsSuccess)
        {
            throw new ApplicationException(result.ToMessageShowString());
        }
    }

    protected override TValue[] Read<TValue>(VarAddress varAddress, DataEncoder dataEncoder)
    {
        if (typeof(TValue) == typeof(bool))
        {
            OperateResult<bool[]>? result = this.HslPlc.ReadBool(varAddress.Address, ushort.Parse($"{varAddress.Count}"));
            this.ThrowResultException(result);
            return result.Content.Cast<TValue>().ToArray();
        }
        else
        {
            int byteCount = dataEncoder.GetByteCount<TValue>((int)varAddress.Count);
            ushort count = (ushort)Math.Ceiling(byteCount / 2d);
            OperateResult<byte[]>? result = this.HslPlc.Read(varAddress.Address, ushort.Parse($"{count}"));
            this.ThrowResultException(result);
            return [.. dataEncoder.GetValues<TValue>(result.Content, 0, result.Content.Length)];
        }
    }

    protected override void Write<TValue>(VarAddress varAddress, DataEncoder dataEncoder, IEnumerable<TValue> values)
    {
        if (typeof(TValue) == typeof(bool))
        {
            this.ThrowResultException(this.HslPlc.Write(varAddress.Address, values.Cast<bool>().ToArray()));
        }
        else
        {
            this.ThrowResultException(this.HslPlc.Write(varAddress.Address, dataEncoder.GetBytes(values).ToArray()));
        }
    }
}