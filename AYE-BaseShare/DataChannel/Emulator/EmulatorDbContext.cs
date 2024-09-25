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

namespace Microsoft.Extensions.DataChannel.Emulator;

public class EmulatorDbContext : DbContext
{
    private readonly Microsoft.EntityFrameworkCore.Abstractions.DbType _dbType;
    private readonly string _connectionString;

    public EmulatorDbContext(Microsoft.EntityFrameworkCore.Abstractions.DbType dbType, string connectionString)
    {
        this._dbType = dbType;
        this._connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _ = this._dbType switch
        {
            //DbType.SqlServer => optionsBuilder.UseSqlServer(this._connectionString),
            Microsoft.EntityFrameworkCore.Abstractions.DbType.PostgreSql => optionsBuilder.UseNpgsql(this._connectionString),
            Microsoft.EntityFrameworkCore.Abstractions.DbType.MySql => optionsBuilder.UseMySql(this._connectionString, new MySqlServerVersion(MySqlServerVersion.LatestSupportedServerVersion)),
            //DbType.Oracle => optionsBuilder.UseOracle(this._connectionString),
            Microsoft.EntityFrameworkCore.Abstractions.DbType.Sqlite => optionsBuilder.UseSqlite(this._connectionString),
            Microsoft.EntityFrameworkCore.Abstractions.DbType.InMemory => optionsBuilder.UseInMemoryDatabase(this._connectionString),
            _ => optionsBuilder.UseInMemoryDatabase(this._connectionString),
        };
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EmulatorAggregateRoot>(builder =>
        {
            builder.ToTable("DataChannelEmulator");
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => new
            {
                e.ChannelName,
                e.Name,
                e.Address,
                e.Count,
                e.Type
            });
        });
    }

}
