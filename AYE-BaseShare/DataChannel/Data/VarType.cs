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
///变量类型
/// </summary>
public enum VarType
{
    /// <summary>
    /// 无
    /// </summary>
    [Description("无")]
    None,
    /// <summary>
    ///布尔
    /// </summary>
    [Description("布尔")]
    Bool,
    /// <summary>
    ///有符号字节，-128-127
    /// </summary>
    [Description("有符号字节，-128-127")]
    SByte,
    /// <summary>
    ///无符号字节，0-255
    /// </summary>
    [Description("无符号字节，0-255")]
    Byte,
    /// <summary>
    /// 短整型，-32768-32767
    /// </summary>
    [Description("短整型，-32768-32767")]
    Short,
    /// <summary>
    /// 无符号短整型，0-65535
    /// </summary>
    [Description("无符号短整型，0-65535")]
    UShort,
    /// <summary>
    /// 整型，-2147489648-2147483647
    /// </summary>
    [Description("整型，-2147489648-2147483647")]
    Int,
    /// <summary>
    /// 无符号整型，0-42994967295
    /// </summary>
    [Description("无符号整型，0-42994967295")]
    UInt,
    /// <summary>
    /// 长整型，-2^63-2^63
    /// </summary>
    [Description("长整型，-2^63-2^63")]
    Long,
    /// <summary>
    ///无符号长整型， 0-2^64
    /// </summary>
    [Description("无符号长整型， 0-2^64")]
    ULong,
    /// <summary>
    ///单精度浮点型， 1.5*10^-45-3.4*10^38
    /// </summary>
    [Description("单精度浮点型， 1.5*10^-45-3.4*10^38")]
    Float,
    /// <summary>
    /// 双精度浮点型，5.0*10^-324-1.7*10^308
    /// </summary>
    [Description("双精度浮点型，5.0*10^-324-1.7*10^308")]
    Double,
    /// <summary>
    /// 字符串
    /// </summary>
    [Description("字符串")]
    String,
    /// <summary>
    ///布尔数组
    /// </summary>
    [Description("布尔数组")]
    BoolArray,
    /// <summary>
    ///字节数组
    /// </summary>
    [Description("字节数组")]
    ByteArray,
    /// <summary>
    ///无符号字节数组
    /// </summary>
    [Description("无符号字节数组")]
    SByteArray,
    ///<summary>
    /// 短整型数组
    /// </summary>
    [Description("短整型数组")]
    ShortArray,
    /// <summary>
    /// 无符号短整型数组
    /// </summary>
    [Description("无符号短整型数组")]
    UShortArray,
    /// <summary>
    /// 整型数组
    /// </summary>
    [Description("整型数组")]
    IntArray,
    /// <summary>
    /// 无符号整型数组
    /// </summary>
    [Description("无符号整型数组")]
    UIntArray,
    /// <summary>
    /// 长整型数组
    /// </summary>
    [Description("长整型数组")]
    LongArray,
    /// <summary>
    ///无符号长整型数组
    /// </summary>
    [Description("无符号长整型数组")]
    ULongArray,
    /// <summary>
    ///单精度浮点型数组
    /// </summary>
    [Description("单精度浮点型数组")]
    FloatArray,
    /// <summary>
    /// 双精度浮点型数组
    /// </summary>
    [Description("双精度浮点型数组")]
    DoubleArray,
    /// <summary>
    /// 字符串数组
    /// </summary>
    [Description("字符串数组")]
    StringArray,
}