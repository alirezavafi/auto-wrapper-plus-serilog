using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;

namespace AutoWrapper.Sample.AspNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseSerilogPlus(ConfigureLogger).UseStartup<Startup>(); });

        private static void ConfigureLogger(LoggerConfiguration logConfig)
        {
            logConfig.WriteTo.File(new CompactJsonFormatter(), "App_Data/Logs/log.json");
        }
    }
}