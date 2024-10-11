


using System.ComponentModel;

namespace Microsoft.Extensions.DataChannel.Data;

/// <summary>
/// 数据编码类型
/// <![CDATA[
/// 举个例子： 41  8C  E8  EE
/// 字节顺序： A   B   C   D
/// 
/// ABCD：41  8C  E8  EE
/// BADC：8C  41  EE  E8
/// CDAB：E8  EE  41  8C
/// DCBA：EE  E8  8C  41
/// ]]>
/// </summary>
public enum DataEncordingType
{
    /// <summary>
    /// ABCD。按照顺序排序
    /// </summary>
    [Description("ABCD。按照顺序排序")]
    ABCD,
    /// <summary>
    /// BADC。按照单字反转
    /// </summary>
    [Description("BADC。按照单字反转")]
    BADC,
    /// <summary>
    /// CDAB。按照双字反转 (大部分PLC默认排序方法)
    /// </summary>
    [Description("CDAB。按照双字反转 (大部分PLC默认排序方法)")]
    CDAB,
    /// <summary>
    /// DCBA。按照倒序排序
    /// </summary>
    [Description("DCBA。按照倒序排序")]
    DCBA
}
