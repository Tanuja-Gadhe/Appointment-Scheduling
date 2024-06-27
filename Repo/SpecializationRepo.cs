using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Repo
{
    public class SpecializationRepo
    {
        private readonly AppointmentSchedulingContext _context;

        public SpecializationRepo(AppointmentSchedulingContext context)
        {
            _context = context;
        }

        public List<TblSpecialization> GetSepcialiazations()
        {
            var specializations = _context.TblSpecializations.ToList();
            return specializations;
        }

        public List<TblDoctorSpecialization> GetDoctorSpecialization()
        {
            return _context.TblDoctorSpecializations.ToList();
        }

        public async Task<Guid> AddSpecialization(string specializationName,string degree)
        {
            var record = _context.TblSpecializations.FirstOrDefault(specialization => specialization.SpecializationName == specializationName && specialization.DegreeName == degree);
            if(record == null)
            {
                record = new TblSpecialization() { SpecializationName = specializationName, DegreeName = degree };
                await _context.TblSpecializations.AddAsync(record);
                await _context.SaveChangesAsync();
            }
            return record.SpecializationId;
        }

        public async Task MapDoctorSpecialization(Guid doctorId,Guid specializationId)
        {
            try
            {
                string sql = $"INSERT INTO tbl_doctor_specialization (SpecializationId, DoctorId) VALUES ({specializationId}, {doctorId})";

                await _context.Database.ExecuteSqlRawAsync(sql);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("An error occurred while adding the record: " + ex.Message);
            }

        }
    }
}
