using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EliteForce.AppWideHelpers;
using EliteForce.Data;
using EliteForce.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CockpitEventsController : ControllerBase
    {
        private readonly IEventsRepository _eventsRepo;
        private readonly IConfirmResp _conf;
        public CockpitEventsController(IEventsRepository eventsRepo, IConfirmResp conf)
        {
            _eventsRepo = eventsRepo;
            _conf = conf;
        }
       
        [HttpGet("getevents")]
        public async Task<IEnumerable<EventObj>> Get()
        {
            return await _eventsRepo.GetEvents();
        }


        [HttpPost("createevent")]
        public async Task<ActionResult> CreateEvent(EventObj eventObj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var numberAdded = await _eventsRepo.AddEvent(eventObj);

            if (numberAdded == 0)
            {
                return BadRequest("Event add failed.");
            }

            var resp = _conf.ConfirmResponse(true, "An Event item was created successfully");
            return Ok(resp);
        }

        [HttpPut("updateevent")]
        public async Task<ActionResult> Put(EventObj updateObj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var numUpdated = await _eventsRepo.UpdateAnEvent(updateObj);
            if (numUpdated == 0)
            {
                throw new Exception("Event item update failed.");
            }

            var resp = _conf.ConfirmResponse(true, "An Event item was updated successfully");
            return Ok(resp);

        }

 
        [HttpDelete("deleteevent/{eventId}")]
        public async Task<ActionResult> Delete(int eventId)
        {
            var resp = await _eventsRepo.DeleteEvent(eventId);
            if (resp == 0)
            {
                return BadRequest("Delete event failed");
            }
            var conf = _conf.ConfirmResponse(true, "Event was deleted successfully");
            return Ok(conf);
        }
    }
}
