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
