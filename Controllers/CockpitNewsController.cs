using EliteForce.AppWideHelpers;
using EliteForce.AuthorizationRequirements;
using EliteForce.Data;
using EliteForce.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;
        

        public CockpitNewsController(INewsRepository newsRepo, IConfirmResp conf, ILogger<CockpitNewsController> logger)
        {
            _newsRepo = newsRepo;
            _conf = conf;
            _logger = logger;
        }

        [HttpPost("addANewsItem")]
        [Authorize(Policy = Policies.News)]
        public async Task<ActionResult> AddANewsItem(NewsPostDto newsInput)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("In cnews controller,  addnews item model invalid");
                return BadRequest(ModelState);
            }

            var num = await _newsRepo.AddANewsItem(newsInput);

            if (num < 1)
            {
                _logger.LogError("In cnews controller, nothing returned from repo add-news item");
                return BadRequest("The News item was not added.");
            }
            var confirm = _conf.ConfirmResponse(true, "A News item has been added successfully.");
            return Ok(confirm);
        }



        [HttpPut("updateNewsArticle/{newsId}")]
        [Authorize(Policy = Policies.News)]
        public async Task<ActionResult> UpdateNewsArticle(NewsUpdateDto newsInput, int newsId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("In cnews controller, failed");
                return BadRequest(ModelState);
            }

            var num = await _newsRepo.UpdateNewsItem(newsInput, newsId);

            if (num < 1)
            {
                _logger.LogError("In cnews controller, update news article returned nothing from repo.");
                return BadRequest("The News item was not updated.");
            }
            var confirm = _conf.ConfirmResponse(true, "A News item has been updated successfully.");
            return Ok(confirm);
        }



        [HttpPost("addScrollingNewsMessage")]
        [Authorize(Policy = Policies.News)]
        public async Task<ActionResult> AddAScollingNewsMessage(ScrollNewsPostDto scrollNewsInput)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var num = await _newsRepo.AddAScrollNewsItem(scrollNewsInput);

            if (num < 1)
            {
                _logger.LogError("In cnews controller, addScrollnews returned nothing from repo");
                return BadRequest("The Scrolling message was not added.");
            }
            var confirm = _conf.ConfirmResponse(true, "A Scrolling message has been added successfully.");
            return Ok(confirm);
        }

    }
}
