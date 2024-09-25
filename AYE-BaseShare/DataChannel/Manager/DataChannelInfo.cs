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

using Microsoft.Extensions.DataChannel.VarReaders;
using Microsoft.Extensions.DataChannel.VarWriters;

namespace Microsoft.Extensions.DataChannel.Manager;

public class DataChannelInfo
{
    /// <summary>
    /// 连接名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 连接码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 协议类型
    /// </summary>
    public string ProtocolType { get; set; }

    /// <summary>
    /// 读策略
    /// </summary>
    public string ReadStrategy { get; set; } = nameof(VarReaderDirectly);

    /// <summary>
    /// 写策略
    /// </summary>
    public string WriteStrategy { get; set; } = nameof(VarWriterDirectly);

    /// <summary>
    /// 失败重试次数
    /// </summary>
    public int RetryCount { get; set; } = 10;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 启用读写分离
    /// </summary>
    public bool IsRWSplitting { get; set; } = false;

    /// <summary>
    /// 失败重试时间
    /// </summary>
    public int RetryTimeout { get; set; } = 1000;

    /// <summary>
    /// 连接参数
    /// </summary>
    public ConnectionStrings ConnectionString { get; set; }

    public void Check()
    {
        ThrowHelper.IfNullOrWhiteSpace(this.Code);
        ThrowHelper.IfNullOrWhiteSpace(this.ProtocolType);
        ThrowHelper.IfNullOrWhiteSpace(this.ReadStrategy);
        ThrowHelper.IfNullOrWhiteSpace(this.WriteStrategy);
        ThrowHelper.IfNull(this.ConnectionString);
    }
}
