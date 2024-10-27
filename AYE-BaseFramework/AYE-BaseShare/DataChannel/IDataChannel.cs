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

using Microsoft.Extensions.DataChannel.ComponentModel;
using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel;

public interface IDataChannel : IDisposable
{
    Guid Id { get; }

    /// <summary>
    /// 名称
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// 连接字符串，形如数据库连接字符串
    /// </summary>
    ConnectionStrings ConnectionString { get; set; }

    /// <summary>
    /// 是否已连接
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// 连接状态改变事件
    /// </summary>
    event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

    /// <summary>
    /// 打开
    /// </summary>
    IDataChannel Open();

    /// <summary>
    /// 关闭
    /// </summary>
    IDataChannel Close();

    TValue[] Read<TValue>(VarAddress varAddress);

    void Write<TValue>(VarAddress varAddress, IEnumerable<TValue> values);

    DataEncoder GetDataEncoder(VarAddress varAddress);

    DataEncoder GetDataEncoder(VarType varType);
}