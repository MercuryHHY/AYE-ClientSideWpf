using AYE.BaseFramework.MqttClientCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AYE.BaseFramework.MqttClientCore.Models;

public interface IMqttClientConfig
{

    /// <summary>
    /// 客户端ID
    /// </summary>
    string ClientId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    string UserName { get; set; }


    /// <summary>
    /// 密码
    /// </summary>
    string Password { get; set; }



    /// <summary>
    /// 订阅的主题
    /// </summary>
    List<MqttTopic> SubscribeTopics { get; set; } 


    /// <summary>
    /// 成功订阅的主题
    /// </summary>
    List<string> SubscribeSucceeds { get; set; } 


    /// <summary>
    /// 发布的主题
    /// </summary>
    string PublishTopic { get; set; } 

    /// <summary>
    /// 消息质量等级
    /// </summary>
    QosLevel QosLevel { get; set; }


    /// <summary>
    /// 是否为保留消息
    /// </summary>
    bool IsRetain { get; set; }

    /// <summary>
    /// IP
    /// </summary>
    string ServerIp { get; set; } 

    /// <summary>
    /// 端口
    /// </summary>
    int ServerPort { get; set; }

    /// <summary>
    /// 是否开启
    /// </summary>
    bool IsOpened { get; set; } 

    /// <summary>
    /// 自动重新连接
    /// </summary>
    bool AutoReconnect { get; set; } 


    /// <summary>
    /// 接收消息的格式
    /// </summary>
    MqttPayloadType ReceivePaylodType { get; set; }

    /// <summary>
    /// 发送消息的格式
    /// </summary>
    MqttPayloadType SendPaylodType { get; set; } 





}
