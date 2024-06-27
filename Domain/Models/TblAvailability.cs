using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblAvailability
{
    public Guid AvailabilityId { get; set; }

    public Guid DoctorId { get; set; }

    public DateTime AvailabilityStartDatetime { get; set; }

    public DateTime AvailabilityEndDatetime { get; set; }

    public int AppointmentCount { get; set; }

    public virtual TblUser Doctor { get; set; } = null!;

    public TblAvailability()
    {
        AvailabilityId = Guid.NewGuid();
    }
}
