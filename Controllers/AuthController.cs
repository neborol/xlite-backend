using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EliteForce.Data;
using EliteForce.Dtos;
using EliteForce.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Authorization;
using EliteForce.AppWideHelpers;
using System.ComponentModel.Design;
using NETCore.MailKit.Core;
using Microsoft.AspNetCore.Routing;
using EliteForce.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace EliteForce.Controllers
{
    [EnableCors("ElitePolice")]
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

        public AuthController(
            IAuthRepository repo,
            IConfiguration config,
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfirmResp confirmResp,
            IMailService mailService,
            LinkGenerator linkGenerator
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
                return BadRequest(ModelState);
            }

            // Before registering the new user, first check if there is a user with this email already registered.
            var user2verify = await _userManager.FindByEmailAsync(useremail);

            // If user searched is not = null, then there is a user registered with this email, hence send back bad request.
            if (user2verify != null)
            {
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


            // If User got created, send a confirm email. 
            if (result.Succeeded)
            {
                // Now go ahead and create the claims in the claims table.
                var claims = new List<Claim>();
                claims.Add(new Claim("pilot", "false"));
                claims.Add(new Claim("news", "false"));
                claims.Add(new Claim("manager", "false"));
                claims.Add(new Claim("superAdmin", "false"));

                await _userManager.AddClaimsAsync(user, claims);


                // Generate an email
                // Generate email confirmation code and link first
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                // Within the user email, clicking on this link would call the VerifyEmail action method
                var regConfirmationLink = Url.Action(nameof(VerifEmail), "Auth", new { code, email = user.Email}, Request.Scheme);
                // Pass the link to the email body
                await _mailService.SendEmailAsync( "takang33@yahoo.de", "Confirm Your Registration", "<h1>Confirm Your Registration</h1><p>Hello, Please Confirm your Registration with Elite Force by clicking on the button. <br/><button href=\"" + regConfirmationLink + "\">Confirm Registration</button></p>");

                
                // return RedirectToAction(nameof(SuccessRegistration));
                
                //// var link = Url.Action("VerifyEmail", "Auth", new {userEmail = user.Email });
                //var link = _linkGenerator.GetPathByAction(HttpContext, "VerifEmail", "Auth", new {userId = user.Id, code });
                //await _emailService.SendAsync("takang33@yahoo.com", "Email Verify", link, true);
                //var conf = _confirmResp.ConfirmResponse(true, "An Email has been sent to you");
                //return Ok(conf);

                var conf = _confirmResp.ConfirmResponse(true, "Almost there, an email has been sent to you.");
                // await _mailService.SendEmailAsync("takang33@yahoo.de", "Confirm Your Registration", "<h1>Confirm Your Registration</h1><p>Hello, Please Confirm your Registration with Elite Force.</p>");
                // return Ok(conf);
                return Ok(conf);
            }
            else
            {
                //foreach (var error in result.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, error.Description);
                //}
                var conf = _confirmResp.ConfirmResponse(false, "Sorry registration failed.");
                return BadRequest(conf);
            }

        }


        public async Task<IActionResult> VerifEmail(string userId, string code)
        {
            var user2Activate = await _userManager.FindByIdAsync(userId);
            if (user2Activate == null)
            {
                return BadRequest();
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
                return Unauthorized(conf);
            }
            var orignal_userName = user.UserName;

            user.UserName = userForLogin.UserEmail; // SignInManager's CheckPaswordSignInAsync uses the user name to validate the user, registration used but the email, so set user name to equal email
            var signInResp = await _signInManager.CheckPasswordSignInAsync(user, userForLogin.Password, true);
            if (!signInResp.Succeeded)
            {
                var conf = _confirmResp.ConfirmResponse(false, "Authentication failed");
                return Unauthorized(conf);
            }
            // Put the UserName back as it was.
            user.UserName = orignal_userName;

            // Check if the user status is active or inactive before loging the user in, else, no login possible.
            if (user.Status == "INACTIVE")
            {
                var conf = _confirmResp.ConfirmResponse(false, "Not authorized");
                return Unauthorized(conf);
            }

            // Get all the claims from the database and add the common ones to it.
            var storedClaims = await _userManager.GetClaimsAsync(user);
            storedClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            storedClaims.Add(new Claim(ClaimTypes.Email, user.Email));
            storedClaims.Add(new Claim("CodeId", user.CodeNr));
            storedClaims.Add(new Claim(ClaimTypes.Name, user.UserName));

            // If the compiler makes it up to this point, then the user exists the credentials are correct, and so token should be generated.
            /* Token should be sent back at this stage.    return Ok(user.UserName); */
            //var claims = new[]
            //{
            //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            //    new Claim(ClaimTypes.Email, user.Email),
            //    new Claim("CodeId", user.CodeNr),
            //    new Claim(ClaimTypes.Name, user.UserName),
            //    new Claim(ClaimTypes.Role, "Admin")
            //};

            // Create a key that would not be readable in our token itself, which should be encrypted in byte[]... 
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("Data:AppSettings:Token").Value)); // Remember this should be stored in environment variables

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // Takes our security key above and the algorithm to hash it.

            // So now we have our signin credentials, we need to create a security token descriptor which will contain our claims, expirydate, and the sign-in credentials
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(storedClaims),
                Expires = DateTime.Now.AddDays(1), // Expires in a day's time // or .. .AddHours(2) or ... AddDays(1)
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


        [AcceptVerbs("Get", "Post")] // equivalent to [HttpGet][HttpPost]
       [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            AknowledgementDto conf;
            if(user == null)
            {
               conf = _confirmResp.ConfirmResponse(true, "");
               return Ok(conf);
            }
            else
            {
                //var message = "Email " + email + " is already in use.";
                var message = $"Email {email} is already in use.";
                conf = _confirmResp.ConfirmResponse(false, message);
                return Ok(conf);
            }
        }


        public ActionResult<IEnumerable<bool>> SendEmail(string emailAddress, string emailSubject, string emailData)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("EliteForceTeam", "neboroland@gmail.com"));
                message.To.Add(new MailboxAddress("TheCodeBuzz", emailAddress));
                message.Subject = emailSubject;
                message.Body = new TextPart("plain")
                {
                    Text = emailData
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {

                    client.Connect("smtp.gmail.com", 587, false);

                    //SMTP server authentication if needed
                    client.Authenticate("neboroland@gmail.com", "xxxxx");

                    client.Send(message);

                    client.Disconnect(true);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Error occured");
            }

            return Ok(true);
        }

    }
}











