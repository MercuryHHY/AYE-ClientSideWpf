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

using Polly;
using Polly.Timeout;

namespace Microsoft.Extensions.DataChannel.Manager;

public interface IDataChannelProvider
{
    bool Open();

    bool IsConnected { get; }

    IVarReader VarReader { get; }

    IVarWriter VarWriter { get; }

    void Close();

    VarValue Read(VarAddress address);

    VarValue<TValue> Read<TValue>(VarAddress address);

    VarValue[] Read(IEnumerable<VarAddress> addresss);

    void Write(VarAddress address, object value);

    void Write(IDictionary<VarAddress, object> addresses);
}

public abstract class DataChannelProviderBase : IDataChannelProvider
{
    private readonly ILogger? _logger;

    protected DataChannelProviderBase(IDataChannelProviderSource source)
    {
        this._logger = ServiceLocator.Current?.GetService(typeof(ILogger<>).MakeGenericType(this.GetType())) as ILogger;
        this.Source = source;

        if (source.DataChannelInfo.IsRWSplitting)
        {
            this.VarReader = source.CreateVarReader(this.CreateChannel()) ?? throw new Exception();
            this.VarWriter = source.CreateVarWriter(this.CreateChannel()) ?? throw new Exception();
        }
        else
        {
            IDataChannel channel = this.CreateChannel();
            this.VarReader = source.CreateVarReader(channel) ?? throw new Exception();
            this.VarWriter = source.CreateVarWriter(channel) ?? throw new Exception();
        }
    }

    protected IDataChannelProviderSource Source { get; }

    public IVarReader VarReader { get; }

    public IVarWriter VarWriter { get; }

    public bool IsConnected =>
        this.VarReader != null && this.VarReader.Channel != null && this.VarReader.Channel.IsConnected
        &&
        (!this.Source.DataChannelInfo.IsRWSplitting ||
            (this.VarWriter != null && this.VarWriter.Channel != null && this.VarWriter.Channel.IsConnected));

    protected abstract IDataChannel CreateChannel();

    public virtual bool Open()
    {
        this.VarReader.Open();
        this.VarWriter.Open();
        return true;
    }

    public virtual void Close()
    {
        this.VarReader.Close();
        this.VarWriter.Close();
    }

    public VarValue Read(VarAddress address)
    {
        return this.ReadRetry(
            this.VarReader.Channel,
            () => this.VarReader.Read(address),
            this.Source.DataChannelInfo.RetryCount, this.Source.DataChannelInfo.RetryTimeout);
    }

    public VarValue<TValue> Read<TValue>(VarAddress address)
    {
        return this.ReadRetry(
            this.VarReader.Channel,
            () => this.VarReader.Read<TValue>(address),
            this.Source.DataChannelInfo.RetryCount, this.Source.DataChannelInfo.RetryTimeout);
    }

    public VarValue[] Read(IEnumerable<VarAddress> addresss)
    {
        return this.ReadRetry(
            this.VarReader.Channel,
            () => this.VarReader.Read(addresss),
            this.Source.DataChannelInfo.RetryCount, this.Source.DataChannelInfo.RetryTimeout);
    }

    public void Write(VarAddress address, object value)
    {
        this.WriteRetry(
            this.VarWriter.Channel,
            () => this.VarWriter.Write(address, value),
            this.Source.DataChannelInfo.RetryCount, this.Source.DataChannelInfo.RetryTimeout);
    }

    public void Write(IDictionary<VarAddress, object> addresses)
    {
        this.WriteRetry(
            this.VarWriter.Channel,
            () => this.VarWriter.Write(addresses),
            this.Source.DataChannelInfo.RetryCount, this.Source.DataChannelInfo.RetryTimeout);
    }

    protected virtual void WriteRetry(IDataChannel dataChannel, Action writerFunc, int retryCount = 3, int timeoutSeconds = 10)
    {
        Policy.Wrap(
            Policy.Timeout(timeoutSeconds, TimeoutStrategy.Pessimistic),
            Policy.Handle<Exception>().Retry(retryCount, (ex, count, context) =>
            {
                this._logger?.LogWarning($"{dataChannel.Name}数据写入异常，异常信息为:{ex}");

                if (!dataChannel.IsConnected)
                {
                    this.OnRetry(dataChannel, ex, count, context);
                }
            }))
            .Execute(() => writerFunc.Invoke());
    }

    protected virtual VarValue ReadRetry(IDataChannel dataChannel, Func<VarValue> readerFunc, int retryCount = 3, int timeoutSeconds = 10)
    {
        return Policy.Wrap(
            Policy.Timeout(timeoutSeconds, TimeoutStrategy.Pessimistic),
            Policy.Handle<Exception>().Retry(retryCount, (ex, count, context) =>
            {
                this._logger?.LogWarning($"{dataChannel.Name}数据读取异常，异常信息为:{ex}");

                if (!dataChannel.IsConnected)
                {
                    this.OnRetry(dataChannel, ex, count, context);
                }
            }))
            .Execute(() => readerFunc.Invoke());
    }

    protected virtual VarValue<TValue> ReadRetry<TValue>(IDataChannel dataChannel, Func<VarValue<TValue>> readerFunc, int retryCount = 3, int timeoutSeconds = 10)
    {
        return Policy.Wrap(
            Policy.Timeout(timeoutSeconds, TimeoutStrategy.Pessimistic),
            Policy.Handle<Exception>().Retry(retryCount, (ex, count, context) =>
            {
                this._logger?.LogWarning($"{dataChannel.Name}数据读取异常，异常信息为:{ex}");

                if (!dataChannel.IsConnected)
                {
                    this.OnRetry(dataChannel, ex, count, context);
                }
            }))
            .Execute(() => readerFunc.Invoke());
    }

    protected virtual void OnRetry(IDataChannel dataChannel, Exception exception, int retryCount, Context keyValuePairs)
    {
        this._logger?.LogWarning($"{dataChannel.Name}断开连接，正在开始尝试第{retryCount}次重连...");
        dataChannel.Close();
        dataChannel.Open();
        this._logger?.LogWarning($"{dataChannel.Name}尝试重连完成，重连{(dataChannel.IsConnected ? "成功" : "失败")}");
    }

    protected virtual VarValue[] ReadRetry(IDataChannel dataChannel, Func<VarValue[]> readerFunc, int retryCount = 3, int timeoutSeconds = 10)
    {
        return Policy.Wrap(
            Policy.Timeout(timeoutSeconds, TimeoutStrategy.Pessimistic),
            Policy.Handle<Exception>().Retry(retryCount, (ex, count, context) =>
            {
                this._logger?.LogWarning($"{dataChannel.Name}数据读取异常，异常信息为:{ex}");

                if (!dataChannel.IsConnected)
                {
                    this.OnRetry(dataChannel, ex, count, context);
                }
            }))
            .Execute(() => readerFunc.Invoke());
    }

}