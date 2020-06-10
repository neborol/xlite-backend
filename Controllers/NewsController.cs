using EliteForce.AppWideHelpers;
using EliteForce.Data;
using EliteForce.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        INewsRepository _newsRepo;
        IConfirmResp _confirmResp;
        public NewsController(INewsRepository newsRepo, IConfirmResp confirmResp)
        {
            _newsRepo = newsRepo;
            _confirmResp = confirmResp;
        }

        [HttpGet("getArticles/{category}")]
        public async Task<ActionResult> GetArticles(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return BadRequest("The category of the news article must be provided.");
            }

            var articles = await _newsRepo.GetnewsArticles(category);

            if (articles.Count == 0)
            {
                return BadRequest("Category not found");
            }
            return Ok(articles);
        }


        [HttpGet("getnewsflash")]
        public async Task<ActionResult> GetnewsFlash()
        {
            var newsFlash = await _newsRepo.GetnewsNewsFlash();

            if (string.IsNullOrEmpty(newsFlash))
            {
                return NotFound("News flash not found");
            }
            // return newsFlash;
            var successConf = _confirmResp.ConfirmResponse(true, newsFlash);
            return Ok(successConf);
        }

    }
}

