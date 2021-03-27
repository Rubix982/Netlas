using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Web.Http;

namespace server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ForbiddenController : ControllerBase
    {

        private String path = @"forbiddenDomains.json";

        private String data = "";

        [HttpGet("")]
        public FilterJSON Get()
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
            return deserializedProduct;
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
