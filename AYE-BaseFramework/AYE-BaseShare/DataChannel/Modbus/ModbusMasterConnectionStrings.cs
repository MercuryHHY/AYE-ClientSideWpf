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


using System.IO.Ports;

using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel.Modbus;

public class ModbusMasterConnectionStrings : ConnectionStrings
{
    /// <summary>
    /// 连接模式，Rtu、Ascii、Tcp、Udp、RtuTcp、RtuUdp
    /// </summary>
    [DisplayName("连接模式")]
    [Description("支持Rtu、Ascii、Tcp、Udp、RtuTcp、RtuUdp")]
    public ModbusMasterConnectionStrings WithMode(ModbusMasterMode mode)
    {
        this.Set(nameof(ModbusMasterConfigDefine.Mode), mode);
        return this;
    }

    /// <summary>
    /// 停止位
    /// </summary>
    [DisplayName("停止位")]
    [Description("支持One、Two、OnePointFive")]
    public ModbusMasterConnectionStrings WithStopBits(StopBits stopBits)
    {
        this.Set(nameof(ModbusMasterConfigDefine.StopBits), stopBits);
        return this;
    }

    /// <summary>
    /// 校验位
    /// </summary>
    [DisplayName("校验位")]
    [Description("支持Odd、Even、Mark、Space")]
    public ModbusMasterConnectionStrings WithParity(Parity parity)
    {
        this.Set(nameof(ModbusMasterConfigDefine.Parity), parity);
        return this;
    }

    /// <summary>
    /// 波特率
    /// </summary>
    [DisplayName("波特率")]
    [Description("可选1200，2400，4800，9600，19200，38400，57600，115200等")]
    public ModbusMasterConnectionStrings WithBaudRate(int baudRate)
    {
        this.Set(nameof(ModbusMasterConfigDefine.BaudRate), baudRate);
        return this;
    }

    /// <summary>
    /// 端口名称
    /// </summary>
    [DisplayName("端口名称")]
    [Description("常见的波特率是“COM1”、“COM2”等")]
    public ModbusMasterConnectionStrings WithPortName(string portName)
    {
        this.Set(nameof(ModbusMasterConfigDefine.PortName), portName);
        return this;
    }

    /// <summary>
    /// 数据位
    /// </summary>
    [DisplayName("数据位")]
    [Description("标准的值是5、6、7和8")]
    public ModbusMasterConnectionStrings WithDataBits(int dataBits)
    {
        this.Set(nameof(ModbusMasterConfigDefine.DataBits), dataBits);
        return this;
    }

    /// <summary>
    /// 远程IP
    /// </summary>
    [DisplayName("远程IP")]
    [Description("127.0.0.1")]
    public ModbusMasterConnectionStrings WithRemoteIPAddress(string remoteIPAddress)
    {
        this.Set(nameof(ModbusMasterConfigDefine.RemoteIPAddress), remoteIPAddress);
        return this;
    }

    /// <summary>
    /// 读取超时，单位ms，默认值为3000
    /// </summary>
    [DisplayName("读取超时")]
    [Description("默认值为3000")]
    public ModbusMasterConnectionStrings WithReadTimeout(int readTimeout)
    {
        this.Set(nameof(ModbusMasterConfigDefine.ReadTimeout), readTimeout);
        return this;
    }

    /// <summary>
    /// 写入超时，单位ms，默认值为3000
    /// </summary>
    [DisplayName("写入超时")]
    [Description("默认值为3000")]
    public ModbusMasterConnectionStrings WithWriteTimeout(int writeTimeout)
    {
        this.Set(nameof(ModbusMasterConfigDefine.WriteTimeout), writeTimeout);
        return this;
    }

    /// <summary>
    /// 从站号
    /// </summary>
    [DisplayName("从站号")]
    [Description("Modbus从站地址，取值1~247")]
    public ModbusMasterConnectionStrings WithSlaveId(ushort slaveId)
    {
        this.Set(nameof(ModbusMasterConfigDefine.SlaveId), slaveId);
        return this;
    }

    /// <summary>
    /// 远程端口
    /// </summary>
    [DisplayName("远程端口")]
    [Description("102")]
    public ModbusMasterConnectionStrings WithRemotePort(int remotePort)
    {
        this.Set(nameof(ModbusMasterConfigDefine.RemotePort), remotePort);
        return this;
    }

    /// <summary>
    /// 字节顺序，支持ABCD、BADC、CDAB、DCBA
    /// </summary>
    [DisplayName("字节顺序")]
    [Description("支持ABCD、BADC、CDAB、DCBA")]
    public ModbusMasterConnectionStrings WithDataEncordingType(DataEncordingType dataEncordingType)
    {
        this.Set(nameof(ModbusMasterConfigDefine.ByteOrder), dataEncordingType);
        return this;
    }

    /// <summary>
    /// 字节顺序，支持ABCD、BADC、CDAB、DCBA
    /// </summary>
    [DisplayName("字节顺序")]
    [Description("支持ABCD、BADC、CDAB、DCBA")]
    public ModbusMasterConnectionStrings WithDataEncordingType(VarType varType, DataEncordingType dataEncordingType)
    {
        VarTypeInfo info = varType;
        this.Set(info.ElementVarType.ToString(), dataEncordingType);
        return this;
    }

    /// <summary>
    /// 字符串编码，支持utf-7、utf-8、utf-16、utf-32、us-ascii等，默认为us-ascii
    /// </summary>
    [DisplayName("字符串编码")]
    [Description("支持utf-7、utf-8、utf-16、utf-32、us-ascii等，默认为us-ascii")]
    public ModbusMasterConnectionStrings WithStringEncoding(string stringEncoding)
    {
        this.Set(nameof(ModbusMasterConfigDefine.StringEncoding), stringEncoding);
        return this;
    }


    public static implicit operator string(ModbusMasterConnectionStrings connectionStrings)
    {
        return connectionStrings.ToString();
    }

    public static implicit operator ModbusMasterConnectionStrings(string connectionStrings)
    {
        return ConvertFrom<ModbusMasterConnectionStrings>(connectionStrings);
    }
}
