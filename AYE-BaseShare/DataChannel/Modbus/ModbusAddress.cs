

using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel.Modbus;

internal class ModbusAddress
{
    private readonly VarAddress _address;

    ModbusAddress(VarAddress address)
    {
        this._address = address;
    }

    public ModbusAddressType ModbusAddressType => (ModbusAddressType)Enum.Parse(typeof(ModbusAddressType), this._address.Address.Value.Substring(0, 2));

    public ushort Index => ushort.Parse(this._address.Address.Value.Substring(2));

    public bool ReadOnly => this.ModbusAddressType is ModbusAddressType.DI or ModbusAddressType.AI;

    public bool Digital => this.ModbusAddressType is ModbusAddressType.DI or ModbusAddressType.DO;

    public bool Analog => this.ModbusAddressType is ModbusAddressType.AI or ModbusAddressType.AO;

    public static implicit operator ModbusAddress(VarAddress varAddress)
    {
        return new ModbusAddress(varAddress);
    }
}