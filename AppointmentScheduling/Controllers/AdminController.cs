using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Encodings;
using Services.AuthServices;
using Services.ServiceModels;
namespace AppointmentScheduling.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminServices _adminServices;
        private readonly JwtToken _jwtToken;

        public AdminController(AdminServices adminServices,JwtToken jwtToken)
        {
            _adminServices = adminServices;
            _jwtToken = jwtToken;
        }

        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            var nonVerified = _adminServices.GetNonVerifiedDoctors();
            return View(nonVerified);
        }

        [HttpPut]
        public async Task<IActionResult> VerifyDoctor(string username,string specialization,string degree)
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            var token = _jwtToken.GetToken(jwtToken);
            var usernameClaim = token.Claims.FirstOrDefault(c => c.Type == "username");
            var admin = usernameClaim.Value;
            await _adminServices.VerifyDoctor(username,specialization,degree,admin);
            return Ok(); 
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Patients()
        {
            var patients = _adminServices.GetPatients();
            return View(patients);
        }


        [Authorize(Roles = "admin")]
        public IActionResult Doctors()
        {
            var doctors = _adminServices.GetDoctors();
            return View(doctors);
        }
    }
}
