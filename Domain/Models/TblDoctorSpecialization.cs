using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblDoctorSpecialization
{
    public Guid DoctorId { get; set; }

    public Guid SpecializationId { get; set; }

    public virtual TblUser Doctor { get; set; } = null!;

    public virtual TblSpecialization Specialization { get; set; } = null!;
}
