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

using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel;

internal static class PLCTypeParseCache
{
    internal static Dictionary<string, PLCTypeParse> Cache = new Dictionary<string, PLCTypeParse>(StringComparer.OrdinalIgnoreCase)
    {
        { "BOOL", new PLCTypeParse("BOOL")},
        { "BYTE", new PLCTypeParse("BYTE")},
        { "INT",new PLCTypeParse("INT")},
        { "UInt", new PLCTypeParse("UInt")},
        { "WORD", new PLCTypeParse("WORD")},
        { "DInt", new PLCTypeParse("DInt")},
        { "DWord", new PLCTypeParse("DWord")},
        { "UDINT",new PLCTypeParse("UDINT")},
        { "LInt", new PLCTypeParse("LInt")},
        { "LWORD",new PLCTypeParse("LWORD")},
        { "ULInt", new PLCTypeParse("ULInt")},
        { "REAL",new PLCTypeParse("REAL")},
        { "LReal",new PLCTypeParse("LReal")},
        { "String",new PLCTypeParse("String")},
        { "WString",new PLCTypeParse("WString")},
    };
}

/// <summary>
/// <![CDATA[
/// |---------------|-----------------|
/// |  PLC数据类型  |   C#数据类型    |
/// |---------------|-----------------|
/// |    BOOL       |      bool       |
/// |    BYTE       |      byte       |
/// |    INT        |      short      |
/// |    WORD       |      ushort     |
/// |    DINT       |      int        |
/// |    LINT       |      long       |
/// |    REAL       |      float      |
/// |    LREAL      |      double     |
/// |    STRING     |      string     |
/// |    WSTRING    |      string     |
/// |---------------|-----------------|
/// 数组 ARRAY[0..89] OF PLC数据类型 。比如 ARRAY[0..89] OF BOOL
/// ]]>
/// </summary>
public class PLCTypeParse
{
    public static PLCTypeParse Parse(string plcDataType)
    {
        return PLCTypeParseCache.Cache.TryGetValue(plcDataType, out PLCTypeParse? pLCTypeParse) ? pLCTypeParse : new PLCTypeParse(plcDataType);
    }

