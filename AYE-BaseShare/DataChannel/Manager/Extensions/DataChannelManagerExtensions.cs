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

namespace Microsoft.Extensions.DataChannel.Manager.Extensions;

public static class DataChannelManagerExtensions
{
    public static IDataChannelProvider? FindChannelProvider(this IDataChannelManager dc, DataChannelInfo info)
    {
        return dc.FindChannelProvider(info.Code);
    }

    public static bool IsOpen(this IDataChannelManager dc, DataChannelInfo info)
    {
        return dc.IsOpen(info.Code);
    }

    public static bool Close(this IDataChannelManager dc, DataChannelInfo info)
    {
        return dc.Close(info.Code);
    }

    public static bool IsConnected(this IDataChannelManager dc, DataChannelInfo info)
    {
        return dc.IsConnected(info.Code);
    }

    public static VarValue Read(this IDataChannelManager dc, DataChannelInfo info, VarAddress varAddress)
    {
        return dc.Read(info.Code, varAddress);
    }

    public static VarValue<TValue> Read<TValue>(this IDataChannelManager dc, DataChannelInfo info, VarAddress varAddress)
    {
        return dc.Read<TValue>(info.Code, varAddress);
    }

    public static VarValue[] Read(this IDataChannelManager dc, DataChannelInfo info, IEnumerable<VarAddress> addresss)
    {
        return dc.Read(info.Code, addresss);
    }

    public static void Write(this IDataChannelManager dc, DataChannelInfo info, VarAddress varAddress, object value)
    {
        dc.Write(info.Code, varAddress, value);
    }

    public static void Write(this IDataChannelManager dc, DataChannelInfo info, IDictionary<VarAddress, object> addresses)
    {
        dc.Write(info.Code, addresses);
    }
}