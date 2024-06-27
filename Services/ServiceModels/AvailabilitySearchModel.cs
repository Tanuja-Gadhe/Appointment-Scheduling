using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServiceModels
{
    public class AvailabilityData
    {
        public string Date { get; set; }
        public List<TimeSlot> TimeSlots { get; set; }
    }

    public class TimeSlot
    {
        public Guid AvailabilityId { get; set; }
    
        public int AppointmentCount { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
