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
        public CockpitVideosController(IVideosRepository videoRepo, IConfirmResp confirm)
        {
            _videoRepo = videoRepo;
            _confirm = confirm;
        }

        [HttpPost("addAVideoItem")]
        [Authorize(Policy = Policies.Manager)]
        public async Task<ActionResult> AddAVideoItem(MissionVideoPostDto videoData)
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


        [HttpPut("updateVideoData/{id}")]
        [Authorize(Policy = Policies.Manager)]
        public async Task<ActionResult> Update(VideoUpdateDto videoData, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var num = await _videoRepo.UpdateVideo(videoData, id);

            if (num < 1)
            {
                return BadRequest("The video item was not updated.");
            }
            var confirm = _confirm.ConfirmResponse(true, "A Video item was updated successfully.");
            return Ok(confirm);
        }


        [HttpDelete("deletevideo/{videoId}")]
        [Authorize(Policy = Policies.Manager)]
        public async Task<ActionResult> Delete(int videoId)
        {
            if (videoId == 0)
            {
                return BadRequest("Video id must be provided.");
            }

            var num = await _videoRepo.DeleteVideo(videoId);

            if (num < 1)
            {
                return BadRequest("The video item was not deleted.");
            }
            var confirm = _confirm.ConfirmResponse(true, "A Video item was deleted successfully.");
            return Ok(confirm);
        }
    }
}
