using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblChatroom
{
    public Guid ChatroomId { get; set; }

    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    public virtual TblUser Doctor { get; set; } = null!;

    public virtual TblUser Patient { get; set; } = null!;

    public virtual ICollection<TblMessage> TblMessages { get; set; } = new List<TblMessage>();

    public TblChatroom()
    {
        ChatroomId= Guid.NewGuid();
    }
}
