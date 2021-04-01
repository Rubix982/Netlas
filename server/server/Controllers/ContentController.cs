using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using Microsoft.VisualBasic;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CsvHelper;
using CsvHelper.Configuration;

namespace server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly ILogger<ContentController> _logger;
        private String path = @"forbiddenDomains.json";
        private String data = "";
        private String capturedResponseContent = "";

        public ContentController(ILogger<ContentController> logger)
        {
            _logger = logger;
        }

        // [HttpGet("/{domain}/")]
        // public IActionResult NoParams()
        // {
        //     Console.WriteLine("Heeeerreee");
        //     return new JavaScriptResult("console.log('Hello, World!');");
        // }

        // [HttpGet("/{domain}/", Name = "Only_Domain")]
        // public RedirectResult ParamsOnlyDomain(string domain)
        // {
        //     Console.WriteLine("Heeeerreee - 1");
        //     return Redirect("http://localhost:3000/badrequest");
        // }

        // [HttpGet("/{domain}/{id}/", Name = "Unknown_ID")]
        // public RedirectResult ParamsNoRequestID(string domain, Int32 id)
        // {
        //     Console.WriteLine("Heeeerreee - 2");
        //     return Redirect("http://localhost:3000/badrequest");
        // }

        // public RedirectResult ForbiddenRequest()
        // {
        //     Console.WriteLine("Heeeerreee - 3");
        //     return Redirect("http://localhost/forbidden");
        // }

        // public RedirectResult ResponseEmpty()
        // {
        //     Console.WriteLine("Heeeerreee - 4");
        //     return Redirect("http://localhost:3000/404");
        // }

        // public RedirectResult InternalControllerError()
        // {
        //     Console.WriteLine("Heeeerreee - 5");
        //     return Redirect("http://localhost:3000/internal");
        // }

        [HttpGet("", Name = "All_Content")]
        public async Task<string> Domain(string domain = "",
            Int32 clientId = -1,
            Int32 requestId = -1)
        {
            if (clientId == -1 || requestId == -1 || domain == "")
            {
                return "Parameters not given!";
            }

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
                    // return Redirect("http://localhost:3000/forbidden");
                    RedirectToAction("ForbiddenRequest", "Content");
                }
            }

            domain = ConvertToUTF8Standard(domain);

            // Make HTTP request, yay! Finally
            return await HttpInvokeGetAsync(domain, clientId, requestId);
        }

        private async Task<string>
            HttpInvokeGetAsync(string uri,
            Int32 clientId,
            Int32 requestId)
        {
            // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
            HttpClient client = new HttpClient();

            try
            {
                // https://stackoverflow.com/questions/6045343/how-to-make-an-asynchronous-method-return-a-value
                capturedResponseContent = await AsyncResourceAlloc(client, uri);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught In HttpInvokeGetAsync while retrieving from AsyncResourceAlloc!");
                Console.WriteLine("Message :{0} ", e.Message);

                RedirectToAction("ResponseEmpty", "Content");
            }

            try
            {
                Task<string> t2 = Task<string>.Run(() =>
                {
                    return capturedResponseContent;
                });

                return await t2;
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught In HttpInvokeGetAsync while initiating Response object!");
                Console.WriteLine("Message :{0} ", e.Message);

                // return Redirecting to bad request
                RedirectToAction("InternalControllerError", "Content");
            }

            return "No content found";
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
                    Console.WriteLine($"{uri} {response.Headers}");
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
                    // Console.WriteLine(csv.ToString());
                    // server.Encoding record = csv.GetRecord<Encoding>();
                    // if (uri.Contains(record.UTF8String))
                    // {
                    //     uri.Replace(record.UTF8String, record.UTF8Encoding);
                    // }
                }
                return uri;
            }
        }
    }
}
