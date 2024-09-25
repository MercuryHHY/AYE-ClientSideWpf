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


namespace Microsoft.Extensions.DataChannel.Modbus;

/// <summary>
/// 地址类型
/// </summary>
public enum ModbusAddressType
{
    /// <summary>
    /// digital output
    /// 数字输出，线圈输出，<![CDATA[一个地址一个数据位]]>用户可以置位、复位，可以回读状态，比如继电器输出，电机的启停控制信号。
    /// 线圈（DO）地址：00000~09999
    /// </summary>
    [Description("数字输出")]
    DO,
    /// <summary>
    /// digital input
    /// 数字输入，离散输入，<![CDATA[一个地址一个数据位]]>，用户只能读取它的状态，不能修改。比如面板上的按键、开关状态，电机的故障状态。
    /// 触点（DI）地址：10000~19999
    /// </summary>
    [Description("数字输入")]
    DI,
    /// <summary>
    /// Analog Output
    /// 模拟输出，保持寄存器，<![CDATA[一个地址16位数据]]>，用户可以写，也可以回读，比如一个控制变频器的电流值。
    /// 输出寄存器（AO）地址：40000~49999
    /// </summary>
    [Description("模拟输出")]
    AO,
    /// <summary>
    /// Analog Input
    /// 模拟输入，输入寄存器，<![CDATA[一个地址16位数据]]>，用户只能读，不能修改，比如一个电压值的读数。
    /// 输入寄存器（AI）地址：30000~39999
    /// </summary>
    [Description("模拟输入")]
    AI
}