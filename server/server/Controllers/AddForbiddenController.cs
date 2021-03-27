using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddForbiddenController : ControllerBase
    {
        private readonly ILogger<AddForbiddenController> _logger;

        private String path = @"forbiddenDomains.json";

        private String data = "";

        public AddForbiddenController(ILogger<AddForbiddenController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        public ForbiddenResponse Get(string newForbidden)
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
            // Console.WriteLine(data.Domains)
            FilterJSON deserializedProduct = JsonConvert.DeserializeObject<FilterJSON>(data);

            // Check if the entry already exists with a set list of domains
            foreach (var entry in deserializedProduct.Domains)
            {
                if (entry == newForbidden)
                {
                    return new ForbiddenResponse
                    {
                        StatusCode = "304",
                        ResponseMessage = "Not Modified",
                        HydratedContent = deserializedProduct
                    };
                }
            }

            // Adds new forbidden domain to a list of domains
            deserializedProduct.Domains.Add(newForbidden);

            // 1. Overwrite the current files
            // 2. Dumps to the file again the new contents
            System.IO.File.WriteAllText(path,
                JsonConvert.SerializeObject(deserializedProduct));

            // Return a new response
            return new ForbiddenResponse
            {
                StatusCode = "200",
                ResponseMessage = "OK",
                HydratedContent = deserializedProduct
            };  
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

    }
}
