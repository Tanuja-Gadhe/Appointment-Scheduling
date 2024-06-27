using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.AuthServices;
using Services.ServiceModels;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Common;
using Repo;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using common;
using Domain.Models;
using System.Numerics;

namespace AppointmentScheduling.Controllers
{
    public class DoctorController : Controller
    {
        private readonly DoctorServices _doctorServices;
        private readonly JwtToken _jwtToken;
        private readonly Authentication _authentication;

        // GET: DoctorController
        public DoctorController(DoctorServices doctorServices,JwtToken jwtToken,Authentication authentication)
        {
            _doctorServices = doctorServices;
            _jwtToken = jwtToken;
            _authentication = authentication;
        }

        [Authorize(Roles ="doctor")]
        public ActionResult Index()
        {
            return View();
        }

        public  IActionResult DoctorVerification(string username)
        {
            var doctorVerifiaction = new DoctorVerificationModel() { UserName = username };
            return View(doctorVerifiaction);
        }
        
        [HttpPost]
        public async Task<IActionResult> DoctorVerification(DoctorVerificationModel model)
        {
          
            if (model.MedicalLicense != null)
            {
                string fileExtension = Path.GetExtension(model.MedicalLicense.FileName);
                if (fileExtension != ".pdf")
                {
                    ModelState.AddModelError("MedicalLicense", "Please select a file with .pdf extension");
                }
            }
            if (model.IdProof != null)
            {
                string fileExtension = Path.GetExtension(model.IdProof.FileName);
                if (fileExtension != ".pdf")
                {
                    ModelState.AddModelError("IdProof", "Please select a file with .pdf extension");
                }
            }
            if (model.MedicalDegreeCertificate != null)
            {
                string fileExtension = Path.GetExtension(model.MedicalDegreeCertificate.FileName);
                if (fileExtension != ".pdf")
                {
                    ModelState.AddModelError("MedicalDegreeCertificate", "Please select a file with .pdf extension");
                }
            }
            if (model.PostgraduateMedicalDegreeCertificate != null)
            {
                string fileExtension = Path.GetExtension(model.PostgraduateMedicalDegreeCertificate.FileName);
                if (fileExtension != ".pdf")
                {
                    ModelState.AddModelError("PostgraduateMedicalDegreeCertificate", "Please select a file with .pdf extension");
                }
            }
            if (model.SpecializationCertificate != null)
            {
                string fileExtension = Path.GetExtension(model.SpecializationCertificate.FileName);
                if (fileExtension != ".pdf")
                {
                    ModelState.AddModelError("SpecializationCertificate", "Please select a file with .pdf extension");
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
               await  _authentication.Verify(model);
                return View("VerificationStatus");
            }
            
        }

        public IActionResult VerificationStatus()
        {
            return View();
        }

        [HttpGet]
        [Route("/Doctor/GetDoctors")]
        public IActionResult GetDoctors()
        {
            try
            {
                var doctors = _doctorServices.GetDoctors()
                                .Select(user => new {
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    PhoneNumber = user.PhoneNumber,
                                    Address = user.Address,
                                    ImagePath = user.UserImage,
                                    Email = user.Email,
                                    Specialization = user.Specialization,
                                }).ToArray();

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                  
                };
                var json = JsonSerializer.Serialize(doctors, options);
                return Ok(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching doctors: {ex.Message}");
                return StatusCode(500); 
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchByName(string doctor)
        {
            var doctors = _doctorServices.GetDoctorsByName(doctor);
            TempData["SearchType"] = "name";
            TempData["SearchParameter"] = doctor;
            return View("Search",doctors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchBySpecialization(string specialization , string location)
        {
            var doctors = _doctorServices.GetDoctorsBySpecialization(specialization , location);
            TempData["SearchType"] = "specialization";
            TempData["SearchParameter"] = specialization+", "+location;
            return View("Search", doctors);
        }

        public IActionResult GetDoctorAvailability(string email)
        {

            var doctorsAvailability = _doctorServices.GetAvailability( email).ToArray();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,

            };
            var json = JsonSerializer.Serialize(doctorsAvailability, options);
            return Ok(json);
        }
        public IActionResult GetAppointmentsForWeek()
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            var token = _jwtToken.GetToken(jwtToken);
            var usernameClaim = token.Claims.FirstOrDefault(c => c.Type == "username");
            var username = usernameClaim.Value;
            var appointments = _doctorServices.GetAppointmentsForWeek(username).ToArray();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,

            };
            var json = JsonSerializer.Serialize(appointments, options);
            return Ok(json);
        }

        public IActionResult GetTodaysAvailabilityAndAppointments()
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            var token = _jwtToken.GetToken(jwtToken);
            var usernameClaim = token.Claims.FirstOrDefault(c => c.Type == "username");
            var username = usernameClaim.Value;
            var result = _doctorServices.GetTodaysAvailabilityAndAppointments(username).ToArray();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,

            };
            var json = JsonSerializer.Serialize(result, options);
            return Ok(json);
      
        }

