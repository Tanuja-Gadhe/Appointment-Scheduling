using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo
{
    public class WaitListRepo
    {
        private readonly AppointmentSchedulingContext _context;

        public WaitListRepo(AppointmentSchedulingContext context)
        {
            _context = context;
        }

        public async Task Create(Guid appointmentId, DateOnly date)
        {
            var waitlist = new TblWaitlist()
            {
                WaitlistDate = date,
                AppointmentId = appointmentId
            };
            await _context.TblWaitlists.AddAsync(waitlist);
            await _context.SaveChangesAsync();
        }
    }
}
