using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblUser
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public Guid RoleId { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateOnly CreatedDate { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateOnly ModifiedDate { get; set; }

    public string UserImage { get; set; } = null!;

    public virtual TblRole Role { get; set; } = null!;

    public virtual ICollection<TblAppointment> TblAppointmentDoctors { get; set; } = new List<TblAppointment>();

    public virtual ICollection<TblAppointment> TblAppointmentPatients { get; set; } = new List<TblAppointment>();

    public virtual ICollection<TblAvailability> TblAvailabilities { get; set; } = new List<TblAvailability>();

    public virtual ICollection<TblChatroom> TblChatroomDoctors { get; set; } = new List<TblChatroom>();

    public virtual ICollection<TblChatroom> TblChatroomPatients { get; set; } = new List<TblChatroom>();

    public virtual ICollection<TblDoctorVerification> TblDoctorVerifications { get; set; } = new List<TblDoctorVerification>();

    public virtual ICollection<TblDocument> TblDocuments { get; set; } = new List<TblDocument>();

    public TblUser()
    {
        UserId = Guid.NewGuid();
    }
}
