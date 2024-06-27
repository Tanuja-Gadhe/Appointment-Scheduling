using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblMessage
{
    public Guid MessageId { get; set; }

    public Guid ChatroomId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public string Sender { get; set; } = null!;

    public Guid SenderId { get; set; }

    public string Reciever { get; set; } = null!;

    public Guid RecieverId { get; set; }

    public virtual TblChatroom Chatroom { get; set; } = null!;

    public TblMessage()
    {
        MessageId = Guid.NewGuid();
    }
}
