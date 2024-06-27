using Domain.Models;
using Repo;
using Services.AuthModels;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using Common;
using Microsoft.Extensions.Configuration;
using Services.ServiceModels;
using Microsoft.IdentityModel.Tokens;
using System.Numerics;
using Microsoft.DiaSymReader;
namespace Services.AuthServices
{
    public class Authentication
    {
        private readonly UserRepo userRepo;
        private readonly RoleRepo roleRepo;
        private readonly IConfiguration config;
        private readonly Encryption encryption;
        private readonly Uploads uploads;
        private readonly DoctorVerificationRepo doctorVerificationRepo;
        private readonly DocumentRepo documentRepo;

        public Authentication(UserRepo userRepo,RoleRepo roleRepo,IConfiguration config,Encryption encryption,Uploads uploads,DoctorVerificationRepo doctorVerificationRepo,DocumentRepo documentRepo)
        {
            this.userRepo = userRepo;
            this.roleRepo = roleRepo;
            this.config = config;
            this.encryption = encryption;   
            this.uploads = uploads;
            this.doctorVerificationRepo = doctorVerificationRepo;
            this.documentRepo = documentRepo;
        }

        public  async Task RegisterUser(RegisterModel model)
        {
           
            Guid Role =  roleRepo.GetRoleId(model.Role.ToLower());
            string key = config["PasswordKey"];
            string imagePath = await uploads.UploadImage(model.ImageFile);
            string passwordHash = encryption.Encrypt(model.Password, out string passwordSalt, key);
            TblUser user = new TblUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash,
                RoleId = Role,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedBy = model.FirstName + " " + model.LastName,
                CreatedDate = DateOnly.Parse(DateTime.Today.ToString("yyyy-MM-dd")),
                ModifiedBy = model.FirstName + " " + model.LastName,
                ModifiedDate = DateOnly.Parse(DateTime.Today.ToString("yyyy-MM-dd")),
                UserImage = imagePath,
            };
            await userRepo.CreateUser(user);

        }

        public TblUser AuthenticateUser(LoginModel model)
        {
            TblUser user = userRepo.GetUser(model.UsernameOrEmail);
            if(user!=null)
            {
                string passwordSalt = user.PasswordSalt;
                string key = config["PasswordKey"];
                string passwordHash = encryption.AuthEncrypt(model.Password, passwordSalt,key);
                if(passwordHash == user.PasswordHash)
                {
                    return user;
                }
                return null;
            }
            return user;
        }

        public void UpdatePassword(string email, string password)
        {
            TblUser user = userRepo.GetUser(email);
            string key = config["PasswordKey"];
            string passwordHash = encryption.Encrypt(password, out string passwordSalt, key);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            userRepo.UpdateUser();
            
        }

        public ProfileDTO GetProfile(string username)
        {
            var user = userRepo.GetUser(username);
            
            var role = roleRepo.GetRoleName(user.RoleId);
            var profile = new ProfileDTO()
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                ImagePath = user.UserImage
            };
            return profile;
        }

        public bool? IsVerified(TblUser user)
        {
            var isVerified = doctorVerificationRepo.IsVerified(user.UserId);
            return isVerified;
        }

        public async Task Verify(DoctorVerificationModel model)
        {
            var doctor = userRepo.GetUser(model.UserName);
            var medicalLicensePath = await uploads.UploadDocument(model.MedicalLicense);
            var idProofPath = await uploads.UploadDocument(model.IdProof);
            var medicalDegreeCertificatePath = await uploads.UploadDocument(model.MedicalDegreeCertificate);
            var postgraduateMedicalDegreeCertificatePath = await uploads.UploadDocument(model.PostgraduateMedicalDegreeCertificate);
            var specializationCertificatePath = await uploads.UploadDocument(model.SpecializationCertificate);

            var medicalLicense = new TblDocument() { DocumentType = DocumentType.MedicalLicense.ToString(), FilePath = medicalLicensePath, DoctorId = doctor.UserId };
            var idProof = new TblDocument() { DocumentType = DocumentType.IdProof.ToString(), FilePath = idProofPath, DoctorId = doctor.UserId };
            var medicalDegreeCertificate = new TblDocument() { DocumentType = DocumentType.MedicalDegreeCertificate.ToString(), FilePath = medicalDegreeCertificatePath, DoctorId = doctor.UserId };

            await documentRepo.AddDocument(medicalLicense);
            await documentRepo.AddDocument(idProof);
            await documentRepo.AddDocument(medicalDegreeCertificate);

            if (!postgraduateMedicalDegreeCertificatePath.IsNullOrEmpty())
            { 
                var postgraduateMedicalDegreeCertificate = new TblDocument() { DocumentType = DocumentType.PostgraduateMedicalDegreeCertificate.ToString(),FilePath = postgraduateMedicalDegreeCertificatePath,DoctorId=doctor.UserId };
                await documentRepo.AddDocument(postgraduateMedicalDegreeCertificate);
            }

            if (!specializationCertificatePath.IsNullOrEmpty())
            {
                var specializationCertificate = new TblDocument() { DocumentType = DocumentType.SpecializationCertificate.ToString(), FilePath = specializationCertificatePath, DoctorId = doctor.UserId };
                await documentRepo.AddDocument(specializationCertificate);
            }

            var verification = new TblDoctorVerification() { DoctorId = doctor.UserId, IsVerified = false };
            await doctorVerificationRepo.AddVerification(verification);

        }

        public async Task<TblUser> UpdateProfile(ProfileDTO model)
        {
            var user = userRepo.GetUser(model.UserName);
            if(model.ImageFile != null) {
                string imagePath = await uploads.UploadImage(model.ImageFile);
                user.UserImage = imagePath;
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;
            
            userRepo.UpdateUser();
            return user;
        }
    }
}