        public IActionResult GetWeeklyBreakdown()
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            var token = _jwtToken.GetToken(jwtToken);
            var usernameClaim = token.Claims.FirstOrDefault(c => c.Type == "username");
            var username = usernameClaim.Value;

            var appointments = _doctorServices.GetWeeklyBreakdown(username);

            var groupedData = appointments.GroupBy(appointment => appointment.AppointmentDatetime.Date)
                                           .Select(group => new
                                           {
                                               DateTime = group.Key,
                                               Statuses = group.Select(g => g.AppointmentStatus ).ToList()
                                           }).ToArray();

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,

            };
            var json = JsonSerializer.Serialize(groupedData, options);
            return Ok(json);
            
        }

        public IActionResult Availability()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Availability(AvailabilityDTO availability)
        {
            if(availability.AppointmentCount == 0)
            {
                ModelState.AddModelError("AppointmentCount", "Appointment Count can not be 0");
            }
            if(availability.AvailabilityStartDatetime < DateTime.Now)
            {
                ModelState.AddModelError("AvailabilityStartDatetime", "Appointment start date and time cannot be past date time");
            }
            if (availability.AvailabilityStartDatetime < DateTime.Now)
            {
                ModelState.AddModelError("AvailabilityEndDatetime", "Appointment end date and time cannot be past date time");
            }
            if (ModelState.IsValid)
            {
                string jwtToken = HttpContext.Request.Cookies["JWTToken"];
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);
                Claim usernameClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "username");
                var username = usernameClaim.Value;

                await _doctorServices.AddAvailability(availability,username);
                return View();
            }
            return View(availability);
        }

        [HttpDelete]
        public IActionResult DeleteAvailability(Guid availabilityId)
        {
            _doctorServices.DeleteAvailability(availabilityId);
            return Ok();
        }

       
        public IActionResult UpdateAvailability(Guid availabilityId)
        {
            var availability = _doctorServices.GetAvailability(availabilityId);
            var model = new AvailabilityDTO()
            {
                AvailabilityStartDatetime = availability.AvailabilityStartDatetime,
                AvailabilityEndDatetime = availability.AvailabilityEndDatetime,
                AppointmentCount = availability.AppointmentCount,

            };

            var editModel = new EditAvailability()
            {
                AvailabilityId = availability.AvailabilityId,
                Availability = model,
            };
         
            return View("UpdateAvailability",editModel);
        }
        [HttpPost]
        public async  Task<IActionResult> UpdateAvailability(EditAvailability editModel)
        {
            if (editModel.Availability.AppointmentCount == 0)
            {
                ModelState.AddModelError("Availability.AppointmentCount", "Appointment Count can not be 0");
            }
            
            if (ModelState.IsValid)
            {

                await _doctorServices.UpdateAvailability(editModel.AvailabilityId,editModel.Availability);
                TempData["SuccessMessage"] = "Availability updated successfully!";
                return View("Availability");
            }
            
            return View(editModel);
        }

        public IActionResult Appointments(string status = "all")
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            var token = _jwtToken.GetToken(jwtToken);
            var usernameClaim = token.Claims.FirstOrDefault(c => c.Type == "username");
            var username = usernameClaim.Value;
            var appointments = _doctorServices.GetAppointments(username);
            appointments = appointments.Where(appointment => appointment.Status.ToLower() == status.ToLower() || status.ToLower() == "all").ToList();
            return View("Appointments", appointments);
        }
    }
}
