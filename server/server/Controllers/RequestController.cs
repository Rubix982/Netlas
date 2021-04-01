using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CsvHelper;
using CsvHelper.Configuration;

namespace server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly ILogger<RequestController> _logger;
        private String path = @"forbiddenDomains.json";
        private String data = "";
        private String capturedResponseContent = "";

        private bool isFromCache { get; set; } = true;

        static private RedisLRUMechanism LRUMechanism { get; set; } = new RedisLRUMechanism("abcd", 2020, 3);

        static private RedisLFUMechanism LFUMechanism { get; set; } = new RedisLFUMechanism("abcd", 2020, 3);

        public RequestController(ILogger<RequestController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<server.Request> Domain(string domain,
            Int32 clientId,
            Int32 requestId)
        {
            try
            {
                checkPathExistence(path);
                // Read a text file line by line.  
                string[] lines = System.IO.File.ReadAllLines(path);

                foreach (string line in lines)
                    data = data + line;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException Caught In Domain!");
                Console.WriteLine("Message :{0} ", ex.Message);
            }

            FilterJSON deserializedProduct = JsonConvert.DeserializeObject<FilterJSON>(data);

            foreach (string name in deserializedProduct.Domains)
            {
                if ($"http://www.{name}" == domain ||
                $"www.{name}" == domain ||
                $"{name}" == domain)
                {
                    server.Request response = new server.Request()
                    {
                        Domain = name,
                        Content = "Forbidden Yaar!\nAdministrator se baat kerni paregi",
                        Title = name,
                        ClientId = clientId,
                        RequestId = requestId,
                        StatusCode = 404,
                        isFromCache = false,
                    };

                    return response;
                }
            }

            domain = ConvertToUTF8Standard(domain);

            // Make HTTP request, yay! Finally
            return await HttpInvokeGetAsync(domain, clientId, requestId);
        }

        private async Task<server.Request>
            HttpInvokeGetAsync(string uri,
            Int32 clientId,
            Int32 requestId)
        {
            // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
            HttpClient client = new HttpClient();

            Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();

            try
            {
                capturedResponseContent = LRUMechanism.GetKey(uri);
                if (capturedResponseContent == "")
                {
                    goto LFUCache;
                }
                goto ToTheEnd;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        LFUCache:
            try
            {
                capturedResponseContent = LFUMechanism.GetKey(uri);
                if (capturedResponseContent == "")
                {
                    goto GetAsyncResult;
                }
                LRUMechanism.PutKey(uri, capturedResponseContent);
                goto ToTheEnd;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        GetAsyncResult:
            try
            {
                isFromCache = false;
                capturedResponseContent = await AsyncResourceAlloc(client, uri);
                // LFUMechanism.SetKey(uri, capturedResponseContent);
                LRUMechanism.PutKey(uri, capturedResponseContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        ToTheEnd:
            stopWatch.Stop();

            try
            {
                // Stop the stop watch

                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}.{4:00}:{5:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10, ts.TotalMilliseconds / 100, ts.Ticks);

                Task<server.Request> t1 = Task<server.Request>.Run(() =>
                {
                    server.Request response = new server.Request()
                    {
                        Domain = uri,
                        Content = capturedResponseContent,
                        Title = uri,
                        ClientId = clientId,
                        RequestId = requestId,
                        StatusCode = 400, // Bad Request, apparently. :S
                        RequestTimeMeasured = elapsedTime, // the time calculation
                        isFromCache = this.isFromCache// NOPE! Lazy insaan
                    };

                    return response;
                });

                return await t1;
            }
            catch (Exception e)
            {
                // Error messages    
                Console.WriteLine("\nException Caught In HttpInvokeGetAsync while retrieving from AsyncResourceAlloc!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            // if ((capturedResponseContent = LRUMechanism.GetKey(uri)) == "")
            // {
            //     if ((capturedResponseContent = LFUMechanism.GetKey(uri)) == "")
            //     {
            //         try
            //         {
            //             isFromCache = false;
            //             capturedResponseContent = await AsyncResourceAlloc(client, uri);
            //             LFUMechanism.PutKey(uri, capturedResponseContent);
            //         }
            //         catch (Exception e)
            //         {
            //             // Stop the stop watch
            //             stopWatch.Stop();

            //             // Get the elapsed time as a TimeSpan value.
            //             TimeSpan ts = stopWatch.Elapsed;

            //             // Format and display the TimeSpan value.
            //             string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}.{4:00}:{5:00}",
            //                 ts.Hours, ts.Minutes, ts.Seconds,
            //                 ts.Milliseconds / 10, ts.TotalMilliseconds / 100, ts.Ticks);

            //             // Error messages    
            //             Console.WriteLine("\nException Caught In HttpInvokeGetAsync while retrieving from AsyncResourceAlloc!");
            //             Console.WriteLine("Message :{0} ", e.Message);

            //             Task<server.Request> t1 = Task<server.Request>.Run(() =>
            //             {
            //                 server.Request response = new server.Request()
            //                 {
            //                     Domain = uri,
            //                     Content = e.Message,
            //                     Title = uri,
            //                     ClientId = clientId,
            //                     RequestId = requestId,
            //                     StatusCode = 400, // Bad Request, apparently. :S
            //                     RequestTimeMeasured = elapsedTime, // the time calculation
            //                     isFromCache = false // NOPE! Lazy insaan
            //                 };

            //                 return response;
            //             });

            //             return await t1;
            //         }
            //     }
            //     LRUMechanism.PutKey(uri, capturedResponseContent);
            // }

            try
            {
                // Stop the stop watch
                stopWatch.Stop();

                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);

                Task<server.Request> t2 = Task<server.Request>.Run(() =>
                {
                    server.Request response = new server.Request()
                    {
                        Domain = uri,
                        Content = capturedResponseContent,
                        Title = uri,
                        ClientId = clientId,
                        RequestId = requestId,
                        StatusCode = 200, // OK!!!! :D
                        RequestTimeMeasured = elapsedTime,
                        isFromCache = isFromCache, // NOPE
                    };

                    return response;
                });

                return await t2;
            }
            catch (Exception e)
            {

                // Stop the stop watch
                stopWatch.Stop();

                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);

                // Error message
                Console.WriteLine("\nException Caught In HttpInvokeGetAsync while initiating Response object!");
                Console.WriteLine("Message :{0} ", e.Message);

                server.Request response = new server.Request()
                {
                    Domain = uri,
                    Content = e.Message,
                    Title = uri,
                    ClientId = clientId,
                    RequestId = requestId,
                    StatusCode = 500, // Bad news, brother
                    RequestTimeMeasured = elapsedTime, // Time calculation
                    isFromCache = false, // NOPE!
                };

                return response;
            }
        }

        static async Task<string> AsyncResourceAlloc(HttpClient client, string uri)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                // string responseBody = await client.GetStringAsync(uri);
                // Use the above if only the actual content body is required

                Task<string> t1 = Task<string>.Run(async () =>
                {
                    HttpResponseMessage response = await client.GetAsync(uri);
                    response.EnsureSuccessStatusCode();
                    // Console.WriteLine($"{uri} {response.Headers}");
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                });

                return await t1;
            }
            catch (HttpRequestException e)
            {
                Task<string> t2 = Task<string>.Run(() =>
                {
                    Console.WriteLine("\nException Caught In AsyncResourceAlloc!");
                    Console.WriteLine("Message :{0} ", e.Message);
                    return e.Message;
                });

                return await t2;
            }
        }

        static void checkPathExistence(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("forbiddenDomains.json file was not found: ", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.NotFound
                };
                throw new HttpResponseException(errorResponse);
            }
        }

        static private String ConvertToUTF8Standard(string uri)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = false,
                HeaderValidated = null,
                BadDataFound = null,
            };

            using (var reader = new StreamReader("encodings.csv"))
            using (var csv = new CsvReader(reader, config))
            {
                while (csv.Read())
                {
                    server.Encoding record = csv.GetRecord<Encoding>();
                    if (uri.Contains(record.UTF8String))
                    {
                        uri.Replace(record.UTF8String, record.UTF8Encoding);
                    }
                }
                return uri;
            }
        }
    }
}
