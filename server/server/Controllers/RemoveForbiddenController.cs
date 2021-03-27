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
    public class RemoveForbiddenController : ControllerBase
    {
        private readonly ILogger<RemoveForbiddenController> _logger;

        private String path = @"forbiddenDomains.json";

        private String data = "";

        public RemoveForbiddenController(ILogger<RemoveForbiddenController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        public ForbiddenResponse Get(string domainToRemove)
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

            // Check if the entry already exists with a set list of domains
            foreach (var entry in deserializedProduct.Domains)
            {
                if (entry == domainToRemove)
                {
                    // Remove the found domain
                    deserializedProduct.Domains.Remove(domainToRemove);

                    // 1. Overwrite the current files
                    // 2. Dumps to the file again the new contents
                    System.IO.File.WriteAllText(path,
                        JsonConvert.SerializeObject(deserializedProduct));
                    return new ForbiddenResponse
                    {
                        StatusCode = "200",
                        ResponseMessage = "OK",
                        HydratedContent = deserializedProduct
                    };
                }
            }

            // If no such domain was even found
            return new ForbiddenResponse
            {
                StatusCode = "400",
                ResponseMessage = "BAD REQUEST",
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
