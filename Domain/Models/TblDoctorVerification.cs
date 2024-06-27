using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblDoctorVerification
{
    public Guid Id { get; set; }

    public Guid? DoctorId { get; set; }

    public bool IsVerified { get; set; }

    public DateTime? VerificationDate { get; set; }

    public string? VerifiedBy { get; set; }

    public virtual TblUser? Doctor { get; set; }

    public TblDoctorVerification()
    {
        Id = Guid.NewGuid();
    }
}
