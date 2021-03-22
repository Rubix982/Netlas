using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilterJSONController : ControllerBase
    {
        private readonly ILogger<FilterJSONController> _logger;
        private String path = @"data.json"; 
        private String data = "";

        public FilterJSONController(ILogger<FilterJSONController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        public String Testing(string domainName)
        {

            if (!System.IO.File.Exists(path))
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Employee doesn't exist", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.NotFound
                };

                throw new HttpResponseException(errorResponse);                
            }
            else
            {
                Console.WriteLine("File exists!");

                // Read a text file line by line.  
                string[] lines = System.IO.File.ReadAllLines(path);  
                
                foreach (string line in lines)  
                    data = data + line; 
            }

            FilterJSON deserializedProduct = JsonConvert.DeserializeObject<FilterJSON>(data);

            foreach (string name in deserializedProduct.DomainNames)
            {
                // Console.WriteLine($"{domainName}, {name}");
                if ( domainName == name )
                {
                    Console.WriteLine("Match found!");
                    return "Forbidden page!";
                }
            }
            
            return "Request is valid";
        }
    }
}
