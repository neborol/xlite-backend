using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EliteForce.Data;
using EliteForce.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using EliteForce.AuthorizationRequirements;
using EliteForce.Dtos;
using AutoMapper;
using EliteForce.AppWideHelpers;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly EliteDataContext _context;
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfirmResp _confirmResp;

        public UsersController(
            IUserRepository userRepo, 
            EliteDataContext context, 
            IUnitOfWork uof, 
            UserManager<User> userManager,
            IMapper mapper,
            IConfirmResp confirmResponse
        )
        {
            _userRepo = userRepo;
            _context = context;
            _unitOfWork = uof;
            _userManager = userManager;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _confirmResp = confirmResponse;
        }


        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Policy = "NewsManager")]
        [HttpGet("getusers")]
        [Authorize(Policy = Policies.News)]
        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // var user = ClaimsPrincipal.Current.Claims;
            var userId = _userManager.GetUserId(HttpContext.User);
            var usr = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            var clms = await _userManager.GetClaimsAsync(usr);
            var users = await _userRepo.GetUsers();
            return Ok(users);
        }


        /* Working well with automapper */
        //[HttpGet("getuserssuper")]
        ////       [Authorize(Policy = Policies.SuperAdmin)]
        //public async Task<ActionResult<IEnumerable<UserForFunctionsDto>>> getuserssuper()
        //{
        //    var users = await _userRepo.GetUsers();
        //    var usersForFunctions = new List<UserForFunctionsDto>();
        //    foreach (var user in users)
        //    {
        //        usersForFunctions.Add(new UserForFunctionsDto() { 
        //            Id = user.Id,
        //            FirstName = user.FirstName,
        //            LastName = user.LastName,
        //            CodeNr = user.CodeNr
        //        });
        //    }
        //    return Ok(usersForFunctions);
        //}


        [HttpGet("getuserssuper")]
        [Authorize(Policy = Policies.SuperAdmin)]
        public async Task<ActionResult<IEnumerable<UserForFunctionsDto>>> getuserssuper()
        {
            var users = await _userRepo.GetUsers();
            return Ok(_mapper.Map<IEnumerable<UserForFunctionsDto>>(users));
        }


        [HttpGet("getclaims/{id}")]
        [Authorize(Policy = Policies.SuperAdmin)]
        public async Task<ActionResult<IEnumerable<Claim>>> Getclaims(string Id)
        {
            var claims = await _userRepo.GetClaims(Id);
            var selectedClaims = _mapper.Map<IEnumerable<ClaimsDto>>(claims);
            return Ok(selectedClaims);
        }

        [HttpPut("updateclaims/{id}")]
        [Authorize(Policy = Policies.SuperAdmin)]
        public async Task<ActionResult<ConfirmResp>> Updateclaims(string Id, IList<UpdateClaimsDto> ClaimsObj)
        {
            // var claims = await _userRepo.GetClaims(Id);
            var updatedClaims = _mapper.Map<IList<Claim>>(ClaimsObj);
            var adjustedUser = await _userManager.FindByIdAsync(Id);

            var IdResult = await _userRepo.UpdateClaims(Id, updatedClaims);
            if (!IdResult.Succeeded)
            { 
                var errorConf = _confirmResp.ConfirmResponse(false, "Claims " + adjustedUser.FirstName + " " + adjustedUser.LastName + " did not update");
                return BadRequest(errorConf);
            }
            var successConf = _confirmResp.ConfirmResponse(true, "Success! " + adjustedUser.FirstName + " " + adjustedUser.LastName + " Claims updated");
            return Ok(successConf);
        }


        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            //var user = await _userRepo.GetSingleUser(id);
            //if (user == null)
            //{
            //    return NotFound();
            //}
            //return Ok(user);
            return Ok();
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;  // The use of dbContext should be limited to the Repositories and the unitOfWork only.

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _unitOfWork.CompleteAsync();

            // return CreatedAtAction("GetUser", new { id = user.Id }, user);
            return Ok("Testing");
        }



        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        // GET: api/Users
        // [HttpGet("Check")]
        // [Authorize(Policy = "Claim.DoB")]
        // [Authorize(Roles = "Admin")]
        //public IActionResult Check()
        //{
        //    var grandmaClaims = new List<Claim>()
        //    {
        //        new Claim(ClaimTypes.Name, "Bob"),
        //        new Claim(ClaimTypes.Email, "Bob@fmail.com"),
        //        new Claim(ClaimTypes.DateOfBirth, "11/11/2000"),
        //        new Claim(ClaimTypes.Role, "Admin"),
        //        new Claim("Grandma.Says", "Very nice Boy"),
        //    };

        //    var licenseClaims = new List<Claim>()
        //    {
        //        new Claim(ClaimTypes.Name, "Bob Ferma"),
        //        new Claim("DrivingLicense", "A+"),
        //    };

        //    var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
        //    var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government Identity");

        //    // create a user principal
        //    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenseIdentity });
        //    HttpContext.SignInAsync(userPrincipal);
        //}
    }
}






