using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.MqttClientCore.Enums;

public enum QosLevel
{
    [Description("Qos0")]
    AtLeastOnce = 0,
    [Description("Qos1")]
    AtMostOnce = 1,
    [Description("Qos2")]
    ExactlyOnce = 2
}
