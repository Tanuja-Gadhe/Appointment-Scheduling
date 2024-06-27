using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServiceModels
{
   
    public class Slot
    {
        public string EndTime { get; set; }
        public string Status { get; set; }
    }

    public class AvailabilityAppointment
    {
        public string StartTime { get; set; }

        public List<Slot> EndTimeAndStatus { get; set; }
    }

}
