using EliteForce.AppWideHelpers;
using EliteForce.AuthorizationRequirements;
using EliteForce.Data;
using EliteForce.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CockpitVideosController : ControllerBase
    {
        private readonly IVideosRepository _videoRepo;
        private readonly IConfirmResp _confirm;
        public CockpitVideosController(IVideosRepository videoRepo, IConfirmResp conf)
        {
            _videoRepo = videoRepo;
        }

        [HttpPost("addAVideoItem")]
        public async Task<ActionResult> AddAVideoItem(MissionVideoDto videoData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var num = await _videoRepo.AddAVideoItem(videoData);

            if (num < 1)
            {
                return BadRequest("The video item was not created.");
            }
            var confirm = _confirm.ConfirmResponse(true, "A Video item was created successfully.");
            return Ok(confirm);
        }
    }
}
