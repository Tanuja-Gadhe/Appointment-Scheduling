using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServiceModels
{
    public class MessageDTO
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string Sender { get; set; }  
        public string Reciever { get; set; }
    }
}
