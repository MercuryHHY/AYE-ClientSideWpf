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


using Microsoft.Extensions.DataChannel.VarReaders;
using Microsoft.Extensions.DataChannel.VarWriters;

namespace Microsoft.Extensions.DataChannel.Emulator.DependencyInjection;

public static class DataChannelDependencyInjection
{
    public static IServiceCollection AddEmulatorDataChannel(this IServiceCollection services, IConfiguration configuration, string optionName = nameof(DataChannelOptions))
    {
        DataChannelOptions options = configuration.TryGetValue<DataChannelOptions>(optionName);

        return services.AddEmulatorDataChannel(options);
    }

    public static IServiceCollection AddEmulatorDataChannel(this IServiceCollection services, DataChannelOptions options)
    {
        Microsoft.EntityFrameworkCore.Abstractions.DbType DbType = options.ConnectionString.GetOrDefault<Microsoft.EntityFrameworkCore.Abstractions.DbType>(nameof(DbType), Microsoft.EntityFrameworkCore.Abstractions.DbType.Sqlite);
        options.ConnectionString.RemoveExtraProperty(nameof(DbType));
        IDataChannel dataChannel = new EmulatorDataChannel(options.Name, DbType, options.ConnectionString.ToString());
        IVarReader varReader = new VarReaderDirectly(dataChannel);
        IVarWriter varWriter = new VarWriterDirectly(options.IsRWSplitting ? new EmulatorDataChannel(options.Name, DbType, options.ConnectionString.ToString()) : dataChannel);

        //builder.Services.AddSingleton<Func<string, IVarReader?>>(serviceProvider => name =>
        //{
        //    return name switch
        //    {
        //        "Modbus" => serviceProvider.GetServices<IVarReader>().First(s => s.Channel.Name == ""),
        //        "FinsTcp" => serviceProvider.GetServices<IVarReader>().First(s => s.Channel.Name == ""),
        //        "EtherNetIP" => serviceProvider.GetServices<IVarReader>().First(s => s.Channel.Name == ""),
        //        _ => throw new KeyNotFoundException()
        //    };
        //});



        services.AddSingleton<IVarReader>(services =>
        {
            if (options.AutoOpen)
            {
                varReader.Open();
            }
            return varReader;
        });
        services.AddSingleton<IVarWriter>(services =>
        {
            if (options.AutoOpen)
            {
                varWriter.Open();
            }
            return varWriter;
        });


        services.AddSingleton<Func<string, IVarReader?>>(serviceProvider => name =>
        {

            IVarReader IR = serviceProvider.GetServices<IVarReader>().First(s => s.Channel.Name == name);

            if (options.AutoOpen && !IR.Channel.IsConnected)
            {
                IR.Open();
            }
            return IR;
        });
        services.AddSingleton<Func<string, IVarWriter?>>(serviceProvider => name =>
        {
            IVarWriter iw = serviceProvider.GetServices<IVarWriter>().First(s => s.Channel.Name == name);
            if (options.AutoOpen && iw.Channel.IsConnected)
            {
                iw.Open();
            }
            return iw;
        });
        return services;
    }
}