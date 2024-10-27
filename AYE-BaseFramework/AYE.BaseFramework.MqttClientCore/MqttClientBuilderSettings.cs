using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.MqttClientCore;

public class MqttClientBuilderSettings
{
    public string BrokerAddress { get; set; }
    public int BrokerPort { get; set; }
    public string ClientId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool UseTls { get; set; }
    public bool CleanSession { get; set; }
}
