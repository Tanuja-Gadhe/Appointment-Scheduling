using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblSpecialization
{
    public Guid SpecializationId { get; set; }

    public string SpecializationName { get; set; } = null!;

    public string DegreeName { get; set; } = null!;

    public TblSpecialization()
    {
        SpecializationId = Guid.NewGuid();
    }
}
