using Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Services.ServiceModels;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using NuGet.Protocol;
namespace Services.AuthServices
{
    public class DoctorServices
    {
        private readonly UserRepo _userRepo;
        private readonly SpecializationRepo _specializationRepo;
        private readonly AvailabilityRepo _availabilityRepo;
        private readonly AppointmentRepo _appointmentRepo;

        public DoctorServices(UserRepo userRepo,SpecializationRepo specializationRepo, AvailabilityRepo availabilityRepo,AppointmentRepo appointmentRepo)
        {
            _userRepo = userRepo;
            _specializationRepo = specializationRepo;
            _availabilityRepo = availabilityRepo;
            _appointmentRepo = appointmentRepo;

        }

        public List<SearchModel> GetDoctors()
        {
            var doctors = _userRepo.GetByRole(Roles.doctor.ToString());
            var specializations = _specializationRepo.GetSepcialiazations();
            var doctor_specailizations = _specializationRepo.GetDoctorSpecialization();

            var query = from doctor in doctors
                        join doctor_specialization in doctor_specailizations
                        on doctor.UserId equals doctor_specialization.DoctorId
                        select new
                        {
                            FirstName = doctor.FirstName,
                            LastName = doctor.LastName,
                            Email = doctor.Email,
                            PhoneNumber = doctor.PhoneNumber,
                            Address = doctor.Address,
                            UserImage = doctor.UserImage,
                            SpecializationId = doctor_specialization.SpecializationId
                        };
            var result = (from doctor in query
                          join specialization in specializations
                          on doctor.SpecializationId equals specialization.SpecializationId
                          select new SearchModel(
                               doctor.FirstName,
                             doctor.LastName,
                             doctor.Email,
                              doctor.PhoneNumber,
                              doctor.Address,
                              doctor.UserImage,
                              specialization.SpecializationName,
                              specialization.DegreeName
                              )).ToList();
            return result;
        }

        public List<SearchModel> GetDoctorsByName(string name)
        {
            var doctors = _userRepo.GetByRole(Roles.doctor.ToString());
            var specializations = _specializationRepo.GetSepcialiazations();
            var doctor_specailizations = _specializationRepo.GetDoctorSpecialization();
            string[] nameParts = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
            string lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            var searchResult = new List<TblUser>();
            if(nameParts.Length > 1)
            {
                 searchResult = doctors
               .Where(doctor => string.IsNullOrEmpty(firstName) || doctor.FirstName.ToLower().Contains(firstName.ToLower()))
               .Where(doctor => string.IsNullOrEmpty(lastName) || doctor.LastName.ToLower().Contains(lastName.ToLower()))
               .ToList();
            }
            else
            {
                searchResult = doctors
                              .Where(doctor => doctor.FirstName.ToLower().Contains(firstName.ToLower()) ||
                                       doctor.LastName.ToLower().Contains(firstName.ToLower())).ToList();
            }

           var query = from doctor in searchResult
                        join doctor_specialization in doctor_specailizations
                        on doctor.UserId equals doctor_specialization.DoctorId
                        select new
                        {
                            FirstName = doctor.FirstName,
                            LastName = doctor.LastName,
                            Email = doctor.Email,
                            PhoneNumber =doctor.PhoneNumber,
                            Address = doctor.Address,
                            UserImage = doctor.UserImage,
                            SpecializationId = doctor_specialization.SpecializationId
                        };
            var result = (from doctor in query
                         join specialization in specializations
                         on doctor.SpecializationId equals specialization.SpecializationId
                         select new SearchModel(
                              doctor.FirstName,
                            doctor.LastName,
                            doctor.Email,
                             doctor.PhoneNumber,
                             doctor.Address,
                             doctor.UserImage,
                             specialization.SpecializationName,
                             specialization.DegreeName
                             )).ToList();
            return result;

        }

