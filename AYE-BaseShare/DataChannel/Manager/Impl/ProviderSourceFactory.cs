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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DataChannel.VarReaders;
using Microsoft.Extensions.DataChannel.VarWriters;

namespace Microsoft.Extensions.DataChannel.Manager.Impl;

internal class ProviderSourceFactory : IProviderSourceFactory
{
    private readonly ConcurrentDictionary<string, Func<IVarReader>> _readerCahce = new ConcurrentDictionary<string, Func<IVarReader>>(StringComparer.CurrentCultureIgnoreCase);
    private readonly ConcurrentDictionary<string, Func<IVarWriter>> _writerCahce = new ConcurrentDictionary<string, Func<IVarWriter>>(StringComparer.CurrentCultureIgnoreCase);
    private readonly ConcurrentDictionary<string, Func<DataChannelProviderSourceBase>> _protocolCahce = new ConcurrentDictionary<string, Func<DataChannelProviderSourceBase>>(StringComparer.CurrentCultureIgnoreCase);

    public ProviderSourceFactory()
    {
        this._readerCahce.TryAdd("VarReaderDirectly", () => new VarReaderDirectly());
        this._writerCahce.TryAdd("VarWriterDirectly", () => new VarWriterDirectly());
    }

    public void AddProtocolFunc(string protocol, Func<DataChannelProviderSourceBase> func)
    {
        ThrowHelper.IfNullOrWhiteSpace(protocol);
        ThrowHelper.IfNull(func);
        this._protocolCahce.TryAdd(protocol, func);
    }

    public void AddReaderFunc(string readStrategy, Func<IVarReader> func)
    {
        ThrowHelper.IfNullOrWhiteSpace(readStrategy);
        ThrowHelper.IfNull(func);
        this._readerCahce.TryAdd(readStrategy, func);
    }

    public void AddWriterFuc(string writerStrategy, Func<IVarWriter> func)
    {
        ThrowHelper.IfNullOrWhiteSpace(writerStrategy);
        ThrowHelper.IfNull(func);
        this._writerCahce.TryAdd(writerStrategy, func);
    }

    public DataChannelProviderSourceBase CreateProviderSource(string protocol)
    {
        return this._protocolCahce.TryGetValue(protocol, out Func<DataChannelProviderSourceBase>? func)
            ? func.Invoke()
            : throw new NotSupportedException($"不支持当前协议{protocol}");
    }

    public IVarReader CreateReader(string readStrategy)
    {
        return this._readerCahce.TryGetValue(readStrategy, out Func<IVarReader>? func)
            ? func.Invoke()
            : throw new NotSupportedException($"不支持当读策略{readStrategy}");
    }

    public IVarWriter CreateWriter(string writerStrategy)
    {
        return this._writerCahce.TryGetValue(writerStrategy, out Func<IVarWriter>? func)
            ? func.Invoke()
            : throw new NotSupportedException($"不支持当前写策略{writerStrategy}");
    }

    public IList<string> GetSupportprotocol()
    {
        return this._protocolCahce.Keys.ToList();
    }

    public IList<string> GetSupportReadStrategy()
    {
        return this._readerCahce.Keys.ToList();
    }

    public IList<string> GetSupportWriterStrategy()
    {
        return this._writerCahce.Keys.ToList();
    }
}