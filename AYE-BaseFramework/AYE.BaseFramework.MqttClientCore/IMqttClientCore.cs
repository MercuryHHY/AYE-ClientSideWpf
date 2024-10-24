using AYE.BaseFramework.MqttClientCore.Models;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.MqttClientCore;

public interface IMqttClientCore
{
    IMqttClient Client { get; set; }

    IMqttClientConfig MqttClientConfig { get; set; }


    /// <summary>
    /// 发布消息
    /// </summary>
    /// <returns></returns>
    Task Publish();


    /// <summary>
    /// 关闭客户端
    /// </summary>
    /// <returns></returns>
    Task CloseMqttClient();

    /// <summary>
    /// 打开客户端
    /// </summary>
    /// <returns></returns>
    Task<bool> OpenMqttClient();


    Task Client_ConnectingAsync(MqttClientConnectingEventArgs arg);


    Task Client_ConnectedAsync(MqttClientConnectedEventArgs arg);


    /// <summary>
    /// 客户端接收事件
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    Task Client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg);


    /// <summary>
    /// 掉线自动调用该方法
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    Task Client_DisconnectedAsync(MqttClientDisconnectedEventArgs arg);

    /// <summary>
    /// 订阅
    /// </summary>
    /// <returns></returns>
    Task SubscribeTopic();

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <returns></returns>
    Task UnsubscribeTopic();


    /// <summary>
    /// 移除待订阅的主题
    /// </summary>
    /// <param name="obj"></param>
    void RemoveSubTopic(MqttTopic obj);


    void ExportConfig();



    void ImportConfig();



    void ImportConfig(FileInfo obj);



}
