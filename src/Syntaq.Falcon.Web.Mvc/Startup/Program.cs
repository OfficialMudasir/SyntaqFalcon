﻿using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Syntaq.Falcon.Web.Helpers;

namespace Syntaq.Falcon.Web.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CurrentDirectoryHelpers.SetCurrentDirectory();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return new WebHostBuilder()
                .UseKestrel(opt =>
                {
                    opt.AddServerHeader = false;
                    opt.Limits.MaxRequestLineSize = 16 * 1024;
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddEventSourceLogger(); // Add Serilog Event Source logger
                    logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                })
                .UseIIS()
                .UseIISIntegration()
                .UseStartup<Startup>();
        }
    }
}