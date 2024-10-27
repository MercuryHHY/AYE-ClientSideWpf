

using HslCommunication;
using HslCommunication.Core;
using HslCommunication.Profinet.Inovance;

using Microsoft.Extensions.DataChannel.Data;
using Microsoft.Extensions.DataChannel.Hsl;

namespace Microsoft.Extensions.DataChannel.Inovance;

public class InovanceTcp : HslChannelBase
{
    private readonly InovanceTcpNet _client;
    private readonly bool _stringHasOffset;

    public InovanceTcp(InovanceConnectionStrings connectionString) : this(nameof(InovanceTcp), connectionString) { }

    public InovanceTcp(string name, InovanceConnectionStrings connectionString) : base(name, connectionString)
    {
        string ip = this.ConnectionString.GetOrDefault(InovanceConfigDefine.RemoteIPAddress, "127.0.0.1");
        int port = this.ConnectionString.GetOrDefault(InovanceConfigDefine.RemotePort, 502);
        byte station = this.ConnectionString.GetOrDefault(InovanceConfigDefine.RemotePort, (byte)1);
        int connectTimeOut = this.ConnectionString.GetOrDefault(InovanceConfigDefine.ConnectionTimeout, 3000);
        InovanceSeries inovanceSeries = this.ConnectionString.GetOrDefault(InovanceConfigDefine.InovanceSeries, InovanceSeries.AM);
        this._client = new InovanceTcpNet(ip, port, station)
        {
            Series = inovanceSeries,
            ConnectTimeOut = connectTimeOut
        };
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
