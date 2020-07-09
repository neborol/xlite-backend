using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EliteForce.AppWideHelpers;
using EliteForce.AuthorizationRequirements;
using EliteForce.Data;
using EliteForce.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CockpitContributionsController : ControllerBase
    {
        IContributionsRepository _contributionsRepo;
        IConfirmResp _confirm;
        private readonly ILogger _logger;
       
        public CockpitContributionsController(
            IContributionsRepository contributionsRepo, 
            IConfirmResp confirm,
            ILogger<CockpitContributionsController> logger
        )
        {
            _contributionsRepo = contributionsRepo;
            _confirm = confirm;
            _logger = logger;
        }


        [HttpPost("addContribution")]
        [Authorize(Policy = Policies.Pilot)]
        public async Task<ActionResult> AddContribution(ContributionsPostDto contributionsObj)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("In contributions controller model state failed " + ModelState);
                return BadRequest(ModelState);
            }

            var num = await _contributionsRepo.AddContribution(contributionsObj);

            if (num != 1)
            {
                _logger.LogError("In contributions controller nothing returned from contributions saving");
                return BadRequest("The User Contribution was not updated");
            }
            var confirm = _confirm.ConfirmResponse(true, "The user contribution has been added.");
            return Ok(confirm);
        }
    }
}