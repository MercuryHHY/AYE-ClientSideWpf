using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.MqttClientCore;

public interface IMqtt5ClientService
{
    Task ConnectAsync();
    Task PublishAsync(string topic, string payload, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce, bool retainFlag = false, Dictionary<string, string> userProperties = null);
    Task SubscribeAsync(string topic, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce);
    Task UnsubscribeAsync(string topic);
    Task DisconnectAsync();
    Task SubscribeToSharedTopicAsync(string group, string topic, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce);
    bool IsConnected();
}
