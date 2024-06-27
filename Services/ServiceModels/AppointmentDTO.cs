using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;


namespace Services.ServiceModels
{
    
    public class AppointmentDTO
    {
        public Guid AppointmentId { get; set; }
        public DateTime AppointmentsDateTime { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public Guid? PatientId { get;set; }
        public AppointmentDTO(Guid appointmentId,string name, DateTime appointmentDatetIime , string status, string notes)
        {
            AppointmentId=appointmentId;
            Name = name;
            AppointmentsDateTime = appointmentDatetIime;
            Status = status;
            Notes = notes;
        }
        public AppointmentDTO(Guid appointmentId, string name, DateTime appointmentDatetIime, string status, string notes,Guid patientId)
        {
            AppointmentId = appointmentId;
            Name = name;
            AppointmentsDateTime = appointmentDatetIime;
            Status = status;
            Notes = notes;
            PatientId = patientId;
        }
    }
}
