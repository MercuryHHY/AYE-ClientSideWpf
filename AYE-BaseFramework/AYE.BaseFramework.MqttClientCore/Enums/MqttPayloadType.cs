using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.MqttClientCore.Enums;

public enum MqttPayloadType
{
    [Description("纯文本 UTF-8")]
    Plaintext,
    [Description("Hex 16进制")]
    Hex,
    [Description("Base64")]
    Base64,
    [Description("Json")]
    Json,

}

