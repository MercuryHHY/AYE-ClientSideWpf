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

using HslCommunication.Profinet.Inovance;

using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel.Inovance;

public class InovanceConnectionStrings : ConnectionStrings
{
    /// <summary>
    /// 远程IP
    /// </summary>
    [DisplayName("远程IP")]
    [Description("127.0.0.1")]
    public InovanceConnectionStrings WithRemoteIPAddress(string remoteIPAddress)
    {
        this.Set(nameof(InovanceConfigDefine.RemoteIPAddress), remoteIPAddress);
        return this;
    }
    /// <summary>
    /// 远程IP
    /// </summary>
    [DisplayName("PLC系类")]
    [Description("默认AM")]
    public InovanceConnectionStrings WithInovanceSeries(InovanceSeries inovanceSeries)
    {
        this.Set(nameof(InovanceConfigDefine.InovanceSeries), inovanceSeries);
        return this;
    }

    /// <summary>
    /// 远程端口，可缺省
    /// </summary>
    [DisplayName("远程端口")]
    [Description("可缺省")]
    public InovanceConnectionStrings WithRemotePort(int RemotePort)
    {
        this.Set(nameof(InovanceConfigDefine.RemotePort), RemotePort);
        return this;
    }

    /// <summary>
    /// 远程端口，可缺省
    /// </summary>
    [DisplayName("远程端口")]
    [Description("可缺省")]
    public InovanceConnectionStrings WithStation(byte station)
    {
        this.Set(nameof(InovanceConfigDefine.Station), station);
        return this;
    }

    /// <summary>
    /// 连接的超时时间，单位是毫秒,默认-1
    /// </summary>
    [DisplayName("连接的超时时间")]
    [Description("默认-1")]
    public InovanceConnectionStrings WithConnectionTimeout(int connectionTimeout)
    {
        this.Set(nameof(InovanceConfigDefine.ConnectionTimeout), connectionTimeout);
        return this;
    }

    /// <summary>
    /// 字节顺序，支持ABCD、BADC、CDAB、DCBA
    /// </summary>
    [DisplayName("字节顺序")]
    [Description("支持ABCD、BADC、CDAB、DCBA")]
    public InovanceConnectionStrings WithDataEncordingType(DataEncordingType dataEncordingType)
    {
        this.Set(nameof(InovanceConfigDefine.ByteOrder), dataEncordingType);
        return this;
    }

    /// <summary>
    /// 字节顺序，支持ABCD、BADC、CDAB、DCBA
    /// </summary>
    [DisplayName("字节顺序")]
    [Description("支持ABCD、BADC、CDAB、DCBA")]
    public InovanceConnectionStrings WithDataEncordingType(VarType varType, DataEncordingType dataEncordingType)
    {
        VarTypeInfo info = varType;
        this.Set(info.ElementVarType.ToString(), dataEncordingType);
        return this;
    }


    public static implicit operator string(InovanceConnectionStrings connectionStrings)
    {
        return connectionStrings.ToString();
    }

    public static implicit operator InovanceConnectionStrings(string connectionStrings)
    {
        return ConvertFrom<InovanceConnectionStrings>(connectionStrings);
    }
}
