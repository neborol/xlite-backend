using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EliteForce.AppWideHelpers;
using EliteForce.Data;
using EliteForce.Dtos;
using Microsoft.AspNetCore.Authorization;
using EliteForce.AuthorizationRequirements;
using Microsoft.Extensions.Logging;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CockpitFinancesController : ControllerBase
    {
        IFinancesRepository _financesRepo;
        private readonly ILogger _logger;
        
        public CockpitFinancesController(IFinancesRepository financesRepo, ILogger<CockpitFinancesController> logger)
        {
            _financesRepo = financesRepo;
            _logger = logger;
        }

        [HttpGet("getSubscriptions/{userId}")]
        [Authorize(Policy = Policies.Pilot)]
        public async Task<ActionResult> GetSubscriptions(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogInformation("In finances controller,  no userid received");
                return BadRequest("The user id is not received.");
            }

            var subscriptionsList = await _financesRepo.GetSubscriptions(userId);

            //if (subscriptionsList.Count == 0)
            //{
            //    return NotFound("No contributions were found");
            //}

            return Ok(subscriptionsList);
        }


        [HttpGet("contributions")]
        [Authorize(Policy = Policies.Pilot)]
        public async Task<ActionResult> GetTotalMembersAmount()
        {
            var total = await _financesRepo.GetTotalForMembers();
            return Ok(total);
        }

    }
}
