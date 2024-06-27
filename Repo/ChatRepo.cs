using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo
{
    public class ChatRepo
    {

        private readonly AppointmentSchedulingContext _context;

        public ChatRepo(AppointmentSchedulingContext context)
        {
            _context = context;
        }

        public async Task CreateMessage(TblMessage message)
        {
            _context.TblMessages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task CreateChatRoom(TblChatroom chatroom)
        {

            _context.TblChatrooms.Add(chatroom);
            await _context.SaveChangesAsync();
        }

        public TblChatroom? GetChatRoom(Guid patientId,Guid doctorId)
        {
            return _context.TblChatrooms.FirstOrDefault(cr => cr.PatientId == patientId && cr.DoctorId == doctorId);

        }

        public  List<TblChatroom> GetChatRooms(Guid userId,string role)
        {
            if(role == Roles.patient.ToString())
            {
                return _context.TblChatrooms.Where(chatroom => chatroom.PatientId == userId).ToList();
            }
            else
            {
                return _context.TblChatrooms.Where(chatroom => chatroom.DoctorId == userId).ToList();
            }
        }

        public List<TblMessage> GetMessages(Guid patientId,Guid doctorId)
        {
            return _context.TblMessages.Where(message => (message.SenderId == patientId || message.SenderId == doctorId) && (message.RecieverId == patientId || message.RecieverId == doctorId)).ToList();
        }
    }
}
