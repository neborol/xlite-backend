using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EliteForce.AppWideHelpers;
using EliteForce.Data;
using EliteForce.Dtos;
using EliteForce.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CockpitPhotosController : ControllerBase
    {
        private readonly IMissionPhotosRepository _missionPhotosRepo;
        private readonly IConfirmResp _confirm;
        private readonly UserManager<User> _userManager;
        public CockpitPhotosController(
            IMissionPhotosRepository missionPhotosRepo, 
            IConfirmResp confirm,
            UserManager<User> userManager
            )
        {
            _missionPhotosRepo = missionPhotosRepo;
            _confirm = confirm;
            _userManager = userManager;
        }

        [HttpPut("updateMissionPhoto/{photoId}")]
        public async Task<ActionResult> UpdateMissionPhoto(PhotoUpdateDto updateObject, int photoId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.GetUserAsync(User);
            var num = await _missionPhotosRepo.UpdateMissionPhoto(updateObject, photoId, user.CodeNr);

            if (num < 1)
            {
                return BadRequest("The photograph was not updated");
            }
            var confirm = _confirm.ConfirmResponse(true, "A Mission has been updated successfully.");
            return Ok(confirm);
        }



        [HttpDelete("deleteMissionPhoto/{photoId}")]
        public async Task<ActionResult> DeleteMissionPhoto(int photoId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var num = await _missionPhotosRepo.DeleteMissionPhoto(photoId);

            if (num < 1)
            {
                return BadRequest("The photograph could not be deleted");
            }
            var confirm = _confirm.ConfirmResponse(true, "A Mission Photo has been deleted.");
            return Ok(confirm);
        }

    }
}