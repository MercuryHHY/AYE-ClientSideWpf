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
/// 数据编码器
/// </summary>
public class DataEncoder
{
    public delegate byte[] ValuesConvertToBytesHandler(IEnumerable<object> values);
    public delegate object[] BytesConvertToValuessHandler(IEnumerable<byte> bytes, int bytesStartIndex, int valueCount);

    private readonly Dictionary<Type, ValuesConvertToBytesHandler> _vs_2_Bytes;
    private readonly Dictionary<Type, BytesConvertToValuessHandler> _bys_2_Vs;

    public static DataEncoder Create(DataEncordingType dataEncordingType = DataEncordingType.CDAB, Encoding? encoding = null)
    {
        return new DataEncoder(dataEncordingType, encoding ?? Encoding.ASCII);
    }

    private DataEncoder(DataEncordingType dataEncordingType, Encoding encoding)
    {
        this._vs_2_Bytes = new Dictionary<Type, ValuesConvertToBytesHandler>
        {
            [typeof(bool)] = values => this.GetBytes(values, e => [(byte)((bool)e ? 1 : 0)]),
            [typeof(byte)] = values => this.GetBytes(values, e => [(byte)e]),
            [typeof(sbyte)] = values => this.GetBytes(values, e => [(byte)(sbyte)e]),
            [typeof(short)] = values => this.GetBytes(values, e => BitConverter.GetBytes((short)e)),
            [typeof(ushort)] = values => this.GetBytes(values, e => BitConverter.GetBytes((ushort)e)),
            [typeof(int)] = values => this.GetBytes(values, e => BitConverter.GetBytes((int)e)),
            [typeof(uint)] = values => this.GetBytes(values, e => BitConverter.GetBytes((uint)e)),
            [typeof(long)] = values => this.GetBytes(values, e => BitConverter.GetBytes((long)e)),
            [typeof(ulong)] = values => this.GetBytes(values, e => BitConverter.GetBytes((ulong)e)),
            [typeof(float)] = values => this.GetBytes(values, e => BitConverter.GetBytes((float)e)),
            [typeof(double)] = values => this.GetBytes(values, e => BitConverter.GetBytes((double)e)),
            [typeof(string)] = values =>
            {
                string? value = values.SingleOrDefault()?.ToString();
                return string.IsNullOrWhiteSpace(value) ? (new byte[this.GetByteCount<string>()]) : this.StringEncoding!.GetBytes(value);
            }
        };

        this._bys_2_Vs = new Dictionary<Type, BytesConvertToValuessHandler>
        {
            [typeof(bool)] = (bytes, index, count) => this.GetValues(bytes, index, count, sizeof(bool), buffer => buffer[0] == 1),
            [typeof(byte)] = (bytes, index, count) => this.GetValues(bytes, index, count, sizeof(byte), buffer => buffer[0]),
            [typeof(sbyte)] = (bytes, index, count) => this.GetValues(bytes, index, count, sizeof(sbyte), buffer => (sbyte)buffer[0]),
            [typeof(short)] = (bytes, index, count) => this.GetValues(bytes, index, count, sizeof(short), buffer => BitConverter.ToInt16(buffer, 0)),
            [typeof(ushort)] = (bytes, index, count) => this.GetValues(bytes, index, count, sizeof(ushort), buffer => BitConverter.ToUInt16(buffer, 0)),
            [typeof(int)] = (bytes, index, count) => this.GetValues(bytes, index, count, sizeof(int), buffer => BitConverter.ToInt32(buffer, 0)),
            [typeof(uint)] = (bytes, index, count) => this.GetValues(bytes, index, count, sizeof(uint), buffer => BitConverter.ToUInt32(buffer, 0)),
            [typeof(long)] = (bytes, index, count) => this.GetValues(bytes, index, count, sizeof(long), buffer => BitConverter.ToInt64(buffer, 0)),
            [typeof(ulong)] = (bytes, index, count) => this.GetValues(bytes, index, count, sizeof(ulong), buffer => BitConverter.ToUInt64(buffer, 0)),
            [typeof(float)] = (bytes, index, count) => this.GetValues(bytes, index, count, sizeof(float), buffer => BitConverter.ToSingle(buffer, 0)),
            [typeof(double)] = (bytes, index, count) => this.GetValues(bytes, index, count, sizeof(double), buffer => BitConverter.ToDouble(buffer, 0)),
            [typeof(string)] = (bytes, index, count) =>
            {
                string? result = this.StringEncoding!.GetString(bytes.ToArray(), index, count);
                return [result.Split('\0')?.First() ?? string.Empty];
            }
        };

        this.StringEncoding = encoding;
        this.DataEncordingType = dataEncordingType;
    }

