using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblAppointment
{
    public Guid AppointmentId { get; set; }

    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    public string AppointmentStatus { get; set; } = null!;

    public string Notes { get; set; } = null!;

    public DateTime AppointmentDatetime { get; set; }

    public virtual TblUser Doctor { get; set; } = null!;

    public virtual TblUser Patient { get; set; } = null!;

    public virtual ICollection<TblWaitlist> TblWaitlists { get; set; } = new List<TblWaitlist>();

    public TblAppointment()
    {
        AppointmentId = Guid.NewGuid();
    }
}
