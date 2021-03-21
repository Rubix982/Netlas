using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]   // GET /api/test2
        public String ListProducts()
        {
            return "Hello, World";
        }

        [HttpGet("{id}")]   // GET /api/test2/xyz
        public String GetProduct(string id)
        {            
            return "Hello, World" + id;
        }

        [HttpGet("int/{id:int}")] // GET /api/test2/int/3
        public String GetIntProduct(int id)
        {
            return "Hello, World" + (id);
        }

        [HttpGet("int2/{id}")]  // GET /api/test2/int2/3
        public String GetInt2Product(int id)
        {
            return "Hello, World" + (id);
        }

        [HttpGet("ABC/ABC")]
        public String ABCFunc(string domainName)
        {
            return domainName;
        }

        // [HttpGet]
        // public IEnumerable<WeatherForecast> Get()
        // {
            // return "Hello, World!";
            // var rng = new Random();
            // // Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
            // return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            // {
            //     Date = DateTime.Now.AddDays(index),
            //     TemperatureC = rng.Next(-20, 55),
            //     Summary = Summaries[rng.Next(Summaries.Length)]
            // })
            // .ToArray();
        // }
    }
}
