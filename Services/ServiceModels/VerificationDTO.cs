using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServiceModels
{
    public class VerificationDTO
    {
        public string Username { set; get; }
        public string ImagePath { set; get; }
        public string Name { set; get; }
        public string Email { set; get; }
        public string Address { set; get; }
        public string? MedicalLicense { get; set; }
        public string? IdProof { get; set; }
        public string? MedicalDegreeCertificate { get; set; }
        public string? PostgraduateMedicalDegreeCertificate { get; set; }
        public string? SpecializationCertificate { get; set; }
        public string? Specialization { set; get; }
        public string? Degree { set; get; }
        public bool Verify { set; get; }

    }
}