    /// <summary>
    /// 字符串编码
    /// </summary>
    public Encoding StringEncoding { get; }

    /// <summary>
    /// 数据编码类型
    /// </summary>
    public DataEncordingType DataEncordingType { get; }

    public void GetBytesFuncAddOrUpdate(Type type, ValuesConvertToBytesHandler func)
    {
        this._vs_2_Bytes.AddOrUpdate(type, func);
    }

    public void GetValuesFuncAddOrUpdate(Type type, BytesConvertToValuessHandler func)
    {
        this._bys_2_Vs.AddOrUpdate(type, func);
    }

    #region GetBytes
    public IEnumerable<byte> GetBytes<TValue>(IEnumerable<TValue> values)
    {
        return this.GetBytes(values.Cast<object>(), typeof(TValue));
    }

    public IEnumerable<byte> GetBytes(IEnumerable<object> values, Type type)
    {
        return this._vs_2_Bytes.ContainsKey(type)
            ? this._vs_2_Bytes[type].Invoke(values)
            : IMSMarshalExtension.GetBytesByClass(values, type);
    }

    private byte[] GetBytes(IEnumerable<object> values, Func<object, byte[]> getBytesFunc)
    {
        List<byte> result = [];

        foreach (object obj in values)
        {
            byte[] buffer = getBytesFunc.Invoke(obj);
            buffer = this.GetBytesByEndian(buffer).ToArray();
            result.AddRange(buffer);
        }

        return [.. result];
    }
    #endregion

    #region GetValues
    public TValue[] GetValues<TValue>(IEnumerable<byte> bytes, int bytesStartIndex, int valueCount)
    {
        return this.GetValues(bytes, bytesStartIndex, valueCount, typeof(TValue)).Cast<TValue>().ToArray();
    }

    public IEnumerable<object> GetValues(IEnumerable<byte> bytes, int bytesStartIndex, int valueCount, Type type)
    {
        return this._bys_2_Vs.ContainsKey(type)
            ? this._bys_2_Vs[type](bytes, bytesStartIndex, valueCount)
            : IMSMarshalExtension.GetValuesByClass(bytes, bytesStartIndex, valueCount, type);
    }

    private object[] GetValues(IEnumerable<byte> bytes, int index, int count, int size, Func<byte[], object> toFunc)
    {
        byte[] valuesBuffer = bytes.Skip(index).ToArray();
        List<object> result = [];

        if (valuesBuffer.Length < size)
        {
            throw new ApplicationException($"数据解析错误，数据字节异常，当前字节数为{valuesBuffer.Length}，最小字节数为{size}");
        }

        for (int i = 0; i < count / size; i++)
        {
            byte[] buffer = new byte[size];
            Array.Copy(valuesBuffer, i * size, buffer, 0, size);
            buffer = this.GetBytesByEndian(buffer).ToArray();
            result.Add(toFunc(buffer));
        }
        return [.. result];
    }
    #endregion

    #region GetByteCount
    public int GetByteCount<TValue>(int count = 1)
    {
        return this.GetByteCount(count, typeof(TValue));
    }

    public int GetByteCount(int count, Type type)
    {
        return IMSTypeExtensions.Sizeof(type, this.StringEncoding) * count;
    }
    #endregion

    /// <summary>
    /// 根据字节序获取字节，仅支持2、4、8字节
    /// </summary>
    public IEnumerable<byte> GetBytesByEndian(IEnumerable<byte> bytes)
    {
        byte[] buffer = bytes.ToArray();

        switch (buffer.Length)
        {
            case 2:
                buffer = this.Get2Bytes(buffer);
                break;
            case 4:
                buffer = this.Get4Bytes(buffer);
                break;
            case 8:
                buffer = this.Get8Bytes(buffer);
                break;
            default:
                break;
        }

        if (!BitConverter.IsLittleEndian)
        {
            buffer = buffer.Reverse().ToArray();
        }

        return buffer;
    }

