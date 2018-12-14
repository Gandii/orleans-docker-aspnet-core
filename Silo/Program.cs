using Grains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace Silo
{
    public class Program
    {
        private static ISiloHost silo;
        private static readonly ManualResetEvent siloStopped = new ManualResetEvent(false);

        static void Main(string[] args)
        {

            var configBuilder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              // Add docker envs.
              .AddEnvironmentVariables();

            var config = configBuilder.Build();

            string connectionString = config.GetConnectionString("default");

            var name = Dns.GetHostName(); // get container id
            var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

            silo = new SiloHostBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "orleans-docker";
                    options.ServiceId = "AspNetSampleApp";
                })
                .UseAdoNetClustering(options =>
                {
                    options.ConnectionString = connectionString;
                    options.Invariant = "System.Data.SqlClient";
                })
                .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
                    .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = ip)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ValueGrain).Assembly).WithReferences())
                .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Information).AddConsole())
                .Build();

            Task.Run(StartSilo);

            AssemblyLoadContext.Default.Unloading += context =>
            {
                Task.Run(StopSilo);
                siloStopped.WaitOne();
            };

            siloStopped.WaitOne();
        }

        private static async Task StartSilo()
        {
            await silo.StartAsync();
            Console.WriteLine("Silo started");
        }

        private static async Task StopSilo()
        {
            await silo.StopAsync();
            Console.WriteLine("Silo stopped");
            siloStopped.Set();
        }
    }
}
