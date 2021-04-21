﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoWrapper.Filters;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AutoWrapper.Samples.AspNetCore.DedicatedRequestResponseLogOutput.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger _logger;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public HomeController(ILogger logger)
        {
            _logger = logger;
            _logger.Information("HomeController instantiated");
        }

        [HttpGet("error")]
        public WeatherForecast Error()
        {
            _logger.Error("Error occured");
            throw new InvalidOperationException("operation failed.");
        }
      
        [HttpGet("bad-request")]
        public WeatherForecast BadRequest()
        {
            throw new ApiException("invalid data");
        }

        
        [HttpGet("unauthorized")]
        public WeatherForecast Unauthorized()
        {
            throw new UnauthorizedAccessException();
        }

        [HttpGet("save")]
        public IEnumerable<WeatherForecast> Save(WeatherForecast f)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }

        [HttpGet("list")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.Information("returning data");
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
        
        [HttpGet("exclude")]
        [IgnoreLogHttpRequestResponse]
        public IEnumerable<WeatherForecast> Exclude()
        {
            _logger.Information("returning data");
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}