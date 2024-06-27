using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo
{
    public class DoctorVerificationRepo
    {
        private readonly AppointmentSchedulingContext _context;

        public DoctorVerificationRepo(AppointmentSchedulingContext context)
        {
            _context = context;
        }

        public bool? IsVerified(Guid DoctorID)
        {
            return _context.TblDoctorVerifications.FirstOrDefault(doctor => doctor.DoctorId == DoctorID)?.IsVerified;
        }

        public async Task AddVerification(TblDoctorVerification verification)
        {
            await _context.TblDoctorVerifications.AddAsync(verification);
            await _context.SaveChangesAsync();
        }

        public List<TblDoctorVerification> GetNonVerifiedDoctors()
        {
            return _context.TblDoctorVerifications.Where(verification => verification.IsVerified == false).ToList();
        }

        public List<TblDoctorVerification> GetVerifiedDoctors()
        {
            return _context.TblDoctorVerifications.Where(verification =>verification.IsVerified == true).ToList();  
        }

        public async Task Verify(Guid DoctorID,string admin)
        {
            var verificationObj = _context.TblDoctorVerifications.FirstOrDefault(doctor => doctor.DoctorId == DoctorID);
            verificationObj.IsVerified = true;
            verificationObj.VerifiedBy = admin;
            verificationObj.VerificationDate = DateTime.Now;
            await _context.SaveChangesAsync();


        }

        public List<TblDoctorVerification> GetVerification()
        {
            return _context.TblDoctorVerifications.ToList();
        }
    }
}
