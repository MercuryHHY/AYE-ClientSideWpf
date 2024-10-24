using AYE.BaseFramework.MqttClientCore.Models;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.MqttClientCore;

public class MqttClientCore : IMqttClientCore
{
    public IMqttClient Client { get ; set ; }
    public IMqttClientConfig MqttClientConfig { get; set; }




    public Task Client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        throw new NotImplementedException();
    }

    public Task Client_ConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        throw new NotImplementedException();
    }

    public Task Client_ConnectingAsync(MqttClientConnectingEventArgs arg)
    {
        throw new NotImplementedException();
    }

    public Task Client_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
    {
        throw new NotImplementedException();
    }

    public Task CloseMqttClient()
    {
        throw new NotImplementedException();
    }

    public void ExportConfig()
    {
        throw new NotImplementedException();
    }

    public void ImportConfig()
    {
        throw new NotImplementedException();
    }

    public void ImportConfig(FileInfo obj)
    {
        throw new NotImplementedException();
    }

    public Task<bool> OpenMqttClient()
    {
        throw new NotImplementedException();
    }

    public Task Publish()
    {
        throw new NotImplementedException();
    }

    public void RemoveSubTopic(MqttTopic obj)
    {
        throw new NotImplementedException();
    }

    public Task SubscribeTopic()
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeTopic()
    {
        throw new NotImplementedException();
    }
}
