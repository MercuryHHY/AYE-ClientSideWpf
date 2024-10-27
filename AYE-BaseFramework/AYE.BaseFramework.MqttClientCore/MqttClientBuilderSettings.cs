using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AYE.BaseFramework.MqttClientCore;

public class MqttClientBuilderSettings
{

    /// <summary>
    /// MQTT 代理的地址。可以是域名（例如 broker.example.com）或 IP 地址（例如 192.168.1.100）。
    /// 客户端将连接到这个地址的 Broker 上。
    /// </summary>
    public string BrokerAddress { get; set; }


    /// <summary>
    /// MQTT 代理使用的端口号。默认的 MQTT 端口为 1883，如果使用 TLS 则为 8883
    /// 指定客户端连接所用的网络端口，需要确保这个端口在 Broker 上开放。
    /// </summary>
    public int BrokerPort { get; set; }

    /// <summary>
    /// 用于标识 MQTT 客户端的唯一 ID。在同一个 MQTT 代理（Broker）中，ClientId 必须是唯一的。
    /// 如果两个客户端使用相同的 ClientId 连接到同一个 Broker，将导致第一个连接被断开。
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// 可选项，不填写时可以匿名连接（前提是 Broker 支持匿名连接）。
    /// Broker 校验通过后，允许客户端成功连接。
    /// </summary>
    public string? Username { get; set; }
    public string? Password { get; set; }

    /// <summary>
    /// 指示客户端是否使用 TLS（Secure Sockets Layer）来加密数据传输
    /// 如果设置为 true，将使用加密连接，通常默认端口为 8883
    /// 需要确保 Broker 配置支持 TLS，否则连接会失败
    /// </summary>
    public bool UseTls { get; set; }

    /// <summary>
    /// 决定客户端与 Broker 之间的会话状态是否被清除
    /// 如果设置为 true，客户端连接断开后，订阅及未收到的消息会被清除
    /// 如果设置为 false，断开连接后再次连接可以恢复之前的订阅
    /// </summary>
    public bool CleanSession { get; set; }
}