//namespace EliteForce.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly IAuthRepository _repo;
//        private readonly IConfiguration _config;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly UserManager<User> _userManager;
//        private readonly SignInManager<User> _signInManager;
//        private readonly IConfirmResp _confirmResp;
//        private Random _random = new Random();
//        private readonly IEmailService _emailService;
//        private readonly LinkGenerator _linkGenerator;
//        // public UserManager<IdentityUser> _userManager { get; }

//        public AuthController(
//            IAuthRepository repo,
//            IConfiguration config,
//            RoleManager<IdentityRole> roleManager,
//            UserManager<User> userManager,
//            SignInManager<User> signInManager,
//            IConfirmResp confirmResp,
//            IEmailService emailService,
//            LinkGenerator linkGenerator
//            )
//        {
//            _repo = repo;
//            _config = config;
//            _roleManager = roleManager;
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _confirmResp = confirmResp;
//            _emailService = emailService;
//            _linkGenerator = linkGenerator;
//        }


//        //[HttpPost("register")]
//        //public async Task<IActionResult> Register(UserForRegisterDto userForRegister)
//        //{
//        //    var username = userForRegister.Username.ToLower();
//        //    var useremail = userForRegister.UserEmail.ToLower();

//        //    if (!ModelState.IsValid)
//        //    {
//        //        return BadRequest("Invalid Request");
//        //    }

