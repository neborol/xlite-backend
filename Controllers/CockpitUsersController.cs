using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EliteForce.AppWideHelpers;
using EliteForce.Data;
using EliteForce.Dtos;
using EliteForce.Entities;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using EliteForce.AuthorizationRequirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using EliteForce.NLoging;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CockpitUsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfirmResp _confirm;
        private readonly ILogger _logger;
        private readonly ILoggerManager _nLogger;


        public CockpitUsersController(
            IUserRepository userRepo,
            EliteDataContext context,
            IUnitOfWork uof,
            UserManager<User> userManager,
            IMapper mapper,
            IConfirmResp confirmResponse,
            ILogger<CockpitUsersController> logger,
            ILoggerManager nLogger
        )
        {
            _userRepo = userRepo;
            _unitOfWork = uof;
            _userManager = userManager;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _confirm = confirmResponse;
            _logger = logger;
            _nLogger = nLogger;
        }



        [HttpPut("updateStatusActive/{userEmail}")]
        [Authorize(Policy = Policies.Pilot)]
        public async Task<ActionResult> UpdateUserStatusActive(string userEmail)
        {
            if (String.IsNullOrEmpty(userEmail))
            {
                _logger.LogError("In cusers controller,  update status: incorrect model");
                _nLogger.LogError("In cusers controller nlog,  update status: incorrect model");
                return BadRequest(ModelState);
            }

            var result = await _userRepo.UpdateUserStatusActive(userEmail);

            if (!result.Succeeded)
            {
                _logger.LogError("In cusers controller,  update user status failed " + userEmail);
                _nLogger.LogError("In cusers controller nlog,  update user status failed " + userEmail);
                return BadRequest("The User Status was not updated");
            }
            _nLogger.LogInfo("The user with email: " + userEmail + " has been activated.");
            var confirm = _confirm.ConfirmResponse(true, "User with email " + userEmail + " is now Active");
            return Ok(confirm);
        }


        [HttpPut("updateStatusInActive/{userEmail}")]
        [Authorize(Policy = Policies.Pilot)]
        public async Task<ActionResult> UpdateUserStatusInActive(string userEmail)
        {
            if (String.IsNullOrEmpty(userEmail))
            {
                return BadRequest(ModelState);
            }

            var result = await _userRepo.UpdateUserStatusInActive(userEmail);

            if (!result.Succeeded)
            {
                _logger.LogError("In cusers controller,  update user status did not succeed at repo");
                _nLogger.LogError("In cusers controller nlog,  update user status did not succeed at repo");
                return BadRequest("The User Status was not updated");
            }
            var confirm = _confirm.ConfirmResponse(true, "User with email " + userEmail + " is now InActive");
            return Ok(confirm);
        }


        [HttpDelete("deleteUser/{userEmail}")]
        [Authorize(Policy = Policies.Pilot)]
        public async Task<ActionResult> DeleteUser(string userEmail)
        {
            if (String.IsNullOrEmpty(userEmail))
            {
                return BadRequest(ModelState);
            }

            var num = await _userRepo.DeleteUser(userEmail);

            if (num < 1)
            {
                _logger.LogError("In cusers controller,  delete user got nothing back from repo");
                _nLogger.LogError("In cusers controller nlog,  delete user got nothing back from repo");
                return BadRequest("The User could not be deleted");
            }
            var confirm = _confirm.ConfirmResponse(true, "The User " + userEmail + " has been deleted.");
            return Ok(confirm);
        }
    }
}