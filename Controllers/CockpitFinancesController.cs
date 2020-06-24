using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EliteForce.AppWideHelpers;
using EliteForce.Data;
using EliteForce.Dtos;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CockpitFinancesController : ControllerBase
    {
        IFinancesRepository _financesRepo;
        public CockpitFinancesController(IFinancesRepository financesRepo)
        {
            _financesRepo = financesRepo;
        }

        [HttpGet("getSubscriptions/{userId}")]
        public async Task<ActionResult> GetSubscriptions(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
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
        public async Task<ActionResult> GetTotalMembersAmount()
        {
            var total = await _financesRepo.GetTotalForMembers();
            return Ok(total);
        }

    }
}
