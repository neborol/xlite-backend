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
    public class CockpitNewsController : ControllerBase
    {
        INewsRepository _newsRepo;
        IConfirmResp _conf;
        public CockpitNewsController(INewsRepository newsRepo, IConfirmResp conf)
        {
            _newsRepo = newsRepo;
            _conf = conf;
        }

 //       [Authorize(Policy = Policies.News)]
        [HttpPost("addANewsItem")]
        public async Task<ActionResult> AddANewsItem(NewsPostDto newsInput)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var num = await _newsRepo.AddANewsItem(newsInput);

            if (num < 1)
            {
                return BadRequest("The News item was not added.");
            }
            var confirm = _conf.ConfirmResponse(true, "A News item has been added successfully.");
            return Ok(confirm);
        }



        [HttpPut("updateNewsArticle/{newsId}")]
        public async Task<ActionResult> UpdateNewsArticle(NewsUpdateDto newsInput, int newsId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var num = await _newsRepo.UpdateNewsItem(newsInput, newsId);

            if (num < 1)
            {
                return BadRequest("The News item was not updated.");
            }
            var confirm = _conf.ConfirmResponse(true, "A News item has been updated successfully.");
            return Ok(confirm);
        }



        [HttpPost("addScrollingNewsMessage")]
        public async Task<ActionResult> AddAScollingNewsMessage(ScrollNewsPostDto scrollNewsInput)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var num = await _newsRepo.AddAScrollNewsItem(scrollNewsInput);

            if (num < 1)
            {
                return BadRequest("The Scrolling message was not added.");
            }
            var confirm = _conf.ConfirmResponse(true, "A Scrolling message has been added successfully.");
            return Ok(confirm);
        }

    }
}
