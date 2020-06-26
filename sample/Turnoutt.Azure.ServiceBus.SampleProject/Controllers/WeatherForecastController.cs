using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Turnoutt.Azure.ServiceBus.Core;

namespace Turnoutt.Azure.ServiceBus.SampleProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IServiceBusClientPool _clientPool;

        public WeatherForecastController(IServiceBusClientPool clientPool)
        {
            _clientPool = clientPool;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateForecast()
        {
            await _clientPool.SendQueueMessageAsync(new WeatherForecast());

            return Ok();
        }
    }
}