//        //    if (await _repo.UserExists(useremail))
//        //        return BadRequest("A User with this email " + useremail + " already exists");

//        //    // If User does not exist, go ahead and create a new User.
//        //    var userToCreate = new User
//        //    {
//        //        UserName = userForRegister.Username.ToLower(),
//        //        Email = userForRegister.UserEmail.ToLower(),
//        //        FirstName = userForRegister.FirstName.ToLower(),
//        //        LastName = userForRegister.LastName.ToLower(),
//        //        Phone = userForRegister.Phone,
//        //        City = userForRegister.City,
//        //        Status = "INACTIVE",
//        //        Subscriptions = new List<Entities.Subscription>()
//        //    };

//        //    // Create a user
//        //    var userCreatedResult = await _userManager.CreateAsync(userToCreate, userForRegister.Password);

//        //    // Check if the user is created successfully and then do the sign-in
//        //    if (userCreatedResult.Succeeded)
//        //    {
//        //        _signInManager.SignInAsync(userToCreate, )
//        //    }

//        //    // var createdUser = await _repo.Register(userToCreate, userForRegister.Password);

//        //    // return CreatedAtRoute();
//        //    return Ok(new
//        //    {
//        //        Success = true
//        //    });

//        //}


//        [HttpPost("register")]
//        public async Task<IActionResult> Register(UserForRegisterDto userForRegister)
//        {

//            var username = userForRegister.Username.ToLower();
//            var useremail = userForRegister.UserEmail.ToLower();
//            var user = new User { UserName = username, Email = useremail };

//            //if (await _repo.UserExists(useremail))
//            //    return BadRequest("A User with this email " + useremail + " already exists");

//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var user2verify = await _userManager.FindByEmailAsync(useremail);
//            if (user2verify != null)
//            {
//                return BadRequest("A User with this email " + useremail + " already exists");
//            }

//            // Generate the random code needed by the user
//            Code randCode = await _repo.GetSCEFRandomNr();

//            // Add other user properties to the empty user instance created above
//            try
//            {
//                user.CodeNr = "SCEF-" + randCode.CodeNr;
//                user.FirstName = userForRegister.FirstName;
//                user.LastName = userForRegister.LastName;
//                user.City = userForRegister.City ?? "";
//                user.PhoneNumber = userForRegister.Phone;
//                user.Status = "INACTIVE";
//                if (randCode.CodeNr.Length == 4 || !string.IsNullOrWhiteSpace(randCode.CodeNr))
//                {
//                    _repo.RemoveSCEFRandomNr(randCode);
//                }
//            }
//            catch
//            {
//                throw (new Exception("Radomisation failed"));
//            }

//            // If everything goes well so far, add the user to the data base
//            var result = await _userManager.CreateAsync(user, userForRegister.Password);


//            // If User got created, send a confirm email. 
//            if (result.Succeeded)
//            {
//                // Generate an email
//                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
//                // var link = Url.Action("VerifyEmail", "Auth", new {userEmail = user.Email });
//                var link = _linkGenerator.GetPathByAction(HttpContext, "VerifEmail", "Auth", new { userId = user.Id, code });
//                await _emailService.SendAsync("takang33@yahoo.com", "Email Verify", link, true);
//                var conf = _confirmResp.ConfirmSuccess(true, "An Email has been sent to you");
//                return Ok(conf);
//                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
//                //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
//                //var callbackUrl = Url.Page(
//                //"/Account/ConfirmEmail",
//                //pageHandler: null,
//                //values: new { area = "Identity", userId = user.Id, code = code },
//                //protocol: Request.Scheme);

//                //SendEmail(useremail, "Confirm your email",
//                //$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

