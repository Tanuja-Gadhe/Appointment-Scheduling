using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo
{
    public class AvailabilityRepo
    {
        private readonly AppointmentSchedulingContext _context;

        public AvailabilityRepo(AppointmentSchedulingContext context)
        {
            _context = context;
        }

        public List<TblAvailability> GetAvailability(Guid doctorId)
        {
            return _context.TblAvailabilities.Where(availability => availability.DoctorId == doctorId).ToList();
        }

        public TblAvailability GetAvailabilityById(Guid availabilityId)
        {
            return _context.TblAvailabilities.FirstOrDefault(availability => availability.AvailabilityId == availabilityId);
        }
        public async Task CreateAvailability(TblAvailability availability)
        {
            await _context.TblAvailabilities.AddAsync(availability);
            await _context.SaveChangesAsync();
        }
        public DateTime UpdateAvailability(Guid availabilityId,bool isWaiting = false)
        {
           var availability =_context.TblAvailabilities.FirstOrDefault(availability => availability.AvailabilityId == availabilityId);
            if(availability !=null)
            {
                if(!isWaiting)
                {
                    availability.AppointmentCount--;
                    _context.SaveChanges();
                }
                var datetime = availability.AvailabilityStartDatetime;
                return datetime;
            }
            return default(DateTime);
        }

        public void DeleteAvailability(Guid availabilityId)
        {
            var availability = _context.TblAvailabilities.FirstOrDefault(availability=> availability.AvailabilityId==availabilityId);

            if (availability != null)
            {
             
                _context.TblAvailabilities.Remove(availability);
                _context.SaveChanges();
            }
        }

        public async Task UpdateAvailability(TblAvailability  availability)
        {
            _context.Entry(availability).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
