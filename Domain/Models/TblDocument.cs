using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblDocument
{
    public Guid DocumentId { get; set; }

    public string DocumentType { get; set; } = null!;

    public Guid DoctorId { get; set; }

    public string? FilePath { get; set; }

    public virtual TblUser Doctor { get; set; } = null!;

    public TblDocument()
    {
        DocumentId = Guid.NewGuid();
    }
}