//namespace EliteForce.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UsersController : ControllerBase
//    {
//        private readonly EliteDataContext _context;
//        private readonly IUserRepository _userRepo;
//        private readonly IUnitOfWork _unitOfWork;
//        public UsersController(IUserRepository userRepo, EliteDataContext context, IUnitOfWork uof)
//        {
//            _userRepo = userRepo;
//            _context = context;
//            _unitOfWork = uof;
//        }


//        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
//        {
//            var users = await _userRepo.GetUsers();
//            return Ok(users);
//        }

//        // GET: api/Users/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<User>> GetUser(string id)
//        {
//            var user = await _userRepo.GetSingleUser(id);
//            if (user == null)
//            {
//                return NotFound();
//            }
//            return Ok(user);
//        }

//        // PUT: api/Users/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
//        // more details see https://aka.ms/RazorPagesCRUD.
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutUser(string id, User user)
//        {
//            if (id != user.Id)
//            {
//                return BadRequest();
//            }

//            _context.Entry(user).State = EntityState.Modified;  // The use of dbContext should be limited to the Repositories and the unitOfWork only.

//            try
//            {
//                await _unitOfWork.CompleteAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!string.IsNullOrEmpty(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // POST: api/Users
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
//        // more details see https://aka.ms/RazorPagesCRUD.
//        [HttpPost]
//        public async Task<ActionResult<User>> PostUser(User user)
//        {
//            _context.Users.Add(user);
//            await _unitOfWork.CompleteAsync();

//            return CreatedAtAction("GetUser", new { id = user.Id }, user);
//        }

//        // DELETE: api/Users/5
//        [HttpDelete("{id}")]
//        public async Task<ActionResult<User>> DeleteUser(int id)
//        {
//            var user = await _context.Users.FindAsync(id);
//            if (user == null)
//            {
//                return NotFound();
//            }

//            _context.Users.Remove(user);

//            // await _context.SaveChangesAsync();
//            await _unitOfWork.CompleteAsync();

//            return user;
//        }

//        private bool UserExists(string id)
//        {
//            return _context.Users.Any(e => e.Id == id);
//        }

//        // GET: api/Users
//        [HttpGet("Check")]
//        // [Authorize(Policy = "Claim.DoB")]
//        [Authorize(Roles = "Admin")]
//        public IActionResult Check()
//        {
//            var grandmaClaims = new List<Claim>()
//            {
//                new Claim(ClaimTypes.Name, "Bob"),
//                new Claim(ClaimTypes.Email, "Bob@fmail.com"),
//                new Claim(ClaimTypes.DateOfBirth, "11/11/2000"),
//                new Claim(ClaimTypes.Role, "Admin"),
//                new Claim("Grandma.Says", "Very nice Boy"),
//            };

//            var licenseClaims = new List<Claim>()
//            {
//                new Claim(ClaimTypes.Name, "Bob Ferma"),
//                new Claim("DrivingLicense", "A+"),
//            };

//            var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
//            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government Identity");

//            // create a user principal
//            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenseIdentity });
//            HttpContext.SignInAsync(userPrincipal);
//            return RedirectToAction("GetUsers");
//        }
//    }
//}