        public List<SearchModel> GetDoctorsBySpecialization(string speciality, string location)
        {
            var doctors = _userRepo.GetByRole(Roles.doctor.ToString());
            var specializations = _specializationRepo.GetSepcialiazations();
            var doctor_specailizations = _specializationRepo.GetDoctorSpecialization();


            var query = from doctor in doctors
                        join doctor_specialization in doctor_specailizations
                        on doctor.UserId equals doctor_specialization.DoctorId
                        select new
                        {
                            FirstName = doctor.FirstName,
                            LastName = doctor.LastName,
                            Email = doctor.Email,
                            PhoneNumber = doctor.PhoneNumber,
                            Address = doctor.Address,
                            UserImage = doctor.UserImage,
                            SpecializationId = doctor_specialization.SpecializationId
                        };
            var query1 = (from doctor in query
                          join specialization in specializations
                          on doctor.SpecializationId equals specialization.SpecializationId
                          select new SearchModel(
                               doctor.FirstName,
                             doctor.LastName,
                             doctor.Email,
                              doctor.PhoneNumber,
                              doctor.Address,
                              doctor.UserImage,
                              specialization.SpecializationName,
                              specialization.DegreeName
                              )).ToList();
            var searchResult = query1
                               .Where(doctor => doctor.Specialization.ToLower().Contains(speciality.ToLower()) && doctor.Address.ToLower().Contains(location.ToLower()))
                               .ToList();

            return searchResult;

        }

        public List<AvailabilityData> GetAvailability(string email)
        {
            var doctor = _userRepo.GetUser(email);
            var availability = _availabilityRepo.GetAvailability(doctor.UserId).ToList();
            //var todaysDate = DateTime.Today;
            //availability = availability.Where(a => a.AvailabilityStartDatetime.Date >= todaysDate).ToList();
            var groupedByDate = availability.GroupBy(a => a.AvailabilityStartDatetime.Date);

            List<AvailabilityData> availabilityData = groupedByDate.Select(group =>
             new AvailabilityData
             {
                 Date = group.Key.ToString("yyyy-MM-dd"),
                 TimeSlots = group.Select(a => new TimeSlot
                 {
                     AvailabilityId = a.AvailabilityId,
                     AppointmentCount = a.AppointmentCount,
                     StartTime = a.AvailabilityStartDatetime.ToString("HH:mm"),
                     EndTime = a.AvailabilityEndDatetime.ToString("HH:mm")
                 }).OrderBy(slot => DateTime.Parse(slot.StartTime)).ToList() 
             }).OrderBy(data => DateTime.Parse(data.Date)).ToList();

            return availabilityData;

        }

        public List<AppointmentDTO> GetAppointmentsForWeek(string username)
        {
            var doctor = _userRepo.GetUser(username);
            var appointments = _appointmentRepo.GetDoctorAppointments(doctor.UserId);

            int day = (int)DateTime.Today.DayOfWeek;
            DateTime startDate = DateTime.Today.AddDays(-(day - 1));
            DateTime endDate = startDate.AddDays(5);

            var appointmentsThisWeek = appointments.Where(appointment => appointment.AppointmentDatetime >= startDate && appointment.AppointmentDatetime <= endDate);
            var patients = _userRepo.GetByRole(Roles.patient.ToString());

            var results  = (from appointment in appointmentsThisWeek
                                join patient in patients
                                on appointment.PatientId equals patient.UserId
                                select new AppointmentDTO(appointment.AppointmentId, patient.FirstName + " " + patient.LastName, appointment.AppointmentDatetime, appointment.AppointmentStatus, appointment.Notes,appointment.PatientId)).ToList();
            return results.OrderByDescending(appointment => appointment.Status).ThenBy(appointment => appointment.AppointmentsDateTime.Date).ThenBy(appointment => DateTime.Parse(appointment.AppointmentsDateTime.ToString("HH:mm"))).ToList();

        }