//                //if (_userManager.Options.SignIn.RequireConfirmedAccount)
//                //{
//                //    return RedirectToPage("RegisterConfirmation", new { email = useremail });
//                //}
//                //else
//                //{
//                //    await _signInManager.SignInAsync(user, isPersistent: false);
//                //    //return LocalRedirect(returnUrl);
//                //    // RedirectToAction("SuccessfullyRegistered", "Auth");
//                //    var conf = _confirmResp.ConfirmSuccess(true, "You successfully registered");
//                //    return Ok(conf);
//                //}
//            }
//            else
//            {
//                foreach (var error in result.Errors)
//                {
//                    ModelState.AddModelError(string.Empty, error.Description);
//                }
//            }



//            // If User does not exist, go ahead and create a new User.
//            //var userToCreate = new User
//            //{
//            //    UserName = username,
//            //    Email = useremail,
//            //    FirstName = userForRegister.FirstName.ToLower(),
//            //    LastName = userForRegister.LastName.ToLower(),
//            //    PhoneNumber = userForRegister.Phone,
//            //    City = userForRegister.City,
//            //    Status = "INACTIVE",
//            //    Subscriptions = new List<Entities.Subscription>()
//            //};

//            //var createdUser = await _repo.Register(userToCreate, userForRegister.Password);

//            // return CreatedAtRoute();
//            return Ok(new
//            {
//                Success = true
//            });

//        }


//        public async Task<IActionResult> VerifEmail(string userId, string code)
//        {
//            var user2Activate = await _userManager.FindByIdAsync(userId);
//            if (user2Activate == null)
//            {
//                return BadRequest();
//            }

//            var result = await _userManager.ConfirmEmailAsync(user2Activate, code);
//            if (result.Succeeded)
//            {
//                var conf = _confirmResp.ConfirmSuccess(true, "Email Verified");
//                return Ok(conf);
//            }
//            var conffail = _confirmResp.ConfirmFailure(false, "Email Verification failed");
//            return NotFound(conffail);
//            //if (user2Activate != null)
//            //{
//            //    user2Activate.Status = "ACTIVE";
//            //    await _userManager.UpdateAsync(user2Activate);
//            //}
//            //var conf = _confirmResp.ConfirmSuccess(true, "Email Verified");
//            //return Ok(conf);
//        }


//        [HttpPost("Trylogin")]
//        public async Task<IActionResult> TryLogin(UserForLoginDto userForLogin, [FromHeader()] string currentUser)
//        {
//            var user = await _userManager.FindByEmailAsync(userForLogin.UserEmail);
//            if (user == null)
//            {
//                var conf = _confirmResp.ConfirmFailure(false, "Credentials failed");
//                return Unauthorized(conf);
//            }

//            user.UserName = userForLogin.UserEmail; // SignInManager's CheckPaswordSignInAsync uses the user name to validate the user, registration used but the email, so set user name to equal email
//            var signInRes = await _signInManager.CheckPasswordSignInAsync(user, userForLogin.Password, true);
//            if (!signInRes.Succeeded)
//            {
//                var conf = _confirmResp.ConfirmFailure(false, "Authentication failed");
//                return Unauthorized(conf);
//            }

//            /* Token should be sent back at this stage.    return Ok(user.UserName); */

//            var claims = new[]
//            {
//                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                new Claim(ClaimTypes.Name, user.UserName),
//                new Claim(ClaimTypes.Role, "Admin")
//            };

//            // Create a key that would not be readable in our token itself, which should be encrypted in byte[]... 
//            var key = new SymmetricSecurityKey(Encoding.UTF8
//                .GetBytes(_config.GetSection("Data:AppSettings:Token").Value)); // Remember this should be stored in environment variables

//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // Takes our security key above and the algorithm to hash it.

//            // So now we have our signin credentials, we need to create a security token descriptor which will contain our claims, expirydate, and the sign-in credentials
//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(claims),
//                Expires = DateTime.Now.AddDays(1), // Expires in a day's time // or .. .AddHours(2)
//                SigningCredentials = creds
//            };

