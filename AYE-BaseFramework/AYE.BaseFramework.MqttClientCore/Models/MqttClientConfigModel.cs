using AYE.BaseFramework.MqttClientCore.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AYE.BaseFramework.MqttClientCore.Models;

public class MqttClientConfigModel: IMqttClientConfig
{

    /// <summary>
    /// 客户端ID
    /// </summary>
    public string ClientId { get; set; } = "ClientId";

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = "UserName";


    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = "Password";



    /// <summary>
    /// 订阅的主题
    /// </summary>
    public List<MqttTopic> SubscribeTopics { get; set; } = new();


    /// <summary>
    /// 成功订阅的主题
    /// </summary>
    [JsonIgnore]
    public List<string> SubscribeSucceeds { get; set; } = new();


    /// <summary>
    /// 发布的主题
    /// </summary>
    public string PublishTopic { get; set; } = string.Empty;

    /// <summary>
    /// 消息质量等级
    /// </summary>
    public QosLevel QosLevel { get; set; } = QosLevel.AtMostOnce;


    /// <summary>
    /// 是否为保留消息
    /// </summary>
    public bool IsRetain { get; set; }

    /// <summary>
    /// IP
    /// </summary>
    public string ServerIp { get; set; } = "127.0.0.1";

    /// <summary>
    /// 端口
    /// </summary>
    public int ServerPort { get; set; } = 1883;

    /// <summary>
    /// 是否开启
    /// </summary>
    public bool IsOpened { get; set; } = false;

    /// <summary>
    /// 自动重新连接
    /// </summary>
    public bool AutoReconnect { get; set; } = true;


    /// <summary>
    /// 接收消息的格式
    /// </summary>
    public MqttPayloadType ReceivePaylodType { get; set; } = MqttPayloadType.Plaintext;

    /// <summary>
    /// 发送消息的格式
    /// </summary>
    public MqttPayloadType SendPaylodType { get; set; } = MqttPayloadType.Plaintext;


}
