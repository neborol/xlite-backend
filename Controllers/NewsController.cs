﻿using EliteForce.AppWideHelpers;
using EliteForce.Data;
using EliteForce.Entities;
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
    public class NewsController : ControllerBase
    {
        private readonly INewsRepository _newsRepo;
        private readonly IConfirmResp _confirmResp;
        private readonly ILogger _logger;
        public NewsController(
            INewsRepository newsRepo, 
            IConfirmResp confirmResp,
            ILogger<NewsController> logger
        )
        {
            _newsRepo = newsRepo;
            _confirmResp = confirmResp;
            _logger = logger;
        }

        [HttpGet("getArticles/{category}")]
        public async Task<ActionResult> GetArticles(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                _logger.LogWarning("In news controller, get articels: The category of the news article must be provided.");
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
                _logger.LogError("In news controller, get news flash came back empty from repo");
                return NotFound("News flash not found");
            }
            // return newsFlash;
            var successConf = _confirmResp.ConfirmResponse(true, newsFlash);
            return Ok(successConf);
        }

    }
}

