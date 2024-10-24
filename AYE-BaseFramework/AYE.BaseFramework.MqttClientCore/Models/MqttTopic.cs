using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.MqttClientCore.Models;

/// <summary>
/// Mqtt的主题 需要主题作为属性才能实现修改通知
/// </summary>
public class MqttTopic 
{
    public MqttTopic(string topic)
    {
        Topic = topic;
    }

    /// <summary>
    /// 主题
    /// </summary>
    public string Topic { get ; set; } 


}
