using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.AuthServices;
using Domain.Models;
using Repo;
using Services.ServiceModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Text.Json;
namespace AppointmentScheduling.Controllers
{
    public class ChatController : Controller
    {
        private readonly UserRepo _userRepo;
        private readonly ChatServices _chatServices;

        public ChatController(ChatServices chatServices,UserRepo userRepo)
        {
            _userRepo = userRepo;
            _chatServices = chatServices;
        }

        public  IActionResult Chats(string? name,string? email)
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);
            var username = parsedToken.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
            var role  = parsedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
               var chats = _chatServices.GetChats(username,role, email,name);
            if(!email.IsNullOrEmpty())
            {
                TempData["Email"] = email;
            }
            else
            {
                TempData["Email"] = "";
            }
            return View(chats);
        }
        [HttpPost]
        public async  Task<IActionResult> Chat(string patient, string doctor)
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);
            var role = parsedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;


            var patientId = _userRepo.GetUser(patient).UserId;
            var doctorId =_userRepo.GetUser(doctor).UserId;
            var chatRoom = _chatServices.GetChatRoom(patientId,doctorId);

            if (chatRoom == null)
            {
                
                chatRoom = new TblChatroom
                {
                    PatientId = patientId,
                    DoctorId = doctorId
                };
                await _chatServices.CreateChatRoom(chatRoom); 
            }
            var model = new ChatRoomModel
            {
                ChatRoomId = chatRoom.ChatroomId,
                SenderId = role == Roles.patient.ToString()?patientId : doctorId, 
                ReceiverId = role == Roles.patient.ToString() ? doctorId : patientId 
            };

            return PartialView(model);
        }

        public IActionResult GetHistoryChats(string patientId,string doctorId)
        {
            var chats = _chatServices.GetHistoryChats(patientId, doctorId);
                 var options = new JsonSerializerOptions
                 {
                     ReferenceHandler = ReferenceHandler.Preserve,

                 };
            var json = JsonSerializer.Serialize(chats, options);
            return Ok(json);
           
        }

        public IActionResult NewConversation()
        {

            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);
            var username = parsedToken.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
            var role = parsedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            var newChats = _chatServices.GetNewChats(username, role);

            return View(newChats);
        }
    }
}
