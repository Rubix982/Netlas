using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateEmptyJSON();
            CreateHostBuilder(args).Build().Run();
        }

        public static void CreateEmptyJSON()
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-5-0
            var json =
                @"{""Domains"": [""facebook.com""]}";
    
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
            };

            var forecast = JsonSerializer.Deserialize<FilterJSON>(json, options);

            var blockedDomains = new List<string>(){ "twitter.com", 
                        "linkedin.com", "instagram.com", 
                        "reddit.com", "youtube.com"};
            foreach (string domain in blockedDomains)
                forecast.Domains.Add(domain);

            var roundTrippedJson =
                JsonSerializer.Serialize<FilterJSON>(forecast, options);

            System.IO.File.WriteAllText(@"data.json", roundTrippedJson);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
