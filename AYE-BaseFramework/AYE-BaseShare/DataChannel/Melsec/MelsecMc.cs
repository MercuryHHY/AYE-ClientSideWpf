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
using HslCommunication.Profinet.Melsec;

using Microsoft.Extensions.DataChannel.Hsl;

namespace Microsoft.Extensions.DataChannel.Melsec;

public sealed class MelsecMc : HslChannelBase
{
    private readonly MelsecMcNet _client;

    public MelsecMc(MelsecMcConnectionStrings connectionString) : this(nameof(MelsecMc), connectionString)
    {
    }

    public MelsecMc(string name, MelsecMcConnectionStrings connectionString) : base(name, connectionString)
    {
        string ip = this.ConnectionString.GetOrDefault(MelsecMcConfigDefine.RemoteIPAddress, "127.0.0.1");
        int port = this.ConnectionString.GetOrDefault(MelsecMcConfigDefine.RemotePort, 6000);
        this._client = new MelsecMcNet(ip, port);
    }

    protected override IReadWriteNet HslPlc => this._client;

    protected override void Open()
    {
        OperateResult result = this._client.ConnectServer();
        if (!result.IsSuccess)
        {
            throw new ArgumentException($"PLC 连接失败，请检查网络是否正常,错误消息：{result.Message}");
        }
    }

    protected override void Close()
    {
        OperateResult result = this._client.ConnectClose();
        if (!result.IsSuccess)
        {
            throw new ArgumentException($"关闭 PLC 失败，错误消息：{result.Message}");
        }
    }
}
