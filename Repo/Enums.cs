using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo
{
    public enum Roles { patient,doctor,admin}
    public enum Status { scheduled,cancelled,completed,waiting,unknown}

    public enum DocumentType {MedicalLicense,IdProof, MedicalDegreeCertificate, PostgraduateMedicalDegreeCertificate, SpecializationCertificate }
}
