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

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CockpitContributionsController : ControllerBase
    {
        IContributionsRepository _contributionsRepo;
        IConfirmResp _confirm;
        public CockpitContributionsController(IContributionsRepository contributionsRepo, IConfirmResp confirm)
        {
            _contributionsRepo = contributionsRepo;
            _confirm = confirm;
        }


        [HttpPost("addContribution")]
        [Authorize(Policy = Policies.Pilot)]
        public async Task<ActionResult> AddContribution(ContributionsPostDto contributionsObj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var num = await _contributionsRepo.AddContribution(contributionsObj);

            if (num != 1)
            {
                return BadRequest("The User Contribution was not updated");
            }
            var confirm = _confirm.ConfirmResponse(true, "The user contribution has been added.");
            return Ok(confirm);
        }
    }
}