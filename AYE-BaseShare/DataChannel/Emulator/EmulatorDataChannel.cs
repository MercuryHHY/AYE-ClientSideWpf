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


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.Extensions.DataChannel.Data;

namespace Microsoft.Extensions.DataChannel.Emulator;

public class EmulatorDataChannel : DataChannelBase
{
    private static readonly ConcurrentDictionary<string, object> MS_LockDict = new ConcurrentDictionary<string, object>();

    public EmulatorDataChannel(Microsoft.EntityFrameworkCore.Abstractions.DbType dbType, ConnectionStrings connectionString) : this(nameof(EmulatorDataChannel), dbType, connectionString)
    {
    }

    public EmulatorDataChannel(string name, Microsoft.EntityFrameworkCore.Abstractions.DbType dbType, ConnectionStrings connectionString) : base(name, connectionString)
    {
        this.DbType = dbType;
    }

    public Microsoft.EntityFrameworkCore.Abstractions.DbType DbType { get; set; }

    protected override void Open()
    {
        using EmulatorDbContext dbContext = new EmulatorDbContext(this.DbType, this.ConnectionString);
        dbContext.Database.EnsureCreated();
        if (!dbContext.TableIsExists<EmulatorAggregateRoot>())
        {
            dbContext.Database.ExecuteSqlRaw(dbContext.Database.GenerateCreateScript());
        }
    }

    protected override void Close() { }

    protected override TValue[] Read<TValue>(VarAddress varAddress, DataEncoder dataEncoder)
    {
        string lockKey = $"{this.Name ?? string.Empty}-{varAddress.Name}-{varAddress.Address}-{varAddress.Count}-{varAddress.Type}";
        object @lock = MS_LockDict.GetOrAdd(lockKey, new object());
        lock (@lock)
        {
            using EmulatorDbContext dbContext = new EmulatorDbContext(this.DbType, this.ConnectionString);
            DbSet<EmulatorAggregateRoot> dbSet = dbContext.Set<EmulatorAggregateRoot>();
            EmulatorAggregateRoot aggregateRoot = this.GetOrAddEmulatorAggregateRoot<TValue>(dbContext, dbSet, varAddress);
            TValue[] values = JsonConvert.DeserializeObject<TValue[]>(aggregateRoot.Value)!;
            return values;
        }
    }

    protected override void Write<TValue>(VarAddress varAddress, DataEncoder dataEncoder, IEnumerable<TValue> values)
    {
        string lockKey = $"{this.Name ?? string.Empty}-{varAddress.Name}-{varAddress.Address}-{varAddress.Count}-{varAddress.Type}";
        object @lock = MS_LockDict.GetOrAdd(lockKey, new object());
        lock (@lock)
        {
            using EmulatorDbContext dbContext = new EmulatorDbContext(this.DbType, this.ConnectionString);
            DbSet<EmulatorAggregateRoot> dbSet = dbContext.Set<EmulatorAggregateRoot>();
            EmulatorAggregateRoot aggregateRoot = this.GetOrAddEmulatorAggregateRoot<TValue>(dbContext, dbSet, varAddress);
            aggregateRoot.UpdateValue(values.ToJson());
            dbSet.Update(aggregateRoot);
            dbContext.SaveChanges();
        }
    }

    private EmulatorAggregateRoot GetOrAddEmulatorAggregateRoot<TValue>(EmulatorDbContext dbContext, DbSet<EmulatorAggregateRoot> dbSet, VarAddress varAddress)
    {
        string address = varAddress.Address;
        EmulatorAggregateRoot? aggregateRoot = dbSet.SingleOrDefault(x =>
        x.ChannelName == this.Name
        && x.Name == varAddress.Name
        && x.Address == address
        && x.Count == varAddress.Count
        && x.Type == varAddress.Type);
        if (aggregateRoot is null)
        {
            string stringValue = "";

            if (typeof(TValue) == typeof(string))
            {
                if (varAddress.Type == VarType.String)
                {
                    stringValue = "".PadRight((int)varAddress.Count);
                }
                else
                {
                    string[] vs = new string[varAddress.StringArrayCounts.Length];
                    IMSCollectionExtensions.Populate(vs, index => "".PadRight(varAddress.StringArrayCounts[index]));
                    stringValue = vs.ToJson();
                }
            }
            else
            {
                TValue[] vs = new TValue[varAddress.Count];
                IMSCollectionExtensions.Populate(vs, index => (TValue)IMSObjectExtensions.GetDefaultValue(typeof(TValue))!);
                stringValue = vs.ToJson();
            }

            aggregateRoot = new EmulatorAggregateRoot()
            {
                ChannelName = this.Name,
                Name = varAddress.Name,
                Address = address,
                Value = stringValue,
                Count = varAddress.Count,
                Type = varAddress.Type,
            };

            dbSet.Add(aggregateRoot);
            dbContext.SaveChanges();
        }
        return aggregateRoot;
    }
}