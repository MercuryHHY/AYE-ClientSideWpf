
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
