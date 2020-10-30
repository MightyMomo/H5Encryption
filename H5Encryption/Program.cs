using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace H5Encryption
{
    public class Program
    {
        public static class HostConfig
        {
            public static string CertPath { get; set; }
            public static string CertPassword { get; set; }
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    HostConfig.CertPath = context.Configuration["CertPath"];
                    HostConfig.CertPassword = context.Configuration["CertPassword"];
                })

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var host = Dns.GetHostEntry("H5Encryption.dk");
                    webBuilder.ConfigureKestrel(opt =>
                    {
                        opt.Listen(host.AddressList[0], 80);
                        opt.Listen(host.AddressList[0], 443, listOpt => {
                            listOpt.UseHttps(HostConfig.CertPath, HostConfig.CertPassword);
                        });
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
