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

namespace Microsoft.Extensions.DataChannel.Manager.Impl;

internal class DataChannelManager : IDataChannelManager
{
    private record DataChannelItem(IDataChannelProvider Provider, DataChannelInfo Info);

    private readonly ConcurrentDictionary<string, DataChannelItem> _cache = new ConcurrentDictionary<string, DataChannelItem>();
    private readonly IProviderSourceFactory _factory;
    private readonly ILogger _logger;

    public DataChannelManager(ILogger<DataChannelManager> logger, IProviderSourceFactory factory)
    {
        this._logger = logger;
        this._factory = factory;
    }

    public IDataChannelProvider? FindChannelProvider(string channelCode)
    {
        this._cache.TryGetValue(channelCode, out DataChannelItem? item);
        return item?.Provider;
    }

    public IReadOnlyCollection<IDataChannelProvider> GetChannelProviders()
    {
        return this._cache.Values.Select(x => x.Provider).AsReadOnly();
    }

    public bool IsOpen(string channelCode)
    {
        return this._cache.ContainsKey(channelCode);
    }

    public bool Open(DataChannelInfo info)
    {
        bool ret = false;
        if (this.IsOpen(info.Code))
        {
            throw new InvalidOperationException("连接已经打开。");
        }
        DataChannelProviderSourceBase sourceBase = this._factory.CreateProviderSource(info.ProtocolType);
        sourceBase.DataChannelInfo = info;
        sourceBase.Factory = this._factory;
        IDataChannelProvider provider = sourceBase.CreateProvider();
        if (provider.Open())
        {
            ret = this._cache.TryAdd(info.Code, new DataChannelItem(provider, info));
        }
        return ret;
    }

    public bool Close(string channelCode)
    {
        bool ret = true;
        if (this.IsOpen(channelCode))
        {
            this._cache[channelCode].Provider.Close();
            ret = this._cache.TryRemove(channelCode, out _);
        }
        return ret;
    }

    public bool IsConnected(string channelCode)
    {
        return this.IsOpen(channelCode) && this._cache[channelCode].Provider.IsConnected;
    }

    public VarValue Read(string channelCode, VarAddress varAddress)
    {
        this.ThrowWhenNotOpen(channelCode);
        return this._cache[channelCode].Provider.Read(varAddress);
    }

    public VarValue[] Read(string channelCode, IEnumerable<VarAddress> addresss)
    {
        this.ThrowWhenNotOpen(channelCode);
        return this._cache[channelCode].Provider.Read(addresss);
    }

    public VarValue<TValue> Read<TValue>(string channelCode, VarAddress varAddress)
    {
        this.ThrowWhenNotOpen(channelCode);
        return this._cache[channelCode].Provider.Read<TValue>(varAddress);
    }

    public void Write(string channelCode, VarAddress varAddress, object value)
    {
        this.ThrowWhenNotOpen(channelCode);
        this._cache[channelCode].Provider.Write(varAddress, value);
    }

    public  void Write(string channelCode, IDictionary<VarAddress, object> addresses)
    {
        this.ThrowWhenNotOpen(channelCode);
        this._cache[channelCode].Provider.Write(addresses);
    }

    protected void ThrowWhenNotOpen(string channelCode)
    {
        if (!this.IsOpen(channelCode))
        {
            throw new DataChannelNotOpenException(channelCode);
        }
    } 
}
