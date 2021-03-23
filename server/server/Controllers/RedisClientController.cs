using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BeetleX.Redis;

namespace server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RedisClientController : ControllerBase
    {
        private readonly ILogger<RedisClientController> _logger;

        public RedisClientController(ILogger<RedisClientController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        public String Get()
        {

            RedisDB DB = new RedisDB(1);
            DB.DataFormater = new JsonFormater();

            // SSL
            DB.Host.AddWriteHost("localhost", 6379, true);            

            return "Hello from RedisClientController!";
        }
    }
}
