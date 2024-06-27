using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServiceModels
{
    public class AppointmentDetailsDTO
    {
        public Guid AppointmentId { get; set; }
        public string PatientName { get; set; }
        public string PatientPhoneNumber {  get; set; }
        public string PatientEmail { get; set; }
        public string PatientAddress { get; set; }
        public SearchModel DoctorsDetail { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string Status { get;set; }
        public string Notes { get;set; }
    }
}
