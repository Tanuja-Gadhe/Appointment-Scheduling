

using System;
using common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Services.AuthServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using Repo;
using Common;

namespace AppointmentScheduling.Controllers
{
    public class PatientController : Controller
    {
        private readonly PatientServices patientServices;
        private readonly JwtToken _jwtToken;

        public PatientController(PatientServices patientServices,JwtToken jwtToken)
        {
            this.patientServices = patientServices;
            _jwtToken = jwtToken;
        }
        [NoCache]
        [Authorize(Roles ="patient")]
        public IActionResult Index()
        {
            
            return View();
        }

        [HttpPost]
        [Authorize(Roles ="patient")] 
        public async Task<IActionResult> BookAppointment(Guid availabilityId,string doctorEmail)
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            var token = _jwtToken.GetToken(jwtToken);
            var usernameClaim = token.Claims.FirstOrDefault(c => c.Type == "username");
            await patientServices.BookAppointment(usernameClaim.Value,doctorEmail,availabilityId);
            return Ok();
        }

        public async Task<IActionResult> AddAppointmentToWait(Guid availabilityId, string doctorEmail)
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            var token = _jwtToken.GetToken(jwtToken);
            var usernameClaim = token.Claims.FirstOrDefault(c => c.Type == "username");
            await patientServices.BookAppointment(usernameClaim.Value, doctorEmail, availabilityId,true);
            return Ok();
        }


        [Authorize(Roles = "patient")]
        public IActionResult Appointments(string status="all")
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            var token = _jwtToken.GetToken(jwtToken);
            var usernameClaim = token.Claims.FirstOrDefault(c => c.Type == "username");
            var username = usernameClaim.Value;
            var appointments = patientServices.GetAppointments(username);
            appointments = appointments.Where(appointment => appointment.Status.ToLower() == status.ToLower() || status.ToLower() == "all").ToList();
            return View("Appointments",appointments);
        }

        [HttpGet]
        public IActionResult ViewAppointment(Guid appointmentId)
        {
            var appointmentDetails = patientServices.ViewAppointment(appointmentId);
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,

            };
            var json = JsonSerializer.Serialize(appointmentDetails, options);
            return Ok(json);
        }

        [HttpPut]
        [Authorize(Roles ="patient,doctor")]
        public async Task<IActionResult> CancelAppointment(Guid appointmentId)
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            var token = _jwtToken.GetToken(jwtToken);
            var roleClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            await patientServices.UpdateAppointmentStatus(appointmentId,Status.cancelled.ToString(),roleClaim.Value);
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "patient,doctor")]
        public async Task<IActionResult> CompleteAppointment(Guid appointmentId)
        {
            await patientServices.UpdateAppointmentStatus(appointmentId,Status.completed.ToString());
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "doctor")]
        public async Task<IActionResult> AddAppointmentNotes(Guid appointmentId,string notes)
        {
            var updatednotes = await patientServices.UpdateAppointmentNotes(appointmentId, notes);
            return Ok(updatednotes);
        }
    }
}
