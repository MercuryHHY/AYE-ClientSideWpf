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


using System.IO.Ports;
using System.Net.Sockets;

using Microsoft.Extensions.DataChannel;
using Microsoft.Extensions.DataChannel.Data;

using NModbus;
using NModbus.IO;
using NModbus.Serial;

namespace Microsoft.Extensions.DataChannel.Modbus;

public class ModbusMaster : DataChannelBase
{
    private IModbusMaster _modbusMaster;
    private byte _slaveId;

    public ModbusMaster(ModbusMasterConnectionStrings connectionString) : this(nameof(ModbusMaster), connectionString)
    {
    }

    public ModbusMaster(string name, ModbusMasterConnectionStrings connectionString) : base(name, connectionString)
    {
    }

    protected override void Open()
    {
        this._slaveId = this.ConnectionString.Get<byte>(ModbusMasterConfigDefine.SlaveId);
        ModbusMasterMode mode = this.ConnectionString.Get<ModbusMasterMode>(ModbusMasterConfigDefine.Mode);

        ModbusFactory factory = new ModbusFactory();
        this._modbusMaster = mode switch
        {
            ModbusMasterMode.RTU => factory.CreateRtuMaster(this.CreateAndOpenSerialPort()),
            ModbusMasterMode.ASCII => factory.CreateAsciiMaster(this.CreateAndOpenSerialPort()),
            ModbusMasterMode.TCP => factory.CreateMaster(this.CreateTcpOpenClient()),
            ModbusMasterMode.UDP => factory.CreateMaster(this.CreateUdpAndOpenClient()),
            ModbusMasterMode.RTUTCP => factory.CreateRtuMaster(new TcpClientAdapter(this.CreateTcpOpenClient())),
            ModbusMasterMode.RTUUDP => factory.CreateRtuMaster(new UdpClientAdapter(this.CreateUdpAndOpenClient())),
            _ => throw new InvalidOperationException($"不支持{mode}"),
        };
    }

    protected override void Close()
    {
        this._modbusMaster?.Dispose();
    }

    protected override TValue[] Read<TValue>(VarAddress varAddress, DataEncoder dataEncoder)
    {
        ModbusAddress modbusAddress = varAddress;
        if (modbusAddress.Digital && typeof(TValue) != typeof(bool))
        {
            throw new NotSupportedException("DO/DI类型不支持非Bool类型数据读写");
        }
        if (modbusAddress.Digital)
        {
            bool[] bs = modbusAddress.ReadOnly
                ? this._modbusMaster.ReadInputs(this._slaveId, modbusAddress.Index, (ushort)varAddress.Count)
                : this._modbusMaster.ReadCoils(this._slaveId, modbusAddress.Index, (ushort)varAddress.Count);
            return bs.Cast<TValue>().ToArray();
        }
        else
        {
            int byteCount = dataEncoder.GetByteCount<TValue>((int)varAddress.Count);
            ushort count = (ushort)Math.Ceiling(byteCount / 2d);

            ushort[] ushorts = modbusAddress.ReadOnly
                ? this._modbusMaster.ReadInputRegisters(this._slaveId, modbusAddress.Index, count)
                : this._modbusMaster.ReadHoldingRegisters(this._slaveId, modbusAddress.Index, count);


            if (typeof(TValue) == typeof(string))
            {
                byte[] bs = ushorts.Select(x => BitConverter.GetBytes(x)).Two2One().ToArray();
                if (varAddress.Type == VarType.String)
                {
                    return dataEncoder.GetValues<TValue>(bs, 0, bs.Length).Take((int)varAddress.Count).ToArray();
                }
                else
                {
                    List<string> vs = new List<string>();
                    int bytesStartIndex = 0;
                    varAddress.StringArrayCounts.ForEach((x, index) =>
                    {
                        string str = dataEncoder.GetValues<string>(bs, bytesStartIndex, x)[0];
                        bytesStartIndex += x;
                        vs.Add(str);
                    });
                    return vs.Cast<TValue>().ToArray();
                }
            }
            else
            {
                byte[] bs = dataEncoder.GetBytes(ushorts).ToArray();
                return dataEncoder.GetValues<TValue>(bs, 0, bs.Length).Take((int)varAddress.Count).Cast<TValue>().ToArray();
            }
        }
    }

