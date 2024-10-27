using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Protocol;
using Microsoft.Extensions.Logging;

namespace AYE.BaseFramework.MqttClientCore;

public class Mqtt5ClientService: IMqtt5ClientService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;
    private readonly Queue<MqttApplicationMessage> _unacknowledgedMessages = new Queue<MqttApplicationMessage>();
    private readonly ILogger<Mqtt5ClientService> _logger;

    public Mqtt5ClientService(MqttClientBuilderSettings settings, ILogger<Mqtt5ClientService> logger)
    {
        _logger = logger;

        var builder = new MqttClientOptionsBuilder()
            .WithClientId(settings.ClientId)
            .WithTcpServer(settings.BrokerAddress, settings.BrokerPort)
            .WithCleanSession(settings.CleanSession);

        if (!string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password))
        {
            builder.WithCredentials(settings.Username, settings.Password);
        }

        if (settings.UseTls)
        {
            builder.WithTls();
        }

        _mqttOptions = builder.Build();

        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        _mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
        _mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
        _mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
    }

    /// <summary>
    /// 连接成功 之后
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs e)
    {
        LogInfo("Connected successfully.");
        // Execute additional operations after successful connection if needed.
        return Task.CompletedTask;
    }


    /// <summary>
    /// 断开连接之后
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private async Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs e)
    {
        LogInfo("Disconnected from the broker.");

        if (e.ClientWasConnected)
        {
            await HandleDisconnectionAsync();
        }
    }


    /// <summary>
    /// 对接收消息的处理
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        var topic = e.ApplicationMessage.Topic;
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        LogInfo($"Message received: Topic = {topic}, Payload = {payload}");

        // Process received message if needed
        return Task.CompletedTask;
    }


    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <returns></returns>
    public async Task ConnectAsync()
    {
        try
        {
            await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
            LogInfo("MQTT client connected.");
        }
        catch (Exception ex)
        {
            LogError($"Connection failed: {ex.Message}");
        }
    }




    public async Task PublishAsync(string topic, string payload,
        MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce, 
        bool retainFlag = false, 
        Dictionary<string, string> userProperties = null)
    {
        var message = BuildMessage(topic, payload, qos, retainFlag, userProperties);

        if (_mqttClient.IsConnected)
        {
            await TryPublishAsync(message);
        }
        else
        {
            LogInfo("Client is not connected. Adding message to queue.");
            _unacknowledgedMessages.Enqueue(message);
        }
    }



    public async Task SubscribeAsync(string topic,
        MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce)
    {
        if (!_mqttClient.IsConnected)
        {
            LogInfo("Client is not connected. Subscription failed.");
            return;
        }

        await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic(topic)
            .WithQualityOfServiceLevel(qos)
            .Build());

        LogInfo($"Subscribed to topic: {topic}");
    }

    public async Task UnsubscribeAsync(string topic)
    {
        if (!_mqttClient.IsConnected)
        {
            LogInfo("Client is not connected. Unsubscription failed.");
            return;
        }

        await _mqttClient.UnsubscribeAsync(topic);
        LogInfo($"Unsubscribed from topic: {topic}");
    }

    public async Task DisconnectAsync()
    {
        if (_mqttClient.IsConnected)
        {
            await _mqttClient.DisconnectAsync();
            LogInfo("MQTT client disconnected.");
        }
    }

    public async Task SubscribeToSharedTopicAsync(string group, string topic, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce)
    {
        var sharedTopic = $"$share/{group}/{topic}";
        await SubscribeAsync(sharedTopic, qos);
    }

    private async Task HandleDisconnectionAsync()
    {
        var retryInterval = TimeSpan.FromSeconds(5);

        while (true)
        {
            try
            {
                LogInfo("Attempting to reconnect...");
                await Task.Delay(retryInterval);
                await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
                LogInfo("Reconnected successfully.");
                await ResendMessagesAsync(); // 可以在重连成功后重新发送未确认的消息
                break;
            }
            catch (Exception ex)
            {
                LogError($"Reconnection attempt failed: {ex.Message}. Retrying in {retryInterval.TotalSeconds} seconds...");
                retryInterval = TimeSpan.FromSeconds(Math.Min(retryInterval.TotalSeconds * 2, 60)); // 指数退避
            }
        }
    }


    /// <summary>
    /// 消息 重发
    /// </summary>
    /// <returns></returns>
    private async Task ResendMessagesAsync()
    {
        while (_unacknowledgedMessages.Count > 0)
        {
            var message = _unacknowledgedMessages.Dequeue();
            await TryPublishAsync(message);
        }
    }

    private async Task TryPublishAsync(MqttApplicationMessage message)
    {
        try
        {
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.PublishAsync(message, CancellationToken.None);
                LogInfo($"Message published to topic {message.Topic}");
            }
            else
            {
                LogInfo("Client is not connected, re-queueing message.");
                _unacknowledgedMessages.Enqueue(message);
            }
        }
        catch (Exception ex)
        {
            LogError($"Failed to resend message: {ex.Message}");
            _unacknowledgedMessages.Enqueue(message);
        }
    }

    /// <summary>
    /// Message构建
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="payload"></param>
    /// <param name="qos"></param>
    /// <param name="retainFlag"></param>
    /// <param name="userProperties"></param>
    /// <returns></returns>
    private MqttApplicationMessage BuildMessage(string topic, string payload, MqttQualityOfServiceLevel qos, bool retainFlag, Dictionary<string, string> userProperties)
    {
        var messageBuilder = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(qos)
            .WithRetainFlag(retainFlag);

        if (userProperties != null)
        {
            foreach (var prop in userProperties)
            {
                messageBuilder.WithUserProperty(prop.Key, prop.Value);
            }
        }

        return messageBuilder.Build();
    }

    public bool IsConnected()
    {
        if (!_mqttClient.IsConnected)
        {
            LogInfo("Client is not connected. Operation failed.");
            return false;
        }
        return true;
    }


    private void LogInfo(string message)
    {
        _logger.LogDebug(message);
    }

    private void LogError(string message)
    {
        _logger.LogError(message);
    }
}
