using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.MqttServerCore;

public interface IMqttBrokerService
{
    Task StartAsync();
    Task StopAsync();
}
