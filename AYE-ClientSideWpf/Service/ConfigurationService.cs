using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_ClientSideWpf.Service
{
    public interface IConfigurationService
    {
        IConfiguration Configuration { get; }
    }

    public class ConfigurationService : IConfigurationService
    {
        public IConfiguration Configuration { get; }

        public ConfigurationService()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }
    }
}
