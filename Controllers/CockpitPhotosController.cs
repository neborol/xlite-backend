using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EliteForce.AppWideHelpers;
using EliteForce.AuthorizationRequirements;
using EliteForce.Data;
using EliteForce.Dtos;
using EliteForce.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CockpitPhotosController : ControllerBase
    {
        private readonly IMissionPhotosRepository _missionPhotosRepo;
        private readonly IConfirmResp _confirm;
        private readonly UserManager<User> _userManager;
        private readonly ILogger _logger;
        

        public CockpitPhotosController(
            IMissionPhotosRepository missionPhotosRepo, 
            IConfirmResp confirm,
            UserManager<User> userManager,
            ILogger<CockpitPhotosController> logger
            )
        {
            _missionPhotosRepo = missionPhotosRepo;
            _confirm = confirm;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPut("updateMissionPhoto/{photoId}")]
        [Authorize(Policy = Policies.Manager)]
        public async Task<ActionResult> UpdateMissionPhoto(PhotoUpdateDto updateObject, int photoId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("In cphoto controller,  update mission photo model not good");
                return BadRequest(ModelState);
            }
            var user = await _userManager.GetUserAsync(User);
            var num = await _missionPhotosRepo.UpdateMissionPhoto(updateObject, photoId, user.CodeNr);

            if (num < 1)
            {
                _logger.LogError("In cphotos controller,  update mission photo returned 0 from repo");
                return BadRequest("The photograph was not updated");
            }
            var confirm = _confirm.ConfirmResponse(true, "A Mission has been updated successfully.");
            return Ok(confirm);
        }



        [HttpDelete("deleteMissionPhoto/{photoId}")]
        [Authorize(Policy = Policies.Manager)]
        public async Task<ActionResult> DeleteMissionPhoto(int photoId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("In cphotos controller,  delete mission photo model invalid");
                return BadRequest(ModelState);
            }

            var num = await _missionPhotosRepo.DeleteMissionPhoto(photoId);

            if (num < 1)
            {
                _logger.LogError("In cphotos controller,  delete mission photos returned 0 from repo");
                return BadRequest("The photograph could not be deleted");
            }
            var confirm = _confirm.ConfirmResponse(true, "A Mission Photo has been deleted.");
            return Ok(confirm);
        }

    }
}