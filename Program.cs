using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace _7454E74E_B5DE_4630_A0FE_2DD6994282CD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) => 
                {
                    configuration.AddJsonFile("appsettings.json", optional: true);
                    configuration.AddEnvironmentVariables();
                    configuration.AddCommandLine(args);
                    configuration.AddUserSecrets("6B0846D5-2CC0-4E99-A6B4-1AAEC6634E69");
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug();   
                })
                .UseStartup<Startup>();
    }
}
