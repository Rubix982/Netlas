using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
            {
                forecast.Domains.Add("https://www." + domain);
                forecast.Domains.Add("https://" + domain);
                forecast.Domains.Add("http://www." + domain);
                forecast.Domains.Add("http://" + domain);
                forecast.Domains.Add(domain);
            }

            var roundTrippedJson =
                JsonSerializer.Serialize<FilterJSON>(forecast, options);

            System.IO.File.WriteAllText(@"forbiddenDomains.json", roundTrippedJson);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Add the following line:
                    webBuilder.UseSentry("https://648c28ba5aa84b3ca6872e92ff9dbacf@o521037.ingest.sentry.io/5687909");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
