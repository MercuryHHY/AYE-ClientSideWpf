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


namespace Microsoft.Extensions.DataChannel.Modbus;

public static class ModbusMasterConfigDefine
{
    /// <summary>
    /// 连接模式，Rtu、Ascii、Tcp、Udp、RtuTcp、RtuUdp
    /// </summary>
    public const string Mode = nameof(Mode);

    /// <summary>
    /// 从站号
    /// </summary>
    public const string SlaveId = nameof(SlaveId);

    /// <summary>
    /// 写入超时，单位ms，默认值为3000
    /// </summary>
    public const string WriteTimeout = nameof(WriteTimeout);

    /// <summary>
    /// 读取超时，单位ms，默认值为3000
    /// </summary>
    public const string ReadTimeout = nameof(ReadTimeout);

    /// <summary>
    /// 远程IP
    /// </summary>
    public const string RemoteIPAddress = nameof(RemoteIPAddress);

    /// <summary>
    /// 远程端口
    /// </summary>
    public const string RemotePort = nameof(RemotePort);

    /// <summary>
    /// 波特率
    /// </summary>
    public const string PortName = nameof(PortName);

    /// <summary>
    /// 波特率
    /// </summary>
    public const string BaudRate = nameof(BaudRate);

    /// <summary>
    /// 数据位
    /// </summary>
    public const string DataBits = nameof(DataBits);

    /// <summary>
    /// 校验位
    /// </summary>
    public const string Parity = nameof(Parity);

    /// <summary>
    /// 停止位
    /// </summary>
    public const string StopBits = nameof(StopBits);

    /// <summary>
    /// 字节顺序，支持ABCD、BADC、CDAB、DCBA
    /// </summary>
    public const string ByteOrder = nameof(ByteOrder);

    /// <summary>
    /// 字符串编码，支持utf-7、utf-8、utf-16、utf-32、us-ascii等，默认为us-ascii
    /// </summary>
    /// <remarks>
    /// 参考：https://docs.microsoft.com/zh-cn/dotnet/api/system.text.encoding?view=net-6.0
    /// </remarks>
    public const string StringEncoding = nameof(StringEncoding);
}
