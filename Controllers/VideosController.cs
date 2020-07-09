using AutoMapper;
using EliteForce.AppWideHelpers;
using EliteForce.Data;
using EliteForce.Dtos;
using EliteForce.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using EliteForce.AuthorizationRequirements;
using Microsoft.Extensions.Logging;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly IVideosRepository _videosRepo;
        private readonly IConfirmResp _confirmResp;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        

        public VideosController(
            IVideosRepository videosRepo, 
            IConfirmResp confirmResp, IMapper mapper,
            ILogger<VideosController> logger
            )
        {
            _videosRepo = videosRepo;
            _confirmResp = confirmResp;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger;
        }

        [HttpGet("getVideos")]
        public async Task<ActionResult> GetVideos()
        {
            var videos = await _videosRepo.GetVideos();
            var filteredVideos = _mapper.Map<List<MissionVideoGetDto>>(videos);

            if (filteredVideos.Count == 0)
            {
                _logger.LogError("In videos controller,  get videos, filterd videos was 0");
                return NotFound("No videos found");
            }
            return Ok(filteredVideos);
        }


        [HttpGet("getCategorizedVideos/{category}")]
        public async Task<ActionResult> GetCategorizedVideos(string category)
        {
            var videos = await _videosRepo.GetCategorizedVideos(category);
            var filteredVideos = _mapper.Map<List<MissionVideoGetDto>>(videos);

            if (filteredVideos.Count == 0)
            {
                _logger.LogError("In videos controller,  get categorized videos counted was 0");
                return NotFound("No videos found");
            }
            return Ok(filteredVideos);
        }

        [HttpPost("postRatings")]
        public async Task<ActionResult> PostRatings(RatingsDto rating)
        {
            var conf = await _videosRepo.RateVideo(rating);

            if (!conf)
            {
                _logger.LogError("In videos controller,  post ratings, no ratings returned from repo.");
                return NotFound("Rating failed");
            }
            var resp = _confirmResp.ConfirmResponse(true, "Rating Submitted");
            return Ok(resp);
        }

    }
}



