

using Microsoft.Extensions.DataChannel.ComponentModel;
using Microsoft.Extensions.DataChannel.Data;
using Microsoft.Extensions.DataChannel.Exceptions;

namespace Microsoft.Extensions.DataChannel;

public abstract class DataChannelBase : EventDisposableBase, IDataChannel
{
    private bool _isConnected;
    private long _readErrorCount;
    private long _writeErrorCount;
    private ConnectionStrings _connectionString = new ConnectionStrings();
    private readonly ConcurrentDictionary<VarType, DataEncoder> _dict = new ConcurrentDictionary<VarType, DataEncoder>();

    protected DataChannelBase(string name, ConnectionStrings connectionString)
    {
        this.Name = name;
        this.ConnectionString = connectionString;
#if NET7_0_OR_GREATER
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
    }

    public IDataChannelListener? Listener { get; set; }

    public Guid Id { get; } = SequentialGuidGenerator.Create();

    public Encoding StringEncoding { get; protected set; }

    public string Name { get; set; }

    public ConnectionStrings ConnectionString
    {
        get => this._connectionString;
        set
        {
            //if (this.IsConnected)
            //{
            //    throw new InvalidOperationException("连接已经打开，无法修改连接字符串。");
            //}
            this._connectionString = value;
            this.OnConnectionStringChanged();
        }
    }

    public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

    public long ReadErrorCount => this._readErrorCount;

    public long WriteErrorCount => this._writeErrorCount;

    public virtual bool IsConnected
    {
        get => this._isConnected;
        protected set
        {
            if (this._isConnected != value)
            {
                this._isConnected = value;
                this.Listener?.OnConnectionStatusChangedAsync(this, value).Wait();
                this.OnConnectionStatusChanged(value);
            }
        }
    }

    protected virtual void OnConnectionStatusChanged(bool connectionStatus)
    {
        this.ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs(connectionStatus));
    }

    protected abstract void Open();

    protected abstract void Close();

    protected abstract TValue[] Read<TValue>(VarAddress varAddress, DataEncoder dataEncoder);

    protected abstract void Write<TValue>(VarAddress varAddress, DataEncoder dataEncoder, IEnumerable<TValue> values);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ((IDataChannel)this).Close();
        }
        base.Dispose(disposing);
    }


    IDataChannel IDataChannel.Open()
    {
        this.ThrowIfDisposed();
        try
        {
            this.Listener?.OpenningAsync(this).Wait();
            this.Open();
            this.IsConnected = true;
            this.Listener?.OpenedAsync(this).Wait();
        }
        catch (Exception ex)
        {
            this.IsConnected = false;
            this.Listener?.OpenExceptionAsync(this, ex).Wait();
            throw new DataChannelOpenException(this, ex);
        }
        return this;
    }

    IDataChannel IDataChannel.Close()
    {
        this.ThrowIfDisposed();
        try
        {
            this.Listener?.ClosingAsync(this).Wait();
            this.Close();
            this.IsConnected = false;
            this.Listener?.ClosedAsync(this).Wait();
        }
        catch (Exception ex)
        {
            this.Listener?.ClosedExceptionAsync(this, ex).Wait();
            throw new DataChannelCloseException(this, ex);
        }
        return this;
    }

    TValue[] IDataChannel.Read<TValue>(VarAddress varAddress)
    {
        this.ThrowIfDisposed();

        try
        {
            this.Listener?.ReadingAsync(this, varAddress).Wait();

            TValue[] values = this.Read<TValue>(varAddress, this.GetDataEncoder(varAddress));
            this.AssertValuesCount(varAddress, values);
            this.IsConnected = true;

            this.Listener?.ReadedAsync(this, varAddress, values.Cast<object>().ToArray()).Wait();
            this.OnReadSuccess(varAddress);
            return values;
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref this._readErrorCount);
            this.IsConnected = false;
            this.OnReadException(varAddress, ex);
            this.Listener?.ReadExceptionAsync(this, varAddress, ex).Wait();
            throw new DataChannelReadException(this, varAddress, ex);
        }
    }

    void IDataChannel.Write<TValue>(VarAddress varAddress, IEnumerable<TValue> values)
    {
        this.ThrowIfDisposed();
        try
        {
            this.Listener?.WritingAsync(this, varAddress, values.Cast<object>().ToArray()).Wait();

            this.AssertValuesCount(varAddress, values);
            this.Write(varAddress, this.GetDataEncoder(varAddress), values);
            this.IsConnected = true;
            this.OnWriteSuccess(varAddress);
            this.Listener?.WritedAsync(this, varAddress).Wait();
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref this._writeErrorCount);
            this.OnWriteException(varAddress, ex);
            this.Listener?.WriteExceptionAsync(this, varAddress, values.Cast<object>().ToArray(), ex).Wait();
            throw new DataChannelWriteException(this, varAddress, ex);
        }
    }

    protected virtual void OnReadSuccess(VarAddress varAddress)
    {
    }

    protected virtual void OnReadException(VarAddress varAddress, Exception ex)
    {
    }

    protected virtual void OnWriteSuccess(VarAddress varAddress)
    {
    }

    protected virtual void OnWriteException(VarAddress varAddress, Exception ex)
    {
    }

    protected virtual void OnConnectionStringChanged()
    {
        this.StringEncoding = this.GetOrDefault(nameof(this.StringEncoding), Encoding.GetEncoding("us-ascii"));
    }


    protected TValue GetOrDefault<TValue>(string propertyName, TValue defaultValue)
    {
        return this._connectionString.GetOrDefault(propertyName, defaultValue);
    }

    protected TValue Get<TValue>(string propertyName)
    {
        return this._connectionString.Get<TValue>(propertyName);
    }

    protected virtual void AssertValuesCount<TValue>(VarAddress address, IEnumerable<TValue> values)
    {
        if (typeof(TValue) != typeof(string) && values.Count() != address.Count)
        {
            throw new InvalidOperationException("地址长度与数据长度不符");
        }
    }

    public DataEncoder GetDataEncoder(VarAddress varAddress)
    {
        return this.GetDataEncoder(varAddress.VarTypeInfo.ElementVarType);
    }

    public DataEncoder GetDataEncoder(VarType  varType)
    {
        if (this._dict.TryGetValue(varType, out DataEncoder? dataEncoder))
        {
            if (dataEncoder.StringEncoding == this.StringEncoding)
            {
                return dataEncoder;
            }
            else
            {
                dataEncoder = DataEncoder.Create(dataEncoder.DataEncordingType, this.StringEncoding);
                this._dict[varType] = dataEncoder;
                return dataEncoder;
            }
        }
        else
        {
            string elementVarTypeName = varType.ToString();
            DataEncordingType dataEncordingType = DataEncordingType.CDAB;
            if (this._connectionString.HasExtraProperty(elementVarTypeName))
            {
                dataEncordingType = this.Get<DataEncordingType>(elementVarTypeName);
            }
            else if (this._connectionString.HasExtraProperty("ByteOrder"))
            {
                dataEncordingType = this.Get<DataEncordingType>("ByteOrder");
            }
            dataEncoder = DataEncoder.Create(dataEncordingType, this.StringEncoding);
            this._dict.TryAdd(varType, dataEncoder);
            return dataEncoder;
        }
    }
}