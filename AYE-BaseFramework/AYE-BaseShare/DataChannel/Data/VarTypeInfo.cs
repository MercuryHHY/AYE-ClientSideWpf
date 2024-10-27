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

[DebuggerDisplay("{VarType}，IsArray = {IsArray}, Type = {CSharpType}, ElementType = {CSharpElementType}")]
public readonly struct VarTypeInfo
{
    private VarTypeInfo(VarType varType)
    {
        this.VarType = varType;
        if (this.VarType == VarType.None)
        {
            this.IsNone = true;
        }
        else
        {
            this.IsNone = false;
            this.IsArray = varType.ToString().Contains("Array");
            this.ElementVarType = this.IsArray ? GetElementVarType(this.VarType) : this.VarType;
            this.CSharpType = GetType(this.VarType);
            this.CSharpElementType = GetElementType(this.VarType);
        }
    }

    public VarType VarType { get; }

    public VarType ElementVarType { get; }

    public bool IsArray { get; }

    public Type CSharpType { get; }

    public Type CSharpElementType { get; }

    public bool IsNone { get; }

    public bool IsBool => this.VarType is VarType.Bool or VarType.BoolArray;
    public bool IsByte => this.VarType is VarType.Byte or VarType.ByteArray;
    public bool IsSByte => this.VarType is VarType.SByte or VarType.SByteArray;
    public bool IsShort => this.VarType is VarType.Short or VarType.ShortArray;
    public bool IsUShort => this.VarType is VarType.UShort or VarType.UShortArray;
    public bool IsInt => this.VarType is VarType.Int or VarType.IntArray;
    public bool IsUInt => this.VarType is VarType.UInt or VarType.UIntArray;
    public bool IsLong => this.VarType is VarType.Long or VarType.LongArray;
    public bool IsULong => this.VarType is VarType.ULong or VarType.ULongArray;
    public bool IsFloat => this.VarType is VarType.Float or VarType.FloatArray;
    public bool IsDouble => this.VarType is VarType.Double or VarType.DoubleArray;
    public bool IsString => this.VarType is VarType.String or VarType.StringArray;

    public static implicit operator VarTypeInfo(VarType type)
    {
        return new VarTypeInfo(type);
    }

    public static implicit operator VarType(VarTypeInfo varTypeInfo)
    {
        return varTypeInfo.VarType;
    }

    /// <summary>
    /// 获取变量类型
    /// </summary>
    /// <param name="varType">变量类型</param>
    /// <returns>变量类型</returns>
    private static Type GetType(VarType varType)
    {
        return varType switch
        {
            VarType.Bool => typeof(bool),
            VarType.Byte => typeof(byte),
            VarType.SByte => typeof(sbyte),
            VarType.Short => typeof(short),
            VarType.UShort => typeof(ushort),
            VarType.Int => typeof(int),
            VarType.UInt => typeof(uint),
            VarType.Long => typeof(long),
            VarType.ULong => typeof(ulong),
            VarType.Float => typeof(float),
            VarType.Double => typeof(double),
            VarType.String => typeof(string),
            VarType.BoolArray => typeof(bool[]),
            VarType.ByteArray => typeof(byte[]),
            VarType.SByteArray => typeof(sbyte[]),
            VarType.ShortArray => typeof(short[]),
            VarType.UShortArray => typeof(ushort[]),
            VarType.IntArray => typeof(int[]),
            VarType.UIntArray => typeof(uint[]),
            VarType.LongArray => typeof(long[]),
            VarType.ULongArray => typeof(ulong[]),
            VarType.FloatArray => typeof(float[]),
            VarType.DoubleArray => typeof(double[]),
            VarType.StringArray => typeof(string[]),
            _ => throw new InvalidOperationException($"不支持的VarType类型{varType}"),
        };
    }

    /// <summary>
    /// 获取变量元素类型
    /// </summary>
    /// <param name="varType">变量类型</param>
    /// <returns>变量元素类型</returns>
    private static Type GetElementType(VarType varType)
    {
        return varType switch
        {
            VarType.Bool or VarType.BoolArray => typeof(bool),
            VarType.SByte or VarType.SByteArray => typeof(sbyte),
            VarType.Byte or VarType.ByteArray => typeof(byte),
            VarType.Short or VarType.ShortArray => typeof(short),
            VarType.UShort or VarType.UShortArray => typeof(ushort),
            VarType.Int or VarType.IntArray => typeof(int),
            VarType.UInt or VarType.UIntArray => typeof(uint),
            VarType.Long or VarType.LongArray => typeof(long),
            VarType.ULong or VarType.ULongArray => typeof(ulong),
            VarType.Float or VarType.FloatArray => typeof(float),
            VarType.Double or VarType.DoubleArray => typeof(double),
            VarType.String or VarType.StringArray => typeof(string),
            _ => throw new InvalidOperationException($"不支持的VarType类型{varType}"),
        };
    }

    private static VarType GetElementVarType(VarType varType)
    {
        return varType switch
        {
            VarType.Bool or VarType.BoolArray => VarType.Bool,
            VarType.SByte or VarType.SByteArray => VarType.SByte,
            VarType.Byte or VarType.ByteArray => VarType.Byte,
            VarType.Short or VarType.ShortArray => VarType.Short,
            VarType.UShort or VarType.UShortArray => VarType.UShort,
            VarType.Int or VarType.IntArray => VarType.Int,
            VarType.UInt or VarType.UIntArray => VarType.UInt,
            VarType.Long or VarType.LongArray => VarType.Long,
            VarType.ULong or VarType.ULongArray => VarType.ULong,
            VarType.Float or VarType.FloatArray => VarType.Float,
            VarType.Double or VarType.DoubleArray => VarType.Double,
            VarType.String or VarType.StringArray => VarType.String,
            _ => throw new InvalidOperationException($"不支持的VarType类型{varType}"),
        };
    }

    public static VarTypeInfo GetVarTypeInfo(Type type)
    {
        static bool IsArray<T>(Type type)
        {
            return type.IsCollection() && type.GetElementType() == typeof(T);
        }

        VarType result = VarType.None;
#pragma warning disable IDE0011 // 添加大括号
        if (type == typeof(bool)) result = VarType.Bool;
        else if (type == typeof(sbyte)) result = VarType.SByte;
        else if (type == typeof(byte)) result = VarType.Byte;
        else if (type == typeof(short)) result = VarType.Short;
        else if (type == typeof(ushort)) result = VarType.UShort;
        else if (type == typeof(int)) result = VarType.Int;
        else if (type == typeof(uint)) result = VarType.UInt;
        else if (type == typeof(long)) result = VarType.Long;
        else if (type == typeof(ulong)) result = VarType.ULong;
        else if (type == typeof(float)) result = VarType.Float;
        else if (type == typeof(double)) result = VarType.Double;
        else if (type == typeof(string)) result = VarType.String;
        else if (IsArray<bool>(type)) result = VarType.BoolArray;
        else if (IsArray<sbyte>(type)) result = VarType.SByteArray;
        else if (IsArray<byte>(type)) result = VarType.ByteArray;
        else if (IsArray<short>(type)) result = VarType.ShortArray;
        else if (IsArray<ushort>(type)) result = VarType.UShortArray;
        else if (IsArray<int>(type)) result = VarType.IntArray;
        else if (IsArray<uint>(type)) result = VarType.UIntArray;
        else if (IsArray<long>(type)) result = VarType.LongArray;
        else if (IsArray<ulong>(type)) result = VarType.ULongArray;
        else if (IsArray<float>(type)) result = VarType.FloatArray;
        else if (IsArray<double>(type)) result = VarType.DoubleArray;
        else if (IsArray<string>(type)) result = VarType.StringArray;
#pragma warning restore IDE0011 // 添加大括号
        return result;
    }
}
