
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Repo;
using Domain.Models;

namespace AppointmentScheduling.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatRepo _chatRepo;
        private readonly UserRepo _userRepo;

        public ChatHub(ChatRepo chatRepo,UserRepo userRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
        }

        public async Task SendMessage(Guid chatRoomId, Guid senderId, Guid receiverId, string messageContent)
        {
            var timestamp = DateTime.UtcNow;

            var sender = _userRepo.GetUserById(senderId);
            var receiver = _userRepo.GetUserById(receiverId);
            var message = new TblMessage
            {
                ChatroomId = chatRoomId,
                SenderId = senderId,
                Sender = sender.UserName,
                RecieverId = receiverId,
                Reciever =receiver.UserName,
                Timestamp = timestamp,
                Content = messageContent
            };

            await _chatRepo.CreateMessage(message);
            

            // Notify the receiver by sending the message to their group
            await Clients.Group(chatRoomId.ToString()).SendAsync("ReceiveMessage", new
            {
                Sender = sender.UserName,
                SenderId = sender.UserId,
                Receiver = receiver.UserName,
                Timestamp = timestamp,
                Content = messageContent
            });
        }

        public override async Task OnConnectedAsync()
        {
            var chatRoomId = Context.GetHttpContext().Request.Query["chatRoomId"];
            if (!string.IsNullOrEmpty(chatRoomId))
            {
                // Add the user to the chat room group
                await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var chatRoomId = Context.GetHttpContext().Request.Query["chatRoomId"];
            if (!string.IsNullOrEmpty(chatRoomId))
            {
                // Remove the user from the chat room group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
