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


namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly IVideosRepository _videosRepo;
        private readonly IConfirmResp _confirmResp;
        private readonly IMapper _mapper;
        public VideosController(IVideosRepository videosRepo, IConfirmResp confirmResp, IMapper mapper)
        {
            _videosRepo = videosRepo;
            _confirmResp = confirmResp;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("getVideos")]
        [Authorize(Policy = Policies.Manager)]
        public async Task<ActionResult> GetVideos()
        {
            var videos = await _videosRepo.GetVideos();
            var filteredVideos = _mapper.Map<List<MissionVideoGetDto>>(videos);

            if (filteredVideos.Count == 0)
            {
                return NotFound("No videos found");
            }
            return Ok(filteredVideos);
        }


        [HttpGet("getCategorizedVideos/{category}")]
        [Authorize(Policy = Policies.Manager)]
        public async Task<ActionResult> GetCategorizedVideos(string category)
        {
            var videos = await _videosRepo.GetCategorizedVideos(category);
            var filteredVideos = _mapper.Map<List<MissionVideoGetDto>>(videos);

            if (filteredVideos.Count == 0)
            {
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
                return NotFound("Rating failed");
            }
            var resp = _confirmResp.ConfirmResponse(true, "Rating Submitted");
            return Ok(resp);
        }

    }
}