    private byte[] Get2Bytes(byte[] buffer)
    {
        byte[] result = new byte[2];
        switch (this.DataEncordingType)
        {
            case DataEncordingType.ABCD:
                result = buffer.Reverse().ToArray();
                break;
            case DataEncordingType.CDAB:
                result[0] = buffer[1];
                result[1] = buffer[0];
                break;
            case DataEncordingType.BADC:
                result[0] = buffer[0];
                result[1] = buffer[1];
                break;
            case DataEncordingType.DCBA:
                Array.Copy(buffer, result, buffer.Length);
                break;
            default:
                break;
        }
        return result;
    }

    private byte[] Get4Bytes(byte[] buffer)
    {
        byte[] result = new byte[4];

        switch (this.DataEncordingType)
        {
            case DataEncordingType.ABCD:
                result = buffer.Reverse().ToArray();
                break;
            case DataEncordingType.BADC:
                result[0] = buffer[2];
                result[1] = buffer[3];
                result[2] = buffer[0];
                result[3] = buffer[1];
                break;
            case DataEncordingType.CDAB:
                result[0] = buffer[1];
                result[1] = buffer[0];
                result[2] = buffer[3];
                result[3] = buffer[2];
                break;
            case DataEncordingType.DCBA:
                Array.Copy(buffer, result, buffer.Length);
                break;
            default:
                break;
        }
        return result;
    }

    private byte[] Get8Bytes(byte[] buffer)
    {
        byte[] result = new byte[8];

        switch (this.DataEncordingType)
        {
            case DataEncordingType.ABCD:
                result = buffer.Reverse().ToArray();
                break;
            case DataEncordingType.BADC:
                result[0] = buffer[6];
                result[1] = buffer[7];
                result[2] = buffer[4];
                result[3] = buffer[5];
                result[4] = buffer[2];
                result[5] = buffer[3];
                result[6] = buffer[0];
                result[7] = buffer[1];
                break;
            case DataEncordingType.CDAB:
                result[0] = buffer[1];
                result[1] = buffer[0];
                result[2] = buffer[3];
                result[3] = buffer[2];
                result[4] = buffer[5];
                result[5] = buffer[4];
                result[6] = buffer[7];
                result[7] = buffer[6];
                break;
            case DataEncordingType.DCBA:
                Array.Copy(buffer, result, buffer.Length);
                break;
            default:
                break;
        }
        return result;
    }

    /// <summary>
    /// 将byte数组按照双字节进行反转，如果为单数的情况，则自动补齐
    /// </summary>
    /// <param name="inBytes">输入的字节信息</param>
    /// <returns>反转后的数据</returns>
    public static byte[]? BytesReverseByWord(byte[] inBytes)
    {
        if (inBytes == null)
        {
            return null;
        }

        if (inBytes.Length == 0)
        {
            return [];
        }
        byte[] buffer = DataEncoder.ArrayExpandToLengthEven(inBytes);

        for (int i = 0; i < buffer.Length / 2; i++)
        {
            (buffer[(i * 2) + 1], buffer[(i * 2) + 0]) = (buffer[(i * 2) + 0], buffer[(i * 2) + 1]);
        }

        return buffer;
    }

    /// <summary>
    /// 将一个数组进行扩充到偶数长度
    /// </summary>
    /// <typeparam name="T">数组的类型</typeparam>
    /// <param name="data">原先数据的数据</param>
    /// <returns>新数组长度信息</returns>
    public static T[] ArrayExpandToLengthEven<T>(IEnumerable<T> data)
    {
        return data == null
            ? ([])
            : data.Count() % 2 == 1
                ? DataEncoder.ArrayExpandToLength(data, data.Count() + 1)
                : data.ToArray();
    }

    /// <summary>
    /// 将一个数组进行扩充到指定长度，或是缩短到指定长度
    /// </summary>
    /// <typeparam name="T">数组的类型</typeparam>
    /// <param name="data">原先数据的数据</param>
    /// <param name="length">新数组的长度</param>
    /// <returns>新数组长度信息</returns>
    public static T[] ArrayExpandToLength<T>(IEnumerable<T> data, int length)
    {
        if (data == null)
        {
            return new T[length];
        }

        if (data.Count() == length)
        {
            return data.ToArray();
        }

        T[] buffer = new T[length];

        Array.Copy(data.ToArray(), buffer, Math.Min(data.Count(), buffer.Length));

        return buffer;
    }
}
