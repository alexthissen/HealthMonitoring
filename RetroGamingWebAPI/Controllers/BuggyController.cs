﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using RetroGamingWebAPI.HealthChecks;

namespace RetroGamingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : ControllerBase
    {
        private readonly IReadOnlyPolicyRegistry<string> registry;
        private readonly TripwireHealthCheck healthCheck;
        private readonly ForcedHealthCheck forcedHealthCheck;

        public BuggyController(
            IReadOnlyPolicyRegistry<string> registry,
            TripwireHealthCheck tripWireHealthCheck, ForcedHealthCheck forcedHealthCheck)
        {
            this.registry = registry;
            this.healthCheck = tripWireHealthCheck;
            this.forcedHealthCheck = forcedHealthCheck;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            CircuitBreakerPolicy breaker = registry.Get<CircuitBreakerPolicy>("DefaultBreaker");
            breaker.Isolate();
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<int> Get(int id)
        {
            return healthCheck.Trip();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromQuery] string status)
        {
            forcedHealthCheck.Force(status, Environment.MachineName);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
