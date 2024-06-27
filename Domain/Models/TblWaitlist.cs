using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblWaitlist
{
    public Guid WaitlistId { get; set; }

    public DateOnly WaitlistDate { get; set; }

    public Guid AppointmentId { get; set; }

    public virtual TblAppointment Appointment { get; set; } = null!;

    public TblWaitlist()
    {
        WaitlistId = Guid.NewGuid();    
    }
}
