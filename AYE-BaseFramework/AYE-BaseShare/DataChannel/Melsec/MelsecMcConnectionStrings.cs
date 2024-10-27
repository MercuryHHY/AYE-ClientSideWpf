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

namespace Microsoft.Extensions.DataChannel.Melsec;

public sealed class MelsecMcConnectionStrings : ConnectionStrings
{
    /// <summary>
    /// 远程IP
    /// </summary>
    [DisplayName("远程IP")]
    [Description("127.0.0.1")]
    public MelsecMcConnectionStrings WithRemoteIPAddress(string remoteIPAddress)
    {
        this.Set(nameof(MelsecMcConfigDefine.RemoteIPAddress), remoteIPAddress);
        return this;
    }

    /// <summary>
    /// 远程端口，可缺省
    /// </summary>
    [DisplayName("远程端口")]
    [Description("可缺省")]
    public MelsecMcConnectionStrings WithRemotePort(int RemotePort)
    {
        this.Set(nameof(MelsecMcConfigDefine.RemotePort), RemotePort);
        return this;
    }

    /// <summary>
    /// 字节顺序，支持ABCD、BADC、CDAB、DCBA
    /// </summary>
    [DisplayName("字节顺序")]
    [Description("支持ABCD、BADC、CDAB、DCBA")]
    public MelsecMcConnectionStrings WithDataEncordingType(DataEncordingType dataEncordingType)
    {
        this.Set(nameof(MelsecMcConfigDefine.ByteOrder), dataEncordingType);
        return this;
    }

    /// <summary>
    /// 字节顺序，支持ABCD、BADC、CDAB、DCBA
    /// </summary>
    [DisplayName("字节顺序")]
    [Description("支持ABCD、BADC、CDAB、DCBA")]
    public MelsecMcConnectionStrings WithDataEncordingType(VarType varType, DataEncordingType dataEncordingType)
    {
        VarTypeInfo info = varType;
        this.Set(info.ElementVarType.ToString(), dataEncordingType);
        return this;
    }

    public static implicit operator string(MelsecMcConnectionStrings connectionStrings)
    {
        return connectionStrings.ToString();
    }

    public static implicit operator MelsecMcConnectionStrings(string connectionStrings)
    {
        return ConvertFrom<MelsecMcConnectionStrings>(connectionStrings);
    }
}
