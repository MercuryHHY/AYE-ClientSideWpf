#region << License >>

#endregion

namespace Microsoft.Extensions.DataChannel.Data;

[DebuggerDisplay("{Name}，Value = {Value}, Address = {Address}, VarType = {VarType}, Count = {Count}")]
public class VarValue(VarAddress varAddress, object value)
{
    public TimeSpan? ExecuteTime { get; private set; }

    public object Value { get; internal set; } = value;

    public string Name => varAddress.Name;

    public DataAddress Address => varAddress.Address;

    public VarType Type => varAddress.Type;

    public VarTypeInfo VarTypeInfo => varAddress.VarTypeInfo;

    public uint Count => varAddress.Count;

    public IReadOnlyDictionary<string, object> ExtraDatas => varAddress.ExtraDatas;

    public VarValue WithExecuteTime(TimeSpan executeTime)
    {
        this.ExecuteTime = executeTime;
        if(executeTime.TotalMilliseconds > 100)
        {
            Console.WriteLine($"{this.Name} {executeTime.TotalMilliseconds} ms");
        }
        return this;
    }

    public VarValue<TValue> Cast<TValue>()
    {
        return typeof(TValue) != this.Value.GetType()
            ? throw new ArgumentException($"泛型类型错误，返回值类型与泛型不一致，返回值类型为{this.Value.GetType()}，泛型为{typeof(TValue)}")
            : !(typeof(TValue).IsValueType || typeof(TValue) == typeof(string) || typeof(TValue).IsArray)
            ? throw new ArgumentException("泛型类型错误，支持基本类型和基本类型数组，List以及抽象接口类型均不受支持", nameof(TValue))
            : new VarValue<TValue>(this, this.Value);
    }
}


[DebuggerDisplay("{Name}，Value = {Value}, Address = {Address}, VarType = {VarType}, Count = {Count}")]
public class VarValue<TValue>(VarValue varValue, object value)
{
    public VarValue<TValue[]> ToArray()
    {
        return new VarValue<TValue[]>(varValue, new TValue[] { this.Value });
    }

    public TimeSpan? ExecuteTime => varValue.ExecuteTime;

    public TValue Value { get; internal set; } = (TValue)value;

    public string Name => varValue.Name;

    public DataAddress Address => varValue.Address;

    public VarType Type => varValue.Type;

    public VarTypeInfo VarTypeInfo => varValue.VarTypeInfo;

    public uint Count => varValue.Count;

    public IReadOnlyDictionary<string, object> ExtraDatas => varValue.ExtraDatas;
}