    internal PLCTypeParse(string plcDataType)
    {
        this.DataType = plcDataType.Trim();
        if (this.IsArray)
        {
            //ARRAY[0..89] OF BOOL
            string[] vs = this.DataType.ToUpper().Split("OF");
            this.ChildDataType = vs[1].Trim();
            this.Child = new PLCTypeParse(this.ChildDataType, this);
        }
        else
        {
            this.IsArray = this.DataType.StartsWith("ARRAY", StringComparison.OrdinalIgnoreCase);
            this.IsBool = this.DataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase);
            this.IsByte = this.DataType.StartsWith("BYTE", StringComparison.OrdinalIgnoreCase);
            this.IsInt = this.DataType.StartsWith("INT", StringComparison.OrdinalIgnoreCase);
            this.IsUInt = this.DataType.StartsWith("UInt", StringComparison.OrdinalIgnoreCase);
            this.IsWord = this.DataType.StartsWith("WORD", StringComparison.OrdinalIgnoreCase);
            this.IsDInt = this.DataType.StartsWith("DInt", StringComparison.OrdinalIgnoreCase);
            this.IsDWord = this.DataType.StartsWith("DWord", StringComparison.OrdinalIgnoreCase);
            this.IsUDInt = this.DataType.StartsWith("UDINT", StringComparison.OrdinalIgnoreCase);
            this.IsLInt = this.DataType.StartsWith("LInt", StringComparison.OrdinalIgnoreCase);
            this.IsLWord = this.DataType.StartsWith("LWORD", StringComparison.OrdinalIgnoreCase);
            this.IsULInt = this.DataType.StartsWith("ULInt", StringComparison.OrdinalIgnoreCase);
            this.IsReal = this.DataType.StartsWith("REAL", StringComparison.OrdinalIgnoreCase);
            this.IsLReal = this.DataType.StartsWith("LReal", StringComparison.OrdinalIgnoreCase);
            this.IsString = this.DataType.StartsWith("String", StringComparison.OrdinalIgnoreCase);
            this.IsWString = this.DataType.StartsWith("WString", StringComparison.OrdinalIgnoreCase);
        }
    }

    private PLCTypeParse(string plcDataType, PLCTypeParse parent) : this(plcDataType)
    {
        this.Parent = parent;
    }

    public PLCTypeParse? Parent { get; }

    public PLCTypeParse? Child { get; }

    public string ChildDataType { get; }

    public string DataType { get; }

    /// <summary>
    /// ARRAY[0..89] OF WORD
    /// </summary>
    public bool IsArray { get; }
    public bool IsBool { get; }
    public bool IsByte { get; }
    public bool IsInt { get; }
    public bool IsUInt { get; }
    public bool IsWord { get; }
    public bool IsDInt { get; }
    public bool IsDWord { get; }
    public bool IsUDInt { get; }
    public bool IsLInt { get; }
    public bool IsLWord { get; }
    public bool IsULInt { get; }
    public bool IsReal { get; }
    public bool IsLReal { get; }
    public bool IsString { get; }
    public bool IsWString { get; }
    public uint Length => this.GetLength();
    public uint ByteSize => this.GetByteSize();
    public VarTypeInfo VarTypeInfo => VarTypeInfo.GetVarTypeInfo(this.CSharpType);
    public Type CSharpType => this.GetCSharpType();
    private uint GetLength()
    {
        if (this.IsString || this.IsWString)
        {
            string value = this.DataType.Split("[")[1].Replace(']', '\0').Replace('[', newChar: '\0');
            return Convert.ToUInt32(value);
        }
        else if (this.IsArray)
        {
            string value = this.DataType.Split(' ')[0].Split('.')[2].Replace(']', '\0');
            return Convert.ToUInt32(value);
        }
        return 1;
    }

    private uint GetByteSize()
    {
#pragma warning disable IDE0011 // 添加大括号
#pragma warning disable IDE0046 // 转换为条件表达式
        if (this.IsByte) return sizeof(byte);
        else if (this.IsInt) return sizeof(short);
        else if (this.IsUInt) return sizeof(ushort);
        else if (this.IsWord) return sizeof(ushort);
        else if (this.IsDInt) return sizeof(int);
        else if (this.IsDWord) return sizeof(uint);
        else if (this.IsUDInt) return sizeof(uint);
        else if (this.IsLInt) return sizeof(long);
        else if (this.IsLWord) return sizeof(ulong);
        else if (this.IsULInt) return sizeof(ulong);
        else if (this.IsReal) return sizeof(float);
        else return this.IsLReal
            ? sizeof(double)
            : this.IsString
            ? this.Length
            : this.IsWString
            ? this.Length
            : this.IsArray ? this.Child!.ByteSize * this.Length : throw new ApplicationException($"不支持{this.DataType}的CSharp类型获取");
#pragma warning restore IDE0046 // 转换为条件表达式
#pragma warning restore IDE0011 // 添加大括号
    }

    private Type GetCSharpType()
    {
#pragma warning disable IDE0011 // 添加大括号
#pragma warning disable IDE0046 // 转换为条件表达式

        if (this.IsBool) return typeof(bool);
        else if (this.IsByte) return typeof(byte);
        else if (this.IsInt) return typeof(short);
        else if (this.IsUInt) return typeof(ushort);
        else if (this.IsWord) return typeof(ushort);
        else if (this.IsDInt) return typeof(int);
        else if (this.IsDWord) return typeof(uint);
        else if (this.IsUDInt) return typeof(uint);
        else if (this.IsLInt) return typeof(long);
        else if (this.IsULInt) return typeof(ulong);
        else if (this.IsULInt) return typeof(ulong);
        else if (this.IsReal) return typeof(float);
        else return this.IsLReal
            ? typeof(double)
            : this.IsString
            ? typeof(string)
            : this.IsWString
            ? typeof(string)
            : this.IsArray ? this.Child!.CSharpType.MakeArrayType() : throw new ApplicationException($"不支持{this.DataType}的CSharp类型获取");
#pragma warning restore IDE0046 // 转换为条件表达式
#pragma warning restore IDE0011 // 添加大括号
    }
}