    protected override void Write<TValue>(VarAddress varAddress, DataEncoder dataEncoder, IEnumerable<TValue> values)
    {
        ModbusAddress modbusAddress = varAddress;
        if (modbusAddress.Digital && typeof(TValue) != typeof(bool))
        {
            throw new NotSupportedException("DO/DI类型不支持非Bool类型数据读写");
        }

        if (modbusAddress.ReadOnly)
        {
            throw new NotSupportedException("AI/DI类型不支持数据写入");
        }

        if (modbusAddress.Digital)
        {
            this._modbusMaster.WriteMultipleCoils(this._slaveId, modbusAddress.Index, values.Cast<bool>().ToArray());
        }
        else
        {

            ushort[] words = new ushort[] { };

            // 字符串数据不做字节序处理
            if (typeof(TValue) == typeof(string))
            {
                if (varAddress.Type == VarType.String)
                {
                    int count = dataEncoder.GetByteCount<string>((int)varAddress.Count);
                    byte[] buffer = DataEncoder.ArrayExpandToLengthEven(DataEncoder.ArrayExpandToLength(dataEncoder.GetBytes(values), count));

                    for (int i = 0; i < buffer.Length; i += 2)
                    {
                        words = words.Concat(new ushort[] { BitConverter.ToUInt16(buffer, i) }).ToArray();
                    }
                }
                else
                {
                    List<byte> vvvv = new List<byte>();
                    values.ForEach((x, index) =>
                    {
                        int count = dataEncoder.GetByteCount<string>(varAddress.StringArrayCounts[index]);
                        byte[] buffer = DataEncoder.ArrayExpandToLengthEven(DataEncoder.ArrayExpandToLength(dataEncoder.GetBytes(new TValue[] { x }), count));
                        vvvv.AddRange(buffer);
                    });
                    byte[] buffer = vvvv.ToArray();
                    for (int i = 0; i < buffer.Length; i += 2)
                    {
                        words = words.Concat(new ushort[] { BitConverter.ToUInt16(buffer, i) }).ToArray();
                    }
                }

            }
            else
            {
                byte[] buffer = DataEncoder.ArrayExpandToLengthEven(dataEncoder.GetBytes(values));
                words = dataEncoder.GetValues<ushort>(buffer, 0, buffer.Length).ToArray();
            }
            this._modbusMaster.WriteMultipleRegisters(this._slaveId, modbusAddress.Index, words);
        }
    }

    private SerialPort CreateAndOpenSerialPort()
    {
        SerialPort serialPort = new SerialPort(
            this.Get<string>(ModbusMasterConfigDefine.PortName),
            this.GetOrDefault(ModbusMasterConfigDefine.BaudRate, 9600),
            this.GetOrDefault(ModbusMasterConfigDefine.Parity, Parity.None),
            this.GetOrDefault(ModbusMasterConfigDefine.DataBits, 8),
            this.GetOrDefault(ModbusMasterConfigDefine.StopBits, StopBits.One))
        {
            ReadTimeout = this.GetOrDefault(ModbusMasterConfigDefine.WriteTimeout, 3000),
            WriteTimeout = this.GetOrDefault(ModbusMasterConfigDefine.ReadTimeout, 3000)
        };
        serialPort.Open();
        return serialPort;
    }

    private TcpClient CreateTcpOpenClient()
    {
        TcpClient tcpClient = new TcpClient()
        {
            SendTimeout = this.GetOrDefault(ModbusMasterConfigDefine.WriteTimeout, 3000),
            ReceiveTimeout = this.GetOrDefault(ModbusMasterConfigDefine.ReadTimeout, 3000)
        };
        tcpClient.Connect(
            this.Get<string>(ModbusMasterConfigDefine.RemoteIPAddress),
            this.Get<int>(ModbusMasterConfigDefine.RemotePort));
        return tcpClient;
    }

    private UdpClient CreateUdpAndOpenClient()
    {
        UdpClient udpClient = new UdpClient();
        udpClient.Client.SendTimeout = this.GetOrDefault(ModbusMasterConfigDefine.WriteTimeout, 3000);
        udpClient.Client.ReceiveTimeout = this.GetOrDefault(ModbusMasterConfigDefine.ReadTimeout, 3000);
        udpClient.Connect(
            this.Get<string>(ModbusMasterConfigDefine.RemoteIPAddress),
            this.Get<int>(ModbusMasterConfigDefine.RemotePort));

        return udpClient;
    }
}