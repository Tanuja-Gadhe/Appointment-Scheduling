using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repo;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AuthServices
{
    public class ChatServices
    {
        private readonly ChatRepo _chatRepo;
        private readonly UserRepo _userRepo;
        private readonly DoctorVerificationRepo _doctorVerificationRepo;

        public ChatServices(ChatRepo chatRepo,UserRepo userRepo,DoctorVerificationRepo doctorVerificationRepo) { 
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _doctorVerificationRepo = doctorVerificationRepo;
        }

        public TblChatroom? GetChatRoom(Guid patientId,Guid doctorId)
        {
           var chatroom = _chatRepo.GetChatRoom(patientId, doctorId);
            return chatroom;
        }

        public async Task CreateChatRoom(TblChatroom chatroom)
        {
            await _chatRepo.CreateChatRoom(chatroom);
        }

        public List<ChatsDTO> GetChats(string username,string role,string email,string name)
        {
            var user = _userRepo.GetUser(username);
            var chatrooms = _chatRepo.GetChatRooms(user.UserId,role);

            if(role == Roles.patient.ToString())
            {
                var doctors = _userRepo.GetByRole(Roles.doctor.ToString());
                var chats = (from doctor in doctors
                            join chatroom in chatrooms
                            on doctor.UserId equals chatroom.DoctorId
                            select new ChatsDTO()
                            {
                                Email = doctor.Email,
                                Name = doctor.FirstName + " " + doctor.LastName,
                                ImagePath = doctor.UserImage
                            }).ToList();
                if(!email.IsNullOrEmpty()&&!name.IsNullOrEmpty())
                {
                    var isPresent = chats.FirstOrDefault(chat => chat.Email == email);
                    if (isPresent == null)
                    {
                        chats.Add(new ChatsDTO() { Email = email, Name = name });
                    }
                }
                return chats;
            }
            else
            {
                var patients = _userRepo.GetByRole(Roles.patient.ToString());
                var chats = (from patient in patients
                             join chatroom in chatrooms
                             on patient.UserId equals chatroom.PatientId
                             select new ChatsDTO()
                             {
                                 Email = patient.Email,
                                 Name = patient.FirstName + " " + patient.LastName,
                                 ImagePath = patient.UserImage
                             }).ToList();
                if (!email.IsNullOrEmpty() && !name.IsNullOrEmpty())
                {
                    var isPresent = chats.FirstOrDefault(chat => chat.Email == email);
                    if (isPresent == null)
                    {
                        chats.Add(new ChatsDTO() { Email = email, Name = name });
                    }
                }
                return chats;
            }
        }


        public List<MessageDTO> GetHistoryChats(string patientId,string doctorId)
        {
            var patient = _userRepo.GetUser(patientId);
            var doctor = _userRepo.GetUser(doctorId);
            var chats = _chatRepo.GetMessages(patient.UserId, doctor.UserId);
            var messages = chats.Select(message => new MessageDTO()
            {
                Content = message.Content,
                Timestamp = message.Timestamp,
                Sender = message.Sender,
                Reciever = message.Reciever,
            }).OrderBy(message =>message.Timestamp).ToList();
            return messages;

        }

        public List<ChatsDTO> GetNewChats(string username,string role)
        {
            var user = _userRepo.GetUser(username);
            if(role == Roles.patient.ToString())
            {
                var doctors = _userRepo.GetByRole(Roles.doctor.ToString());
                var  verifiedDoctors = _doctorVerificationRepo.GetVerifiedDoctors();
                doctors = doctors.Where(doctor => verifiedDoctors.Any(verifiedDoctor => verifiedDoctor.DoctorId == doctor.UserId)).ToList();
                var chatrooms = _chatRepo.GetChatRooms(user.UserId, role);
                var doctorsNotInChatrooms = doctors
                                            .Where(doctor => !chatrooms.Any(chatroom => chatroom.DoctorId == doctor.UserId))
                                            .Select(doctor => new ChatsDTO { 
                                                Name = doctor.FirstName+" "+doctor.LastName,
                                                Email = doctor.Email,
                                                ImagePath = doctor.UserImage
                                            }).ToList();
                return doctorsNotInChatrooms;
            }
            else
            {
                var patients = _userRepo.GetByRole(Roles.patient.ToString());
                var chatrooms = _chatRepo.GetChatRooms(user.UserId, role);
                var patientsNotInChatrooms = patients
                                            .Where(patient => !chatrooms.Any(chatroom => chatroom.PatientId == patient.UserId))
                                            .Select(patient => new ChatsDTO
                                            {
                                                Name = patient.FirstName + " " + patient.LastName,
                                                Email = patient.Email,
                                                ImagePath = patient.UserImage
                                            }).ToList();
                return patientsNotInChatrooms;
            }

        }
    }
}
