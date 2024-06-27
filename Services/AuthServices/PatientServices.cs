using Domain.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Repo;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Policy;
using Common;
namespace Services.AuthServices
{
    public class PatientServices
    {
        private readonly UserRepo userRepo;
        private readonly AvailabilityRepo availabilityRepo;
        private readonly AppointmentRepo appointmentRepo;
        private readonly SpecializationRepo specializationRepo;
        private readonly WaitListRepo waitListRepo;
        private readonly Email email;
        public PatientServices(UserRepo userRepo,AvailabilityRepo availabilityRepo,AppointmentRepo appointmentRepo,
            SpecializationRepo specializationRepo,WaitListRepo waitListRepo,Email email)
        {

            this.userRepo = userRepo;
            this.availabilityRepo = availabilityRepo;
            this.appointmentRepo = appointmentRepo;
            this.specializationRepo = specializationRepo;
            this.waitListRepo = waitListRepo;
            this.email = email;
        }

        public async Task BookAppointment(string username, string doctorEmail, Guid availabilityId,bool isWaiting = false)
        {
            var user = userRepo.GetUser(username);
            var doctor = userRepo.GetUser(doctorEmail);
            var datetime = availabilityRepo.UpdateAvailability(availabilityId,isWaiting);
            var appointment = await appointmentRepo.Create(user.UserId, doctor.UserId, datetime,isWaiting);
            if(isWaiting)
            {
               await waitListRepo.Create(appointment.AppointmentId, DateOnly.Parse(datetime.ToString("yyyy-MM-dd")));

            }
        }

        public List<AppointmentDTO> GetAppointments(string username)
        {
            var patient = userRepo.GetUser(username);
            var doctors = userRepo.GetByRole(Roles.doctor.ToString());
            var results = appointmentRepo.GetPatientAppointments(patient.UserId);
            var appointments = (from appointment in results
                               join doctor in doctors
                               on appointment.DoctorId equals doctor.UserId
                               select new AppointmentDTO(appointment.AppointmentId,doctor.FirstName+" "+doctor.LastName, appointment.AppointmentDatetime,appointment.AppointmentStatus,appointment.Notes)).ToList();
                              
            return appointments.OrderByDescending(appointment => appointment.Status).ThenBy(appointment => appointment.AppointmentsDateTime).ToList();
        }

        public AppointmentDetailsDTO ViewAppointment(Guid appointmentId)
        {
            var appointment = appointmentRepo.GetAppointment(appointmentId);
            var patient = userRepo.GetUserById(appointment.PatientId);
            var doctor = userRepo.GetUserById(appointment.DoctorId);
            var specializations = specializationRepo.GetSepcialiazations();
            var doctorSpecailizations = specializationRepo.GetDoctorSpecialization();

            var doctorSpecialization = doctorSpecailizations.FirstOrDefault(doctorSpecailization => doctorSpecailization.DoctorId == doctor.UserId);
            var specialization = specializations.FirstOrDefault(specialization => specialization.SpecializationId == doctorSpecialization.SpecializationId);
            var appointmentDetails = new AppointmentDetailsDTO()
            {
                PatientName = patient.FirstName + " " + patient.LastName,
                PatientAddress = patient.Address,
                PatientEmail = patient.Email,
                PatientPhoneNumber = patient.PhoneNumber,
                DoctorsDetail = new SearchModel(
                                 doctor.FirstName,
                                 doctor.LastName,
                                 doctor.Email,
                                 doctor.PhoneNumber,
                                 doctor.Address,
                                 doctor.UserImage,
                                 specialization.SpecializationName,
                                 specialization.DegreeName),
                AppointmentDateTime= appointment.AppointmentDatetime,
                Status = appointment.AppointmentStatus,
                Notes = appointment.Notes,
                AppointmentId = appointment.AppointmentId,
            };

            return appointmentDetails;

        }

        public async Task UpdateAppointmentStatus(Guid appointmentId,string status,string role = "")
        {
            var appointment = appointmentRepo.GetAppointment(appointmentId);
            appointment.AppointmentStatus = status.ToLower();
            await appointmentRepo.UpdateAppointment();
            if(status == Status.cancelled.ToString() && !role.IsNullOrEmpty())
            {
                if(role == Roles.doctor.ToString())
                {
                    var patient = userRepo.GetUserById(appointment.PatientId);
                    var doctor = userRepo.GetUserById(appointment.DoctorId);
                    string recipient = patient.Email;
                    string subject = "Appointment Cancelled ";
                    string body = $"<b>Your appointment Scheduled on {appointment.AppointmentDatetime.ToString("dd-MM-yyyy")} - {appointment.AppointmentDatetime.ToString("HH:mm")} with " +
                        $"Dr.{doctor.FirstName} {doctor.LastName} has been cancelled by the doctor.</b><br/>" ;
                    email.SendEmail(recipient, subject, body);
                }

            }
        }

        public async Task<string> UpdateAppointmentNotes(Guid appointmentId, string notes)
        {
            var appointment = appointmentRepo.GetAppointment(appointmentId);
            appointment.Notes += notes;
            await appointmentRepo.UpdateAppointment();
            return appointment.Notes;
        }

    }
}
