using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Common;
using Domain.Models;
using Org.BouncyCastle.Asn1;
using Repo;
using Services.ServiceModels;


namespace Services.AuthServices
{
    public class AdminServices
    {
        private readonly UserRepo _userRepo;
        private readonly DoctorVerificationRepo _doctorVerificationRepo;
        private readonly DocumentRepo _documentRepo;
        private readonly SpecializationRepo _specializationRepo;
        private readonly Email _email;

        public AdminServices(UserRepo userRepo, DoctorVerificationRepo doctorVerificationRepo,DocumentRepo documentRepo,SpecializationRepo specializationRepo,Email email)
        {
            _userRepo = userRepo;
            _doctorVerificationRepo = doctorVerificationRepo;
            _documentRepo = documentRepo;
            _specializationRepo = specializationRepo;
            _email = email;

        }
        public List<VerificationDTO> GetNonVerifiedDoctors()
        {
            var doctors = _userRepo.GetByRole(Roles.doctor.ToString());
            var nonVerifiedDoctors = _doctorVerificationRepo.GetNonVerifiedDoctors();
            var documents = _documentRepo.GetAllDocuments().GroupBy(document => document.DoctorId);
            var query1 = from doctor in doctors
                         join nonVerified in nonVerifiedDoctors
                         on doctor.UserId equals nonVerified.DoctorId
                         select new
                         {
                             Id = doctor.UserId,
                             Username = doctor.UserName,
                             ImagePath = doctor.UserImage,
                             Name = doctor.FirstName + " " + doctor.LastName,
                             Email = doctor.Email,
                             Address = doctor.Address,
                             Verify = nonVerified.IsVerified
                         };
            var result = from doctor in query1
                         join document in documents
                         on doctor.Id equals document.Key
                         select new VerificationDTO()
                         {

                             Username=doctor.Username,
                             ImagePath=doctor.ImagePath,
                             Name=doctor.Name,
                             Email=doctor.Email,
                             Address=doctor.Address,
                             MedicalLicense =document.FirstOrDefault(d => d.DocumentType == DocumentType.MedicalLicense.ToString())?.FilePath,
                             IdProof= document.FirstOrDefault(d => d.DocumentType == DocumentType.IdProof.ToString())?.FilePath,
                             MedicalDegreeCertificate= document.FirstOrDefault(d => d.DocumentType == DocumentType.MedicalDegreeCertificate.ToString())?.FilePath,
                             PostgraduateMedicalDegreeCertificate= document.FirstOrDefault(d => d.DocumentType == DocumentType.PostgraduateMedicalDegreeCertificate.ToString())?.FilePath,
                             SpecializationCertificate= document.FirstOrDefault(d => d.DocumentType == DocumentType.SpecializationCertificate.ToString())?.FilePath,
                             Verify = doctor.Verify
                         };
            return result.OrderBy(doctor => doctor.Username).ThenBy(doctor=> doctor.Name).ToList();
        }

        public async Task VerifyDoctor(string username,string specialization,string degree,string admin)
        {
           var specializationId = await _specializationRepo.AddSpecialization(specialization, degree);
           var doctor = _userRepo.GetUser(username);
           await _specializationRepo.MapDoctorSpecialization(doctor.UserId, specializationId);
          
           await  _doctorVerificationRepo.Verify(doctor.UserId,admin);

            string recipient = doctor.Email;
            string subject = "Verification Successfull";
            string body = "<b>Your Document Verification has been done successfully , now you can start with your work.</b>";
            _email.SendEmail(recipient, subject, body);

        }

        public List<PatientDTO> GetPatients()
        {
            var patients = _userRepo.GetByRole(Roles.patient.ToString()).Select(patient => new PatientDTO()
                                                                                {
                                                                                    Username = patient.UserName,
                                                                                    FirstName= patient.FirstName,
                                                                                    LastName = patient.LastName,
                                                                                    Email = patient.Email,
                                                                                    Phone = patient.PhoneNumber,
                                                                                    Address = patient.Address,
                                                                                    ImagePath = patient.UserImage
                                                                                });
            return patients.OrderBy(patient => patient.Username).ThenBy(patient => patient.FirstName).ToList();

        }

