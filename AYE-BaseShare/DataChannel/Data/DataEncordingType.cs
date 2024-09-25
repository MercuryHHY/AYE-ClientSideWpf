#region << License >>
// MIT License
// 
// 2024 - 上位机软件
//
// Copyright (c) @ Daniel大妞（guanhu）. All rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion


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
