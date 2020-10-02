using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace acount.api
{
    public class Program
    {
        private static object _lock;
        public static IConfiguration Configuration { get; set; }
        public const string AllowSpecificOrigins = "_AllowSpecificOrigins";
        public static string Secret = "fedaf7d8863b48e197b9287d492b708e";

        public static string DataBaseHost { get { lock (_lock) { return Configuration.ReadConfig<string>("MongoDb", "Host"); } } }
        public static int DataBasePort { get { lock (_lock) { return Configuration.ReadConfig<int>("MongoDb", "Port"); } } }
        internal static string DataBaseUser { get { lock (_lock) { return "atlasUser"; } } }
        internal static string DataBasePws { get { lock (_lock) { return "itsgallus"; } } }
        public static string DataBaseAuth { get { lock (_lock) { return Configuration.ReadConfig<string>("MongoDb", "Auth"); } } }
        public static string DataBaseName { get { lock (_lock) { return Configuration.ReadConfig<string>("MongoDb", "DataBase"); } } }
        public static Uri PostalCodeApi { get; internal set; }

        public static void Main(string[] args)
        {
            _lock = new { id = Guid.NewGuid() };
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
