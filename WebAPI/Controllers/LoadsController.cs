﻿using EventUtils;
using Imports.Events;
using Microsoft.AspNetCore.Mvc;
using TestApps.DDD.DomainEvents.WebAPI.Requests;

namespace TestApps.DDD.DomainEvents.WebAPI.Controllers
{
    [ApiController]
    [Route("loads")]
    public class LoadsController : Controller
    {
        private readonly IEventHub _eventHub;

        public LoadsController(IEventHub eventHub)
        {
            _eventHub = eventHub;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Import(ImportLoadRequest model)
        {
            var domainEvent = new LoadImportedEvent()
            {
                BolNumber = model.BolNumber,
                Customer = model.Customer,
            };

            await _eventHub.Publish(domainEvent);

            return Ok();
        }
    }
}
