using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Protocol;

namespace AYE.BaseFramework.MqttClientCore;

public class Mqtt5ClientService
{
    private IMqttClient _mqttClient;
    private MqttClientOptions _mqttOptions;
    private Queue<MqttApplicationMessage> _unacknowledgedMessages = new Queue<MqttApplicationMessage>();

    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<string, string> OnMessageReceived; // (topic, payload)

    public Mqtt5ClientService(string brokerAddress, int brokerPort, string clientId, string username = null, string password = null, bool useTls = false, bool cleanSession = true)
    {
        InitializeMqttClient();
        BuildMqttOptions(brokerAddress, brokerPort, clientId, username, password, useTls, cleanSession);
    }

    // 初始化 MQTT 客户端
    private void InitializeMqttClient()
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        // 连接事件处理
        _mqttClient.UseConnectedHandler(async e =>
        {
            Console.WriteLine("Connected to the MQTT broker.");
            OnConnected?.Invoke();
        });

        // 断线处理
        _mqttClient.UseDisconnectedHandler(async e =>
        {
            Console.WriteLine($"Disconnected from MQTT broker. Reason: {e.Reason}");
            OnDisconnected?.Invoke();

            // 自动重连机制
            await Task.Delay(TimeSpan.FromSeconds(5));
            try
            {
                await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
                Console.WriteLine("Reconnected successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reconnection failed: {ex.Message}");
            }
        });

        // 接收消息处理
        _mqttClient.UseApplicationMessageReceivedHandler(e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Console.WriteLine($"Received message: Topic = {topic}, Payload = {payload}");

            // 触发消息接收事件
            OnMessageReceived?.Invoke(topic, payload);
        });


        _mqttClient.UseConnectedHandler(e => {
            Log.Information("Connected to MQTT broker.");
            OnConnected?.Invoke();
        });

        _mqttClient.UseDisconnectedHandler(async e => {
            Log.Warning("Disconnected from MQTT broker.");
            OnDisconnected?.Invoke();

            // 自动重连日志
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
                Log.Information("Reconnected successfully.");
            }
            catch (Exception ex)
            {
                Log.Error($"Reconnection failed: {ex.Message}");
            }
        });

        _mqttClient.UseApplicationMessageReceivedHandler(e => {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Log.Information($"Received message: Topic = {topic}, Payload = {payload}");

            OnMessageReceived?.Invoke(topic, payload);
        });


    }

    // 构建 MQTT 客户端选项
    private void BuildMqttOptions(string brokerAddress, int brokerPort, string clientId, string username, string password, bool useTls, bool cleanSession)
    {
        var builder = new MqttClientOptionsBuilder()
            .WithClientId(clientId)
            .WithTcpServer(brokerAddress, brokerPort)
            .WithCleanSession(cleanSession);

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            builder.WithCredentials(username, password);
        }

        if (useTls)
        {
            builder.WithTls();
        }

        _mqttOptions = builder.Build();
    }

    // 连接到 MQTT Broker
    public async Task ConnectAsync()
    {
        try
        {
            await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
            Console.WriteLine("MQTT client connected.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection failed: {ex.Message}");
        }
    }

    // 发布消息
    public async Task PublishAsync(string topic, string payload, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce, bool retainFlag = false, Dictionary<string, string> userProperties = null)
    {
        if (!_mqttClient.IsConnected)
        {
            Console.WriteLine("Client is not connected. Publishing failed.");
            return;
        }

        var messageBuilder = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(qos)
            .WithRetainFlag(retainFlag);

        // MQTT 5.0 用户自定义属性
        if (userProperties != null)
        {
            foreach (var prop in userProperties)
            {
                messageBuilder.WithUserProperty(prop.Key, prop.Value);
            }
        }

        var message = messageBuilder.Build();
        await _mqttClient.PublishAsync(message, CancellationToken.None);
        Console.WriteLine($"Message published to topic {topic}: {payload}");
    }

    // 订阅主题
    public async Task SubscribeAsync(string topic, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce)
    {
        if (!_mqttClient.IsConnected)
        {
            Console.WriteLine("Client is not connected. Subscription failed.");
            return;
        }

        await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic(topic)
            .WithQualityOfServiceLevel(qos)
            .Build());

        Console.WriteLine($"Subscribed to topic: {topic}");
    }

    // 取消订阅主题
    public async Task UnsubscribeAsync(string topic)
    {
        if (!_mqttClient.IsConnected)
        {
            Console.WriteLine("Client is not connected. Unsubscription failed.");
            return;
        }

        await _mqttClient.UnsubscribeAsync(topic);
        Console.WriteLine($"Unsubscribed from topic: {topic}");
    }

    // 断开连接
    public async Task DisconnectAsync()
    {
        if (_mqttClient.IsConnected)
        {
            await _mqttClient.DisconnectAsync();
            Console.WriteLine("MQTT client disconnected.");
        }
    }

    /// <summary>
    /// 共享订阅允许多个客户端共同处理来自同一主题的消息。通常用于负载均衡，格式为 $share/{GroupName}/{Topic}：
    /// </summary>
    /// <param name="group"></param>
    /// <param name="topic"></param>
    /// <param name="qos"></param>
    /// <returns></returns>
    public async Task SubscribeToSharedTopicAsync(string group, string topic, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce)
    {
        var sharedTopic = $"$share/{group}/{topic}";
        await SubscribeAsync(sharedTopic, qos);
    }


    // 在断线处理程序中持久化保存
    public async Task PublishAsync(...)
    {
        if (!_mqttClient.IsConnected)
        {
            Log.Warning("Client is not connected. Adding message to queue.");
            _unacknowledgedMessages.Enqueue(messageBuilder.Build());
            return;
        }

        var message = messageBuilder.Build();
        await _mqttClient.PublishAsync(message, CancellationToken.None);
        Log.Information($"Message published to topic {topic}: {payload}");
    }

    // 在重连后重新发布未确认的消息
    private async Task ResendMessagesAsync()
    {
        while (_unacknowledgedMessages.Count > 0)
        {
            var message = _unacknowledgedMessages.Dequeue();
            await _mqttClient.PublishAsync(message, CancellationToken.None);
            Log.Information($"Resending stored message to topic {message.Topic}");
        }
    }


    /// <summary>
    /// 处理复杂的断线重连逻辑
    /// 断线后，根据不同的断开原因调整重连策略，可以考虑采取指数退避等策略进行重连：
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private async Task HandleDisconnectionAsync(MqttClientDisconnectedEventArgs e)
    {
        var retryInterval = TimeSpan.FromSeconds(5);

        while (true)
        {
            try
            {
                await Task.Delay(retryInterval);
                retryInterval = TimeSpan.FromSeconds(Math.Min(retryInterval.TotalSeconds * 2, 60));  // 指数退避
                await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
                Log.Information("Reconnected successfully after disconnection.");
                break;
            }
            catch (Exception ex)
            {
                Log.Error($"Reconnection attempt failed: {ex.Message}. Retrying in {retryInterval.Seconds} seconds...");
            }
        }
    }


}
