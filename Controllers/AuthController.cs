using EliteForce.AppWideHelpers;
using EliteForce.Data;
using EliteForce.Dtos;
using EliteForce.Entities;
using EliteForce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EliteForce.Controllers
{
    [EnableCors("EliteCorsPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfirmResp _confirmResp;
        private Random _random = new Random();
        private readonly IMailService _mailService;
        private readonly LinkGenerator _linkGenerator;
        // public UserManager<IdentityUser> _userManager { get; }
        private readonly ILogger _logger;
        private IConfiguration _configuration { get; }



        public AuthController(
            IAuthRepository repo,
            IConfiguration config,
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfirmResp confirmResp,
            IMailService mailService,
            LinkGenerator linkGenerator,
            ILogger<NewsController> logger,
            IConfiguration configuration
            )
        {
            _repo = repo;
            _config = config;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _confirmResp = confirmResp;
            _mailService = mailService;
            _linkGenerator = linkGenerator;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegister)
        {
            var username = userForRegister.Username.ToLower();
            var useremail = userForRegister.UserEmail.ToLower();
            var user = new User { UserName = username, Email = useremail };

            //if (await _repo.UserExists(useremail))
            //    return BadRequest("A User with this email " + useremail + " already exists");

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model passed to User Registration, AuthController");
                return BadRequest(ModelState);
            }

            // Before registering the new user, first check if there is a user with this email already registered.
            var user2verify = await _userManager.FindByEmailAsync(useremail);

            // If user searched is not = null, then there is a user registered with this email, hence send back bad request.
            if (user2verify != null)
            {
                _logger.LogWarning("User2Verify could not be found");
                return BadRequest("A User with this email " + useremail + " already exists");
            }

            // Before saving the new user in the Database, Generate the random code needed by the business logic.
            Code randCode = await _repo.GetSCEFRandomNr();

            // Add other required business properties to the empty user instance created above before saving in the database.
            try
            {
                user.CodeNr = "SCEF-" + randCode.CodeNr;
                user.FirstName = userForRegister.FirstName;
                user.LastName = userForRegister.LastName;
                user.City = userForRegister.City ?? "";
                user.PhoneNumber = userForRegister.Phone;
                user.DateJoined = DateTime.UtcNow;
                user.Status = "INACTIVE";
                if (randCode.CodeNr.Length == 4 || !string.IsNullOrWhiteSpace(randCode.CodeNr))
                {
                    _repo.RemoveSCEFRandomNr(randCode);
                }
            }
            catch
            {
                throw (new Exception("Radomisation failed"));
            }

            // If everything goes well so far, add the new user to the database and also provide user password for automatic encryption.
            var result = await _userManager.CreateAsync(user, userForRegister.Password);


            // If User got created, next send a confirmation email. 
            if (result.Succeeded)
            {
                // Now go ahead and create the claims in the claims table.
                var claims = new List<Claim>();
                claims.Add(new Claim("pilot", "false"));
                claims.Add(new Claim("news", "false"));
                claims.Add(new Claim("manager", "false"));
                claims.Add(new Claim("superAdmin", "false"));

                // Store the user claims under this user and save it to the database
                await _userManager.AddClaimsAsync(user, claims);

                // Generate email confirmation code and link first
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Within the user email, clicking on this link would call the VerifyEmail action method
                var regConfirmationLink = Url.Action("VerifEmail", "Auth", new { code, userId = user.Id}, Request.Scheme);
                
                // Pass the link to the email body
                await _mailService.SendEmailAsync( "neboroland@gmail.com", "Confirm Your Registration", $"<h1>Confirm Your Registration</h1><p>Hello" +
                    user.FirstName + " " + user.LastName + ", Please Confirm your Registration with Elite Force by clicking on the following link. <br/><a href=\"{regConfirmationLink}\">Confirm Registration</a></p>");

                // Sent the registered user his code number:
                await _mailService.SendEmailAsync(useremail, "Your Membership Code-Number", "<p>Hello" + user.FirstName +
                    " " + user.LastName + ", Thanks for registering. Your membership code number is : <h1>" + user.CodeNr + "</h1> </p>");

                // return RedirectToAction(nameof(SuccessRegistration));

                //// var link = Url.Action("VerifyEmail", "Auth", new {userEmail = user.Email });
                //var link = _linkGenerator.GetPathByAction(HttpContext, "VerifEmail", "Auth", new {userId = user.Id, code });
                //await _emailService.SendAsync("takang33@yahoo.com", "Email Verify", link, true);
                //var conf = _confirmResp.ConfirmResponse(true, "An Email has been sent to you");
                //return Ok(conf);

                var conf = _confirmResp.ConfirmResponse(true, "Almost there, wait for the admin to activate your account.");
                // await _mailService.SendEmailAsync("takang33@yahoo.de", "Confirm Your Registration", 
                // "<h1>Confirm Your Registration</h1><p>Hello, Please Confirm your Registration with Elite Force.</p>");
                // return Ok(conf);
                _logger.LogInformation("Registration went through but pending email confirmation");
                return Ok(conf);
            }
            else
            {
                //foreach (var error in result.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, error.Description);
                //}
                var conf = _confirmResp.ConfirmResponse(false, "Sorry registration failed.");
                _logger.LogError("Sorry, registration failed.");
                return BadRequest(conf);
            }

        }


        public async Task<IActionResult> VerifEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest("Something not right with the credentials");
            }
            var user2Activate = await _userManager.FindByIdAsync(userId);
            if (user2Activate == null)
            {
                return BadRequest("Expected validation can not be found");
            }

            var result = await _userManager.ConfirmEmailAsync(user2Activate, code);
            if (result.Succeeded)
            {
                var conf = _confirmResp.ConfirmResponse(true, "Email Verified");
                return Ok(conf);
            }
            var conffail = _confirmResp.ConfirmResponse(false, "Email Verification failed");
            return NotFound(conffail);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLogin)
        {
            var user = await _userManager.FindByEmailAsync(userForLogin.UserEmail);
            if (user == null)
            {
               var conf = _confirmResp.ConfirmResponse(false, "Credentials failed");
                _logger.LogError("Login credentials failed for " + userForLogin.UserEmail );
                return Unauthorized(conf);
            }
            var orignal_userName = user.UserName;

            user.UserName = userForLogin.UserEmail; // SignInManager's CheckPaswordSignInAsync uses the user name to validate the user, registration used but the email, so set user name to equal email
            var signInResp = await _signInManager.CheckPasswordSignInAsync(user, userForLogin.Password, true);
            if (!signInResp.Succeeded)
            {
                var conf = _confirmResp.ConfirmResponse(false, "Authentication failed");
                _logger.LogError("Authentication failed for " + user.FirstName + " with the email: " + userForLogin.UserEmail);
                return Unauthorized(conf);
            }
            // Put the UserName back as it was.
            user.UserName = orignal_userName;

            // Check if the user status is active or inactive before loging the user in, else, no login possible.
            if (user.Status == "INACTIVE")
            {
                var conf = _confirmResp.ConfirmResponse(false, "Not authorized");
                _logger.LogError("User with first name " + user.FirstName + " and with the email: " + userForLogin.UserEmail + ", not active.");
                return Unauthorized(conf);
            }

            // Get all the claims from the database and add the common ones to it.
            var storedClaims = await _userManager.GetClaimsAsync(user);
            storedClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            storedClaims.Add(new Claim(ClaimTypes.Email, user.Email));
            storedClaims.Add(new Claim("CodeId", user.CodeNr));
            storedClaims.Add(new Claim(ClaimTypes.Name, user.UserName));

          
            // Create a key that would not be readable in our token itself, which should be encrypted in byte[]... 
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("Data:AppSettings:Token").Value)); // Remember this should be stored in environment variables

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // Takes our security key above and the algorithm to hash it.

            // So now we have our signin credentials, we need to create a security token descriptor which will contain our claims, expirydate, and the sign-in credentials
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(storedClaims),
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddHours(2),
                // Expires = DateTime.Now.AddDays(1), // Expires in a day's time // or .. .AddHours(2) or ... AddDays(1)
                SigningCredentials = creds
            };

            // As well as a token descriptor above, we also need a token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Now, using our token handler above, we can create a token 
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Now return our token as an object
            return Ok(new
            {
                Success = true,
                token = tokenHandler.WriteToken(token)
            });
        }


        // Generate a random 4 digit number
        public string GenerateSCEFRandomNo()
        {
            return _random.Next(0, 9999).ToString("D4");
        }



        [HttpPost("resetpw")]
        public async Task<ActionResult> ChangePassword(PasswordResetDto pwrDto)
        {
            if (pwrDto.Email != (_configuration["Improvised:PwReset:Email"]).ToLower())
            {
                return Unauthorized();
            }

            var userWhosePwIs2bReset = await _userManager.FindByEmailAsync(pwrDto.Email);
            // If user searched is not = null, then there is a user registered with this email, hence send back bad request.
            if (userWhosePwIs2bReset == null)
            {
                _logger.LogWarning("Something went wrong code 3");
                return BadRequest("Reset did not work out");
            }

            // Generate email confirmation token required by password reset.
            var tooken = await _userManager.GeneratePasswordResetTokenAsync(userWhosePwIs2bReset);

            var result = await _userManager.ResetPasswordAsync(userWhosePwIs2bReset, tooken, pwrDto.Password);

            if (result.Succeeded)
            {
                await _mailService.SendEmailAsync(pwrDto.Email, "Password Reset", "<p>Thanks! Your Password has been reset with the use of the email: " + pwrDto.Email + ".</p>");

                var successConf = _confirmResp.ConfirmResponse(true, "Success! " + "Your password has been reset");
                return Ok(successConf);
            }
            return BadRequest("Password reset failed completely");
        }


    }
}

