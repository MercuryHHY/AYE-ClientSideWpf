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

namespace Microsoft.Extensions.DataChannel.Manager;

/// <summary>
/// 连接配置集合
/// </summary>
public interface IConnectionMonitor : ICurrentConnection
{
    /// <summary>
    /// 获取全部连接配置
    /// </summary>
    /// <returns></returns>
    ICollection<DataChannelInfo> GetMonitorInfos();

    /// <summary>
    /// 索引支持
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    DataChannelInfo this[int index] { get; }

    /// <summary>
    /// 编号支持
    /// </summary>
    /// <param name="connectionCode"></param>
    /// <returns></returns>
    DataChannelInfo this[string connectionCode] { get; }

    /// <summary>
    /// 添加一个连接配置
    /// </summary>
    /// <param name="connectionConfigInfo"></param>
    /// <returns></returns>
    IConnectionMonitor Add(DataChannelInfo connectionConfigInfo);

    /// <summary>
    /// 设置当前默认连接plc、编号支持
    /// </summary>
    /// <param name="connectionCode"></param>
    /// <returns></returns>
    IConnectionMonitor SetCurrent(string connectionCode);

    /// <summary>
    /// 设置当前默认连接plc、连接支持
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IConnectionMonitor SetCurrent(int index);
}