        public List<AvailabilityAppointment> GetTodaysAvailabilityAndAppointments(string username)
        {
            var doctor = _userRepo.GetUser(username);

            var availabilities = _availabilityRepo.GetAvailability(doctor.UserId);

            var appointments = _appointmentRepo.GetDoctorAppointments(doctor.UserId);

            var todaysAvailability = availabilities.Where(availability => availability.AvailabilityStartDatetime.Date.Equals(DateTime.Now.Date));

            var todaysAppointments = appointments.Where(appointment => appointment.AppointmentDatetime.Date.Equals(DateTime.Now.Date));

            var result = from availability in todaysAvailability
                         join appointment in todaysAppointments
                         on availability.AvailabilityStartDatetime.Date equals appointment.AppointmentDatetime.Date
                         select new
                         {
                            
                             AvailabilityStartTime = availability.AvailabilityStartDatetime,
                             AvailabilityEndTime = availability.AvailabilityEndDatetime,
                             Status = availability.AvailabilityStartDatetime.ToString("HH:mm") == appointment.AppointmentDatetime.ToString("HH:mm")?appointment.AppointmentStatus:Status.unknown.ToString()
                         };
            var groupedData = result.GroupBy(a => a.AvailabilityStartTime);

            var data = groupedData.Select(group =>
               new AvailabilityAppointment
               {
                   StartTime = group.Key.ToString("HH:mm"),
                   EndTimeAndStatus = group.Select(a => new Slot
                   {
                       EndTime = a.AvailabilityEndTime.ToString("HH:mm"),
                       Status = a.Status
                   }).ToList()
               }
            ).ToList();

            return data;
        }


        public List<TblAppointment> GetWeeklyBreakdown( string username)
        {
            var doctor = _userRepo.GetUser(username);
            // var appointments = _appointmentRepo.GetDoctorAppointments(doctor.UserId);
            int day = (int)DateTime.Today.DayOfWeek;
            DateTime startDate = DateTime.Today.AddDays(-(day - 1));
            DateTime endDate = startDate.AddDays(5);
            var appointments = _appointmentRepo.GetWeeklyAppointment(startDate, endDate, doctor.UserId).OrderBy(appointment => appointment.AppointmentDatetime).ThenBy(appointment => DateTime.Parse(appointment.AppointmentDatetime.ToString("HH:mm"))); ;
           

           // var appointmentsThisWeek = appointments.Where(appointment => appointment.AppointmentDatetime >= startDate && appointment.AppointmentDatetime <= endDate)
             //                                       .OrderBy(appointment => appointment.AppointmentDatetime).ThenBy(appointment => DateTime.Parse(appointment.AppointmentDatetime.ToString("HH:mm")));

            return appointments.ToList();

        }

        public async Task AddAvailability(AvailabilityDTO model,string username)
        {
            var doctor = _userRepo.GetUser(username);
            var availability = new TblAvailability() {
                                  DoctorId = doctor.UserId,
                                  AvailabilityStartDatetime = model.AvailabilityStartDatetime,
                                  AvailabilityEndDatetime = model.AvailabilityEndDatetime,
                                  AppointmentCount = model.AppointmentCount};
            
           await _availabilityRepo.CreateAvailability(availability);
        }

        public void DeleteAvailability(Guid availabilityId)
        {
           
            _availabilityRepo.DeleteAvailability(availabilityId);
        }

        public TblAvailability GetAvailability(Guid availabilityId)
        {
            var availability = _availabilityRepo.GetAvailabilityById(availabilityId);
            return availability;
        }

        public async Task UpdateAvailability(Guid availabilityId,AvailabilityDTO model)
        {
            var availability = _availabilityRepo.GetAvailabilityById(availabilityId);
            availability.AvailabilityStartDatetime = model.AvailabilityStartDatetime;
            availability.AvailabilityEndDatetime = model.AvailabilityEndDatetime;
            availability.AppointmentCount = model.AppointmentCount;
            await _availabilityRepo.UpdateAvailability(availability);
       
        }

        public List<AppointmentDTO> GetAppointments(string username)
        {
            var doctor = _userRepo.GetUser(username);
            var patients = _userRepo.GetByRole(Roles.patient.ToString());
            var results = _appointmentRepo.GetDoctorAppointments(doctor.UserId);
            var appointments = (from appointment in results
                                join patient in patients
                                on appointment.PatientId equals patient.UserId
                                select new AppointmentDTO(appointment.AppointmentId, patient.FirstName + " " + patient.LastName, appointment.AppointmentDatetime, appointment.AppointmentStatus, appointment.Notes)).ToList();

            return appointments.OrderByDescending(appointment => appointment.Status).ThenBy(appointment => appointment.AppointmentsDateTime).ToList();
        }

    }
}
