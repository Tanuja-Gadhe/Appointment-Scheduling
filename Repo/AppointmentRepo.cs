using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repo
{

    public class AppointmentRepo
    {
        private readonly AppointmentSchedulingContext _context;

        public AppointmentRepo(AppointmentSchedulingContext context)
        {
            _context = context;
        }

        public async Task<TblAppointment> Create(Guid patientId, Guid doctorId, DateTime appointmenDateTime, bool isWaiting = false)
        {
            var appointment = new TblAppointment()
            {
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentStatus = isWaiting?Status.waiting.ToString(): Status.scheduled.ToString(),
                Notes = string.Empty,
                AppointmentDatetime = appointmenDateTime
            };
            await _context.TblAppointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            return appointment;

        }

        public List<TblAppointment> GetPatientAppointments(Guid patientID)
        {
            var appointments = _context.TblAppointments.Where(appointment => appointment.PatientId == patientID).ToList();
            return appointments;
        }

        public List<TblAppointment> GetDoctorAppointments(Guid doctorID)
        {
            var appointments = _context.TblAppointments.Where(appointment => appointment.DoctorId == doctorID).ToList();
            return appointments;
        }
        public TblAppointment? GetAppointment(Guid appointmentId)
        {
            return _context.TblAppointments.FirstOrDefault(appointment => appointment.AppointmentId == appointmentId);
        }

        public async Task UpdateAppointment()
        {
           
            await _context.SaveChangesAsync();
        }

        public List<TblAppointment> GetWeeklyAppointment(DateTime startDate,DateTime endDate,Guid doctorId)
        {
            return _context.TblAppointments.Where(appointment => appointment.DoctorId == doctorId && (appointment.AppointmentDatetime >= startDate && appointment.AppointmentDatetime <= endDate)).ToList();
        }
    }
}
