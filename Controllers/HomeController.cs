using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EliteForce.Controllers
{
    [ApiController]
    [Route("api/home")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;


        public HomeController(
            ILogger<HomeController> logger, 
            IConfiguration iConfig
            )
        {
            _logger = logger;
            _configuration = iConfig;
        }

        [HttpGet]
        public JsonResult Get()
        {
            _logger.LogDebug("");
            return new JsonResult(new { test = "Home Testing" });
        }

    }

}
