using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.MqttServerCore;

public class MqttBrokerService
{
    private readonly MqttServer _mqttServer;
    private readonly ILogger<MqttBrokerService> _logger;

    public MqttBrokerService(ILogger<MqttBrokerService> logger)
    {
        _logger = logger;

        var optionsBuilder = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(1883); // 设置默认端口

        var factory = new MqttFactory();
        _mqttServer = factory.CreateMqttServer(optionsBuilder.Build());

        _mqttServer.ClientConnectedAsync += OnClientConnectedAsync;
        _mqttServer.ClientDisconnectedAsync += OnClientDisconnectedAsync;
        _mqttServer.InterceptingPublishAsync += OnInterceptingPublishAsync;
    }


    private Task OnClientConnectedAsync(ClientConnectedEventArgs e)
    {
        LogInfo($"Client connected: {e.ClientId}");
        return Task.CompletedTask;
    }

    private Task OnClientDisconnectedAsync(ClientDisconnectedEventArgs e)
    {
        LogInfo($"Client disconnected: {e.ClientId}");
        return Task.CompletedTask;
    }

    private Task OnInterceptingPublishAsync(InterceptingPublishEventArgs e)
    {
        string payload = e.ApplicationMessage.Payload == null
            ? null
            : Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        LogInfo($"Message intercepted: Client = {e.ClientId}, Topic = {e.ApplicationMessage.Topic}, Payload = {payload}");
        return Task.CompletedTask;
    }

    public async Task StartAsync()
    {
        if (_mqttServer.IsStarted)
        {
            LogInfo("MQTT broker already running.");
            return;
        }

        LogInfo("Starting MQTT broker...");

        await _mqttServer.StartAsync();
        LogInfo("MQTT broker started.");
    }

    public async Task StopAsync()
    {
        if (!_mqttServer.IsStarted)
        {
            LogInfo("MQTT broker is not running.");
            return;
        }

        LogInfo("Stopping MQTT broker...");
        await _mqttServer.StopAsync();
        LogInfo("MQTT broker stopped.");
    }

    private void LogInfo(string message)
    {
        _logger.LogInformation(message);
    }

    private void LogError(string message)
    {
        _logger.LogError(message);
    }
}
