using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EliteForce.AppWideHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EliteForce.Data;
using EliteForce.Entities;
using EliteForce.Dtos;
using Microsoft.AspNetCore.Authorization;
using EliteForce.AuthorizationRequirements;
using Microsoft.Extensions.Logging;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CockpitFaqController : ControllerBase
    {
        IFaqRepository _faqRepo;
        IConfirmResp _conf;
        private readonly ILogger _logger;
        

        public CockpitFaqController(IFaqRepository faqRepo, IConfirmResp conf, ILogger<CockpitFaqController> logger)
        {
            _faqRepo = faqRepo;
            _conf = conf;
            _logger = logger;
        }

        [HttpPost("addfaq")]
        [Authorize(Policy = Policies.Manager)]
        public async Task<ActionResult> AddAnFaq(FaqPostDto faq)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("In cfaq controller,  model not correct");
                return BadRequest(ModelState);
            }
            var num = await _faqRepo.AddAnFaq(faq);
            if (num < 1)
            {
                _logger.LogError("In cfaq controller,  failed");
                return BadRequest("The FAQ was not added.");
            }
            var confirm = _conf.ConfirmResponse(true, "An FAQ Item has been added successfully.");
            return Ok(confirm);
        }


        [HttpGet("getfaqs")]
        public async Task<ActionResult> GetFaqs()
        {
            var faqs = await _faqRepo.GetFaqs();
            return Ok(faqs);
        }


        [HttpPut("editfaq")]
        [Authorize(Policy = Policies.Manager)]
        public async Task<ActionResult> EditFaq(Faq faqToEdit)
        {
            if (string.IsNullOrEmpty(faqToEdit.FaqId)) 
            {
                _logger.LogError("In cfaq controller,  editfaq bad model");
                return BadRequest("No item Available.");
            }

            var numbAffected = await _faqRepo.EditAnFaq(faqToEdit);
            if (numbAffected == 0)
            {
                _logger.LogError("In cfaq controller,  nothing returned from edit save");
                return NotFound("The FAQ was not updated.");
            }

            var confirm = _conf.ConfirmResponse(true, "The FAQ Item has been edited successfully.");
            return Ok(confirm);
        }


        [HttpDelete("deletefaq/{id}")]
        [Authorize(Policy = Policies.Manager)]
        public async Task<ActionResult> DeleteFaq(string id)
        {
            if (string.IsNullOrEmpty(id)) 
            {
                _logger.LogError("In cfaq controller,  deletefaq failed");
                return BadRequest("No item Available.");
            }

            var numbAffected = await _faqRepo.DeleteAnFaq(id);
            if (numbAffected == 0)
            {
                _logger.LogError("In cfaq controller,  repo returned nothing from delete.");
                return NotFound("The FAQ was not deleted.");
            }

            var confirm = _conf.ConfirmResponse(true, "The FAQ Item has been deleted successfully.");
            return Ok(confirm);
        }
    }
}
