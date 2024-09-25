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

public partial class VarAddress
{
    public static VarAddress Bool(DataAddress dataAddress, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = 1, Type = VarType.Bool };
    }

    public static VarAddress SByte(DataAddress dataAddress, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = 1, Type = VarType.SByte };
    }

    public static VarAddress Byte(DataAddress dataAddress, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = 1, Type = VarType.Byte };
    }

    public static VarAddress Short(DataAddress dataAddress, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = 1, Type = VarType.Short };
    }

    public static VarAddress UShort(DataAddress dataAddress, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = 1, Type = VarType.UShort };
    }

    public static VarAddress Int(DataAddress dataAddress, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = 1, Type = VarType.Int };
    }

    public static VarAddress UInt(DataAddress dataAddress, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = 1, Type = VarType.UInt };
    }

    public static VarAddress Long(DataAddress dataAddress, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = 1, Type = VarType.Long };
    }

    public static VarAddress ULong(DataAddress dataAddress, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = 1, Type = VarType.ULong };
    }
    public static VarAddress Float(DataAddress dataAddress, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = 1, Type = VarType.Float };
    }
    public static VarAddress Double(DataAddress dataAddress, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = 1, Type = VarType.Double };
    }
    public static VarAddress String(DataAddress dataAddress, uint stringLength, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = stringLength, Type = VarType.String };
    }
    public static VarAddress BoolArray(DataAddress dataAddress, uint count, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = count, Type = VarType.BoolArray };
    }
    public static VarAddress ByteArray(DataAddress dataAddress, uint count, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = count, Type = VarType.ByteArray };
    }
    public static VarAddress SByteArray(DataAddress dataAddress, uint count, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = count, Type = VarType.SByteArray };
    }
    public static VarAddress ShortArray(DataAddress dataAddress, uint count, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = count, Type = VarType.ShortArray };
    }
    public static VarAddress UShortArray(DataAddress dataAddress, uint count, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = count, Type = VarType.UShortArray };
    }
    public static VarAddress IntArray(DataAddress dataAddress, uint count, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = count, Type = VarType.IntArray };
    }
    public static VarAddress UIntArray(DataAddress dataAddress, uint count, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = count, Type = VarType.UIntArray };
    }
    public static VarAddress LongArray(DataAddress dataAddress, uint count, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = count, Type = VarType.LongArray };
    }
    public static VarAddress ULongArray(DataAddress dataAddress, uint count, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = count, Type = VarType.ULongArray };
    }
    public static VarAddress FloatArray(DataAddress dataAddress, uint count, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = count, Type = VarType.FloatArray };
    }
    public static VarAddress DoubleArray(DataAddress dataAddress, uint count, string name = "")
    {
        return new VarAddress() { Address = dataAddress, Name = name, Count = count, Type = VarType.DoubleArray };
    }
    public static VarAddress StringArray(DataAddress dataAddress, int[] stringArrayCounts, string name = "")
    {
        return new VarAddress()
        {
            Address = dataAddress,
            Name = name,
            Count = (uint)stringArrayCounts.Sum(),
            StringArrayCounts = stringArrayCounts,
            Type = VarType.StringArray
        };
    }
}

[DebuggerDisplay("{Name}，Address = {Address}, VarType = {Type}, Count = {Count}")]
public partial class VarAddress : IHasExtraDatas, IEquatable<VarAddress>
{
    public VarAddress()
    {
        this.ExtraDatas = System.Collections.Generic.StringDictionary.InvariantCultureIgnoreCase();
        this.Type = VarType.None;
        this.Count = 1;
        this.StringArrayCounts = Array.Empty<int>();
    }

    public virtual string Name { get; set; }

    public virtual DataAddress Address { get; set; }

    public virtual VarType Type { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public VarTypeInfo VarTypeInfo => this.Type;

    public virtual uint Count { get; set; }

    public virtual int[] StringArrayCounts { get; set; }

    public virtual StringDictionary<object> ExtraDatas { get; }

    public override bool Equals(object? obj)
    {
        return this.Equals(obj as VarAddress);
    }

    public bool Equals(VarAddress? other)
    {
        return other != null
            && (ReferenceEquals(this, other) || (this.Address == other.Address && this.Type == other.Type && this.Count == this.Count));
    }

    public override int GetHashCode()
    {
        return this.Address.GetHashCode() | this.Type.GetHashCode() | (int)this.Count;
    }

    public override string ToString()
    {
        return
            $"{nameof(this.Name)}：{this.Name}," +
            $"{nameof(this.Address)}：{this.Address}," +
            $"{nameof(this.Type)}：{this.Type}," +
            $"{nameof(this.Count)}：{this.Count}";
    }
}