        public List<VerificationDTO> GetDoctors()
        {
            var doctors = _userRepo.GetByRole(Roles.doctor.ToString());
            var doctorVerification = _doctorVerificationRepo.GetVerification();
            var documents = _documentRepo.GetAllDocuments().GroupBy(document => document.DoctorId);
            var query1 = from doctor in doctors
                         join verification in doctorVerification
                         on doctor.UserId equals verification.DoctorId
                         into ds
                         from s in ds.DefaultIfEmpty()
                         select new
                         {
                             Id = doctor.UserId,
                             Username = doctor.UserName,
                             ImagePath = doctor.UserImage,
                             Name = doctor.FirstName + " " + doctor.LastName,
                             Email = doctor.Email,
                             Address = doctor.Address,
                             Verify = s == null ? false : s.IsVerified
                         };
            var query2 = from doctor in query1
                         join document in documents
                         on doctor.Id equals document.Key
                         into ds
                         from s in ds.DefaultIfEmpty()
                         select new 
                         {
                             DoctorId = doctor.Id,
                             Username = doctor.Username,
                             ImagePath = doctor.ImagePath,
                             Name = doctor.Name,
                             Email = doctor.Email,
                             Address = doctor.Address,
                             MedicalLicense = s == null ? "" : s.FirstOrDefault(d => d.DocumentType == DocumentType.MedicalLicense.ToString())?.FilePath,
                             IdProof =  s == null ? "" : s.FirstOrDefault(d => d.DocumentType == DocumentType.IdProof.ToString())?.FilePath,
                             MedicalDegreeCertificate = s == null ? "" : s.FirstOrDefault(d => d.DocumentType == DocumentType.MedicalDegreeCertificate.ToString())?.FilePath,
                             PostgraduateMedicalDegreeCertificate = s == null ? "" : s.FirstOrDefault(d => d.DocumentType == DocumentType.PostgraduateMedicalDegreeCertificate.ToString())?.FilePath,
                             SpecializationCertificate = s == null ? "" : s.FirstOrDefault(d => d.DocumentType == DocumentType.SpecializationCertificate.ToString())?.FilePath,
                             Verify = doctor.Verify
                         };
            var specializations = _specializationRepo.GetSepcialiazations();
            var doctor_specailizations = _specializationRepo.GetDoctorSpecialization();

            var query3 = from specialization in specializations
                        join doctorSpecialization in doctor_specailizations
                        on specialization.SpecializationId equals doctorSpecialization.SpecializationId
                        select new
                        {
                           DoctorId = doctorSpecialization.DoctorId,
                           Specialization = specialization.SpecializationName,
                           Degree = specialization.DegreeName
                        };

           var result = from doctor in query2 
                        join specialization in query3
                        on doctor.DoctorId equals specialization.DoctorId
                        into ds
                        from s in ds.DefaultIfEmpty()
                        select new VerificationDTO
                        {
                          
                            Username = doctor.Username,
                            ImagePath = doctor.ImagePath,
                            Name = doctor.Name,
                            Email = doctor.Email,
                            Address = doctor.Address,
                            MedicalLicense = doctor.MedicalLicense,
                            IdProof =doctor.IdProof,
                            MedicalDegreeCertificate = doctor.MedicalDegreeCertificate,
                            PostgraduateMedicalDegreeCertificate = doctor.PostgraduateMedicalDegreeCertificate,
                            SpecializationCertificate = doctor.SpecializationCertificate,
                            Verify = doctor.Verify,
                            Specialization = s == null ? "" : s.Specialization,
                            Degree = s == null ? "" :s.Degree,
                        };
            return result.OrderBy(doctor => doctor.Username).ThenBy(doctor => doctor.Name).ToList();
        }
    }
}
