using Common;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Repo;
using Services.AuthModels;
using Services.AuthServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using NuGet.Common;
using System;
using Services.ServiceModels;
using common;
namespace AppointmentScheduling.Controllers
{
    public class AuthController : Controller
    {
        private readonly Authentication authentication;
        private readonly JwtToken jwtToken;
        private readonly Validation validation;
        private readonly UserRepo userRepo;
        private readonly Email _email;
        // GET: AuthController
        public AuthController(Authentication authentication, JwtToken jwtToken, Validation validation, UserRepo userRepo, Email email)
        {
            this.authentication = authentication;
            this.jwtToken = jwtToken;
            this.validation = validation;
            this.userRepo = userRepo;
            this._email = email;

        }
        public ActionResult Index()
        {
            return View();
        }


        // GET: AuthController/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: AuthController/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {

            if (!validation.IsUsernameUnique(model.UserName))
            {
                ModelState.AddModelError("UserName", "Username is already present.");
            }
            if (!validation.IsEmailUnique(model.Email))
            {
                ModelState.AddModelError("Email", "Email is already present.");
            }
            if (model.ImageFile != null)
            {
                string fileExtension = Path.GetExtension(model.ImageFile.FileName);
                if (fileExtension != ".jpeg" && fileExtension != ".jpg" && fileExtension != ".png")
                {
                    ModelState.AddModelError("ImageFile", "Please select a file with .jpeg, .jpg, or .png extension.");
                }
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }

                return View(model);
            }
            else
            {

                await authentication.RegisterUser(model);
                TempData["SuccessMessage"] = "User registered successfully.";
                if(model.Role == Roles.patient.ToString())
                    return RedirectToAction("Login", "Auth");
                return RedirectToAction("DoctorVerification", "Doctor", new { username = model.UserName});

            }

        }

        // GET: AuthController/Register
        public IActionResult Login()
        {
            var unauthorized = HttpContext.Request.Query["unauthorized"] == "true";
            if (unauthorized)
            {
                TempData["ErrorMessage"] = "You are not authorized to access this page. Please log in with appropriate credentials.";
            }
            return View();

        }

        // POST: AuthController/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                TblUser user = authentication.AuthenticateUser(model);
                if (user != null)
                {

                    string token = jwtToken.GenerateToken(user,out string role);

                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    };
                    HttpContext.Response.Cookies.Append("JWTToken", token, cookieOptions);
                    if (role == Roles.patient.ToString()) 
                        return RedirectToAction("Index", "Patient");
                    else if(role == Roles.doctor.ToString())
                    {
                        var isVerified = authentication.IsVerified(user);
                        if (isVerified == null)
                        {

                            return RedirectToAction("DoctorVerification", "Doctor", new { username = user.UserName });
                        }
                        else if (isVerified == true)
                            return RedirectToAction("Index", "Doctor");
                        else
                            return RedirectToAction("VerificationStatus", "Doctor");
                       
                    }
                    else
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                     
                }
                TempData["ErrorMessage"] = "Login Failed!";
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        // GET: AuthController/ForgotPassword

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string email)
        {
            var user = userRepo.GetUser(email);
            if(user!= null)
            {
                string recipient = email;
                string subject = "Reset Password";
                var lnkHref = "<a href='" + Url.Action("ResetPassword", "Auth", new { Email = user.Email, Time = DateTime.Now}, "https") + "'>Reset Password</a>";
                string body = "<b>Please find the Password Reset Link. </b><br/>" + lnkHref + "<br> Link would be active for 1 minute only.";
                _email.SendEmail(recipient, subject, body);
                TempData["SuccessMessage"] = "Email sent";
                 return View("ForgotPassword", "Auth");
            }
            TempData["ErrorMessage"] = "Email is not registered.";
            return View("ForgotPassword", "Auth");
        }

        // GET: AuthController/ForgotPassword
       
        
        public IActionResult ResetPassword(string Email , DateTime Time)
        {
            TimeSpan difference = DateTime.Now - Time;
            var isValid = difference.TotalMinutes <= 1;
            var resetPasswordModel = new ResetPasswordModel() ;

            if (isValid&&!string.IsNullOrEmpty(Email))
            {
               resetPasswordModel.Email = Email;
                return View(resetPasswordModel);
            }
            TempData["ErrorMessage"] = "LinkExpired";
            return View("ResetPassword",resetPasswordModel);
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if(resetPasswordModel.Password != resetPasswordModel.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Password doesn't match.");
            }
            if(!ModelState.IsValid)
            {
                return View(resetPasswordModel);
            }
            authentication.UpdatePassword(resetPasswordModel.Email,resetPasswordModel.Password);
            TempData["SuccessMessage"] = "Password Updated";
            return View("Login");
        }
        [NoCache]
        public IActionResult GetLoggedInUser()
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];

            
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);

            Claim userRole = parsedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            Claim usernameClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "username");
            var user = userRepo.GetUser(usernameClaim.Value);
            var loggedUser = new { UserName = user.UserName, ImagePath = user.UserImage, Role = userRole.Value , Name = user.FirstName+" "+user.LastName};

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,

            };
            var json = JsonSerializer.Serialize(loggedUser, options);
            return Ok(json);
        }

        [NoCache]
        public IActionResult UserProfile()
        {

            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);
            Claim usernameClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "username");

            var profile = authentication.GetProfile(usernameClaim.Value);
            return View(profile);
        }

        public IActionResult EditProfile(string username)
        {
            var profile = authentication.GetProfile(username);
            return View(profile);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileDTO model)
        {
           
            if (model.ImagePath != null)
            {
                string fileExtension = Path.GetExtension(model.ImageFile.FileName);
                if (fileExtension != ".jpeg" && fileExtension != ".jpg" && fileExtension != ".png")
                {
                    ModelState.AddModelError("ImageFile", "Please select a file with .jpeg, .jpg, or .png extension.");
                }
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }

                return View(model);
            }
            else
            {
                var user = await authentication.UpdateProfile(model);
                return RedirectToAction("UserProfile");
            }
            
        }
        public IActionResult Logout()
        {

            
            Response.Cookies.Delete("JWTToken");
            return RedirectToAction("Login", "Auth");
        }
    }
}