//            // As well as a token descriptor above, we also need a token handler
//            var tokenHandler = new JwtSecurityTokenHandler();

//            // Now, using our token handler above, we can create a token 
//            var token = tokenHandler.CreateToken(tokenDescriptor);

//            // Now return our token as an object
//            return Ok(new
//            {
//                token = tokenHandler.WriteToken(token)
//            });


//        }

//        //[HttpPost("login")]
//        //public async Task<IActionResult> Login(UserForLoginDto userForLogin)
//        //{
//        //    var userFromRepo = await _repo.Login(userForLogin.UserEmail.ToLower(), userForLogin.Password);

//        //    // Check if user from Repo exists, if not, stop
//        //    if (userFromRepo == null)
//        //        return Unauthorized();

//        //    // Since user exists, now read the token sent from the browser
//        //    var claims = new[]
//        //    {
//        //        new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
//        //        new Claim(ClaimTypes.Name, userFromRepo.UserName)
//        //    };

//        //    // Create a key that would not be readable in our token itself, which should be encrypted in byte[]... 
//        //    var key = new SymmetricSecurityKey(Encoding.UTF8
//        //        .GetBytes(_config.GetSection("Data:AppSettings:Token").Value)); // Remember this should be stored in environment variables

//        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // Takes our security key above and the algorithm to hash it.

//        //    // So now we have our signin credentials, we need to create a security token descriptor which will contain our claims, expirydate, and the sign-in credentials
//        //    var tokenDescriptor = new SecurityTokenDescriptor
//        //    {
//        //        Subject = new ClaimsIdentity(claims),
//        //        Expires = DateTime.Now.AddDays(1), // Expires in a day's time // or .. .AddHours(2)
//        //        SigningCredentials = creds
//        //    };

//        //    // As well as a token descriptor above, we also need a token handler
//        //    var tokenHandler = new JwtSecurityTokenHandler();

//        //    // Now, using our token handler above, we can create a token 
//        //    var token = tokenHandler.CreateToken(tokenDescriptor);

//        //    // Now return our token as an object
//        //    return Ok(new
//        //    {
//        //        token = tokenHandler.WriteToken(token)
//        //    });
//        //}


//        // Generate a random 4 digit number
//        public string GenerateSCEFRandomNo()
//        {
//            return _random.Next(0, 9999).ToString("D4");
//        }


//        [AcceptVerbs("Get", "Post")] // equivalent to [HttpGet][HttpPost]
//        [AllowAnonymous]
//        public async Task<IActionResult> IsEmailInUse(string email)
//        {
//            var user = await _userManager.FindByEmailAsync(email);
//            AknowledgementDto conf;
//            if (user == null)
//            {
//                conf = _confirmResp.ConfirmSuccess(true, "");
//                return Ok(conf);
//            }
//            else
//            {
//                //var message = "Email " + email + " is already in use.";
//                var message = $"Email {email} is already in use.";
//                conf = _confirmResp.ConfirmFailure(false, message);
//                return Ok(conf);
//            }
//        }


//        public ActionResult<IEnumerable<bool>> SendEmail(string emailAddress, string emailSubject, string emailData)
//        {
//            try
//            {
//                var message = new MimeMessage();
//                message.From.Add(new MailboxAddress("EliteForceTeam", "neboroland@gmail.com"));
//                message.To.Add(new MailboxAddress("TheCodeBuzz", emailAddress));
//                message.Subject = emailSubject;
//                message.Body = new TextPart("plain")
//                {
//                    Text = emailData
//                };

//                using (var client = new MailKit.Net.Smtp.SmtpClient())
//                {

//                    client.Connect("smtp.gmail.com", 587, false);

//                    //SMTP server authentication if needed
//                    client.Authenticate("neboroland@gmail.com", "xxxxx");

//                    client.Send(message);

//                    client.Disconnect(true);
//                }

//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//                return StatusCode(500, "Error occured");
//            }

//            return Ok(true);
//        }

//    }
//}