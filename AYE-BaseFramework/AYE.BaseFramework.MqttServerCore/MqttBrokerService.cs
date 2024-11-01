using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.MqttServerCore;

public class MqttBrokerService: IMqttBrokerService
{
    private readonly MqttServer _mqttServer;
    private readonly ILogger<MqttBrokerService> _logger;

    public MqttBrokerService(ILogger<MqttBrokerService> logger)
    {
        _logger = logger;

        var optionsBuilder = new MqttServerOptionsBuilder()
       .WithDefaultEndpointPort(1883) // 默认端口，用于非加密连接

       .WithEncryptedEndpoint()       // 启用加密端口
       .WithEncryptedEndpointPort(8883) // 自定义加密端口，指定哪个端口用于加密连接。


       //certificatePassword 是用于保护您的 SSL/TLS 证书文件（通常是 .pfx 格式）的密码。当您导出或生成一个 .pfx 证书文件时，创建过程可能会要求设置一个密码，以确保证书的安全。在代码中，该密码用于加载和解密证书，以便您的应用程序能够使用 SSL/TLS 来加密通信。
       //.WithEncryptionCertificate("path/to/certificate.pfx", "certificatePassword") // 配置 SSL/TLS 证书
       
       
       
       .WithPersistentSessions() // 启用持久化会话，以便客户端断开连接后可以恢复与会话相关的信息。
       .WithConnectionValidator(OnConnectionValidating); // 验证连接，添加了连接验证以允许在客户端连接时执行身份验证。确保 OnConnectionValidating 是一个实现了连接验证逻辑的方法。


        // 如果上面的方法不存在，使用以下方式加载证书
        var certificate = new X509Certificate2("path/to/certificate.pfx", "certificatePassword");

        optionsBuilder = optionsBuilder.WithEncryptionSslProtocol(System.Security.Authentication.SslProtocols.Tls12);
        optionsBuilder = optionsBuilder.WithTls(new MqttServerTlsOptions
        {
            ServerCertificate = certificate,
            AllowUntrustedCertificates = false, // 是否允许不受信任的证书（生产环境中应设为 false）
            CertificateValidationHandler = context => true, // 自定义验证逻辑
            UseTls = true // 启用 TLS
        });

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

    private Task OnConnectionValidating(ValidatingConnectionEventArgs e)
    {
        // 用户名密码验证
        if (e.Username != "validUser" || e.Password != "validPassword")
        {
            e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            LogError($"Client {e.ClientId} connection refused: invalid credentials");
            return Task.CompletedTask;
        }

        // 包含遗愿消息的配置
        e.SessionItems.Add("Will", new MqttApplicationMessageBuilder()
            .WithTopic("Client/Disconnect")
            .WithPayload($"{e.ClientId} disconnected unexpectedly")
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build()
        );

        LogInfo($"Client {e.ClientId} authenticated successfully.");
        return Task.CompletedTask;
    }
    
    private string RequestClientId(InterceptingSubscriptionEventArgs e)
    {
        // Logic to request a unique Client Id
        // This can be a simple GUID or a more complex logic depending on the system requirements
        return Guid.NewGuid().ToString();
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
